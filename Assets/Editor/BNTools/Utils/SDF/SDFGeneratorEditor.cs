using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Experimental.Rendering;
using System;

[System.Serializable]
public class SDFGeneratorEditor : EditorWindow
{

    Color colUILine = new Color(0.5f, 0.5f, 0.5f, 0.05f);

    [Flags]
    public enum TextureModes
    {
        R = 0x01, G = 0x02, B = 0x04, A = 0x08,
        RGB = R | G | B,
        RGBA = R | G | B | A,
    }

    public string[] TextureModeOptions = { "R", "G", "B", "A", "RGB", "RGBA" };
    public int TexMode = 0;

    public TextureModes Mode = TextureModes.A;
    public float GradientSize = 0.1f;
    public bool SetImportSettings = true;

    public Texture2D tex2D;

    [MenuItem("BNDataTools/SDFTextureGenerator")]

    static void Init()
    {
        var window = GetWindow<SDFGeneratorEditor>("SDF Texture Generator");
        window.position = new Rect(512, 256, 256, 350);
        window.Show();
    }
    void OnGUI()
    {
        var titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("SDF Generator Settings", titleStyle);
        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("Process Channels", EditorStyles.boldLabel);
        TexMode = EditorGUILayout.Popup(TexMode, TextureModeOptions, GUILayout.Width(128));
        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("How far the SDF should spread", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("(in percentange of texture size)");
        GradientSize = EditorGUILayout.FloatField(GradientSize, GUILayout.Width(128));
        DrawUILine(colUILine, 3, 12);
        EditorGUILayout.LabelField("Source Texture", EditorStyles.boldLabel);
        EditorGUILayout.Space(-16);
        tex2D = TextureField("", tex2D);
        DrawUILine(colUILine, 3, 12);

        if (tex2D != null)
        {

            if (GUILayout.Button("Generate SDF Texture"))
            {
                switch (TexMode)
                {
                    case 0:
                        Mode = TextureModes.R;
                        break;
                    case 1:
                        Mode = TextureModes.G;
                        break;
                    case 2:
                        Mode = TextureModes.B;
                        break;
                    case 3:
                        Mode = TextureModes.A;
                        break;
                    case 4:
                        Mode = TextureModes.RGB;
                        break;
                    case 5:
                        Mode = TextureModes.RGBA;
                        break;
                }

                GenerateStatic();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("You need to assign a Texture to Process, it will be saved at the same path, with suffix sdf.", MessageType.Warning);
        }

        DrawUILine(colUILine, 3, 12);

    }

    public void GenerateStatic()
    {
        // Validate the input
        if ((Mode & (TextureModes.RGB)) != 0)
        {
            //foreach (var target in Targets)
            //{
            var path = AssetDatabase.GetAssetPath(tex2D);
            //if (string.IsNullOrEmpty(path)) continue;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            //if (importer == null) continue;
            if (importer.sRGBTexture)
            {
                Debug.LogWarning("Texture " + tex2D + " is sRGB but is being used as an RGB distance field. Consider importing it as linear.");
            }
            //}
        }
        // Configure material
        var material = SDFSettings.CreateGeneratorMaterial();
        material.SetFloat("_Feather", GradientSize);
        // Generate based on source textures
        //foreach (var target in Targets)
        //{
        GenerateAsset(tex2D, material);
        //}
        // Cleanup
        DestroyImmediate(material);
    }

    public static Texture2D Generate(Texture2D texture, Material material, TextureModes mode, int width = -1, int height = -1)
    {
        Texture2D result = null;
        Color32[] pixels = null;
        for (int c = 3; c >= 0; c--)
        {
            if (((int)mode & (1 << c)) == 0) continue;
            material.SetFloat("_Channel", c);
            var resultC = Generate(texture, material, width, height);
            if (result == null)
            {
                // We can use alpha directly (generator outputs in A channel)
                result = resultC;
            }
            else
            {
                // Otherwise we'll just pack on CPU
                if (pixels == null) pixels = result.GetPixels32();
                var resPx = resultC.GetPixels32();
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i][c] = resPx[i][c];
                }
                DestroyImmediate(resultC);
            }
        }
        if (pixels != null)
            result.SetPixels32(pixels);
        return result;
    }

    // Generate a distance field
    // The "material" must be a SDF generating material
    // Optionally push the results to the specified texture (must be a compatible format)
    public static Texture2D Generate(Texture2D texture, Material material, int width = -1, int height = -1)
    {
        // Allocate some temporary buffers
        var stepFormat = new RenderTextureDescriptor(texture.width, texture.height, GraphicsFormat.R16G16B16A16_UNorm, 0, 0);
        stepFormat.sRGB = false;
        var target1 = RenderTexture.GetTemporary(stepFormat);
        var target2 = RenderTexture.GetTemporary(stepFormat);
        target1.filterMode = FilterMode.Point;
        target2.filterMode = FilterMode.Point;
        target1.wrapMode = TextureWrapMode.Clamp;
        target2.wrapMode = TextureWrapMode.Clamp;

        var firstPass = 0;
        var finalPass = material.FindPass("FinalPass");

        // Detect edges of image
        material.EnableKeyword("FIRSTPASS");
        material.SetFloat("_Spread", 1);
        Graphics.Blit(texture, target1, material, firstPass);
        material.DisableKeyword("FIRSTPASS");
        Swap(ref target1, ref target2);

        // Gather nearest edges with varying spread values
        for (int i = 11; i >= 0; i--)
        {
            material.SetFloat("_Spread", Mathf.Pow(2, i));
            Graphics.Blit(target2, target1, material, firstPass);
            Swap(ref target1, ref target2);
        }

        var resultFormat = new RenderTextureDescriptor(texture.width, texture.height, GraphicsFormat.R8G8B8A8_UNorm, 0, 0);
        {
            // TODO: Fix this hack, ideally should work for ALL textures, not just imported ones
            var sourcepath = AssetDatabase.GetAssetPath(texture);
            var sourceimporter = string.IsNullOrEmpty(sourcepath) ? default : AssetImporter.GetAtPath(sourcepath) as TextureImporter;
            if (sourceimporter != null) resultFormat.sRGB = sourceimporter.sRGBTexture;
        }
        var resultTarget = RenderTexture.GetTemporary(resultFormat);
        resultTarget.wrapMode = TextureWrapMode.Clamp;

        // Compute the final distance from nearest edge value
        material.SetTexture("_SourceTex", texture);
        Graphics.Blit(target2, resultTarget, material, finalPass);

        if (width == -1) width = texture.width;
        if (height == -1) height = texture.height;
        var result = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_UNorm, 0, TextureCreationFlags.None);

        // If the texture needs to be resized, resize it here
        if (result.width != texture.width || result.height != texture.height)
        {
            var resultTarget2 = RenderTexture.GetTemporary(result.width, result.height, 0, GraphicsFormat.R8G8B8A8_UNorm);
            resultTarget2.wrapMode = TextureWrapMode.Clamp;
            Graphics.Blit(resultTarget, resultTarget2);
            Swap(ref resultTarget, ref resultTarget2);
            RenderTexture.ReleaseTemporary(resultTarget2);
        }

        // Copy to CPU
        result.ReadPixels(new Rect(0, 0, result.width, result.height), 0, 0);

        // Clean up
        RenderTexture.ReleaseTemporary(resultTarget);
        RenderTexture.ReleaseTemporary(target2);
        RenderTexture.ReleaseTemporary(target1);

        return result;
    }

    private static void Swap<T>(ref T v1, ref T v2)
    {
        var t = v1;
        v1 = v2;
        v2 = t;
    }

    public void GenerateAsset(Texture2D texture, Material material)
    {
        // Generate SDF data
        var result = Generate(texture, material, Mode);

        // Generate the new assest
        var path = AssetDatabase.GetAssetPath(texture);
        path = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + ".sdf.png";
        File.WriteAllBytes(path, result.EncodeToPNG());
        AssetDatabase.Refresh();
        DestroyImmediate(result);

        // Disable compression and use simple format
        if (AssetImporter.GetAtPath(path) is TextureImporter importer && SetImportSettings)
        {
            SetImportParameters(importer, Mode);
            var ogpath = AssetDatabase.GetAssetPath(texture);
            var ogimporter = string.IsNullOrEmpty(ogpath) ? default : AssetImporter.GetAtPath(path) as TextureImporter;
            if (ogimporter != null)
            {
                // Preserve sRGB if not processing any RGB
                if ((Mode & TextureModes.RGB) == 0)
                {
                    importer.sRGBTexture = ogimporter.sRGBTexture;
                }
            }
        }
    }

    public static void SetImportParameters(TextureImporter importer, TextureModes mode)
    {
        importer.sRGBTexture = (mode & TextureModes.RGB) == 0;
        var settings = importer.GetDefaultPlatformTextureSettings();
        settings.format =
            mode == TextureModes.A ? TextureImporterFormat.Alpha8 :
            mode == TextureModes.RGB ? TextureImporterFormat.RGB24 :
            TextureImporterFormat.RGBA32;
        importer.SetPlatformTextureSettings(settings);
        importer.SaveAndReimport();
    }

    // UI DRAW TOOLS

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    public static void DrawUILineVertical(Color color, int thickness = 2, int padding = 10, int lenght = 4)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
        r.height = thickness;
        r.x += padding / 2;
        r.y -= 2;
        r.height += 6 + lenght;
        EditorGUI.DrawRect(r, color);
    }
    public static void DrawUILineVerticalLarge(Color color, int spaceX)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(3));
        r.height = 44;
        r.x += spaceX;
        r.y -= 10;
        r.height += 32;
        EditorGUI.DrawRect(r, color);
    }
}

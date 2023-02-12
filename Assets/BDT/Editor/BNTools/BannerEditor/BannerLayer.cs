using System.Xml;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using UnityEngine.EventSystems;

public class BannerLayer : ScriptableObject
{
    public Texture2D textureOriginal;
    public Texture2D textureWork;
    public string sprite_ID;
    public string color_B_hex;
    public string color_A_hex;
    public string color_B;
    public string color_A;
    public bool stroke;
    public bool mirror;
    public string pos_X;
    public string pos_Y;
    public string scale_height;
    public string scale_width;
    public string rotation;
}

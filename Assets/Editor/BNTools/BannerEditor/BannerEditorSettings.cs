using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BannerEditor_settings.asset", menuName = "BDT/BannerEditor Settings Asset", order = 1)]

public class BannerEditorSettings : ScriptableObject
{
    
   public string[] sprites;
   public string[] sprite_IDs;
   public string[] colors;
   public string[] color_IDs;

}

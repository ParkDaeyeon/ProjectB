using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D.UI;
namespace Ext.UnityEdit.UI
{
    public class OptimizedImagePostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            SpriteRuntimeData.Shared.UpdateAll();
        }

        static void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            SpriteRuntimeData.Shared.UpdateAll();
        }
    }
}

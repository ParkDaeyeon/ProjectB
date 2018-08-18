using UnityEngine;
using System.Collections;
//using System.Reflection;
namespace Ext.Unity3D
{
    public static class ImageResizer
    {
        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] colors = result.GetPixels(0);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int px = 0; px < colors.Length; px++)
            {
                colors[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(colors, 0);
            result.Apply();
            return result;
        }
    }
}

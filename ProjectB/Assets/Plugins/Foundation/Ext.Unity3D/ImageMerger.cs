using UnityEngine;
using System.Collections;
//using System.Reflection;
using System.Runtime.InteropServices;
namespace Ext.Unity3D
{
    public static class ImageMerger
    {
        public static void SetPixels32_src32(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    colors[pixelPos] = colors_src[pixelPos_src];
                }
            }
        }

        public static void SetPixels32_src24(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    colors[pixelPos] = colors_src[pixelPos_src];
                }
            }
        }

        public static void SetPixels24_src24(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    colors[pixelPos] = colors_src[pixelPos_src];
                }
            }
        }

        public static void SetPixels32_AlphaBlending_src32_NoDestAlpha(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            float color_inv = 1 / 255f;

            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    Color32 color = colors[pixelPos];
                    Color32 color_src = colors_src[pixelPos_src];

                    // NOTE: alpha blending

                    //float alpha = color.a;

                    float alpha_src = color_src.a * color_inv;
                    float alpha_src_inv = 1 - alpha_src;

                    color.r = (byte)((color.r * alpha_src_inv) + (color_src.r * alpha_src));
                    color.g = (byte)((color.g * alpha_src_inv) + (color_src.g * alpha_src));
                    color.b = (byte)((color.b * alpha_src_inv) + (color_src.b * alpha_src));
                    color.a = color_src.a; // TODO:

                    //Debug.Log("color_src = " + color_src + ", color_src.a =  " + color_src.a + ", a = " + alpha_src + ", ainv = " + alpha_src_inv + ", " + color);

                    colors[pixelPos] = color;
                }
            }
        }

        public static void SetPixels32_AlphaBlending_src32(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            float color_inv = 1 / 255f;

            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    Color32 color = colors[pixelPos];
                    Color32 color_src = colors_src[pixelPos_src];

                    // NOTE: alpha blending

                    //float alpha = color.a;

                    float alpha_dest = color.a * color_inv;
                    float alpha_src_inv = 1 - color_src.a * color_inv;
                    if (0 == color_src.a)
                        continue;

                    color.r = (byte)((color.r * alpha_src_inv) + (color_src.r * alpha_dest));
                    color.g = (byte)((color.g * alpha_src_inv) + (color_src.g * alpha_dest));
                    color.b = (byte)((color.b * alpha_src_inv) + (color_src.b * alpha_dest));

                    //Debug.Log("color_src = " + color_src + ", color_src.a =  " + color_src.a + ", a = " + alpha_src + ", ainv = " + alpha_src_inv + ", " + color);

                    colors[pixelPos] = color;
                }
            }
        }


        public static void SetPixels32_AlphaClip(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w)
        {
            float color_inv = 1 / 255f;

            int xEnd = x + w;
            int yEnd = y + h;

            for (int yPos = y, yPosSrc = src_y; yPos < yEnd; ++yPos, ++yPosSrc)
            {
                int yOffset = yPos * colorsWidth;
                int yOffset_src = yPosSrc * src_w;

                for (int xPos = x, xPosSrc = src_x; xPos < xEnd; ++xPos, ++xPosSrc)
                {
                    int pixelPos = yOffset + xPos;
                    int pixelPos_src = yOffset_src + xPosSrc;

                    Color32 color = colors[pixelPos];
                    Color32 color_src = colors_src[pixelPos_src];

                    float alpha_src = color_src.a * color_inv;

                    color.a = (byte)(color.a * alpha_src);

                    colors[pixelPos] = color;
                }
            }
        }

        //public static Color32[] SetPixels32_AlphaBlending_src32_dest32(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w, Color32[] colors_dest, int dest_x, int dest_y, int dest_w)
        //{
        //    float color_inv = 1 / 255f;

        //    int xEnd = x + w;
        //    int yEnd = y + h;

        //    for (int yPos = y, yPosSrc = src_y, yPosDst = dest_y; yPos < yEnd; ++yPos, ++yPosSrc, ++yPosDst)
        //    {
        //        int yOffset = yPos * colorsWidth;
        //        int yOffset_src = yPosSrc * src_w;
        //        int yOffset_dest = yPosDst * dest_w;

        //        for (int xPos = x, xPosSrc = src_x, xPosDst = dest_x; xPos < xEnd; ++xPos, ++xPosSrc, ++xPosDst)
        //        {
        //            int pixelPos = yOffset + xPos;
        //            int pixelPos_src = yOffset_src + xPosSrc;
        //            int pixelPos_dest = yOffset_dest + xPosDst;

        //            Color color_src = colors_src[pixelPos_src];
        //            Color color_dest = colors_dest[pixelPos_dest];
        //            Color color;

        //            // NOTE: alpha blending

        //            float alpha_src = color_src.a;

        //            float alpha_dest = color_dest.a * color_inv;
        //            float alpha_dest_inv = 1 - alpha_dest;

        //            color.r = (color_dest.r * alpha_dest) + ((color_src.r * alpha_src) * alpha_dest_inv);
        //            color.g = (color_dest.g * alpha_dest) + ((color_src.g * alpha_src) * alpha_dest_inv);
        //            color.b = (color_dest.b * alpha_dest) + ((color_src.b * alpha_src) * alpha_dest_inv);
        //            color.a = 255; // TODO:

        //            colors[pixelPos] = color;
        //        }
        //    }

        //    //texture.SetPixels32(colors);
        //    return colors;
        //}


        //public static Color32[] SetPixels32_AlphaBlendingFunc_src24_dest32(Color32[] colors, int colorsWidth, int x, int y, int w, int h, Color32[] colors_src, int src_x, int src_y, int src_w, Color32[] colors_dest, int dest_x, int dest_y, int dest_w)
        //{
        //    //Color32[] colors = texture.GetPixels32(0);
        //    //Color32[] colors = new Color32[w * h];

        //    float color_inv = 1 / 255f;

        //    for (int yPos = 0; yPos < h; ++yPos)
        //    {
        //        //int yOffset = yPos * texture.width;
        //        int yOffset = yPos * colorsWidth;
        //        int yOffset_src = yPos * src_w;
        //        int yOffset_dest = yPos * dest_w;

        //        for (int xPos = 0; xPos < w; ++xPos)
        //        {
        //            int pixelPos = yOffset + xPos;
        //            int pixelPos_src = yOffset_src + xPos;
        //            int pixelPos_dest = yOffset_dest + xPos;

        //            Color color_src = colors_src[pixelPos_src];
        //            Color color_dest = colors_dest[pixelPos_dest];
        //            Color color;

        //            // NOTE: alpha blending

        //            float alpha_dest = color_dest.a * color_inv;
        //            float alpha_dest_inv = 1 - alpha_dest;

        //            color.r = (color_dest.r * alpha_dest) + (color_src.r * alpha_dest_inv);
        //            color.g = (color_dest.g * alpha_dest) + (color_src.g * alpha_dest_inv);
        //            color.b = (color_dest.b * alpha_dest) + (color_src.b * alpha_dest_inv);
        //            color.a = 255; // TODO:

        //            colors[pixelPos] = color;
        //        }
        //    }

        //    //texture.SetPixels32(colors);
        //    return colors;
        //}


        public static void SetPixels32_Padding(Color32[] colors, int colorsWidth, int x, int y, int w, int h, int paddL, int paddT, int paddR, int paddB)
        {
            int xEnd = x + w;
            int yEnd = y + h;

            if (0 < paddL || 0 < paddR)
            {
                for (int yPos = y; yPos < yEnd; ++yPos)
                {
                    int yOffset = yPos * colorsWidth;

                    for (int xPos = x, count = 0; xPos < xEnd && count < paddL; ++xPos, ++count)
                    {
                        int pixelPos = yOffset + xPos;

                        Color32 color = colors[pixelPos];

                        color.a = 0;
                        colors[pixelPos] = color;
                    }

                    for (int xPos = (xEnd - 1), count = 0; xPos >= x && count < paddR; --xPos, ++count)
                    {
                        int pixelPos = yOffset + xPos;

                        Color32 color = colors[pixelPos];

                        color.a = 0;
                        colors[pixelPos] = color;
                    }
                }
            }
            if (0 < paddT)
            {
                for (int yPos = (yEnd - 1), count = 0; yPos >= y && count < paddT; --yPos, ++count)
                {
                    int yOffset = yPos * colorsWidth;

                    for (int xPos = x; xPos < xEnd; ++xPos)
                    {
                        int pixelPos = yOffset + xPos;

                        Color32 color = colors[pixelPos];

                        color.a = 0;
                        colors[pixelPos] = color;
                    }
                }
            }
            if (0 < paddB)
            {
                for (int yPos = 0, count = 0; yPos < yEnd && count < paddB; ++yPos, ++count)
                {
                    int yOffset = yPos * colorsWidth;

                    for (int xPos = x; xPos < xEnd; ++xPos)
                    {
                        int pixelPos = yOffset + xPos;

                        Color32 color = colors[pixelPos];

                        color.a = 0;
                        colors[pixelPos] = color;
                    }
                }
            }
        }
    }
}

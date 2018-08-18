using UnityEngine;
using System;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public static class ScreenHelper
    {
        public static float GetAspectRatio(Vector2 size)
        {
            return size.y / size.x;
        }

        public static Vector2 CalcResolutionWithAspectRatio(Vector2 reference, float aspectRatio, float matchWidthOrHeight)
        {
            var referenceRadio = ScreenHelper.GetAspectRatio(reference);

            return Vector2.Lerp
            (
                new Vector2
                {
                    x = reference.x,
                    y = reference.y / (referenceRadio / aspectRatio),
                },
                new Vector2
                {
                    x = reference.x / (aspectRatio / referenceRadio),
                    y = reference.y,
                },
                matchWidthOrHeight
            );
        }

        public static bool IsGreat(Vector2 reference, Vector2 size)
        {
            var scalarRef = reference.x * reference.y;
            var scalar = size.x * size.y;

            return scalarRef < scalar;
        }

        //public static bool IsGreatWithAspectRatio(Vector2 reference, Vector2 size, float matchWidthOrHeight)
        //{
        //    var scalarRef = Mathf.Lerp(reference.x, reference.y, matchWidthOrHeight);
        //    var scalar = Mathf.Lerp(size.x, size.y, matchWidthOrHeight);

        //    return scalarRef < scalar;
        //}
    }
}

using System;

using UnityEngine;

namespace Ext.Unity3D
{
    public static class Angle
    {
        public static float DirToAngle(this Vector2 dir)
        {
            return Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        }

        public static float ToPositiveAngle(float angle)
        {
            if (360 <= angle)
                return angle % 360;
            else if (0 > angle)
                return angle % 360 + 360;

            return angle;
        }

        //public static float To180Angle(float angle)
        //{
        //    if (180 < angle)
        //        return -(180 - (angle - 180));

        //    return angle;
        //}

        public static bool ContainsAngle(float min, float max, float value)
        {
            if (min < max)
            {
                return min <= value && value <= max;
            }
            else if (min > max)
            {
                return min <= value || value >= max;
            }
            else
            {
                return min == value;
            }
        }
    }
}

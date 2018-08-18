using UnityEngine;
using System;
using System.Runtime.InteropServices;

public static class MathHelper
{
    // NOTE: unity3d ver.
    public static float CalculateATS(float startTime, float manualTime, float nextProgressTarget, float nextProgressPrev)
    {
        // NOTE: 너무 빠르면, 느리게 가도록 제동을 걸어준다.
        float delta = Time.realtimeSinceStartup - startTime;
        float timeProgress = delta / manualTime;
        float rate = Mathf.Max(0, 1 - (timeProgress / nextProgressTarget));
        return Mathf.Lerp(nextProgressTarget, nextProgressPrev, rate);
    }


    // NOTE: standalone ver.
    public static float CalculateATS(float currentTime, float startTime, float manualTime, float nextProgressTarget, float nextProgressPrev)
    {
        // NOTE: 너무 빠르면, 느리게 가도록 제동을 걸어준다.
        float delta = currentTime - startTime;
        float timeProgress = delta / manualTime;
        float t = 1 - (timeProgress / nextProgressTarget);
        float rate = 0 < t ? t : 0;
        return nextProgressTarget + ((nextProgressPrev - nextProgressTarget) * rate);
    }


    // NOTE: 고정 프레임 이동 함수
    public static Vector2 Translate4Fixed(Vector2 cur, Vector2 dest, float power, bool clamp = true)
    {
        if (dest == cur)
            return dest;

        // 목표물이 움직일 때 유도탄 효과 등이 필요할 수도 있어서 걍 매 프레임 위치를 다시 세팅하자. (벡터 연산 + sqrt 연산 등이 사용될거라 부하는 크게 없다)
        var distance = dest - cur;
        var length = distance.sqrMagnitude;

        if (clamp && length <= power)
            return dest;// NOTE: 관통하면 안되기에

        // NOTE: 귀찮아서 그냥 사칙 연산 때려박음. 관통 이슈가 있어서...
        var next = cur + distance.normalized * power;

        if (cur.x < dest.x ? next.x > dest.x : next.x < dest.x &&
            cur.y < dest.y ? next.y > dest.y : next.y < dest.y)
            next = dest;

        return next;
    }



    // NOTE: 가변 프레임 이동함수
    public static Vector2 Translate(Vector2 cur, Vector2 dest, float progress, bool clamp = true)
    {
        if (dest == cur)
            return dest;

        if (clamp)
            progress = Mathf.Clamp01(progress);// NOTE: 관통하면 안되기에

        return Vector2.Lerp(cur, dest, progress);
    }
    

    public static int Mod(int x, int m)
    {
        var r = x % m;
        return r < 0 ? r + m : r;
    }
    public static long Mod(long x, long m)
    {
        var r = x % m;
        return r < 0 ? r + m : r;
    }
}
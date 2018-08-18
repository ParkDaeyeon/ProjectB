using UnityEngine;

namespace Ext.Unity3D
{
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static class RectTransformExtension
    {
        public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
        {
            source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

            switch (allign)
            {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            }
        }

        public static void SetPivot(this RectTransform source, PivotPresets preset)
        {

            switch (preset)
            {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
            }
        }


        public static RectTransform Create(string name,
                                           Transform parent,
                                           AnchorPresets anchor,
                                           PivotPresets pivot,
                                           Vector2 size,
                                           Vector2 position)
        {
            var go = new GameObject(name);
            var t = go.AddComponent<RectTransform>();
            t.SetParent(parent);
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.SetAnchor(AnchorPresets.HorStretchTop);
            t.SetPivot(PivotPresets.TopCenter);
            t.sizeDelta = size;
            t.anchoredPosition = position;
            return t;
        }


        public static void SetDefaultScale(this RectTransform trans)
        {
            trans.localScale = new Vector3(1, 1, 1);
        }
        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static Vector2 GetSize(this RectTransform trans)
        {
            if (!trans)
                return Vector2.zero;

            return trans.rect.size;
        }
        public static float GetWidth(this RectTransform trans)
        {
            if (!trans)
                return 0;

            return trans.rect.width;
        }
        public static float GetHeight(this RectTransform trans)
        {
            if (!trans)
                return 0;

            return trans.rect.height;
        }

        public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
        }

        public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            trans.localPosition = new Vector3(newPos.x + (pivot.x * rect.width), newPos.y + (pivot.y * rect.height), trans.localPosition.z);
        }
        public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            trans.localPosition = new Vector3(newPos.x + (pivot.x * rect.width), newPos.y - ((1f - pivot.y) * rect.height), trans.localPosition.z);
        }
        public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            trans.localPosition = new Vector3(newPos.x - ((1f - pivot.x) * rect.width), newPos.y + (pivot.y * rect.height), trans.localPosition.z);
        }
        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            trans.localPosition = new Vector3(newPos.x - ((1f - pivot.x) * rect.width), newPos.y - ((1f - pivot.y) * rect.height), trans.localPosition.z);
        }
        public static void SetLeftPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            float hh = rect.height * 0.5f;
            trans.localPosition = new Vector3(newPos.x + (pivot.x * rect.width), newPos.y + Mathf.Lerp(-hh, hh, pivot.y), trans.localPosition.z);
        }
        public static void SetLeftPosition(this RectTransform trans, float newPos)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            trans.localPosition = new Vector3(newPos + (trans.pivot.x * trans.rect.width), pos.y, pos.z);
        }
        public static void SetTopPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            float wh = rect.width * 0.5f;
            trans.localPosition = new Vector3(newPos.x + Mathf.Lerp(-wh, wh, pivot.x), newPos.y - ((1f - pivot.y) * rect.height), trans.localPosition.z);
        }
        public static void SetTopPosition(this RectTransform trans, float newPos)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            trans.localPosition = new Vector3(pos.x, newPos - ((1f - trans.pivot.y) * trans.rect.height), pos.z);
        }
        public static void SetRightPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            float hh = rect.height * 0.5f;
            trans.localPosition = new Vector3(newPos.x - ((1f - pivot.x) * rect.width), newPos.y + Mathf.Lerp(-hh, hh, pivot.y), trans.localPosition.z);
        }
        public static void SetRightPosition(this RectTransform trans, float newPos)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            trans.localPosition = new Vector3(newPos - ((1f - trans.pivot.x) * trans.rect.width), pos.y, pos.z);
        }
        public static void SetBottomPosition(this RectTransform trans, Vector2 newPos)
        {
            if (!trans)
                return;

            Rect rect = trans.rect;
            Vector2 pivot = trans.pivot;
            float wh = rect.width * 0.5f;
            trans.localPosition = new Vector3(newPos.x + Mathf.Lerp(-wh, wh, pivot.x), newPos.y + (pivot.y * rect.height), trans.localPosition.z);
        }
        public static void SetBottomPosition(this RectTransform trans, float newPos)
        {
            if (!trans)
                return;

            Vector3 pos = trans.localPosition;
            trans.localPosition = new Vector3(pos.x, newPos + (trans.pivot.y * trans.rect.height), pos.z);
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            if (!trans)
                return;

            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin -= new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax += new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }
        public static void SetWidth(this RectTransform trans, float newSize)
        {
            if (!trans)
                return;

            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }
        public static void SetHeight(this RectTransform trans, float newSize)
        {
            if (!trans)
                return;

            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }


        public static void SetSizeWithEdge(this RectTransform trans,
                                           RectTransform.Edge edge,
                                           float size,
                                           Vector2 originOffsetMax,
                                           Vector2 originOffsetMin,
                                           Vector2 originPosition)
        {
            switch (edge)
            {
            case RectTransform.Edge.Bottom:
                trans.offsetMin = new Vector2(trans.offsetMin.x, (originOffsetMin.y - size) + originPosition.y);
                break;
            case RectTransform.Edge.Left:
                trans.offsetMin = new Vector2((originOffsetMin.x - size) + originPosition.x, trans.offsetMin.y);
                break;


            case RectTransform.Edge.Top:
                trans.offsetMax = new Vector2(trans.offsetMax.x, (originOffsetMax.y + size) + originPosition.y);
                break;
            case RectTransform.Edge.Right:
                trans.offsetMax = new Vector2((originOffsetMax.x + size) + originPosition.x, trans.offsetMax.y);
                break;
            }
        }

        public static void GetOffsetWithPivot(this RectTransform trans, out Vector2 offsetMax, out Vector2 offsetMin)
        {
            offsetMax = new Vector2(trans.sizeDelta.x * (1 - trans.pivot.x), trans.sizeDelta.y * (1 - trans.pivot.y));
            offsetMin = new Vector2(trans.sizeDelta.x * trans.pivot.x, trans.sizeDelta.y * trans.pivot.y) * -1;
        }

        
        public struct CenterPositionAB
        {
            public float posA;
            public float posB;
        }
        public static CenterPositionAB CalculateCenterLeftRight(RectTransform left, RectTransform right, float leftPadding = 0)
        {
            CenterPositionAB cen = default(CenterPositionAB);

            float sizeA = left.rect.width;
            float sizeB = right.rect.width;
            float scalar = sizeA + leftPadding + sizeB;
            cen.posA = cen.posB = -(scalar) * 0.5f + sizeA;
            cen.posB += leftPadding;

            //Debug.Log(cen.posA + ", " + cen.posB + ", " + leftPadding + ", A:" + sizeA + ", B:" + sizeB + ", SCA:" + scalar);

            return cen;
        }
    }
}

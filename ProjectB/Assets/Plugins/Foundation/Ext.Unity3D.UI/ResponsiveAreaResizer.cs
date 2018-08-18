using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ext.Unity3D.UI
{
    public class ResponsiveAreaResizer : Responsive
    {
        [SerializeField]
        RectOffset normalizedOffset;

        public enum CalibrateCenter
        {
            None,
            Horizontal,
            Vertical,
            Both,
        }
        [SerializeField]
        CalibrateCenter calibrateCenter;
        public void SetCalibrateCenter(CalibrateCenter value) { this.calibrateCenter = value; }
        public CalibrateCenter GetCalibrateCenter() { return this.calibrateCenter; }

        [SerializeField]
        bool calibrateOverfit;
        public void SetCalibrateOverfit(bool value) { this.calibrateOverfit = value; }
        public bool GetCalibrateOverfit() { return this.calibrateOverfit; }

        Vector2 calibratedValue;
        public Vector2 CalibratedPosition { get { return this.calibratedValue; } }

        [SerializeField]
        Vector2 originPosition;
        public Vector2 OriginPosition
        {
            set { this.originPosition = value; }
            get { return this.originPosition; }
        }
        [SerializeField]
        Vector2 originSize;
        public Vector2 OriginSize
        {
            set { this.originSize = value; }
            get { return this.originSize; }
        }

        [SerializeField]
        Vector2 originOffsetMax;
        [SerializeField]
        Vector2 originOffsetMin;
        
        Action onResizePostProcessListener;
        public Action OnResizePostProcessListener
        {
            set { this.onResizePostProcessListener = value; }
            get { return this.onResizePostProcessListener; }
        }

        protected override void OnResize()
        {
            this.UpdateSize();

            if (null != this.onResizePostProcessListener)
                this.onResizePostProcessListener();
        }

        public override int Order
        {
            get { return 7; }
        }

        public float GetContentScale()
        {
            var areaSize = Responsive.GetAreaSizeByMode(AreaMode.Safe);
            var baseSize = Responsive.GetAreaSizeByMode(AreaMode.Viewport);
            var scale = UnityExtension.GetScaleOfAreaSize(areaSize, baseSize);
            return scale;
        }

        [SerializeField]
        bool ignoreContentTrasnformLeft;
        [SerializeField]
        bool ignoreContentTrasnformRight;
        [SerializeField]
        bool ignoreContentTrasnformTop;
        [SerializeField]
        bool ignoreContentTrasnformBottom;

        [SerializeField]
        bool useSizeLimit = false;
        [SerializeField]
        Vector2 minSize = Vector2.zero;
        [SerializeField]
        Vector2 maxSize = new Vector2(float.MaxValue, float.MaxValue);


        float rightBlankValue;
        float leftBlankValue;
        float topBlankValue;
        float bottomBlankValue;
        public void BlankValue(out float right, out float left, out float top, out float bottom)
        {
            left = this.leftBlankValue;
            right = this.rightBlankValue;
            top = this.topBlankValue;
            bottom = this.bottomBlankValue;
        }

        bool isFirst = true;
        public void UpdateSize()
        {
            // TODO: must be remove after whole components serialize it
            if (this.isFirst)
            {
                this.isFirst = false;
                this.ResetOriginOffsets();
            }

            var position = this.originPosition;
            var size     = this.originSize;

            var scaledScreenSize = Responsive.GetAreaSizeByMode(AreaMode.Screen);

            var baseArea = Responsive.GetAreaRectByMode(AreaMode.Safe);
            var baseContentSize = Responsive.GetAreaSizeByMode(AreaMode.Viewport);

            var contentScale = UnityExtension.GetScaleOfAreaSize(baseArea.size, baseContentSize);
            var scaleFactor = 1f / contentScale;

            var baseAreaRect = baseArea;
            var baseRectSize = new Vector2(baseContentSize.x * contentScale,
                                           baseContentSize.y * contentScale);
            var baseAreaRectSize = new Vector2(baseAreaRect.size.x, baseAreaRect.size.y);

            var actualContentSize = baseRectSize;
            var actualSafeAreaSize = baseAreaRectSize;

            var areaPosition = baseArea.position;
            var halfArea = scaledScreenSize * 0.5f;
            var halfActualContentSize = actualContentSize * 0.5f;
            var halfActualSafeAreaSize = actualSafeAreaSize * 0.5f;
            this.rightBlankValue =      Mathf.CeilToInt(scaledScreenSize.x - (halfArea.x + (this.ignoreContentTrasnformRight ? halfActualSafeAreaSize.x : halfActualContentSize.x)) - areaPosition.x);
            this.leftBlankValue =                             Mathf.CeilToInt(halfArea.x - (this.ignoreContentTrasnformLeft ? halfActualSafeAreaSize.x : halfActualContentSize.x) + areaPosition.x);
            this.topBlankValue =        Mathf.CeilToInt(scaledScreenSize.y - (halfArea.y + (this.ignoreContentTrasnformTop ? halfActualSafeAreaSize.y : halfActualContentSize.y)) - areaPosition.y);
            this.bottomBlankValue =                           Mathf.CeilToInt(halfArea.y - (this.ignoreContentTrasnformBottom ? halfActualSafeAreaSize.y : halfActualContentSize.y) + areaPosition.y);

            var leftSize = this.normalizedOffset.left * this.leftBlankValue;
            var rightSize = this.normalizedOffset.right * this.rightBlankValue;
            var topSize = this.normalizedOffset.top * this.topBlankValue;
            var bottomSize = this.normalizedOffset.bottom * this.bottomBlankValue;


            this.resultLeft = leftSize * scaleFactor;
            this.resultRight = rightSize * scaleFactor;
            this.resultTop = topSize * scaleFactor;
            this.resultBottom = bottomSize * scaleFactor;

            if (this.useSizeLimit)
            {
                var blankMin = (this.minSize - size) * 0.5f;
                var blankMax = (this.maxSize - size) * 0.5f;

                this.resultLeft = Mathf.Clamp(resultLeft, blankMin.x, blankMax.x);
                this.resultRight = Mathf.Clamp(resultRight, blankMin.x, blankMax.x);
                this.resultTop = Mathf.Clamp(resultTop, blankMin.y, blankMax.y);
                this.resultBottom = Mathf.Clamp(resultBottom, blankMin.y, blankMax.y);
            }

            var trans = this.CachedRectTransform;

            //TODO : Remove
#if LOG_DEBUG
            if (null == trans)
                Debug.LogError("rt null");
#endif// LOG_DEBUG

            this.SetSizeWithEdge(trans, RectTransform.Edge.Left, this.resultLeft);
            this.SetSizeWithEdge(trans, RectTransform.Edge.Right, this.resultRight);
            this.SetSizeWithEdge(trans, RectTransform.Edge.Top, this.resultTop);
            this.SetSizeWithEdge(trans, RectTransform.Edge.Bottom, this.resultBottom);

#if LOG_DEBUG
            if (this.calibrateCenter > CalibrateCenter.Both)
                Debug.LogError("Invalid CalibrateCenter");

#endif// LOG_DEBUG

            var anchorMax = trans.anchorMax;
            var anchorMin = trans.anchorMin;
            switch (this.calibrateCenter)
            {
            case CalibrateCenter.Horizontal:
                this.CalibratePositionX(trans, areaPosition, position, anchorMax, anchorMin, scaleFactor);
                break;
            case CalibrateCenter.Vertical:
                this.CalibratePositionY(trans, areaPosition, position, anchorMax, anchorMin, scaleFactor);
                break;
            case CalibrateCenter.Both:
                this.CalibratePositionXY(trans, areaPosition, position, anchorMax, anchorMin, scaleFactor);
                break;
            }
        }

        void SetSizeWithEdge(RectTransform target, RectTransform.Edge edge, float size)
        {
            target.SetSizeWithEdge(edge,
                                   float.IsNaN(size) ? 0 : size,
                                   this.originOffsetMax,
                                   this.originOffsetMin,
                                   this.originPosition);
        }




        float resultLeft = 0f;
        float resultRight = 0f;
        float resultTop = 0f;
        float resultBottom = 0f;

        void CalibratePositionX(RectTransform targetRectTransform, Vector2 areaPosition, Vector2 position, Vector2 anchorMax, Vector2 anchorMin, float scaleFactor)
        {
            var calibrated =
            this.calibratedValue = new Vector2(areaPosition.x * scaleFactor,
                                               position.y/* * scaleFactor.y*/);

            if (this.calibrateOverfit)
            {
                var overfitSize = Mathf.Max(this.resultLeft, this.resultRight);
                this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Left, overfitSize);
                this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Right, overfitSize);
            }
            else
            {
                if (anchorMax.x != anchorMin.x)
                    calibrated.x = -calibrated.x;

                targetRectTransform.anchoredPosition = calibrated;
            }
        }

        void CalibratePositionY(RectTransform targetRectTransform, Vector2 areaPosition, Vector2 position, Vector2 anchorMax, Vector2 anchorMin, float scaleFactor)
        {
            var calibrated =
            this.calibratedValue = new Vector2(position.x/* * scaleFactor.x*/,
                                               areaPosition.y * scaleFactor);

            if (this.calibrateOverfit)
            {
                var overfitSize = Mathf.Max(this.resultTop, this.resultBottom);
                this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Top, overfitSize);
                this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Bottom, overfitSize);
            }
            else
            {
                if (anchorMax.y == anchorMin.y)
                    calibrated.y = -calibrated.y;

                targetRectTransform.anchoredPosition = calibrated;
            }
        }

        void CalibratePositionXY(RectTransform targetRectTransform, Vector2 areaPosition, Vector2 position, Vector2 anchorMax, Vector2 anchorMin, float scaleFactor)
        {
            var calibrated =
            this.calibratedValue = new Vector2(areaPosition.x * scaleFactor,
                                               areaPosition.y * scaleFactor);

            if (this.calibrateOverfit)
            {
                {
                    var overfitSize = Mathf.Max(this.resultLeft, this.resultRight);
                    this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Left, overfitSize);
                    this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Right, overfitSize);
                }

                {
                    var overfitSize = Mathf.Max(this.resultTop, this.resultBottom);
                    this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Top, overfitSize);
                    this.SetSizeWithEdge(targetRectTransform, RectTransform.Edge.Bottom, overfitSize);
                }
            }
            else
            {
                if (anchorMax.x == anchorMin.x)
                    calibrated.x = -calibrated.x;

                if (anchorMax.y == anchorMin.y)
                    calibrated.y = -calibrated.y;

                targetRectTransform.anchoredPosition = calibrated;
            }
        }
        public void ResetOriginValues()
        {
            var trans = this.CachedRectTransform;
            this.originPosition = trans.anchoredPosition;
            this.originSize = trans.rect.size;

            this.ResetOriginOffsets();
        }

        public void ResetOriginOffsets()
        {
            var trans = this.CachedRectTransform;
            trans.GetOffsetWithPivot(out this.originOffsetMax, out this.originOffsetMin);
        }

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild = false;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            if (this.editorRebuild)
            {
                this.editorRebuild = false;

                this.ResetOriginValues();
            }
        }
#endif// UNITY_EDITOR
    }
}

using Ext.Unity3D.UI;
using UnityEngine;

namespace Ext.Unity3D.UI
{
    //TODO : merge with ResponsiveArea
    public class ResponsiveSafeArea : Responsive
    {
        protected override void OnResize()
        {
            var baseSize = Responsive.GetAreaSizeByMode(AreaMode.Viewport);
            var rect = this.CalcWithScreen(UnityExtension.ScreenSafeAreaRect);
            this.UpdateSafeArea(rect, baseSize);
        }

        public override int Order
        {
            get
            {
                return 0;
            }
        }

        [SerializeField]
        bool setActualSize = false;
        public bool SetActualSize
        {
            set { this.setActualSize = value; }
            get { return this.setActualSize; }
        }
        void UpdateSafeArea(Rect safeAreaRect, Vector2 baseSize)
        {
            var trans = this.CachedRectTransform;
            var pos = safeAreaRect.position;
            var size = safeAreaRect.size;
            
            var scale = UnityExtension.GetScaleOfAreaSize(size, baseSize);
            if (float.IsNaN(scale))
                scale = 1f;

            if (this.setActualSize)
            {
                trans.sizeDelta = size / (0f == scale ? 1f : scale);
                trans.localScale = new Vector3(1f, 1f, trans.localScale.z);
            }
            else
            {
                trans.anchoredPosition = pos;
                trans.sizeDelta = size;
                trans.localScale = new Vector3(scale, scale, trans.localScale.z);
            }
        }
        
        Rect CalcWithScreen(Rect safeAreaRect)
        {
            return UnityExtension.GetCalculatedSafeAreaRect(safeAreaRect, Responsive.GetAreaSizeByMode(AreaMode.Viewport));
        }
        protected override void OnAwake()
        {
            base.OnAwake();

#if UNITY_EDITOR
            if (this.editorTestOnAwake)
                this.EditorTesting();
#endif// UNITY_EDITOR
        }

#if UNITY_EDITOR
        [SerializeField]
        Rect editorTestRect;
        [SerializeField]
        bool editorTestWithScreen;        
        [SerializeField]
        bool editorTestOnAwake = false;

        protected override void OnEditorTesting()
        {
            //base.OnEditorTesting();

            var rect = this.editorTestRect;
            if (this.editorTestWithScreen)
                rect = this.CalcWithScreen(rect);
            this.UpdateSafeArea(rect, this.editorTestViewportAreaSize);
        }

        [SerializeField]
        bool editorDrawDummyAreaGizmo = false;
        [SerializeField]
        Color editorDrawDummyAreaColor = Color.yellow;
        protected override void OnEditorUpdateAndDrawGizmos()
        {
            base.OnEditorUpdateAndDrawGizmos();

            if (this.editorDrawDummyAreaGizmo)
            {
                var trans = this.CachedRectTransform;
                if (!trans)
                    return;

                Gizmos.matrix = trans.localToWorldMatrix;
                Gizmos.color = this.editorDrawDummyAreaColor;

                var rect = trans.rect;
                Gizmos.DrawWireCube(rect.center, rect.size / trans.localScale.x);
            }
        }
#endif// UNITY_EDITOR
    }
}

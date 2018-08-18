using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/SoftMask")]
    public class SoftMask : Responsive
    {
        [Serializable]
        public class ShaderData
        {
            public ShaderData(Shader originShader, Shader maskShader)
            {
                this.originShader = originShader;
                this.maskShader = maskShader;
            }

            [SerializeField]
            Shader originShader;
            public Shader OriginShader { get { return this.originShader; } }

            [SerializeField]
            Shader maskShader;
            public Shader MaskShader { get { return this.maskShader; } }

            Material material;
            public Material Material { get { return this.material; } }
            public bool HasMaterial { get { return this.material; } }

            public Material CreateMaterial()
            {
                if (!this.material)
                    this.material = new Material(this.maskShader);

                return this.material;
            }

            public void DestroyMaterial()
            {
                if (this.material)
                {
                    this.material.Destroy();
                    this.material = null;
                }
            }
        }



        [SerializeField]
        List<ShaderData> shaderDatas;
        public List<ShaderData> ShaderDatas { get { return this.shaderDatas; } }


        [SerializeField]
        Vector2 clipSharpness = new Vector2(20, 20);
        public Vector2 ClipSoftness { set { this.clipSharpness = value; } get { return this.clipSharpness; } }


        [SerializeField]
        List<SoftMaskElements> elements = new List<SoftMaskElements>();
        public List<SoftMaskElements> Elements { get { return this.elements; } }
        public bool AddElements(SoftMaskElements maskElements)
        {
            if (!maskElements)
                return false;

            if (this.elements.Contains(maskElements))
                return false;

            this.elements.Add(maskElements);
            return true;
        }

        bool ContainsOriginShader(Shader originShader)
        {
            var datas = this.shaderDatas;
            for (int n = 0, cnt = datas.Count; n < cnt; ++n)
            {
                var data = datas[n];
                if (originShader == data.OriginShader)
                    return true;
            }

            return false;
        }

        public Material GetMaterialByOriginShader(Shader originShader)
        {
            for (int n = 0, cnt = this.shaderDatas.Count; n < cnt; ++n)
            {
                var data = this.shaderDatas[n];
                if (originShader == data.OriginShader)
                    return data.Material;
            }

            return null;
        }


        protected override void OnAwake()
        {
            base.OnAwake();

            if (!this.initialized)
                this.Setup();
        }

        [SerializeField]
        bool updateMaterialsOnEnable;
        public bool UpdateMaterialsOnEnable
        {
            set { this.updateMaterialsOnEnable = value; }
            get { return this.updateMaterialsOnEnable; }
        }

        void OnEnable()
        {
            if (!this.targetTransform)
                return;

            this.UpdateCanvas(false);

            if (this.updateMaterialsOnEnable)
                this.UpdateMaterials();
        }

        void OnTransformParentChanged()
        {
            this.UpdateCanvas(true);
        }
        
        [SerializeField]
        bool updateMaterialsOnResume = true;
        public bool UpdateMaterialsOnResume
        {
            set { this.updateMaterialsOnResume = value; }
            get { return this.updateMaterialsOnResume; }
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
                return;

            this.OnEnable();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
                return;

            this.OnEnable();
        }

        void UpdateCanvas(bool force)
        {
            if (!this.canvas || force)
                this.canvas = this.GetParentCanvas();
        }

        void OnDestroy()
        {
            this.DestroyMaterials();
        }

        bool initialized = false;
        int _clipRange;
        int _clipSharpness;
        public virtual void Setup()
        {
            this.initialized = true;
            this._clipRange = Shader.PropertyToID("_ClipRange");
            this._clipSharpness = Shader.PropertyToID("_ClipSharpness");
        }

        [SerializeField]
        Canvas canvas;
        public Canvas Canvas
        {
            set { this.canvas = value; }
            get { return this.canvas; }
        }

        [SerializeField]
        RectTransform targetTransform;
        public RectTransform TargetTransform
        {
            set { this.targetTransform = value; }
            get { return this.targetTransform; }
        }

        public static Rect GetViewportRect(RectTransform trans, Canvas canvas)
        {
            if (!trans)
                return Rect.zero;

            var rect = trans.rect;
            var size = rect.size;
            var offset = Vector2.Scale(size, trans.pivot);

            var posMin = -offset;
            var posMax = size - offset;

            // TODO: Not supported rotation!
            var camera = canvas ? canvas.worldCamera : null;
            if (!camera)
                camera = Camera.main;
            var min = (Vector2)camera.WorldToViewportPoint(trans.TransformPoint(posMin));
            var max = (Vector2)camera.WorldToViewportPoint(trans.TransformPoint(posMax));

            var resolution = Responsive.GetAreaSizeByMode(AreaMode.Screen);
            var resolutionHalf = resolution * 0.5f;
            var viewportMin = new Vector2(min.x * resolution.x - resolutionHalf.x,
                                          min.y * resolution.y - resolutionHalf.y);
            var viewportMax = new Vector2(max.x * resolution.x - resolutionHalf.x,
                                          max.y * resolution.y - resolutionHalf.y);

            var viewportRect = Rect.MinMaxRect(viewportMin.x, viewportMin.y, viewportMax.x, viewportMax.y);
#if LOG_SOFTMASK
            Debug.Log(new System.Text.StringBuilder().Append("SOFTMASK:GETRECT:")
            .Append("MIN:").Append("(X:").Append(min.x).Append(", Y:").Append(min.y).Append("), ")
            .Append("MAX:").Append("(X:").Append(max.x).Append(", Y:").Append(max.y).Append("),\n")
            //.Append("OFFMIN:").Append(offMin).Append(", ")
            //.Append("OFFMAX:").Append(offMax).Append(", ")
            .Append("VP_MIN:").Append(viewportMin).Append(", ")
            .Append("VP_MAX:").Append(viewportMax).Append(", ")
            .Append("SIZE:").Append(size).Append(", ")
            .Append("RESOLUTION:").Append(resolution).Append(",\n")
            .Append("RC:").Append(rect).Append(", ")
            .Append("VP_RC:").Append(viewportRect).Append(",\n")
            .ToString());
#endif// LOG_SOFTMASK
            return viewportRect;
        }
        
        public static Vector2 GetScale(RectTransform trans, Canvas canvas)
        {
            if (!trans)
                return Vector2.zero;
            
            var scale = trans.lossyScale;
            if (canvas)
            {
                var root = canvas.rootCanvas;
                var baseScale = root.transform.localScale;
                scale.x /= baseScale.x;
                scale.y /= baseScale.y;
                scale *= root.scaleFactor;
            }

            return scale;
        }
        
        public void UpdateMaterials()
        {
            if (!this.initialized)
                this.Setup();

            this.OnApplySoftness();

            for (int n = 0, cnt = this.elements.Count; n < cnt; ++n)
            {
                var elements = this.elements[n];
                if (!elements)
                    continue;

                elements.Mask = this;
                elements.Sync();
            }
        }

        protected virtual void OnApplySoftness()
        {
            var scale = SoftMask.GetScale(this.targetTransform, this.canvas);
            var viewportRect = SoftMask.GetViewportRect(this.targetTransform, this.canvas);
            var size = viewportRect.size;
            var sizeHalf = size * 0.5f;
            var center = viewportRect.center;
            var clipRange = new Vector4(1 / sizeHalf.x, 1 / sizeHalf.y, -center.x / sizeHalf.x, -center.y / sizeHalf.y);

            var scaledClipRange = clipRange;

#if LOG_SOFTMASK
            Debug.Log(new System.Text.StringBuilder().Append("SOFTMASK:UPDATE_MATERIALS:")
            .Append("VP_RC:").Append(viewportRect).Append(", ");sb.Append("")
            .Append("SIZE_HALF:").Append(sizeHalf).Append(", ");sb.Append("")
            .Append("CENTER:").Append(center).Append(", ");sb.Append("")
            .Append("CLIP_RANGE:{")
            .Append("X:").Append(clipRange.x).Append(", ");sb.Append("")
            .Append("Y:").Append(clipRange.y).Append(", ");sb.Append("")
            .Append("Z:").Append(clipRange.z).Append(", ");sb.Append("")
            .Append("W:").Append(clipRange.w).Append(", ");sb.Append("")
            .Append("}, ")
            .Append("SCALE:").Append(scale).Append(", ");sb.Append("")
            .Append("SCALED_CLIP_RANGE:{")
            .Append("X:").Append(scaledClipRange.x).Append(", ");sb.Append("")
            .Append("Y:").Append(scaledClipRange.y).Append(", ");sb.Append("")
            .Append("Z:").Append(scaledClipRange.z).Append(", ");sb.Append("")
            .Append("W:").Append(scaledClipRange.w).Append(", ");sb.Append("")
            .Append("}"));
#endif// LOG_SOFTMASK

            var sharp = this.clipSharpness;
            sharp.x *= scale.x;
            sharp.y *= scale.y;

            if (0 < sharp.x) sharp.x = sizeHalf.x / sharp.x;
            if (0 < sharp.y) sharp.y = sizeHalf.y / sharp.y;

            for (int n = 0, cnt = this.shaderDatas.Count; n < cnt; ++n)
            {
                var data = this.shaderDatas[n];
                var material = data.CreateMaterial();
                material.SetVector(this._clipRange, scaledClipRange);
                material.SetVector(this._clipSharpness, sharp);
            }
        }

        void DestroyMaterials()
        {
            for (int n = 0, cnt = this.elements.Count; n < cnt; ++n)
            {
                var elements = this.elements[n];
                if (!elements)
                    continue;

                elements.Rollback();
            }

            for (int n = 0, cnt = this.shaderDatas.Count; n < cnt; ++n)
                this.shaderDatas[n].DestroyMaterial();
        }

        public void RemoveDestroyedAllTargets()
        {
            for (int n = this.elements.Count - 1; n >= 0; --n)
            {
                if (!this.elements[n])
                    this.elements.RemoveAt(n);
            }
        }

        protected override void OnResize()
        {
            this.UpdateMaterials();
        }

        public override int Order
        {
            get { return 10; }
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
                foreach (var elements in this.elements)
                    elements.EditorSetting();
            }

            foreach (var elements in this.elements)
                elements.Rollback();
        }

        [SerializeField]
        bool editorDrawGizmo = true;
        protected override void OnEditorUpdateAndDrawGizmos()
        {
            base.OnEditorUpdateAndDrawGizmos();

            if (this.editorDrawGizmo)
            {
                var trans = this.targetTransform;
                if (!trans)
                    return;

                Gizmos.matrix = trans.localToWorldMatrix;
                Gizmos.color = Color.red;

                var rect = trans.rect;
                Gizmos.DrawWireCube(rect.center, rect.size);
            }
        }
        

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.UpdateMaterials();
        }
#endif// UNITY_EDITOR
    }
}

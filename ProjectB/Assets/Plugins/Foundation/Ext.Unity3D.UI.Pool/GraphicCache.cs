using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class GraphicCache : _2DCache
    {
        [SerializeField]
        public GraphicPool GPool { get { return this.Pool as GraphicPool; } }

        [SerializeField]
        Graphic graphic;
        public Graphic Graphic { get { return this.graphic; } }

    

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.graphic)
                this.graphic = this.FindComponent<Graphic>();
        }

        public void EditorSetGraphic(Graphic value)
        {
            this.graphic = value;
        }
#endif// UNITY_EDITOR
    }
}
using UnityEngine;
using UnityEngine.UI;
namespace Ext.Unity3D.UI.Pool
{
    public class QuadPolyCache : GraphicCache
    {
        public OptimizedQuadPolyImage Poly { get { return this.Graphic as OptimizedQuadPolyImage; } }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
        }
#endif// UNITY_EDITOR
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class ResponsiveVisible : Responsive
    {
        public override int Order
        {
            get { return 1; }
        }

        protected override void OnResize()
        {
            this.ResetState();
        }
        
        [SerializeField]
        float startAspectRatio = 0;
        [SerializeField]
        float endAspectRatio = 99;

        public bool ContainsAspectRatio(float aspectRatio)
        {
            return this.startAspectRatio <= aspectRatio && aspectRatio <= this.endAspectRatio;
        }
        
        public void ResetState()
        {
            var viewSize = this.GetAreaSizeByCurrent();

            var viewRatio = viewSize.x / viewSize.y;
            var enabled = this.ContainsAspectRatio(viewRatio);
            
            this.SetActive(enabled);
        }
    }
}

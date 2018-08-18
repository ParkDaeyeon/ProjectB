using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ext.Unity3D.UI
{
    public class MultipleColorFxButtonLinker : ButtonLinker<MultipleColorFxButton, Graphic>
    {
        [SerializeField]
        List<Graphic> fxFollowers;
        public List<Graphic> FxFollowers
        {
            set { this.fxFollowers = value; }
            get { return this.fxFollowers; }
        }

        public override void ApplyFollowers()
        {
            base.ApplyFollowers();

            if (!this.target)
                return;

            this.target.FollowersRef = this.followers;
            this.target.FxFollowersRef = this.fxFollowers;
        }
        public override void DisableFollowers()
        {
            base.DisableFollowers();

            if (!this.target)
                return;

            this.target.FollowersRef = null;
            this.target.FxFollowersRef = null;
        }

#if UNITY_EDITOR
        protected override void EditorCollectFollowers()
        {
            base.EditorCollectFollowers();

            this.fxFollowers.Clear();

            var followers = this.followers;

            for (int n = 0, cnt = followers.Count; n < cnt; ++n)
            {
                var item = followers[n];
                var image = item as Image;
                if (image != null)
                {
                    var material = image.material;
                    // NOTE: hardcoded as resource name
                    if (material != null && material.name == "UI-Additive")
                    {
                        this.fxFollowers.Add(image);
                    }
                }
            }
        }
#if LOG_DEBUG
        protected override void EditorDebugInit()
        {
            base.EditorDebugInit();

        }

        protected override StringBuilder EditorDebugGetLog()
        {
            var sb = base.EditorDebugGetLog();

            sb.AppendLine("");

            return sb;
        }

        string EditorDebugGetFxButtonLinkerStateLog()
        {
            var sb = new StringBuilder();

            if (null == this.fxFollowers)
                this.EditorDebugIsError = true;

            var fxFollowersResult = new StringBuilder();

            if (null != this.fxFollowers)
            {
                for (int n = 0, cnt = this.fxFollowers.Count; n < cnt; ++n)
                {
                    var f = this.fxFollowers[n];

                    if (null == f)
                        this.EditorDebugIsError = true;

                    var fResult = null != f ? f.name : "null";

                    fxFollowersResult.AppendLine("index :" + n + "\t name :" + fResult);
                }
            }

            sb.AppendLine("Fx Followers is\t" + "\n{\n" + fxFollowersResult.ToString() + "\n}\n");

            return sb.ToString();
        }
#endif// LOG_DEBUG

#endif// UNITY_EDITOR
    }
}

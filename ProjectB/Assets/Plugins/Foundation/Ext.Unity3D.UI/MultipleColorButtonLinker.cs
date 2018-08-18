using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ext.Unity3D.UI
{
    public class MultipleColorButtonLinker : ButtonLinker<MultipleColorButton, Graphic>
    {
        protected override void ChangeTo(MultipleColorButton newTarget)
        {
            base.ChangeTo(newTarget);

            this.DisableFollowers();
            this.ApplyFollowers();
        }

        public override void ApplyFollowers()
        {
            base.ApplyFollowers();

            if (this.target)
                this.target.FollowersRef = this.followers;
        }

        public override void DisableFollowers()
        {
            base.DisableFollowers();

            if (this.target)
                this.target.FollowersRef = null;
        }
    }
}


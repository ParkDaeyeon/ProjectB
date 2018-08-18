using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D.UI;

namespace Ext.Unity3D
{
    public class Follower : Responsive
    {
        public override int Order
        {
            get
            {
                return 8;
            }
        }

        protected override void OnResize()
        {
            this.UpdateLayout();
        }

        [SerializeField]
        Transform target;
        public Transform Target
        {
            get { return this.target; }
        }

        public enum CoordinateType
        {
            World,
            Local,
        }
        [SerializeField]
        CoordinateType coordinate = CoordinateType.World;
        public CoordinateType Coordinate
        {
            set { this.coordinate = value; }
            get { return this.coordinate; }
        }

        [SerializeField]
        bool updateScale;
        public bool UpdateScale
        {
            set { this.updateScale = value; }
            get { return this.updateScale; }
        }

        [SerializeField]
        bool updateRotation;
        public bool UpdateRotation
        {
            set { this.updateRotation = value; }
            get { return this.updateRotation; }
        }

        [SerializeField]
        bool updatePosition;
        public bool UpdatePosition
        {
            set { this.updatePosition = value; }
            get { return this.updatePosition; }
        }



        [SerializeField]
        bool autoUpdate = true;
        public bool AutoUpdate
        {
            set { this.autoUpdate = value; }
            get { return this.autoUpdate; }
        }
        
        Transform[] followBuffer = new Transform[1] { null, };
        void LateUpdate()
        {
#if !TEST_AUTO_LAYOUT
            if (!this.autoUpdate)
                return;
#endif// !TEST_AUTO_LAYOUT

            this.UpdateLayout();
        }

        public void UpdateLayout()
        {
            this.followBuffer[0] = this.CachedTransform;
            FollowThis.Follow(this.target,
                              this.followBuffer,
                              this.updateScale,
                              this.updateRotation,
                              this.updatePosition,
                              CoordinateType.World == this.coordinate);
        }


#if UNITY_EDITOR
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.UpdateLayout();
        }

        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.UpdateLayout();
        }
#endif// UNITY_EDITOR
    }
}
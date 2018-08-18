using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D.UI;

namespace Ext.Unity3D
{
    public class FollowThis : Responsive
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
        List<Transform> followers = new List<Transform>();
        public List<Transform> Followers
        {
            get { return this.followers; }
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
            FollowThis.Follow(this.CachedTransform,
                              this.followers,
                              this.updateScale,
                              this.updateRotation,
                              this.updatePosition,
                              CoordinateType.World == this.coordinate);
        }

        public static void Follow(Transform target,
                                  IEnumerable<Transform> followers,
                                  bool S,
                                  bool R,
                                  bool T,
                                  bool W)
        {
            if (!target)
                return;

            var parentScale = S && W ? FollowThis.GetParentScale(target) : Vector3.zero;
            var worldRotation = R && W ? target.rotation : Quaternion.identity;
            var worldPosition = T && W ? target.position : Vector3.zero;

            var localScale = S ? target.localScale : Vector3.zero;
            var localRotation = R ? target.localRotation : Quaternion.identity;
            var localPosition = T ? target.localPosition : Vector3.zero;

            var enumerator = followers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var follower = enumerator.Current;
                if (!follower)
                    continue;

                if (W)
                {
                    if (S)
                        FollowThis.SetWorldScale(follower, localScale, parentScale);

                    if (R)
                        follower.rotation = worldRotation;

                    if (T)
                        follower.position = worldPosition;
                }
                else
                {
                    if (S)
                        follower.localScale = localScale;

                    if (R)
                        follower.localRotation = localRotation;

                    if (T)
                        follower.localPosition = localPosition;
                }
            }
        }
        


        public static Vector3 GetParentScale(Transform transform)
        {
            var parent = transform.parent;
            return parent ? parent.lossyScale : Vector3.one;
        }

        public static void SetWorldScale(Transform destTransform, Vector3 sourceLocalScale, Vector3 sourceParentScale)
        {
            var destParentScale = FollowThis.GetParentScale(destTransform);
            destTransform.localScale = new Vector3(sourceLocalScale.x * (sourceParentScale.x / destParentScale.x),
                                                   sourceLocalScale.y * (sourceParentScale.y / destParentScale.y),
                                                   sourceLocalScale.z * (sourceParentScale.z / destParentScale.z));
        }


#if UNITY_EDITOR
        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.LateUpdate();
        }
#endif// UNITY_EDITOR
    }
}
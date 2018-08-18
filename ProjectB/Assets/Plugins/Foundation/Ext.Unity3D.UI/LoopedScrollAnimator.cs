using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
namespace Ext.Unity3D.UI
{
    public class LoopedScrollAnimator : ManagedUIComponent
    {
        [SerializeField]
        List<RectTransform> transforms;

        [SerializeField]
        Vector2 size;
        public Vector2 Size { get { return this.size; } }

        [SerializeField]
        Vector2 padd;
        public Vector2 Padding { get { return this.padd; } }

        [SerializeField]
        float speed;
        public float Speed { set { this.speed = value; } get { return this.speed; } }

        [SerializeField]
        Vector2 start;
        public Vector2 Start { set { this.start = value; } get { return this.start; } }
        [SerializeField]
        Vector2 end;
        public Vector2 End { set { this.end = value; } get { return this.end; } }


        [SerializeField]
        bool isPlaying = true;
        public bool IsPlaying { set { this.isPlaying = value; } get { return this.isPlaying; } }


        void Update()
        {
            if (!this.isPlaying)
                return;

            this.UpdatePositions();
        }

        public void UpdatePositions()
        {
            if (!this.setupOrderList)
                this.ResetOrderList();

            var node = this.orderList.First;
            if (null == node)
                return;

            var v = this.end - this.start;
            var dir = v.normalized;
            var momentum = dir * (this.size + this.padd).magnitude;
            var pos = node.Value.anchoredPosition + (v * this.speed * Time.deltaTime);

            while (null != node)
            {
                var t = node.Value;
                node = node.Next;

                t.anchoredPosition = pos;
                pos += momentum;
            }


            bool isReverse = (this.start.sqrMagnitude < this.end.sqrMagnitude) != (0 < this.speed);
            node = isReverse ? this.orderList.First : this.orderList.Last;
            var tail = isReverse ? this.orderList.Last : this.orderList.First;

            pos = tail.Value.anchoredPosition;


            var sqrtDistance = v.sqrMagnitude;
            var angle = Angle.DirToAngle(dir);
            var angleMin = Angle.ToPositiveAngle(angle + (isReverse ? 90 : -90));
            var angleMax = Angle.ToPositiveAngle(angle + (isReverse ? -90 : 90));

            var anchor = isReverse ? this.end : this.start;

            while (null != node && tail != node)
            {
                var t = node.Value;

                var vObj = t.anchoredPosition - anchor;
                var sqrtDistanceObj = vObj.sqrMagnitude;

                var angleObj = Angle.ToPositiveAngle(Angle.DirToAngle(vObj.normalized));
                if (!Angle.ContainsAngle(angleMin, angleMax, angleObj))
                    sqrtDistanceObj = -sqrtDistanceObj;

                if (sqrtDistance < sqrtDistanceObj)
                {
                    var temp = isReverse ? node.Next : node.Previous;
                    this.orderList.Remove(node);
                    node = temp;

                    pos += isReverse ? momentum : -momentum;
                    t.anchoredPosition = pos;
                    if (isReverse)
                        this.orderList.AddLast(t);
                    else
                        this.orderList.AddFirst(t);
                    continue;
                }
                
                node = isReverse ? node.Next : node.Previous;
            }
        }

        public void ResetPositions()
        {
            var pos = this.start;

            var dir = (this.end - this.start).normalized;
            var momentum = dir * (this.size + this.padd).magnitude;
            for (int n = 0, cnt = this.transforms.Count; n < cnt; ++n)
            {
                var t = this.transforms[n];
                t.anchoredPosition = pos;

                pos += momentum;
            }

            this.ResetOrderList();
        }

        public int Count { get { return null != this.transforms ? this.transforms.Count : 0; } }

        public RectTransform this[int index] { get { return -1 < index && index < this.Count ? this.transforms[index] : null; } }

        public IEnumerable<RectTransform> Transforms { get { return this.transforms; } }

        public IEnumerable<RectTransform> OrderedTransforms
        {
            get
            {
                var node = this.orderList.First;
                while (null != node)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
        }

        void Awake()
        {
            if (!this.setupOrderList)
                this.ResetOrderList();
        }


        CachedList<RectTransform> orderList = new CachedList<RectTransform>();
        bool setupOrderList = false;
        public void ResetOrderList()
        {
            this.setupOrderList = true;
            this.orderList.Clear();
            for (int n = 0, cnt = this.transforms.Count; n < cnt; ++n)
                this.orderList.AddLast(this.transforms[n]);
        }



#if UNITY_EDITOR
        [SerializeField]
        RectTransform editorGenModel;
        [SerializeField]
        int editorGenCount = 0;
        [SerializeField]
        bool editorResetPosition;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorGenModel && 0 < this.editorGenCount)
            {
                int cnt = this.editorGenCount;
                this.editorGenCount = 0;

                if (null == this.transforms)
                    this.transforms = new List<RectTransform>(cnt);

                for (int n = 0; n < cnt; ++n)
                {
                    int idx = this.transforms.Count;

                    var copy = GameObject.Instantiate(this.editorGenModel.gameObject);
                    copy.name = idx.ToString();
                    var copyTrans = copy.GetComponent<RectTransform>();
                    copyTrans.SetParent(this.CachedTransform);
                    copyTrans.localPosition = Vector3.zero;
                    copyTrans.localScale = Vector3.one;
                    copyTrans.localRotation = Quaternion.identity;
                    copyTrans.anchorMin = this.editorGenModel.anchorMin;
                    copyTrans.anchorMax = this.editorGenModel.anchorMax;
                    copyTrans.offsetMin = this.editorGenModel.offsetMin;
                    copyTrans.offsetMax = this.editorGenModel.offsetMax;
                    copyTrans.pivot = this.editorGenModel.pivot;
                    copyTrans.sizeDelta = this.editorGenModel.sizeDelta;
                    this.transforms.Add(copyTrans);
                }

                this.transforms.Sort((x, y) =>
                {
                    int nx = int.Parse(x.gameObject.name);
                    int ny = int.Parse(y.gameObject.name);
                    return (nx < ny ? -1 : nx > ny ? 1 : 0);
                });

                this.ResetPositions();
            }
            else if (this.editorResetPosition)
            {
                this.ResetPositions();
            }
        }
#endif// UNITY_EDITOR
    }
}

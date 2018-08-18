using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif// UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext.Collection.AntiGC;
using Ext.Event;

namespace Ext.Unity3D.UI
{
    public class DragSnap : Responsive, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        RectTransform targetRect = null;
        public RectTransform TargetRect
        {
            get { return this.targetRect; }
        }

        float screenRatio = 1;

        [SerializeField]
        float itemSize = 100;
        public float ItemSize
        {
            set { this.itemSize = value; }
            get { return this.itemSize; }
        }

        [SerializeField]
        float fastSwipeThresholdTime = 0.2f;
        public float FastSwipeThresholdTime
        {
            set { this.fastSwipeThresholdTime = value; }
            get { return this.fastSwipeThresholdTime; }
        }

        [SerializeField]
        float fastSwipeThresholdDistance = 100;
        public float FastSwipeThresholdDistance
        {
            set { this.fastSwipeThresholdDistance = value; }
            get { return this.fastSwipeThresholdDistance; }
        }

        public bool IsAvailableFastSwipe
        {
            get { return 0 < this.fastSwipeThresholdTime; }
        }

        public enum DirectionEnum
        {
            Vertical,
            Horizontal,
        }
        [SerializeField]
        DirectionEnum direction = DirectionEnum.Vertical;
        public DirectionEnum Direction
        {
            set { this.direction = value; }
            get { return this.direction; }
        }


        protected void Start()
        {
            if (null == targetRect)
                return;

            if (DirectionEnum.Horizontal == this.direction)
            {
                this.screenRatio = targetRect.GetWidth() / Screen.width;
                this.itemSize = targetRect.GetWidth();
            }
            else
            {
                this.screenRatio = targetRect.GetHeight() / Screen.height;
                this.itemSize = targetRect.GetHeight();
            }
        }

        public override int Order
        {
            get { return 1; }
        }

        protected override void OnResize()
        {
            ListenerUtility.Calls(this.onResizeListeners);
        }

        LinkedList<Action> onStartDragListeners = new LinkedList<Action>();
        LinkedList<Action> onDraggingListeners = new LinkedList<Action>();
        LinkedList<Action<int>> onEndDragListeners = new LinkedList<Action<int>>();
        LinkedList<Action> onResizeListeners = new LinkedList<Action>();

        public void RegistStartDragListener(Action listener)
        {
            ListenerUtility.Regist(this.onStartDragListeners, listener);
        }
        public void RegistDraggingListener(Action listener)
        {
            ListenerUtility.Regist(this.onDraggingListeners, listener);
        }
        public void RegistEndDragListener(Action<int> listener)
        {
            ListenerUtility.Regist(this.onEndDragListeners, listener);
        }

        public void RegistResizeListener(Action listener)
        {
            ListenerUtility.Regist(this.onResizeListeners, listener);
        }

        public bool UnregistStartDragListener(Action listener)
        {
            return ListenerUtility.Unregist(this.onStartDragListeners, listener);
        }
        public bool UnregistDraggingListener(Action listener)
        {
            return ListenerUtility.Unregist(this.onDraggingListeners, listener);
        }
        public bool UnregistEndDragListener(Action<int> listener)
        {
            return ListenerUtility.Unregist(this.onEndDragListeners, listener);
        }

        public bool UnregistResizeListener(Action listener)
        {
            return ListenerUtility.Unregist(this.onResizeListeners, listener);
        }

        public void ClearStartDragListeners()
        {
            ListenerUtility.Clear(this.onStartDragListeners);
        }
        public void ClearDraggingListeners()
        {
            ListenerUtility.Clear(this.onDraggingListeners);
        }
        public void ClearEndDragListeners()
        {
            ListenerUtility.Clear(this.onEndDragListeners);
        }

        public void ClearResizeListener()
        {
            ListenerUtility.Clear(this.onResizeListeners);
        }



        

        bool pressed;
        public bool IsPressed { get { return this.pressed; } }

        float startPos;

        struct TimeSlice
        {
            public float time;
            public float position;
        }
        CachedList<TimeSlice> timeSlice = new CachedList<TimeSlice>(60);

        float ToPosition(Vector2 position)
        {
            if (DirectionEnum.Vertical == this.direction)
                return position.y;
            else if (DirectionEnum.Horizontal == this.direction)
                return position.x;

            return 0;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.pressed = true;

            var time = Time.realtimeSinceStartup;
            var pos = this.ToPosition(eventData.position);

            this.timeSlice.Clear();
            if (this.IsAvailableFastSwipe)
                this.timeSlice.AddLast(new TimeSlice { time = time, position = pos, });

            this.startPos = pos;

            ListenerUtility.Calls(this.onStartDragListeners);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!this.pressed)
                return;

            var time = Time.realtimeSinceStartup;
            var pos = this.ToPosition(eventData.position);

            if (this.IsAvailableFastSwipe)
            {
                this.timeSlice.AddLast(new TimeSlice { time = time, position = pos, });
                this.UpdateTimeSlice(time);
            }

            ListenerUtility.Calls(this.onDraggingListeners);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!this.pressed)
                return;

            this.pressed = false;

            var time = Time.realtimeSinceStartup;
            var pos = this.ToPosition(eventData.position);

            if (this.IsAvailableFastSwipe)
            {
                this.UpdateTimeSlice(time);
                var node = this.timeSlice.First;
                if (null != node)
                {
                    var ts = node.Value;
                    var fastDelta = pos - ts.position;
                    var fastDeltaAbs = Mathf.Abs(fastDelta);

                    if (this.fastSwipeThresholdDistance <= fastDeltaAbs)
                    {
                        ListenerUtility.CallsWithArg1(this.onEndDragListeners, 0 < fastDelta ? -1 : 1);
                        return;
                    }
                }
            }

            var delta = pos - this.startPos;
            var deltaAbs = Mathf.Abs(delta);

            if ((0.5f * this.itemSize) < deltaAbs * this.screenRatio)
            {
                ListenerUtility.CallsWithArg1(this.onEndDragListeners, 0 < delta ? -1 : 1);
                return;
            }

            ListenerUtility.CallsWithArg1(this.onEndDragListeners, 0);
        }


        void UpdateTimeSlice(float lastTime)
        {
            var node = this.timeSlice.First;
            var nodeTemp = node;
            while (null != node)
            {
                nodeTemp = node;
                node = node.Next;

                var ts = nodeTemp.Value;
                var deltaTime = lastTime - ts.time;
                if (deltaTime >= this.fastSwipeThresholdTime)
                    this.timeSlice.Remove(nodeTemp);
            }
        }
    }
}

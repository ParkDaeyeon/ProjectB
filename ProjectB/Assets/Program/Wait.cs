using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D.UI;
using Ext.Event;
using UnityEngine.UI;

namespace Program.View
{
    public class Wait : ManagedUIComponent
    {
        public const int SCREEN_LOCK = 7774;

        public class Handle
        {
            float timeStart;
            public float TimeStart
            {
                get { return this.timeStart; }
            }

            float timeLength;
            public float TimeLength
            {
                get { return this.timeLength; }
            }

            bool locked;
            public bool Locked
            {
                get { return this.locked; }
            }

            bool useProgress;
            public bool UseProgress
            {
                get { return this.useProgress; }
            }

            OnProgressCallback callback;
            public OnProgressCallback Callback
            {
                get { return this.callback; }
            }

            float progress;
            public float Progress
            {
                set { this.progress = value; }
                get { return this.progress; }
            }

            public Handle(float timeLength, bool locked, bool useProgress, OnProgressCallback c)
            {
                this.timeStart = Time.realtimeSinceStartup;
                this.timeLength = timeLength;
                this.locked = locked;
                this.useProgress = useProgress;
                this.callback = c;
            }

            public void Complete()
            {
                this.progress = 1;

                var callback = this.callback;
                if (null != callback)
                    callback(this);
            }
        }

        LinkedList<Handle> list = new LinkedList<Handle>();

        public static bool IsActive
        {
            private set
            {
                var inst = Wait.instance;
                if (!inst)
                    return;

                if (value == Wait.IsActive)
                    return;

                inst.gameObject.SetActive(value);
            }
            get
            {
                var inst = Wait.instance;
                if (!inst)
                    return false;

                return inst.gameObject.activeSelf;
            }
        }

        public static bool IsLocked
        {
            get
            {
                var inst = Wait.instance;
                if (!inst)
                    return false;

                return 0 < inst.lockCount;
            }
        }

        public delegate void OnProgressCallback(Handle handle);
        

        static Wait instance;

        void Awake()
        {
            // NOTE: 1개만 존재 한다.
            Wait.instance = this;
        }

        void OnDestroy()
        {
            if (this == Wait.instance)
                Wait.instance = null;
        }

        public static Wait.Handle Active(float timeLength, bool locked, OnProgressCallback callback)
        {
            return Wait.FireActive(timeLength, locked, false, callback);
        }

        public static Wait.Handle ActiveWithProgress(float timeLength, bool locked, OnProgressCallback callback)
        {
            return Wait.FireActive(timeLength, locked, true, callback);
        }

        static Wait.Handle FireActive(float timeLength, bool locked, bool useProgress, OnProgressCallback callback)
        {
            var inst = Wait.instance;
            if (!inst)
                return null;

            var data = new Handle(timeLength, locked, useProgress, callback);
            inst.list.AddLast(data);
            if (locked)
                inst.IncreaseLock();

            Wait.IsActive = true;

            return data;
        }



        [SerializeField]
        Text lockObject;

        int lockCount = 0;
        void IncreaseLock()
        {
            ++this.lockCount;

            if (!this.lockObject.enabled)
            {
                this.lockObject.enabled = true;
                BroadcastTunnel<int, bool>.Notify(Wait.SCREEN_LOCK, true);
            }
        }

        void DecreaseLock()
        {
            --this.lockCount;

            if (0 < this.lockCount)
                return;

            this.ClearLock();
        }

        void ClearLock()
        {
            this.lockCount = 0;

            if (this.lockObject.enabled)
                this.lockObject.enabled = false;

            BroadcastTunnel<int, bool>.Notify(Wait.SCREEN_LOCK, false);
        }


        public static bool Stop(Handle handle, bool isCompleted, bool immediate = false)
        {
            var inst = Wait.instance;
            if (!inst)
                return false;
            
            LinkedListNode<Handle> node = inst.list.Find(handle);
            if (null == node)
                return false;

            inst.StopInternal(handle, node);
            
            if (immediate && 0 == inst.list.Count)
                Wait.IsActive = false;

            if (isCompleted)
                handle.Complete();

            return true;
        }

        void StopInternal(Handle handle, LinkedListNode<Handle> node)
        {
            this.list.Remove(node);
            if (handle.Locked)
                Wait.instance.DecreaseLock();
        }

        public static void StopAll(bool isComplete, bool immediate = false)
        {
            var inst = Wait.instance;
            if (!inst)
                return;

            LinkedList<Handle> clone = null;
            if (isComplete)
                clone = new LinkedList<Handle>(inst.list);

            inst.list.Clear();
            Wait.instance.ClearLock();

            if (immediate)
                Wait.IsActive = false;


            if (isComplete)
            {
                var node = clone.First;
                while (null != node)
                {
                    var handle = node.Value;
                    node = node.Next;

                    handle.Complete();
                }
            }
        }
        
        

        // Update is called once per frame
        void Update()
        {
            var currentTime = Time.realtimeSinceStartup;

            var node = this.list.First;
            var nodeTemp = node;
            while (null != node)
            {
                var handle = node.Value;
                nodeTemp = node;
                node = node.Next;

                var deltaTime = currentTime - handle.TimeStart;
                var length = handle.TimeLength;

                if (0 < length)
                    handle.Progress = Mathf.Clamp01(deltaTime / length);
                else
                    handle.Progress = 1f;

                if (1 == handle.Progress)
                {
                    this.StopInternal(handle, nodeTemp);
                    handle.Complete();
                }
                else
                {
                    if (handle.UseProgress)
                    {
                        if (null != handle.Callback)
                            handle.Callback(handle);
                    }
                }
            }

            if (0 == this.list.Count)
                Wait.IsActive = false;
        }


#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();


        }
#endif // UNITY_EDITOR
    }
}
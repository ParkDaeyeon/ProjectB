using System;
using UnityEngine;

namespace Ext.Unity3D
{
    public interface IAsyncHandle
    {
        float Progress
        {
            get;
        }

        bool Done
        {
            get;
        }

        object Data
        {
            get;
        }

        int Priority
        {
            get;
            set;
        }

        bool AllowSceneActivation
        {
            get;
            set;
        }
    }

    public class AsyncHandle_ResourceRequest : IAsyncHandle
    {
        ResourceRequest request;
        public static AsyncHandle_ResourceRequest Create(ResourceRequest request)
        {
            if (null == request)
                return null;

            var handle = new AsyncHandle_ResourceRequest();
            handle.request = request;
            return handle;
        }

        public float Progress
        {
            get { return this.request.progress; }
        }

        public bool Done
        {
            get { return this.request.isDone; }
        }

        public object Data
        {
            get { return this.request.asset; }
        }

        public int Priority
        {
            set { this.request.priority = value; }
            get { return this.request.priority;}
        }

        public bool AllowSceneActivation
        {
            set { this.request.allowSceneActivation = value; }
            get { return this.request.allowSceneActivation; }
        }
    }

    public class AsyncHandle_AssetBundleRequest : IAsyncHandle
    {
        AssetBundleRequest request;
        public static AsyncHandle_AssetBundleRequest Create(AssetBundleRequest request)
        {
            if (null == request)
                return null;

            var handle = new AsyncHandle_AssetBundleRequest();
            handle.request = request;
            return handle;
        }

        public float Progress
        {
            get { return this.request.progress; }
        }

        public bool Done
        {
            get { return this.request.isDone; }
        }

        public object Data
        {
            get { return this.request.asset; }
        }

        public int Priority
        {
            set { this.request.priority = value; }
            get { return this.request.priority; }
        }

        public bool AllowSceneActivation
        {
            set { this.request.allowSceneActivation = value; }
            get { return this.request.allowSceneActivation; }
        }
    }
    

    public class AsyncHandle_Immediate : IAsyncHandle
    {
        public static AsyncHandle_Immediate Create(object data)
        {
            if (null == data)
                return null;

            var handle = new AsyncHandle_Immediate();
            handle.data = data;
            return handle;
        }
        
        public float Progress
        {
            get { return 1; }
        }

        public bool Done
        {
            get { return true; }
        }

        object data;
        public object Data
        {
            get { return this.data; }
        }

        public int Priority
        {
            set { ; }
            get { return 0; }
        }

        public bool AllowSceneActivation
        {
            set { ; }
            get { return true; }
        }
    }

    public class AsyncHandle_Custom : IAsyncHandle
    {
        float progress;
        public float Progress
        {
            get { return this.done ? 1 : this.progress; }
        }

        bool done = false;
        public bool Done
        {
            get { return this.done; }
        }

        object data;
        public object Data
        {
            get { return this.data; }
        }

        public int Priority
        {
            set {; }
            get { return 0; }
        }

        public bool AllowSceneActivation
        {
            set {; }
            get { return true; }
        }

        public void SetProgress(float value)
        {
            this.progress = Mathf.Clamp01(value);
        }

        public void SetComplete(object value)
        {
            this.done = true;
            this.data = value;
        }

        public void Reset()
        {
            this.progress = 0;
            this.done = false;
            this.data = null;
        }
    }
}

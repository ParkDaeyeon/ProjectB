using System;
using System.Threading;
using System.IO;
namespace Ext.Async
{
    public abstract class AbstractAsyncHandler
    {
        public bool IsStarted { private set; get; }

        public bool IsDone { private set; get; }

        public bool IsCanceled { private set; get; }

        public bool IsDestroyed { private set; get; }

        public float Progress { get { return this.progress; } }

        protected Input In { get { return this.input; } }

        public Output Out { get { return this.output; } }


        public void Start(Input input, Action<bool> resultCallback = null, Action<float> updateProgressCallback = null)
        {
            if (this.IsStarted)
                throw new InvalidOperationException(string.Format("ABS_ASYNC_HANDLER_DUPLICATED_START_EXCEPT:{0}",
                                                                  this.GetType().Name));

            this.IsStarted = true;
            this.resultCallback = resultCallback;
            this.updateProgressCallback = updateProgressCallback;
            this.input = input;
            this.output = this.OnCreateOutput();

            this.OnStart();
        }

        protected void Done()
        {
            if (this.IsDone)
                throw new InvalidOperationException(string.Format("ABS_ASYNC_HANDLER_DUPLICATED_DONE_EXCEPT:{0}",
                                                                  this.GetType().Name));
            this.IsDone = true;
            this.SetProgress(1f);
            this.Update();

            if (null != this.resultCallback)
                this.resultCallback(true);

            this.Clear();
        }

        public void Cancel()
        {
            if (this.IsDone)
                return;

            //this.IsDone = true;
            this.IsCanceled = true;

            Input input = this.input;
            if (null != input && null != input.signal)
            {
                if (!CancellableSignal.IsCancelled(input.signal))
                    CancellableSignal.Cancel(input.signal);
            }
        }

        protected void OnCancelDone()
        {
            this.IsDone = true;

            if (null != this.resultCallback)
                this.resultCallback(false);

            this.Destroy();
        }


        public void Destroy()
        {
            if (this.IsDone)
            {
                if (!this.IsDestroyed)
                {
                    this.IsDestroyed = true;

                    this.OnDestroy();
                    this.Clear();
                }
            }
        }

        public void Update()
        {
            if (this.isModifyProgress)
            {
                this.isModifyProgress = false;

                if (null != this.updateProgressCallback)
                    this.updateProgressCallback(this.progress);
            }
        }

        public bool IsModifyProgress { get { return this.isModifyProgress; } }



        // NOTE: input
        public abstract class Input
        {
            public CancellableSignal signal;
        }

        // NOTE: output
        public abstract class Output
        {
        }

        protected abstract Output OnCreateOutput();
        protected abstract void OnStart();
        protected virtual void OnDestroy() { }


        Input input;
        Output output;

        Action<bool> resultCallback;
        Action<float> updateProgressCallback;

        void Clear()
        {
            this.input = null;
            this.resultCallback = null;
            this.updateProgressCallback = null;
        }

        float progress;
        bool isModifyProgress = false;

        float subProgressMin;
        float subProgressMax;
        float subProgressRange;

        protected void SetProgress(float value)
        {
            value = AbstractAsyncHandler.Clamp01(Math.Max(value, this.progress));
            if (this.progress != value)
            {
                this.isModifyProgress = true;
                this.progress = value;
            }
        }

        protected void SetSubProgressRange(float next)
        {
            this.subProgressMin = AbstractAsyncHandler.Clamp01(this.progress);
            this.subProgressMax = AbstractAsyncHandler.Clamp01(Math.Max(next, this.progress));
            this.subProgressRange = this.subProgressMax - this.subProgressMin;
        }

        protected void SetSubProgress(float value)
        {
            //this.progress = AsyncLoader.Clamp01(AsyncLoader.Lerp(this.subProgressMin, this.subProgressMax, value));
            this.SetProgress(AbstractAsyncHandler.Clamp01(this.subProgressMin + (this.subProgressRange * value)));
        }

        protected float GetSubProgress()
        {
            if (0 >= this.subProgressRange)
                return 0;

            float delta = this.Progress - this.subProgressMin;
            if (0 >= delta)
                return 0;
            if (1 < delta)
                return 1;

            return delta / this.subProgressRange;
        }




        private static float Clamp01(float value)
        {
            return 0 > value ? 0 : value > 1f ? 1f : value;
        }

        private static float Lerp(float from, float to, float t)
        {
            return from + ((to - from) * t);
        }
    }
}
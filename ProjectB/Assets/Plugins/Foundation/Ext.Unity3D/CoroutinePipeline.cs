using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ext.Async;
using System;

namespace Ext.Unity3D
{
    public static class CoroutinePipeline<TArgument>
    {
        public delegate IEnumerator Task(Handler handle);

        public class Handler
        {
            public Handler(ICollection<Task> tasks, 
                           TArgument arg,
                           Action<Handler> doneCallback,
                           CancellableSignal overrideSignal)
            {
                this.tasks = new Task[tasks.Count];

                int n = 0;
                foreach (var task in tasks)
                    this.tasks[n++] = task;

                this.arg = arg;
                this.doneCallback = doneCallback;
                this.signal = null != overrideSignal ? overrideSignal : new CancellableSignal();
            }


            Task[] tasks;
            public Task[] Tasks { get { return this.tasks; } }
            public Task CurrentTask
            {
                get
                {
                    return -1 < this.step && this.step < this.Count ? this.tasks[this.step] : null;
                }
            }
            public int Count { get { return this.tasks.Length; } }


            TArgument arg;
            public TArgument Argument { get { return this.arg; } }


            int step = 0;
            public int Step { get { return this.step; } }


            CancellableSignal signal;
            public void Cancel()
            {
                CancellableSignal.Cancel(this.signal);
            }



            public bool IsCancelled { get { return CancellableSignal.IsCancelled(this.signal); } }
            public bool IsSucceed { get { return this.Count == this.step; } }
            public bool IsDone { get { return this.IsSucceed || this.IsCancelled; } }

            Action<Handler> doneCallback;
            public Action<Handler> DoneCallback
            {
                set { this.doneCallback = value; }
                get { return this.doneCallback; }
            }


            public bool GoTo(Task targetTask)
            {
                for (int n = 0, cnt = this.Count; n < cnt; ++n)
                {
                    var task = this.tasks[n];
                    if (task == targetTask)
                    {
                        this.MoveNext(n - this.step);
                        return true;
                    }
                }

                return false;
            }

            public void MoveNext(int amount = 1)
            {
                this.step = Mathf.Clamp(this.step + amount, 0, this.Count);
            }
            public void Complete()
            {
                this.step = this.Count;
            }
        }

        public static Handler Start(ICollection<Task> tasks,
                                    TArgument argument,
                                    Action<Handler> doneCallback = null,
                                    CancellableSignal overrideSignal = null)
        {
            var coHandle = new Handler(tasks, argument, doneCallback, overrideSignal);

            CoroutineTaskManager.AddTask(CoroutinePipeline<TArgument>.MainTask(coHandle));

            return coHandle;
        }

        static IEnumerator MainTask(Handler coHandle)
        {
            while (!coHandle.IsDone)
            {
                var step = coHandle.Step;
                var task = coHandle.CurrentTask;
                if (null == task)
                {
                    coHandle.MoveNext();
                    continue;
                }
#if LOG_DEBUG
                Debug.Log(string.Format("CO_PIPE:STEP:{0}, FUNC:{1}", step, task));
#endif// LOG_DEBUG
                var enumerator = task(coHandle);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                    if (coHandle.IsCancelled)
                    {
#if LOG_DEBUG
                        Debug.Log(string.Format("CO_PIPE:STEP:{0}, CANCELLED", step));
#endif// LOG_DEBUG
                        yield break;
                    }

                    if (step != coHandle.Step)
                        break;
                }

                if (step == coHandle.Step)
                    coHandle.MoveNext();
            }

            var callback = coHandle.DoneCallback;
            if (null != callback)
                callback(coHandle);
        }
    }
}

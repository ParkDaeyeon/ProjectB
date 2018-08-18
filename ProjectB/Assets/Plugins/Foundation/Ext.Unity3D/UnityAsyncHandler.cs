using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using Ext.Async;
namespace Ext.Unity3D
{
    public abstract class UnityAsyncHandler : AbstractAsyncHandler
    {
        public delegate IEnumerator SubRoutine(AbstractAsyncHandler.Input input_, AbstractAsyncHandler.Output output_, SubRoutineOutput subOutput);

        public class SubRoutineOutput
        {
            public float outSubProgress;
        }

        public abstract SubRoutine[] GetSubRoutines();

        protected override void OnStart()
        {
            UnityAsyncHandler.Input input = this.In as UnityAsyncHandler.Input;
            if (null == input)
                throw new ArgumentException(string.Format("UNITY_ASYNC_HANDER_INVALID_INPUT:{0}, INPUT:{1}, INPUT_TYPE:{2}",
                                                          this.GetType().Name,
                                                          null != this.In ? this.In.ToString() : "null",
                                                          null != this.In ? this.In.GetType().Name : "null"));

            SubRoutine[] subRoutines = this.GetSubRoutines();
            if (null == subRoutines)
                throw new ArgumentException(string.Format("UNITY_ASYNC_HANDER_SUBROUTINE_IS_NULL:{0}",
                                                          this.GetType().Name));

            CoroutineTaskManager.AddTask(this.Routine(input, subRoutines));
        }

        IEnumerator Routine(UnityAsyncHandler.Input input, SubRoutine[] subRoutines)
        {
            float subProgressTerm = 1 / (float)subRoutines.Length;
            IEnumerator enumerator;
            SubRoutineOutput subOutput = new SubRoutineOutput();

            for (int n = 0, count = subRoutines.Length; n < count; ++n)
            {
                subOutput.outSubProgress = 0;
                this.SetSubProgressRange(this.Progress + subProgressTerm);

                SubRoutine subRoutine = subRoutines[n];
                enumerator = subRoutine(input, this.Out, subOutput);
                while (enumerator.MoveNext())
                {
                    this.SetSubProgress(subOutput.outSubProgress);
                    this.Update();
                    yield return enumerator.Current;
                    if (CancellableSignal.IsCancelled(input.signal))
                    {
                        if (!this.IsCanceled)
                            this.Cancel();

                        this.OnCancelDone();
                        yield break;
                    }
                }

                this.SetSubProgress(1);
                this.Update();
                yield return null;
                if (CancellableSignal.IsCancelled(input.signal))
                {
                    if (!this.IsCanceled)
                        this.Cancel();

                    this.OnCancelDone();
                    yield break;
                }
            }

            this.Done();
            yield return null;
        }
    }



    public class UnityCommonAsyncHandler : UnityAsyncHandler
    {
        public override SubRoutine[] GetSubRoutines()
        {
            UnityCommonAsyncHandler.Input input = this.In as UnityCommonAsyncHandler.Input;
            if (null == input)
                throw new ArgumentException(string.Format("UNITY_COMMON_ASYNC_HANDER_INVALID_INPUT:{0}, INPUT:{1}, INPUT_TYPE:{2}",
                                                          this.GetType().Name,
                                                          null != this.In ? this.In.ToString() : "null",
                                                          null != this.In ? this.In.GetType().Name : "null"));

            return input.subRoutinesLink;
        }

        public new class Input : AbstractAsyncHandler.Input
        {
            public SubRoutine[] subRoutinesLink;
        }

        public new class Output : AbstractAsyncHandler.Output
        {
            // NOTE: empty
        }

        protected override AbstractAsyncHandler.Output OnCreateOutput()
        {
            return new Output();
        }
    }
}
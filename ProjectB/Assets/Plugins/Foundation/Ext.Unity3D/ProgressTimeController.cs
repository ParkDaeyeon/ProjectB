using UnityEngine;
using System;
using Ext.Unity3D;

namespace Ext.Unity3D
{
    // TODO: Refactoring
    public class ProgressTimeController : ManagedComponent
    {
        [SerializeField]
        ProgressTimer[] timers;
        public ProgressTimer[] GetTimers()
        {
            return this.timers;
        }

        [SerializeField]
        bool[] timerStates;
        public bool[] GetStates()
        {
            return this.timerStates;
        }


        event Action<int, float> onTimeListener;
        public event Action<int, float> TimeListener
        {
            add { this.onTimeListener += value; }
            remove { this.onTimeListener -= value; }
        }

        void Awake()
        {
            for (int n = 0, cnt = this.timers.Length; n < cnt; ++n)
            {
                var timer = this.timers[n];
                if (!timer)
                    continue;

                var index = n;
                timer.OnPlay = () =>
                {
                    this.timerStates[index] = true;
                    if (null != this.onTimeListener)
                        this.onTimeListener(index, 0);
                };
            }
        }

        void LateUpdate()
        {
            for (int n = 0, cnt = this.timers.Length; n < cnt; ++n)
            {
                if (!this.timerStates[n])
                    continue;

                var timer = this.timers[n];
                if (!timer)
                    continue;

                var progress = timer.Progress;
                if (null != this.onTimeListener)
                    this.onTimeListener(n, progress);

                if (1 <= progress)
                    this.timerStates[n] = false;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuildForChilds;
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            if (this.editorRebuildForChilds)
            {
                this.timers = this.FindComponentsInChildren<ProgressTimer>();
                this.timerStates = new bool[this.timers.Length];
            }
        }
#endif// UNITY_EDITOR
    }
}

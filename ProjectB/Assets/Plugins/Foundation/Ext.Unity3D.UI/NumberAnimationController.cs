using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class NumberAnimationController : ManagedUIComponent
    {
        [SerializeField]
        NumberBox numb;
        public NumberBox Number { get { return this.numb; } }
        public void SetNumber(NumberBox value) { this.numb = value; }

        [SerializeField]
        ProgressTimer timer;
        public ProgressTimer Timer { get { return this.timer; } }
        public void SetTimer(ProgressTimer value) { this.timer = value; }

        public uint Value { set { this.numb.Value = this.prev = this.next = value; } get { return this.numb.Value; } }
        public void SetValue(uint value)
        {
            this.numb.SetValue(value);
            this.prev = this.next = value;
        }

        uint prev;
        public uint Prev { get { return this.prev; } }

        uint next;
        public uint Next
        {
            set
            {
                if (this.next == value)
                    return;

                this.prev = this.Value;
                this.next = value;
                this.timer.Play();
            }
            get { return this.next; }
        }

        public bool IsPlaying { get { return this.Value != this.Next; } }

        public void UpdateValue()
        {
            var progress = this.timer.Progress;
            if (1 == progress)
                this.Value = this.Next;
            else
                this.numb.Value = (uint)Mathf.Lerp(this.prev, this.next, progress);
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.numb)
                this.numb = this.GetComponent<NumberBox>();

            if (!this.timer)
                this.timer = this.GetComponent<ProgressTimer>();
        }
#endif// UNITY_EDITOR
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.String;
using System.Globalization;

namespace Ext.Unity3D.UI
{
    public class NumberTextAnimController : ManagedUIComponent
    {
        [SerializeField]
        Text textNumb;
        public Text TextNumber { get { return this.textNumb; } }
        public void SetTextNumber(Text value) { this.textNumb = value; }


        [SerializeField]
        ProgressTimer timer;
        public ProgressTimer Timer { get { return this.timer; } }
        public void SetTimer(ProgressTimer value) { this.timer = value; }

        [SerializeField]
        int useComma;
        public int UseComma
        {
            set { this.useComma = value; }
            get { return this.useComma; }
        }

        public virtual string ToStringValue(uint value)
        {
            if (0 == this.useComma)
                return value.ToString();

            return value.ToCommaSeparatedStringUI(this.useComma);
        }

        public virtual uint ParseUnsignedIntegerValue(string value)
        {
            if (null == value)
                return 0;
            
            uint total = 0;
            uint digit = 1;
            for (int n = value.Length - 1; n >= 0; --n)
            {
                var ch = value[n];
                if ('0' <= ch && ch <= '9')
                {
                    var number = (uint)ch - '0';
                    total += number * digit;
                    digit *= 10;
                }
            }
            return total;
        }


        [SerializeField]
        uint value;
        bool isFirst = true;
        public uint Value
        {
            set
            {
                this.prev = this.next = value;
                if (this.isFirst || value != this.value)
                {
                    this.isFirst = false;
                    this.textNumb.text = this.ToStringValue(this.value = value);
                }
            }
            get { return this.value; }
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
            {
                this.Value = this.Next;
                return;
            }

            var value = (uint)Mathf.Lerp(this.prev, this.next, progress);
            if (value != this.value)
                this.textNumb.text = this.ToStringValue(this.value = value);
        }

        [SerializeField]
        bool autoUpdate;
        void LateUpdate()
        {
#if !TEST_AUTO_LAYOUT
            if (!this.autoUpdate)
                return;
#endif// !TEST_AUTO_LAYOUT

            if (!this.IsPlaying)
                return;

            this.UpdateValue();
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.textNumb)
                this.textNumb = this.GetComponent<Text>();

            if (!this.timer)
                this.timer = this.GetComponent<ProgressTimer>();
        }

        [SerializeField]
        uint editorTestNextValue;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.Next = this.editorTestNextValue;
        }
#endif// UNITY_EDITOR
    }
}
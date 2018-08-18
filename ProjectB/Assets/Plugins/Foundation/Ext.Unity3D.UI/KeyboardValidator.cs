using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ext.Unity3D.UI
{
    public class KeyboardValidator : ManagedUIComponent
    {
        [SerializeField]
        InputField inputField;
        public InputField InputField
        {
            set { this.inputField = value; }
            get { return this.inputField; }
        }

        public enum VerifyMode
        {
            None,
            ValidCharacters,
            IgnoreCharacters,
        }
        [SerializeField]
        VerifyMode verifyMode;
        public VerifyMode Verify
        {
            set { this.verifyMode = value; }
            get { return this.verifyMode; }
        }

        [SerializeField]
        string verifyCharacters;
        public string VerifyCharacters
        {
            set { this.verifyCharacters = value; }
            get { return this.verifyCharacters; }
        }


        [Serializable]
        public class KeyboardValidatorSerializedDictionary : SerializedDictionary<char, char> { }

        [SerializeField]
        KeyboardValidatorSerializedDictionary replacerBuilt = new KeyboardValidatorSerializedDictionary();
        Dictionary<char, char> replacerRaw = null;
        public Dictionary<char, char> Replacer
        {
            get
            {
                if (null == this.replacerRaw)
                    this.replacerRaw = this.replacerBuilt.AsDictionary;

                return this.replacerRaw;
            }
        }


        public bool TryProcess(char before, out char after)
        {
            return this.Replacer.TryGetValue(before, out after);
        }

        public char Process(char before)
        {
            char after;
            if (this.TryProcess(before, out after))
                return after;

            return before;
        }

        public bool TryProcess(string before, out string after)
        {
            return before != (after = this.Process(before));
        }

        public string Process(string before)
        {
            if (string.IsNullOrEmpty(before))
                return "";

            var sb = new StringBuilder(before.Length);
            for (int n = 0, cnt = before.Length; n < cnt; ++n)
            {
                var ch = before[n];
                if (!this.IsValid(ch))
                    continue;

                sb.Append(this.Process(ch));
            }

            return sb.ToString();
        }


        public bool IsValid(char ch)
        {
            switch (this.verifyMode)
            {
            case VerifyMode.ValidCharacters:
                for (int n = 0, cnt = this.verifyCharacters.Length; n < cnt; ++n)
                {
                    if (ch == this.verifyCharacters[n])
                        return true;
                }
                return false;

            case VerifyMode.IgnoreCharacters:
                for (int n = 0, cnt = this.verifyCharacters.Length; n < cnt; ++n)
                {
                    if (ch == this.verifyCharacters[n])
                        return false;
                }
                return true;
            }

            return true;
        }


        void Awake()
        {
            if (this.inputField)
            {
                this.inputField.onValueChanged.AddListener((text) =>
                {
                    this.inputField.text = this.Process(text);
                });
            }
        }



#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (!this.inputField)
                this.inputField = this.FindComponent<InputField>();
        }
#endif// UNITY_EDITOR
    }
}

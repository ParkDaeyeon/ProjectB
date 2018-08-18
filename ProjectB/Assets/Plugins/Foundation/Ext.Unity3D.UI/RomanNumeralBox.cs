using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
    public class RomanNumeralBox : ImageTextBox
    {
        static string romanIndexes = "IVXLCDM()";

        public void ResetRomanIndexes()
        {
            base.Indexes = RomanNumeralBox.romanIndexes;
        }
        
        public uint Value
        {
            set
            {
                this.ClearText();

                // See if it's >= 4000.
                if (value >= 4000)
                {
                    this.AddChar('(');
                    this.AddRoman(value / 1000);
                    this.AddChar(')');

                    value %= 1000;
                }

                this.AddRoman(value);
            }
            get { return (uint)RomanNumerals.ToInt(this.Text); }
        }
        
        void AddRoman(uint value)
        {
            string str;

            // Pull out thousands.
            str = RomanNumerals.ThouLetters[value / 1000];
            if (!string.IsNullOrEmpty(str))
                this.AddString(str);
            value %= 1000;

            // Handle hundreds.
            str = RomanNumerals.HundLetters[value / 100];
            if (!string.IsNullOrEmpty(str))
                this.AddString(str);
            value %= 100;

            // Handle tens.
            str = RomanNumerals.TensLetters[value / 10];
            if (!string.IsNullOrEmpty(str))
                this.AddString(str);
            value %= 10;

            // Handle ones.
            str = RomanNumerals.OnesLetters[value];
            if (!string.IsNullOrEmpty(str))
                this.AddString(str);
        }

#if UNITY_EDITOR
        [SerializeField]
        uint editorTestValue = 0;
        
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            if (!this.sprites)
                this.sprites = this.FindComponent<SpritesComponent>();

            this.ResetRomanIndexes();

            if (0 < this.editorTestValue)
                this.Value = this.editorTestValue;
        }
#endif// UNITY_EDITOR
    }
}

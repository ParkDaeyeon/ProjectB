using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
namespace Ext.Unity3D.UI
{
    // TODO: Refactoring
    [AddComponentMenu("UI/Ext/Badge")]
    public class Badge : ManagedUIComponent
    {
        [SerializeField]
        protected Image bg;
        public Image Bg { get { return this.bg; } }

        [SerializeField]
        protected NumberBox number;
        public NumberBox Number { get { return this.number; } }


        [SerializeField]
        protected Text label;
        public Text Label { get { return this.label; } }

        [SerializeField]
        protected float minWidth;
        public float MinWidth { get { return this.minWidth; } }

        [SerializeField]
        protected float badgePaddWidth;
        public float BadgePaddWidth { get { return this.badgePaddWidth; } }
        

        public uint NumberValue
        {
            set
            {
                if (!this.number)
                    return;

                if (this.label)
                    this.label.gameObject.SetActive(false);

                bool valid = 0 != value;
                this.SetActive(valid);
                if (valid)
                {
                    this.number.SetActive(true);

                    this.number.Value = value;
                    float w = this.badgePaddWidth + (this.number.NumberWidth * Mathf.Max(this.number.Count, 1));
                    this.Width = Mathf.Max(w, this.minWidth);
                }
            }
            get
            {
                return this.IsActivated ? ((this.number && this.number.IsActivated) ? this.number.Value : 0)
                                        : 0;
            }
        }

        public string TextValue
        {
            set
            {
                if (!this.label)
                    return;

                if (this.number)
                    this.number.SetActive(false);

                bool valid = !string.IsNullOrEmpty(value);
                this.SetActive(valid);
                if (valid)
                {
                    if (this.label)
                        this.label.gameObject.SetActive(true);

                    this.label.text = (null != value) ? value : "";
                    float w = this.badgePaddWidth + this.label.preferredWidth;
                    this.Width = Mathf.Max(w, this.minWidth);
                }
            }
            get
            {
                return this.IsActivated ? ((this.label && this.label.gameObject.activeSelf) ? this.label.text : "")
                                         : "";
            }
        }

        public float Width
        {
            set
            {
                if (!this.bg)
                    return;

                this.bg.rectTransform.SetWidth(value);
            }
            get
            {
                if (!this.bg)
                    return 0;

                return this.bg.rectTransform.GetWidth();
            }
        }

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.bg = this.FindComponent<Image>();
            this.number = this.FindComponent<NumberBox>();
            this.label = this.FindComponent<Text>("Text");
        }

        [SerializeField]
        bool editorTestNumber = false;
        [SerializeField]
        uint editorTestNumberValue = 0;
        [SerializeField]
        bool editorTestText = false;
        [SerializeField]
        string editorTestTextValue = "MAX";
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            
            if (this.editorTestNumber)
            {
                this.editorTestNumber = false;
                this.NumberValue = this.editorTestNumberValue;
            }
            else if (this.editorTestText)
            {
                this.editorTestText = false;
                this.TextValue = this.editorTestTextValue;
            }
        }


        [UnityEditor.MenuItem("GameObject/UI/Ext/Badge")]
        static void OnCreateSampleObject()
        {
            Transform parent = UnityEditor.Selection.activeTransform;
            Badge.CreateSampleObject(parent);
        }
        public static Badge CreateSampleObject(Transform parent)
        {
            var component = UIExtension.CreateUIObject<Badge>("Badge", parent, Vector3.zero, new Vector2(55, 25));

            var bg = component.gameObject.AddComponent<Image>();
            bg.color = Color.red;

            var number = NumberBox.CreateSampleObject(component.CachedRectTransform);
            number.CachedRectTransform.sizeDelta = new Vector2(36, 0);
            number.Align = NumberBox.ALIGN.Center;
            number.EditorSetting();

            var text = new GameObject("Text");
            text.transform.SetParent(component.CachedTransform);

            var label = text.gameObject.AddComponent<Text>();
            label.rectTransform.anchoredPosition = new Vector3(-0.5f, 0, 0);
            label.rectTransform.SetSize(Vector2.zero);
            label.rectTransform.localRotation = Quaternion.identity;
            label.alignment = TextAnchor.MiddleCenter;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.text = "MAX";

            component.badgePaddWidth = 10;
            component.EditorSetting();

            return component;
        }
#endif// UNITY_EDITOR
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext.Collection.AntiGC;
namespace Ext.Unity3D.UI
{
    [AddComponentMenu("UI/Ext/Image Text Box")]
    public class ImageTextBox : AlignedBox
    {
        [SerializeField]
        protected SpritesComponent sprites;
        public SpritesComponent Sprites { get { return this.sprites; } }

        [SerializeField]
        string indexes;
        public string Indexes { set { this.indexes = value; } get { return this.indexes; } }
        public int FindIndex(char ch)
        {
            return this.indexes.IndexOf(ch);
        }
        public bool IsValidChar(char ch)
        {
            return -1 != this.FindIndex(ch);
        }


        [SerializeField]
        float spaceWidth;
        public float SpaceWidth { get { return this.spaceWidth; } }
        

        [Serializable]
        public class OverrideLayout
        {
            public OverrideLayout(char character)
            {
                this.character = character;
            }

            [SerializeField]
            char character;
            public char Character
            {
                get { return this.character; }
            }

            [SerializeField]
            Vector2 offset = Vector2.zero;
            public Vector2 Offset
            {
                set { this.offset = value; }
                get { return this.offset; }
            }

            [SerializeField]
            float addSpacing;
            public float AddSpacing
            {
                set { this.addSpacing = value; }
                get { return this.addSpacing; }
            }

            public float GetActualSpacing(float originSpacing)
            {
                return originSpacing + this.addSpacing;
            }

            public override string ToString()
            {
                return string.Format("char:{0}, offset:{1}, addSpacing:{2}",
                                this.character, this.offset, this.addSpacing);
            }
        }
        [SerializeField]
        List<OverrideLayout> overrideLayouts = new List<OverrideLayout>();
        public List<OverrideLayout> OverrideLayouts { get { return this.overrideLayouts; } }
        public OverrideLayout GetLayout(char character)
        {
            for (int n = 0, cnt = this.overrideLayouts.Count; n < cnt; ++n)
                if (this.overrideLayouts[n].Character == character)
                    return this.overrideLayouts[n];

            return null;
        }


        public char GetChar(int index)
        {
            if (0 > index || index >= this.indexBuffer.Count)
                return default(char);
            
            var idxData = this.indexBuffer[index];
            if (0 > idxData.spriteIndex || idxData.spriteIndex >= this.indexes.Length)
                return ' ';

            return this.indexes[idxData.spriteIndex];
        }



        public override float GetLengthAt(int index)
        {
            var indexData = this.GetIndexData(index);
            if (indexData.IsNone)
                return 0;
            else if (-1 == indexData.spriteIndex)
                return this.spaceWidth;
            
            return base.GetLengthAt(indexData.elemIndex);
        }

        public override float GetSpacingAt(int index)
        {
            var ch = this.GetChar(index);
            if (default(char) == ch)
                return 0;

            else if (' ' == ch)
                return base.GetSpacingAt(index);

            var layout = this.GetLayout(ch);
            if (null == layout)
                return base.GetSpacingAt(this.GetIndexData(index).elemIndex);
            
            return layout.GetActualSpacing(this.Spacing);
        }



        float width = 0;
        public override float ActualLength { get { return this.width; } }


        StringBuilder sb = new StringBuilder();
        public override int Count { get { return this.keepText.Length; } }

        [HideInInspector]

        [SerializeField]
        string keepText = "";
        protected override void OnRebuild()
        {
            this.SetText(this.keepText);
        }

        bool isFirst = true;
        public string Text
        {
            set
            {
                if (this.isFirst || this.keepText.Length != value.Length)
                {
                    this.SetText(value);
                    return;
                }

                for (int n = 0, cnt = this.keepText.Length; n < cnt; ++n)
                {
                    var a = this.keepText[n];
                    var b = value[n];
                    if (a != b)
                    {
                        this.SetText(value);
                        return;
                    }
                }
            }
            get
            {
                return this.keepText;
            }
        }


        [SerializeField]
        int bufferCount = 32;
        public struct Index
        {
            public int spriteIndex;
            public int elemIndex;

            public static Index None = new Index { spriteIndex = -2, elemIndex = -2, };
            public bool IsNone { get { return -2 == this.spriteIndex; } }
        }
        FastValueArray<Index> indexBuffer = null;
        protected FastValueArray<Index> IndexBuffer
        {
            get
            {
                if (null == this.indexBuffer)
                    this.indexBuffer = new FastValueArray<Index>(this.bufferCount);

                return this.indexBuffer;
            }
        }

        public Index GetIndexData(int index)
        {
            if (0 > index || index >= this.indexBuffer.Count)
                return Index.None;

            return this.indexBuffer[index];
        }


        int lastElemIndex = 0;
        
        void DoAppend(char ch, int index, int spriteIndex, int elemIndex)
        {
            var buffer = this.IndexBuffer;

            if (0 > index)
                index = 0;

            var count = this.Count;
            if (index >= count)
            {
                this.sb.Append(ch);
                buffer.Add(new Index { spriteIndex = spriteIndex, elemIndex = elemIndex, });
            }
            else
            {
                this.sb.Insert(index, ch);
                buffer.Insert(index, new Index { spriteIndex = spriteIndex, elemIndex = elemIndex, });
                ++count;
                for (int n = index + 1; n < count; ++n)
                {
                    var idxData = buffer[n];
                    if (-1 == idxData.elemIndex)
                        continue;

                    ++idxData.elemIndex;
                    buffer[n] = idxData;
                }
            }
        }

        bool Append(char ch, int index = int.MaxValue)
        {
            int spriteIndex = -1;
            if (' ' == ch || -1 == (spriteIndex = this.FindIndex(ch)))
            {
                this.DoAppend(ch, index, -1, -1);
                return true;
            }

            if (this.lastElemIndex < this.ElementsCount)
            {
                this.DoAppend(ch, index, spriteIndex, this.lastElemIndex++);
                return true;
            }

            return false;
        }

        bool Remove(int index)
        {
            var count = this.Count;
            if (0 > index || index >= count)
                return false;

            this.sb.Remove(index, 1);

            var buffer = this.IndexBuffer;
            buffer.RemoveAt(index);

            --count;
            for (int n = index; n < count; ++n)
            {
                var idxData = buffer[n];
                if (-1 == idxData.elemIndex)
                    continue;

                --idxData.elemIndex;
                buffer[n] = idxData;
            }

            return true;
        }



        void UpdateElements()
        {
            this.keepText = this.sb.ToString();

            this.width = 0;
            this.ShowElems(this.ElementsCount, false);

            var buffer = this.IndexBuffer;
            var count = this.Count;

            var prevSpacing = this.Spacing;
            for (int n = count - 1; n >= 0; --n)
            {
                var idxData = buffer[n];
                if (-1 != idxData.spriteIndex)
                {
                    var co = this.Elements[idxData.elemIndex];
                    this.SetSprite(co, idxData.spriteIndex);

                    var layout = this.GetLayout(this.GetChar(n));
                    var pos = co.CachedRectTransform.anchoredPosition;

                    if (null != layout)
                    {
                        pos.x = -this.width + layout.Offset.x;
                        pos.y = layout.Offset.y;
                    }
                    else
                    {
                        pos.x = -this.width;
                        pos.y = 0;
                    }

                    co.CachedRectTransform.anchoredPosition = pos;

                    if (!co.IsActivated)
                        co.SetActive(true);

                    this.width += base.GetLengthAt(idxData.elemIndex);
                }
                else
                    this.width += this.spaceWidth;

                if (0 != n)
                    this.width += this.GetSpacingAt(n);
            }

            this.UpdateAlignment();
        }
        


        public void SetText(string text)
        {
            this.isFirst = false;

            this.ClearText();
            
            if (0 == this.ElementsCount || string.IsNullOrEmpty(text))
            {
#if UNITY_EDITOR
                this.editorTestString = text;
#endif// UNITY_EDITOR
                return;
            }

            for (int n = 0, cnt = text.Length; n < cnt; ++n)
            {
                var ch = text[n];
                if (!this.Append(ch))
                    break;
            }

            this.UpdateElements();
        }
        
        public bool AddChar(char ch, int index = int.MaxValue)
        {
            if (!this.Append(ch, index))
                return false;
            
            this.UpdateElements();
            return true;
        }
        public int AddString(string str, int index = int.MaxValue)
        {
            var ret = 0;
            for (int n = 0, cnt = str.Length; n < cnt; ++n)
            {
                var ch = str[n];
                if (this.Append(ch, index))
                    ++ret;
            }

            this.UpdateElements();
            return ret;
        }

        public bool RemoveChar(int index)
        {
            if (!this.Remove(index))
                return false;

            this.UpdateElements();
            return true;
        }
        public int RemoveString(int index, int count)
        {
            var ret = 0;
            for (int n = index + count - 1; n >= index; --n)
            {
                if (this.Remove(n))
                    ++ret;
            }

            this.UpdateElements();
            return ret;
        }

        public void ClearText()
        {
            var buffer = this.IndexBuffer;
            buffer.Clear();

            this.sb.Length = 0;
            this.keepText = "";
            this.lastElemIndex = 0;

            this.ShowElems(this.ElementsCount, false);
            this.width = 0;
        }


        protected virtual void SetSprite(UICache co, int spriteIndex)
        {
            if (!co.Graphic)
                return;

            if (!this.sprites)
                return;

            if (co.Graphic.TrySetSprite(this.sprites[spriteIndex]))
                co.Graphic.SetNativeSize();
        }


#if UNITY_EDITOR
        [SerializeField]
        int editorGenElems = 0;
        [SerializeField]
        Graphic editorGenElemsCopyModel = null;

        [SerializeField]
        string editorTestString = "";
        [SerializeField]
        int editorTestAddCharIndex = -1;
        [SerializeField]
        char editorTestAddCharValue = ' ';
        [SerializeField]
        int editorTestRemoveCharIndex = -1;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            if (!this.sprites)
            {
                this.sprites = this.GetComponent<SpritesComponent>();

                if (!this.sprites)
                {
                    Debug.LogWarning("IMAGE_TEXT_BOX_EDITWARN:SPRITES_COMPONENT_IS_NULL!:" + this.name);
                    return;
                }
            }

            int sprCount = this.sprites.Count;
            int idxCount = this.indexes.Length;
            if (sprCount != idxCount)
            {
                Debug.LogWarning("IMAGE_TEXT_BOX_EDITWARN:MISMATCH_COUNT:SPR_COUNT:" + sprCount + ", IDX_COUNT:" + idxCount);
                foreach (SpriteCache sc in this.sprites.Elements)
                    Debug.Log(sc);
            }

            if (null != this.Elements)
            {
                for (int n = this.ElementsCount - 1; -1 < n; --n)
                {
                    var co = this.Elements[n];
                    if (!co)
                        this.Elements.RemoveAt(n);
                }
            }

            var alignBase = this.AlignBase;
            if (alignBase && 0 < this.editorGenElems)
            {
                int cnt = this.editorGenElems;
                this.editorGenElems = 0;

                if (null == this.Elements)
                    this.EditorResetElements(new List<UICache>(cnt));

                var copyModel = this.editorGenElemsCopyModel;
                if (!copyModel && 0 < this.ElementsCount && this.Elements[0])
                    copyModel = this.Elements[0].Graphic;

                for (int n = 0; n < cnt; ++n)
                {
                    int idx = this.ElementsCount;
                    string name = idx.ToString();
                    if (copyModel)
                    {
                        var copyTrans = copyModel.rectTransform;

                        if (copyModel is Image)
                        {
                            var component = UIExtension.CreateUIObject<Image>(name, alignBase, Vector3.zero, copyTrans.sizeDelta, copyTrans.localScale, copyTrans.localRotation);
                            component.sprite = ((Image)copyModel).sprite;
                            var co = this.CreateElement(component.rectTransform, idx);
                            if (co)
                                this.Elements.Add(co);
                            continue;
                        }
                        else if (copyModel is RawImage)
                        {
                            var component = UIExtension.CreateUIObject<RawImage>(name, alignBase, Vector3.zero, copyTrans.sizeDelta, copyTrans.localScale, copyTrans.localRotation);
                            component.texture = ((RawImage)copyModel).texture;
                            var co = this.CreateElement(component.rectTransform, idx);
                            if (co)
                                this.Elements.Add(co);
                            continue;
                        }
                    }

                    // NOTE: 없다. 걍 대충 때려 박는다.
                    var componentDef = UIExtension.CreateUIObject<Image>(this.ElementsCount.ToString(), alignBase, Vector3.zero, new Vector2(20, 20));
                    var coDef = this.CreateElement(componentDef.rectTransform, idx);
                    if (coDef)
                        this.Elements.Add(coDef);
                }

                this.Elements.Sort(this.SetupSort);
            }

            this.SetupPositions();
            this.SetText(this.editorTestString);

            if (-1 != this.editorTestAddCharIndex)
            {
                var ret = this.AddChar(this.editorTestAddCharValue, this.editorTestAddCharIndex);
                Debug.Log("ADD:" + ret);
            }
            if (-1 != this.editorTestRemoveCharIndex)
            {
                var ret = this.RemoveChar(this.editorTestRemoveCharIndex);
                Debug.Log("REMOVE:" + ret);
            }
        }

        protected virtual UICache CreateElement(Transform t, int digit)
        {
            t.name = digit.ToString();

            Image img = t.GetComponent<Image>();
            if (img)
                return new UICache(img.gameObject);

            RawImage imgRaw = t.GetComponent<RawImage>();
            if (imgRaw)
                return new UICache(imgRaw.gameObject);

            return null;
        }

        protected int SetupSort(UICache x, UICache y)
        {
            int nx = int.Parse(x.GameObject.name);
            int ny = int.Parse(y.GameObject.name);
            return (nx < ny ? -1 : nx > ny ? 1 : 0);
        }


        [UnityEditor.MenuItem("GameObject/UI/Ext/Image Text Box")]
        static void OnCreateSampleObject()
        {
            Transform parent = UnityEditor.Selection.activeTransform;
            ImageTextBox.CreateSampleObject(parent);
        }
        public static ImageTextBox CreateSampleObject(Transform parent)
        {
            var rectTransform = UnityExtension.CreateObject<RectTransform>("ImageTextBox", parent);
            var component = rectTransform.gameObject.AddComponent<ImageTextBox>();
            component.CachedRectTransform.sizeDelta = new Vector2(100, 100);
            component.EditorResetAlignBase(UnityExtension.CreateObject<RectTransform>("Align", component.CachedRectTransform));
            component.EditorResetElements(new List<UICache>());
            for (int n = 0; n < 3; ++n)
            {
                Image img = UIExtension.CreateUIObject<Image>(n.ToString(), component.AlignBase, Vector3.zero, new Vector2(20, 20));
                component.Elements.Add(new UICache(img.gameObject));
            }
            component.Align = ALIGN.LeftOrBottom;
            component.OnEditorSetting();
            return component;
        }
#endif// UNITY_EDITOR
    }
}
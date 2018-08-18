using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
	[AddComponentMenu("UI/Ext/Number Box")]
    public class NumberBox : AlignedBox
    {
        [SerializeField]
        bool autoNativeSize = false;
        [SerializeField]
        protected SpritesComponent sprites;
        public SpritesComponent Sprites { get { return this.sprites; } }


        public float NumberWidth { get { return null != this.Elements && 0 < this.ElementsCount ? this.Elements[0].CachedRectTransform.rect.size.x : 0; } }

		[SerializeField]
		protected int fillZero = 0;
		public int FillZero { get { return this.fillZero; } }
		public int ValueCount { protected set; get; }
		public int ZeroCount { protected set; get; }
		public override int Count { get { return this.ValueCount + this.ZeroCount; } }
        public override float ActualLength
        {
            get
            {
                var count = this.Count;
                if (0 >= count)
                    return 0;

                var last = count - 1;

                float width = this.NumberWidth * count + this.Spacing * last;

                if (this.UseComma)
                {
                    var commaCnt = Mathf.Clamp(0 < this.commaVisibleTerm ? last / this.commaVisibleTerm : 0, 0, this.CommaCapacity);
                    if (0 < commaCnt)
                        width += (this.CommaWidth + this.commaSpacingBefore + this.commaSpacingAfter) * commaCnt;
                }

                if (this.UseSign)
                    width += this.SignWidth + this.signPadding;

                return width;
            }
        }

        //public override void OptimizeCaches()
        //{
        //    base.OptimizeCaches();

        //    if (this.UseComma)
        //    {
        //        for (int n = 0, cnt = this.CommaCapacity; n < cnt; ++n)
        //            UICache.OptimizeAnimPairOnetime(this.GetComma(n));
        //    }

        //    if (this.UseSign)
        //    {
        //        for (int n = 0, cnt = this.SignCapacity; n < cnt; ++n)
        //            UICache.OptimizeAnimPairOnetime(this.GetSign(n));
        //    }
        //}



        [SerializeField]
		protected UICache[] commas;
        public int CommaCapacity { get { return null != this.commas ? this.commas.Length : 0; } }
        public UICache GetComma(int n) { return n < this.CommaCapacity ? this.commas[n] : null; }
        public UICache[] Commas { get { return this.commas; } }
		public bool UseComma { get { return 0 < this.CommaCapacity; } }

		[SerializeField]
		protected float commaSpacingBefore = 0;
		[SerializeField]
		protected float commaSpacingAfter = 0;
		public float CommaSpacingBefore { get { return this.commaSpacingBefore; } }
		public float CommaSpacingAfter { get { return this.commaSpacingAfter; } }

		[SerializeField]
		protected int commaVisibleTerm = 3;
		public int CommaVisibleTerm { get { return this.commaVisibleTerm; } }

        public float CommaWidth { get { return this.UseComma ? this.commas[0].CachedRectTransform.rect.size.x : 0; } }



		[SerializeField]
		protected UICache[] signs;
        public int SignCapacity { get { return null != this.signs ? this.signs.Length : 0; } }
        public UICache GetSign(int n) { return n < this.SignCapacity ? this.signs[n] : null; }
        public UICache[] Signs { get { return this.signs; } }
		public bool UseSign { get { return -1 < this.selectedSign && this.selectedSign < this.SignCapacity; } }


		[SerializeField]
		protected float signPadding = 0;
		public float SignPadding { get { return this.signPadding; } }

        [SerializeField]
        protected bool signReversePosition = false;
        public bool SignReversePosition { get { return this.signReversePosition; } }

        int selectedSign = -1;
		public int SelectedSign
        {
            set
            {
                if (value == this.selectedSign)
                    return;

                this.selectedSign = value;
                this.Rebuild();
            }
            get { return this.selectedSign; }
        }
		public UICache SelectedSignObject
		{
			get
			{
				if (this.UseSign)
					return this.GetSign(this.selectedSign);

				return null;
			}
		}

        public float SignWidth
        {
            get
            {
                if (this.UseSign)
                {
                    var sign = this.GetSign(this.selectedSign);
                    if (sign)
                        return sign.CachedRectTransform.rect.size.x;
                }

                return 0;
            }
        }




		public static uint CalcDigitCount(uint value)
		{
			return (0 == value) ? 1u : (uint)(Mathf.Log10((int)value) + 1u);
		}

        bool isFirst = true;
		uint value;
		public uint Value
        {
            set
            {
                if (this.value != value || this.isFirst)
                    this.SetValue(value);
            }
			get { return this.value; }
		}

		public void SetValue(uint value)
		{
            this.isFirst = false;

			if (0 == this.ElementsCount)
			{
				this.value = 0;
				this.ValueCount = 0;
				this.ZeroCount = 0;
                this.Rebuild();
                return;
			}

			this.value = value;
			this.ValueCount = (0 == value) ? 1 : (int)NumberBox.CalcDigitCount(value);
			this.ZeroCount = 0;
#if UNITY_EDITOR
			this.editorTestValue = this.value;
#endif// UNITY_EDITOR
            
			if (this.ElementsCount < this.ValueCount)
			{
				// fill to 999999...

				this.ValueCount = this.ElementsCount;
				this.value = 9;
				for (int digit = 0; digit < this.ElementsCount; ++digit)
				{
					this.value *= 10;
					this.value += 9;

					var co = this.Elements[digit];

					if (!co.IsActivated)
                        co.SetActive(true);

					this.SetSprite(co, 9);
				}

#if UNITY_EDITOR
				this.editorTestValue = this.value;
#endif// UNITY_EDITOR
                this.Rebuild();
                return;
			}


			if (this.ElementsCount > this.ValueCount)
			{
				if (0 != this.fillZero)
				{
					if (this.fillZero > this.ValueCount)
						this.ZeroCount = this.fillZero - this.ValueCount;
					else
						this.ZeroCount = 0;
				}
			}

			for (int digit = this.Count; digit < this.ElementsCount; ++digit)
                this.Elements[digit].SetActive(false);

			// fill to number
			uint remainValue = this.value;
			for (int digit = 0, digitMax = this.ValueCount; digit < digitMax; ++digit)
			{
				uint numberIdx = remainValue % 10;
				remainValue /= 10;

				UICache co = this.Elements[digit];

				if (!co.IsActivated)
                    co.SetActive(true);

				this.SetSprite(co, numberIdx);
			}

			// fill to 0000...
			for (int digit = this.ValueCount, digitMax = digit + this.ZeroCount; digit < digitMax; ++digit)
			{
				UICache co = this.Elements[digit];

				if (!co.IsActivated)
                    co.SetActive(true);

				this.SetSprite(co, 0);
			}

            this.Rebuild();
        }
        
        void ResetCommas()
        {
            if (null == this.commas)
                return;

            int length = this.Count;
            for (int n = 0, nMax = this.commas.Length, n2 = this.commaVisibleTerm; n < nMax; ++n, n2 += this.commaVisibleTerm)
            {
                var comma = this.GetComma(n);
                if (!comma)
                    continue;

                if (n2 >= length)
                {
                    comma.SetActive(false);
                    continue;
                }

                comma.SetActive(true);
            }
        }


        [SerializeField]
        bool fixSignPosition;
        public bool FixSignPosition
        {
            set { this.fixSignPosition = value; }
            get { return this.fixSignPosition; }
        }
        void ResetSigns()
        {
            if (null == this.signs)
                return;

            var selected = this.SelectedSignObject;
            for (int n = 0, cnt = this.SignCapacity; n < cnt; ++n)
            {
                var sign = this.GetSign(n);
                if (!sign)
                    continue;

                sign.SetActive(selected == sign);
            }

            if (!selected || this.fixSignPosition)
                return;

            var count = this.Count;
            if (0 == count)
                return;

            var wd = this.Elements[this.signReversePosition ? 0 : count - 1];

            var wdTrans = wd.CachedRectTransform;
            var signTrans = selected.CachedRectTransform;

            var pos = signTrans.anchoredPosition;
            if (this.signReversePosition)
                pos.x = wdTrans.anchoredPosition.x + this.signPadding;
            else
                pos.x = wdTrans.anchoredPosition.x - this.signPadding - this.NumberWidth;

            signTrans.anchoredPosition = pos;
        }

        protected override void OnRebuild()
        {
            this.ResetCommas();
            this.ResetSigns();
            this.UpdateAlignment();
        }

        protected virtual void SetSprite(UICache co, uint number)
		{
			if (!co.Graphic)
				return;

            if (!this.sprites)
                return;

            if (this.autoNativeSize)
                co.Graphic.SetNativeSize();

			co.Graphic.TrySetSprite(this.sprites[(int)number]);
		}
        
#if UNITY_EDITOR
        [SerializeField]
		protected int editorGenElems = 0;
		[SerializeField]
		protected Graphic editorGenElemsObjCopyModel = null;
		protected override void OnEditorSetting()
		{
			base.OnEditorSetting();
            
            if (!this.sprites)
            {
                this.sprites = this.GetComponent<SpritesComponent>();

                if (!this.sprites)
                {
                    Debug.LogWarning("NUMBER_BOX_EDITWARN:SPRITES_COMPONENT_IS_NULL!:" + this.name);
                    return;
                }
            }

            //int sprCount = this.sprites.Count;
            //if (0 != sprCount && 10 != sprCount)
            //{
            //    Debug.LogWarning("NUMBER_BOX_EDITWARN:MISMATCH_COUNT:SPR_COUNT:" + sprCount);
            //    foreach (SpriteCache sc in this.sprites.Elements)
            //        Debug.Log(sc);
            //}

            var alignBase = this.AlignBase;
            
            if (null != this.Elements && 0 < this.ElementsCount)
			{
				for (int n = this.ElementsCount - 1; -1 < n; --n)
				{
					var co = this.Elements[n];
					if (!co)
						this.Elements.RemoveAt(n);
				}
			}
            else if (alignBase)
            {
                for (int n = 0; n < 999; ++n)
                {
                    var transChild = alignBase.Find(n.ToString());
                    if (!transChild)
                        break;

                    if (null == this.Elements)
                        this.EditorResetElements(new List<UICache>());

                    this.Elements.Add(new UICache(transChild.gameObject));
                }
            }

			if (alignBase && 0 < this.editorGenElems)
			{
				int cnt = this.editorGenElems;
				this.editorGenElems = 0;

				if (null == this.Elements)
                    this.EditorResetElements(new List<UICache>(cnt));

				var copyModel = this.editorGenElemsObjCopyModel;
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


            if (this.UseComma)
            {
                for (int n = 0, cnt = this.CommaCapacity; n < cnt; ++n)
                {
                    var comma = this.GetComma(n);
                    if (comma)
                        comma.Rebuild();
                }
            }

            if (null != this.signs)
            {
                for (int n = 0, cnt = this.SignCapacity; n < cnt; ++n)
                {
                    var sign = this.GetSign(n);
                    if (sign)
                        sign.Rebuild();
                }
            }

			this.SetupPositions();
		}


        [SerializeField]
        protected uint editorTestValue = 0;
        [SerializeField]
        protected int editorTestSignIdx = -1;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();
            this.SelectedSign = this.editorTestSignIdx;
            this.SetValue(this.editorTestValue);
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


		protected override void SetupPositions()
        {
            if (ALIGN.Fixed == this.align)
                return;

            base.SetupPositions();

			float x = 0;
			int commaIdx = 0;
			for (int n = 0, cnt = this.ElementsCount, last = cnt - 1; n < cnt; ++n)
			{
				var obj = this.Elements[n];

				this.SetRightTopAnchor(obj.CachedRectTransform);

				var pos = obj.CachedRectTransform.anchoredPosition;
				pos.x = x;
				obj.CachedRectTransform.anchoredPosition = pos;

				x -= this.NumberWidth;
	
				if (n != last)
				{
					x -= this.Spacing;

					if (this.UseComma)
					{
						if (0 == ((n + 1) % this.commaVisibleTerm))
						{
							x -= this.commaSpacingBefore;
							var comma = this.GetComma(commaIdx++);
                            if (!comma)
                                continue;

							this.SetRightTopAnchor(comma.CachedRectTransform);

							pos = comma.CachedRectTransform.localPosition;
							pos.x = x;
							comma.CachedRectTransform.SetPositionOfPivot(pos);

							x -= this.CommaWidth;
							x -= this.commaSpacingAfter;
						}
					}
				}
			}

			if (null != this.signs)
			{
				for (int n = 0, cnt = this.SignCapacity; n < cnt; ++n)
				{
					var sign = this.GetSign(n);
					if (!sign)
						continue;

					var wd = this.Elements[this.signReversePosition ? 0 : this.ElementsCount - 1];

					var wdTrans = wd.CachedRectTransform;
					var signTrans = sign.CachedRectTransform;

                    if (this.signReversePosition)
                        this.SetLeftBottomAnchor(signTrans);
                    else
                        this.SetRightTopAnchor(signTrans);

					var pos = signTrans.anchoredPosition;
                    if (this.signReversePosition)
                        pos.x = wdTrans.anchoredPosition.x + this.signPadding;
                    else
                        pos.x = wdTrans.anchoredPosition.x - this.signPadding - this.NumberWidth;
					signTrans.anchoredPosition = pos;
				}
			}
		}


		[UnityEditor.MenuItem("GameObject/UI/Ext/Number Box")]
		static void OnCreateSampleObject()
		{
			Transform parent = UnityEditor.Selection.activeTransform;
			NumberBox.CreateSampleObject(parent);
		}
		public static NumberBox CreateSampleObject(Transform parent)
		{
			var rectTransform = UnityExtension.CreateObject<RectTransform>("NumberBox", parent);
			var component = rectTransform.gameObject.AddComponent<NumberBox>();
			component.CachedRectTransform.sizeDelta = new Vector2(100, 100);
			component.EditorResetAlignBase(UnityExtension.CreateObject<RectTransform>("Align", component.CachedRectTransform));
			component.EditorResetElements(new List<UICache>());
			for (int n = 0; n < 3; ++n)
			{
				Image img = UIExtension.CreateUIObject<Image>(n.ToString(), component.AlignBase, Vector3.zero, new Vector2(20, 20));
				component.Elements.Add(new UICache(img.gameObject));
			}
            component.Align = ALIGN.LeftOrBottom;
			component.EditorSetting();
			return component;
        }


        protected override void OnEditorResetByAlignBase(Transform alignedBase)
        {
            Debug.LogError("NUMBER_BOX:NOT_SUPPORTED:OnEditorResetByAlignBase");
        }





        public static void EditorResetForComma(NumberBox box, IEnumerable<Transform> elems)
        {
            if (!box)
                return;

            box.OnEditorResetForComma(elems);
        }

        protected virtual void OnEditorResetForComma(IEnumerable<Transform> elems)
        {
            List<UICache> commaList = new List<UICache>();
            foreach (Transform child in elems)
                commaList.Add(new UICache(child.gameObject));

            this.commas = commaList.ToArray();
        }




        public static void EditorResetForSign(NumberBox box, IEnumerable<Transform> elems)
        {
            if (!box)
                return;

            box.OnEditorResetForSign(elems);
        }

        protected virtual void OnEditorResetForSign(IEnumerable<Transform> elems)
        {
            List<UICache> signList = new List<UICache>();
            foreach (Transform child in elems)
                signList.Add(new UICache(child.gameObject));

            this.signs = signList.ToArray();
        }

#endif// UNITY_EDITOR
    }
}
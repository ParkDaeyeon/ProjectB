using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D.UI
{
	[AddComponentMenu("UI/Ext/Color Animation")]
	public class ColorAnimation : ManagedUIComponent
    {
		[SerializeField]
		protected Graphic graphic;
		public Graphic Graphic
		{
			set
			{
				this.graphic = value;
			}
			get
			{
				return this.graphic;
			}
		}

		public Color Color
		{
			set
            {
                this.SetColor(this.graphic, value);

                this.UpdateColor();
            }
			get
			{
				if (this.graphic)
					return this.graphic.color;

				return Color.black;
			}
		}

        public float Alpha
        {
            set
            {
                var color = this.Color;
                color.a = value;

                this.SetColor(this.graphic, color);

                this.UpdateColor();
            }
            get
            {
                return this.Color.a;
            }
        }

		[SerializeField]
		protected bool alphaOnly;
		public bool AlphaOnly 
		{
			get 
			{
				return this.alphaOnly; 
			} 
		}

		[SerializeField]
		protected Graphic[] colorFollowers;
		public Graphic[] ColorFollowers
		{
			set
			{
				this.colorFollowers = value;
			}
			get
			{
				return this.colorFollowers;
			}
		}

		[SerializeField]
		protected Graphic[] updateFollowers;
		public Graphic[] UpdateFollowers
		{
			set
			{
				this.updateFollowers = value;
			}
			get
			{
				return this.updateFollowers;
			}
		}

		[SerializeField]
		protected bool isAutoUpdate = true;
		public bool IsAutoUpdate
		{
			set
			{
				this.isAutoUpdate = value;
			}
			get
			{
				return this.isAutoUpdate;
			}
		}

		[SerializeField]
		protected Animation dependAnimation;	// NOTE: Optional
		public Animation DependAnimation
		{
			set
			{
				this.dependAnimation = value;
			}
			get
			{
				return this.dependAnimation;
			}
		}


		void OnEnable()
		{
			if (this.graphic)
				this.prevColor = this.graphic.color;

			this.UpdateColor();
		}


		//bool isDirty = false;
		Color prevColor;
		bool isDependAnimLastFrame = true;
		void LateUpdate()
        {
#if !TEST_AUTO_LAYOUT
            if (!this.isAutoUpdate)
				return;
#endif// !TEST_AUTO_LAYOUT

            if (this.graphic && this.graphic.color == this.prevColor)
				return;
			
			if (this.dependAnimation)
			{
				if (!this.dependAnimation.isPlaying)
				{
					if (this.isDependAnimLastFrame)
					{
						this.isDependAnimLastFrame = false;
						return;
					}
				}
				else
					this.isDependAnimLastFrame = true;
			}

			this.UpdateColor();
		}

		public void UpdateColor()
		{
			if (this.graphic)
			{
				this.prevColor = this.graphic.color;
				//this.graphic.SetVerticesDirty();
				this.UpdateColorFollowers(this.colorFollowers, this.graphic.color);
				this.UpdateVertexFollowers(this.updateFollowers);
			}
		}

		void UpdateColorFollowers(Graphic[] graphicArray, Color color)
		{
			if (graphicArray == null)
				return;

			Graphic graphic;
			for (int i = 0, cnt = graphicArray.Length; i < cnt; ++i)
			{
				graphic = graphicArray[i];
				this.SetColor(graphic, color);
			}
		}
		
		void UpdateVertexFollowers(Graphic[] graphicArray)
		{
			if (graphicArray == null)
				return;

			for (int i = 0, cnt = graphicArray.Length; i < cnt; ++i)
			{
				graphicArray[i].SetVerticesDirty();
			}
		}

		void SetColor(Graphic graphic, Color color)
		{
			if (!graphic)
				return;
			
			if (this.alphaOnly)
			{
				Color c = graphic.color;
				c.a = color.a;
				graphic.color = c;
			}
			else
			{
				graphic.color = color;
			}
            graphic.SetVerticesDirty();
        }

#if UNITY_EDITOR
		protected override void OnEditorSetting()
		{
			base.OnEditorSetting();

            if (!this.graphic)
    			this.graphic = this.GetComponent<Graphic>();
		}

        [SerializeField]
        bool editorPostRebuildFollowers;
        [SerializeField]
        Transform[] editorPostRebuildAdditionalFollowerBases;
        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            if (this.editorPostRebuildFollowers)
            {
                var list = new List<Graphic>();

                var colorFws = this.FindComponentsInChildren<Graphic>();
                for (int n = 0, cnt = colorFws.Length; n < cnt; ++n)
                {
                    var colorFw = colorFws[n];
                    if (this.graphic != colorFw)
                        list.Add(colorFw);
                }

                if (null != this.editorPostRebuildAdditionalFollowerBases)
                {
                    foreach (var b in this.editorPostRebuildAdditionalFollowerBases)
                    {
                        var colorFwsAdditional = b.FindComponentsInChildren<Graphic>();
                        for (int n = 0, cnt = colorFwsAdditional.Length; n < cnt; ++n)
                        {
                            var colorFw = colorFwsAdditional[n];
                            if (this.graphic != colorFw)
                                list.Add(colorFw);
                        }
                    }
                }
                this.colorFollowers = list.ToArray();
            }
        }


        protected override void OnEditorTestingLooped()
        {
            base.OnEditorTestingLooped();
            this.UpdateColor();
            this.graphic.SetAllDirty();
        }
#endif// UNITY_EDITOR
    }
}

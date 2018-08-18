using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif// UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext.Collection.AntiGC;

namespace Ext.Unity3D.UI
{
    public class ScrollbarSnap : ManagedUIComponent, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        Scrollbar scrollbar;
        public Scrollbar Scrollbar { get { return this.scrollbar; } }


        Action onStartScroll;
        public Action OnStartScroll
        {
            set { this.onStartScroll = value; }
            get { return this.onStartScroll; }
        }


        Action<float> onEndScroll;
        public Action<float> OnEndScroll
        {
            set { this.onEndScroll = value; }
            get { return this.onEndScroll; }
        }


        bool pressed;
        public bool IsPressed { get { return this.pressed; } }


        public void OnPointerDown(PointerEventData eventData)
        {
            this.pressed = true;

            if (null != this.onStartScroll)
                this.onStartScroll();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!this.pressed)
                return;

            if (null != this.onEndScroll)
                this.onEndScroll(this.scrollbar.value);
        }
    }
}

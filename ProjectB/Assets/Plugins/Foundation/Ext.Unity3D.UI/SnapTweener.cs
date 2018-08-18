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
using Ext.Event;

namespace Ext.Unity3D.UI
{
    public class SnapTweener : ManagedUIComponent, IBeginDragHandler, IEndDragHandler
    {
        LinkedList<Action> onStartDragListeners = new LinkedList<Action>();
        public void RegistStartDragListener(Action listener)
        {
            ListenerUtility.Regist(this.onStartDragListeners, listener);
        }

        LinkedList<Action> onEndDragListeners = new LinkedList<Action>();
        public void RegistEndDragListener(Action listener)
        {
            ListenerUtility.Regist(this.onEndDragListeners, listener);
        }


        bool pressed;
        public bool IsPressed
        {
            get { return this.pressed; }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            this.pressed = true;

            ListenerUtility.Calls(this.onStartDragListeners);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!this.pressed)
                return;

            this.pressed = false;

            ListenerUtility.Calls(this.onEndDragListeners);
        }
    }
}

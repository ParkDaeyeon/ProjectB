using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    [RequireComponent(typeof(RoutedScrollRect))]
    public class RoutedScrollRectSupplier : ManagedUIComponent
    {
        [SerializeField]
        RoutedScrollRect routedScrollRect = null;

        [SerializeField]
        List<Component> parentsHandlerList;

#if UNITY_EDITOR
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();
            
            this.routedScrollRect.CollectionHandler();
            this.parentsHandlerList = this.routedScrollRect.parentsHandlerList;
        }
#endif// UNITY_EDITOR
    }
}
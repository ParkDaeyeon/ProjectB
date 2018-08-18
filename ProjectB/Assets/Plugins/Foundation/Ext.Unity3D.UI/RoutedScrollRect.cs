using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Ext.Unity3D.UI
{
    public class RoutedScrollRect : ScrollRect
    {
        public List<Component> parentsHandlerList = null;
        bool routeToParent = false;
        bool collection = false;

        public void CollectionHandler()
        {
            if (null == this.parentsHandlerList)
                this.parentsHandlerList = new List<Component>();
            else
                this.parentsHandlerList.Clear();

            var parent = transform.parent;
            while (null != parent)
            {
                var components = parent.GetComponents<Component>();

                for (int n = 0, cnt = components.Length; n < cnt; ++n)
                {
                    var component = components[n];

                    if(component is IEventSystemHandler)
                        this.parentsHandlerList.Add(component);
                }

                parent = parent.parent;
            }

            collection = true;
#if UNITY_EDITOR
            Debug.Log("[Routed Scroll Rect] Complete Collecting");
#endif
        }        
        


        /// <summary>
        /// Do action for all parents
        /// </summary>
        void DoForParents<EventDataType, HandlerType>(EventDataType eventData,
                                                      Action<EventDataType, HandlerType> action)
            where EventDataType : BaseEventData
            where HandlerType : IEventSystemHandler
        {

            if (0 < this.parentsHandlerList.Count)
            {
                for (int n = 0, cnt = this.parentsHandlerList.Count; n < cnt; ++n)
                {
                    var component = this.parentsHandlerList[n];

                    if (component is HandlerType)
                        action(eventData, (HandlerType)(IEventSystemHandler)component);
                }
            }
            else if(!collection)
                this.CollectionHandler();
        }
        
        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            this.DoForParents<PointerEventData, IInitializePotentialDragHandler>(eventData, this.OnInitializePotentialDragForParent);
            base.OnInitializePotentialDrag(eventData);
        }
        void OnInitializePotentialDragForParent(PointerEventData eventData, IInitializePotentialDragHandler parent)
        {
            if (null != parent)
                parent.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag(PointerEventData eventData)
        {
            if (this.routeToParent)
                this.DoForParents<PointerEventData, IDragHandler>(eventData, this.OnDragForParent);
            else
                base.OnDrag(eventData);
        }
        void OnDragForParent(PointerEventData eventData, IDragHandler parent)
        {
            if (null != parent)
                parent.OnDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!this.horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                this.routeToParent = true;
            else if (!this.vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                this.routeToParent = true;
            else
                this.routeToParent = false;

            if (this.routeToParent)
                this.DoForParents<PointerEventData, IBeginDragHandler>(eventData, this.OnBeginDragForParent);
            else
                base.OnBeginDrag(eventData);
        }
        void OnBeginDragForParent(PointerEventData eventData, IBeginDragHandler parent)
        {
            if (null != parent)
                parent.OnBeginDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (this.routeToParent)
                this.DoForParents<PointerEventData, IEndDragHandler>(eventData, this.OnEndDragForParent);
            else
                base.OnEndDrag(eventData);
            this.routeToParent = false;
        }
        void OnEndDragForParent(PointerEventData eventData, IEndDragHandler parent)
        {
            if (null != parent)
                parent.OnEndDrag(eventData);
        }
    }
}
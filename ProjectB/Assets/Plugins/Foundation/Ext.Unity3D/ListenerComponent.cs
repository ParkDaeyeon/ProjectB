using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ext.Unity3D
{
    public class ListenerComponent : MonoBehaviour
    {
        Dictionary<int, List<Action>> listeners = new Dictionary<int, List<Action>>();
        public void Add(Action action)
        {
            this.Add(0, action);
        }
        public void Add(int eventId, Action action)
        {
            List<Action> list;
            if (!this.listeners.TryGetValue(eventId, out list))
                this.listeners.Add(eventId, list = new List<Action>());

            list.Add(action);
        }

        public bool Remove(Action action)
        {
            return this.Remove(0, action);
        }
        public bool Remove(int eventId, Action action)
        {
            List<Action> list;
            if (!this.listeners.TryGetValue(eventId, out list))
                return false;

            if (list.Remove(action))
            {
                if (0 == list.Count)
                    this.RemoveEvent(eventId);

                return true;
            }

            return false;
        }

        public bool RemoveEvent(int eventId)
        {
            return this.listeners.Remove(eventId);
        }

        public void Clear()
        {
            this.listeners.Clear();
        }

        public void OnEvent(int eventId = 0)
        {
            List<Action> list;
            if (!this.listeners.TryGetValue(eventId, out list))
                return;
            
            for (int n = 0, cnt = list.Count; n < cnt; ++n)
            {
                var action = list[n];
                if (null == action)
                    continue;

                action();
            }
        }
    }
}

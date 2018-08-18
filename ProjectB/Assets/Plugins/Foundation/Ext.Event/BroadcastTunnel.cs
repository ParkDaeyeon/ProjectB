using System;
using System.Collections.Generic;
namespace Ext.Event
{
    public static class BroadcastTunnel<K, V>
    {
        class Manager
        {
            public HashSet<Action<V>> container = new HashSet<Action<V>>();
            public LinkedList<Action<V>> queue = new LinkedList<Action<V>>();
            public LinkedList<V> nextCall = new LinkedList<V>();
        }

        static Dictionary<string, Dictionary<K, Manager>> groups
            = new Dictionary<string, Dictionary<K, Manager>>();

        static Dictionary<K, Manager> GetGroup(string groupId)
        {
            var groups = BroadcastTunnel<K, V>.groups;
            Dictionary<K, Manager> group;
            if (!groups.TryGetValue(groupId, out group))
                return null;

            return group;
        }

        static Dictionary<K, Manager> AddGroup(string groupId)
        {
            var groups = BroadcastTunnel<K, V>.groups;
            Dictionary<K, Manager> group;
            if (!groups.TryGetValue(groupId, out group))
                groups.Add(groupId, group = new Dictionary<K, Manager>());

            return group;
        }

        static void RemoveGroup(string groupId)
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                return;

            foreach (var mgr in group.Values)
                mgr.container.Clear();

            group.Clear();

            var groups = BroadcastTunnel<K, V>.groups;
            groups.Remove(groupId);
        }

        public static void ChangeGroup(string groupId, string newGroupId)
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                return;
            
            var groups = BroadcastTunnel<K, V>.groups;
            groups.Remove(groupId);
            groups[newGroupId] = group;
        }
        
        public static void Notify(K key, V arg, string groupId = "")
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                return;

            Manager mgr;
            if (group.TryGetValue(key, out mgr))
                BroadcastTunnel<K, V>.DoNotify(mgr, arg);
        }

        static void DoNotify(Manager mgr, V arg)
        {
            var queue = mgr.queue;

            if (0 < queue.Count)
            {
                mgr.nextCall.AddLast(arg);
                return;
            }

            var enumerator = mgr.container.GetEnumerator();
            while (enumerator.MoveNext())
                queue.AddLast(enumerator.Current);

            var node = queue.First;
            var nodeTemp = node;
            while (null != node)
            {
                var action = node.Value;
                nodeTemp = node;
                node = node.Next;
                queue.Remove(nodeTemp);

                if (null != action)
                    action(arg);
            }

            if (0 < mgr.nextCall.Count)
            {
                var nextCallNode = mgr.nextCall.First;
                var nextCallArg = nextCallNode.Value;
                mgr.nextCall.Remove(nextCallNode);
                BroadcastTunnel<K, V>.DoNotify(mgr, nextCallArg);
            }
        }

        public static void Add(K key, Action<V> receiver, string groupId = "")
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                group = BroadcastTunnel<K, V>.AddGroup(groupId);

            Manager mgr;
            if (!group.TryGetValue(key, out mgr))
                group.Add(key, mgr = new Manager());

            if (!mgr.container.Contains(receiver))
                mgr.container.Add(receiver);
        }

        public static void Adds(K key, Action<V>[] receivers, string groupId = "")
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                group = BroadcastTunnel<K, V>.AddGroup(groupId);

            Manager mgr;
            if (!group.TryGetValue(key, out mgr))
                group.Add(key, mgr = new Manager());

            for (int n = 0, cnt = receivers.Length; n < cnt; ++n)
            {
                if (!mgr.container.Contains(receivers[n]))
                    mgr.container.Add(receivers[n]);
            }
        }
        
        public static bool Remove(K key, Action<V> value, string groupId = "")
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                return false;

            Manager mgr;
            if (!group.TryGetValue(key, out mgr))
                return false;

            bool ret = mgr.container.Remove(value);
            if (0 == mgr.container.Count)
            {
                group.Remove(key);
                if (0 == group.Count)
                    BroadcastTunnel<K, V>.RemoveGroup(groupId);
            }

            return ret;
        }

        public static bool RemoveAllByKey(K key, string groupId = "")
        {
            var group = BroadcastTunnel<K, V>.GetGroup(groupId);
            if (null == group)
                return false;
            
            bool ret = group.Remove(key);
            if (0 == group.Count)
                BroadcastTunnel<K, V>.RemoveGroup(groupId);

            return ret;
        }

        public static void RemoveAllByGroup(string groupId = "")
        {
            BroadcastTunnel<K, V>.RemoveGroup(groupId);
        }

        public static void RemoveAll()
        {
            var groupIds = BroadcastTunnel<K, V>.GetGroupIds();
            if (null == groupIds)
                return;
            
            foreach (var groupId in groupIds)
                BroadcastTunnel<K, V>.RemoveGroup(groupId);
        }

        public static string[] GetGroupIds()
        {
            var groups = BroadcastTunnel<K, V>.groups;
            var groupIds = new string[groups.Keys.Count];
            int count = 0;
            foreach (var groupId in groups.Keys)
                groupIds[count++] = groupId;

            return groupIds;
        }
    }
}
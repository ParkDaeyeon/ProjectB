using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Collection
{
    public class MultiSortedDictionary<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
    {
        SortedDictionary<Key, List<Value>> dict;

        public MultiSortedDictionary()
        {
            this.dict = new SortedDictionary<Key, List<Value>>();
        }

        public MultiSortedDictionary(IComparer<Key> comparer)
        {
            this.dict = new SortedDictionary<Key, List<Value>>(comparer);
        }

        public void Add(Key key, Value value)
        {
            List<Value> list = null;

            if (this.dict.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<Value>();
                list.Add(value);
                this.dict.Add(key, list);
            }
        }

        public bool Remove(Key key)
        {
            List<Value> list = null;
            if (this.dict.TryGetValue(key, out list))
                return this.dict.Remove(key);

            return false;
        }

        public void Clear()
        {
            this.dict.Clear();
        }

        public int Count
        {
            get
            {
                return this.dict.Count;
            }
        }

        public bool ContainsKey(Key key)
        {
            return this.dict.ContainsKey(key);
        }

        public bool ContainsValue(Value value)
        {
            foreach (var v in this.Values)
                if (object.Equals(v, value))
                    return true;

            return false;
        }

        public List<Value> this[Key key]
        {
            get
            {
                List<Value> list = null;
                if (!this.dict.TryGetValue(key, out list))
                {
                    list = new List<Value>();
                    this.dict.Add(key, list);
                }

                return list;
            }
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            var enumerator = this.dict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                var key = pair.Key;
                var list = pair.Value;
                for (int n = 0, cnt = list.Count; n < cnt; ++n)
                    yield return new KeyValuePair<Key, Value>(key, list[n]); // NOTE: for struct
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<Key> Keys
        {
            get
            {
                return this.dict.Keys;
            }
        }

        public IEnumerable<Value> Values
        {
            get
            {
                var enumerator = this.dict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var list = enumerator.Current.Value;
                    for (int n = 0, cnt = list.Count; n < cnt; ++n)
                        yield return list[n];
                }
            }
        }

        public IEnumerator<KeyValuePair<Key, List<Value>>> Elements
        {
            get
            {
                return this.dict.GetEnumerator();
            }
        }
    }
}

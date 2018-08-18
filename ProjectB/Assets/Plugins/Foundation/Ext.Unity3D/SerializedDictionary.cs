using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ext.Unity3D
{
    [Serializable]
    public class SerializedDictionary<Key, Value>
    {
        [SerializeField]
        Key[] builtKeys;
        [SerializeField]
        Value[] builtValues;

        public Key[] BuiltKeys
        {
            get { return this.builtKeys; }
        }
        public Key[] BuiltValues
        {
            get { return this.builtKeys; }
        }
        public void ResetBuiltDatas(KeyValuePair<Key, Value>[] pairs)
        {
            var count = null != pairs ? pairs.Length : 0;

            this.builtKeys = new Key[count];
            this.builtValues = new Value[count];

            for (int n = 0; n < count; ++n)
            {
                this.builtKeys[n] = pairs[n].Key;
                this.builtValues[n] = pairs[n].Value;
            }

            this.Rebuild();
        }
        public int BuiltCount
        {
            get { return null != this.builtKeys ? this.builtKeys.Length : 0; }
        }

        Dictionary<Key, Value> dict = null;

        public static implicit operator Dictionary<Key, Value>(SerializedDictionary<Key, Value> sdict)
        {
            return null != sdict ? sdict.AsDictionary : null;
        }

        public Dictionary<Key, Value> AsDictionary
        {
            get
            {
                if (null == this.dict)
                    this.Rebuild();

                return this.dict;
            }
        }

        public void Rebuild()
        {
            if (null == this.dict)
                this.dict = new Dictionary<Key, Value>(this.BuiltCount);
            else
                this.dict.Clear();

            for (int n = 0, cnt = this.BuiltCount; n < cnt; ++n)
                this.dict[this.builtKeys[n]] = this.builtValues[n];
        }
    }
}
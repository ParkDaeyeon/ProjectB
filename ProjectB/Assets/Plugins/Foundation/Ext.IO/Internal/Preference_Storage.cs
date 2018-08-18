using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ext.IO.Internal
{
    internal class Preference_Storage : Preference.Backend
    {
        internal Preference_Storage(string filename, bool load)
        {
            this.filename = filename;
            if (load)
                this.Load();
        }

        internal bool Load()
        {
            var content = default(string);
#if LOG_PREF
            UnityEngine.Debug.Log(string.Format("PREF_READ:{0}", filename));
#endif// LOG_PREF
            if (Storage.Exists(filename))
            {
                if (Storage.OK <= Storage.ReadAllText(filename, out content, Encoding.UTF8))
                {
#if LOG_PREF
                    UnityEngine.Debug.Log(string.Format("PREF_CONTENT:{0}", content));
#endif// LOG_PREF
                    try
                    {
                        var jobj = JObject.Parse(content);
                        this.datas = new Dictionary<string, Data>(jobj.Count);
                        foreach (var pair in jobj)
                        {
                            var key = pair.Key;
                            var value = pair.Value;
#if LOG_PREF
                            UnityEngine.Debug.Log(string.Format("PREF_K:{0}, V:{1}, TY:{2}",
                                                             key, value, (null != value ? value.Type.ToString() : "NullType")));
#endif// LOG_PREF
                            this.datas.Add(key, Data.FromJToken(value));
                        }
                        return true;
                    }
                    catch (Exception)
                    {
                        // ...
                    }
                }
            }
            
#if LOG_PREF
            UnityEngine.Debug.Log(string.Format("PREF_NOTFOUND:{0}", filename));
#endif// LOG_PREF
            this.datas = new Dictionary<string, Data>();
            this.Save();
            return false;
        }
        internal struct Data 
        {
            public enum ValueType
            {
                Int,
                Float,
                String,
            }
            internal Data(int value)
            {
                this.intValue = value;
                this.floatValue = default(float);
                this.stringValue = default(string);
                this.valueType = ValueType.Int;
            }
            internal Data(float value)
            {
                this.intValue = default(int);
                this.floatValue = value;
                this.stringValue = default(string);
                this.valueType = ValueType.Float;
            }
            internal Data(string value)
            {
                this.intValue = default(int);
                this.floatValue = default(float);
                this.stringValue = value;
                this.valueType = ValueType.String;
            }

            int intValue;
            internal int Int
            {
                get { return this.intValue; }
            }

            float floatValue;
            internal float Float
            {
                get { return this.floatValue; }
            }

            string stringValue;
            internal string String
            {
                get { return this.stringValue; }
            }

            ValueType valueType;
            internal ValueType GetValueType()
            {
                return this.valueType;
            }

            internal bool HasInt()
            {
                return ValueType.Int == this.valueType;
            }

            internal bool HasFloat()
            {
                return ValueType.Float == this.valueType;
            }

            internal bool HasString()
            {
                return ValueType.String == this.valueType;
            }
            
            public override string ToString()
            {
                switch (this.valueType)
                {
                case ValueType.Int: return this.intValue.ToString();
                case ValueType.Float: return this.floatValue.ToString();
                default: return this.stringValue;
                }
            }

            internal JToken ToJToken()
            {
                switch (this.valueType)
                {
                case ValueType.Int: return new JValue(this.intValue);
                case ValueType.Float: return new JValue(this.floatValue);
                default: return new JValue(this.stringValue);
                }
            }

            internal static Data FromJToken(JToken jtok)
            {
                var data = new Data();
                if (null != jtok)
                {
                    switch (jtok.Type)
                    {
                    case JTokenType.Integer:
                        data.intValue = jtok.ToObject<int>();
                        data.valueType = ValueType.Int;
                        break;
                    case JTokenType.Float:
                        data.floatValue = jtok.ToObject<float>();
                        data.valueType = ValueType.Float;
                        break;
                    default:
                        data.stringValue = jtok.ToObject<string>();
                        data.valueType = ValueType.String;
                        break;
                    }
                }
                return data;
            }
        }

        string filename;
        Dictionary<string, Data> datas;
        internal override void DeleteAll()
        {
#if LOG_PREF
            UnityEngine.Debug.Log(string.Format("PREF_DELETE_ALL:{0}", this.filename));
#endif// LOG_PREF
            Storage.Delete(this.filename);
        }
        internal override void DeleteKey(string key)
        {
            this.datas.Remove(key);
        }
        internal override float GetFloat(string key, float defaultValue)
        {
            Data data;
            if (this.datas.TryGetValue(key, out data))
            {
                if (data.HasFloat())
                    return data.Float;
            }
            return defaultValue;
        }
        internal override int GetInt(string key, int defaultValue)
        {
            Data data;
            if (this.datas.TryGetValue(key, out data))
            {
                if (data.HasInt())
                    return data.Int;
            }
            return defaultValue;
        }
        internal override string GetString(string key, string defaultValue)
        {
            Data data;
            if (this.datas.TryGetValue(key, out data))
            {
                if (data.HasString())
                    return data.String;
            }
            return defaultValue;
        }
        internal override bool HasKey(string key)
        {
            return this.datas.ContainsKey(key);
        }
        internal override bool Save()
        {
            if (!this.changed)
                return true;

            var jobj = new JObject();
            foreach (var pair in this.datas)
            {
                var key = pair.Key;
                var value = pair.Value;
                var jtok = value.ToJToken();
                if (null != jtok &&
                    JTokenType.Null != jtok.Type &&
                    JTokenType.None != jtok.Type)
                    jobj.Add(key, jtok);
            }

            var content = jobj.ToString(Formatting.None);
#if LOG_PREF
            UnityEngine.Debug.Log(string.Format("PREF_SAVE:{0}", this.filename));
#endif// LOG_PREF
            return Storage.OK <= Storage.WriteAllText(this.filename, content, Encoding.UTF8);
        }
        internal override void SetFloat(string key, float value)
        {
            this.datas[key] = new Data(value);
            this.changed = true;
        }
        internal override void SetInt(string key, int value)
        {
            this.datas[key] = new Data(value);
            this.changed = true;
        }
        internal override void SetString(string key, string value)
        {
            this.datas[key] = new Data(value);
            this.changed = true;
        }

        bool changed = false;
        protected bool Changed
        {
            set { this.changed = value; }
            get { return this.changed; }
        }
    }
}
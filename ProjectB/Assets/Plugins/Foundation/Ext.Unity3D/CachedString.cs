using Ext.IO;
namespace Ext.Unity3D
{
    public class CachedString : CachedValue
    {
        public CachedString(string key, string defaultValue = "")
            : base(key)
        {
            this.defaultValue = defaultValue;
        }


        string defaultValue = "";
        public string DefaultValue
        {
            set { this.defaultValue = value; }
            get { return this.defaultValue; }
        }

        public string Value
        {
            set { Preference.SetString(this.Key, !string.IsNullOrEmpty(value) ? value : ""); }
            get
            {
                var value = Preference.GetString(this.Key, this.defaultValue);
                if (!string.IsNullOrEmpty(value))
                    return value;

                return null;
            }
        }

        public static implicit operator string(CachedString thiz)
        {
            var value = null != thiz ? thiz.Value : "";
            return null != value ? value : "";
        }

        public override string ToString()
        {
            var value = this.Value;
            return null != value ? value : "";
        }

        public override bool Equals(object obj)
        {
            var value = this.Value;
            return null != value ? value.Equals(obj) : null == obj;
        }

        public override int GetHashCode()
        {
            var value = this.Value;
            return null != value ? value.GetHashCode() : 0;
        }
    }
}

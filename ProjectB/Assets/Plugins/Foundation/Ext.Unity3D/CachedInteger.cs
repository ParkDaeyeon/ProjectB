using Ext.IO;
namespace Ext.Unity3D
{
    public class CachedInteger : CachedValue
    {
        public CachedInteger(string key, int defaultValue = 0)
            : base(key)
        {
            this.defaultValue = defaultValue;
        }

        int defaultValue = 0;
        public int DefaultValue
        {
            set { this.defaultValue = value; }
            get { return this.defaultValue; }
        }

        public int Value
        {
            set { Preference.SetInt(this.Key, value); }
            get { return Preference.GetInt(this.Key, this.defaultValue); }
        }

        public static implicit operator int(CachedInteger thiz)
        {
            return null != thiz ? thiz.Value : 0;
        }
        
        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override bool Equals(object obj)
        {
            return this.Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}

using Ext.IO;
namespace Ext.Unity3D
{
    public class CachedFloat : CachedValue
    {
        public CachedFloat(string key, float defaultValue = 0)
            : base(key)
        {
            this.defaultValue = defaultValue;
        }

        float defaultValue = 0;
        public float DefaultValue
        {
            set { this.defaultValue = value; }
            get { return this.defaultValue; }
        }

        public float Value
        {
            set { Preference.SetFloat(this.Key, value); }
            get { return Preference.GetFloat(this.Key, this.defaultValue); }
        }

        public static implicit operator float(CachedFloat thiz)
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

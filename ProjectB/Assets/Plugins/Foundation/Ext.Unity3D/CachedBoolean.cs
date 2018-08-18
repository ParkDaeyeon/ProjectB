namespace Ext.Unity3D
{
    public class CachedBoolean : CachedInteger
    {
        public CachedBoolean(string key, bool defaultValue = false)
            : base(key, defaultValue ? 1 : 0)
        {
        }
        
        public new bool DefaultValue
        {
            set { base.DefaultValue = value ? 1 : 0; }
            get { return 0 != base.DefaultValue; }
        }

        public new bool Value
        {
            set { base.Value = value ? 1 : 0; }
            get { return 0 != base.Value; }
        }

        public static implicit operator bool(CachedBoolean thiz)
        {
            return null != thiz ? thiz.Value : false;
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

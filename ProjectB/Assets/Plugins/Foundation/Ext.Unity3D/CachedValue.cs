using Ext.IO;
namespace Ext.Unity3D
{
    public abstract class CachedValue
    {
        public CachedValue(string key) { this.key = key; }

        string key;
        public string Key { get { return this.key; } }


        public bool HasValue
        {
            get { return Preference.HasKey(this.key); }
        }

        public void Clear()
        {
            if (this.HasValue)
                Preference.DeleteKey(this.key);
        }
    }
}

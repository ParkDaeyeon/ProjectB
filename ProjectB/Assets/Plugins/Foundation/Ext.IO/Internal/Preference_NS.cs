#if UNITY_SWITCH
using Ext.NS;
namespace Ext.IO.Internal
{
    internal class Preference_NS : Preference_Storage
    {
        internal Preference_NS()
            : base("pref:/data.json", false)
        {
            NSEnv.AccessSavedata("pref", base.Load, false);
        }

        internal override bool Save()
        {
            if (!this.Changed)
                return true;

            return NSEnv.AccessSavedata("pref", base.Save, true);
        }
    }
}
#endif// UNITY_SWITCH

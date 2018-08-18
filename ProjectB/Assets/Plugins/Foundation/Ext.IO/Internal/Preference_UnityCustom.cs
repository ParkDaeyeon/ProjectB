using UnityEngine;

namespace Ext.IO.Internal
{
    internal class Preference_UnityCustom : Preference_Storage
    {
        internal Preference_UnityCustom(string name = null)
            : base(Application.persistentDataPath + "/" + (string.IsNullOrEmpty(name) ? "preference" : name), true)
        {
        }
    }
}

#if UNITY_5_6_OR_NEWER
using UnityEngine;

namespace Ext.IO.Internal
{
    class Preference_Unity3D : Preference.Backend
    {
        internal override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
        internal override void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        internal override float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        internal override int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
        internal override string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        internal override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        internal override bool Save()
        {
            PlayerPrefs.Save();
            return true;
        }
        internal override void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        internal override void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        internal override void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}
#endif// UNITY_5_6_OR_NEWER
using Ext.IO;
namespace Ext.Unity3D
{
    public static class KeyChain
    {
        public static void SetValue(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_IOS

#elif !UNITY_EDITOR && UNITY_ANDROID

#else// UNITY_*
            Preference.SetString(key, value);
            Preference.Save();
#endif// UNITY_*
    }

        public static string GetValue(string key, string defaultValue = null)
        {
#if !UNITY_EDITOR && UNITY_IOS
            return "";
#elif !UNITY_EDITOR && UNITY_ANDROID
            return "";
#else// UNITY_*
            return Preference.GetString(key, defaultValue);
#endif// UNITY_*
        }

        public static bool Contains(string key)
        {
#if !UNITY_EDITOR && UNITY_IOS
            return false;
#elif !UNITY_EDITOR && UNITY_ANDROID
            return false;
#else// UNITY_*
            return Preference.HasKey(key);
#endif// UNITY_*
        }

        public static void Delete(string key)
        {
#if !UNITY_EDITOR && UNITY_IOS
#elif !UNITY_EDITOR && UNITY_ANDROID
#else// UNITY_*
            Preference.DeleteKey(key);
            Preference.Save();
#endif// UNITY_*
        }
    }
}

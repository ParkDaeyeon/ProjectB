namespace Ext.IO
{
    public static class Preference
    {
        internal abstract class Backend
        {
            internal abstract void DeleteAll();
            internal abstract void DeleteKey(string key);
            internal abstract float GetFloat(string key, float defaultValue);
            internal abstract int GetInt(string key, int defaultValue);
            internal abstract string GetString(string key, string defaultValue);
            internal abstract bool HasKey(string key);
            internal virtual bool Save() { return true; }
            internal abstract void SetFloat(string key, float value);
            internal abstract void SetInt(string key, int value);
            internal abstract void SetString(string key, string value);
        }

        static Backend backend;
        public static void Open()
        {
            Preference.backend = new Internal.Preference_UnityCustom();
        }
        public static void Close()
        {
            //Preference.backend = null;
        }

        public static void DeleteAll()
        {
            Preference.backend.DeleteAll();
        }
        public static void DeleteKey(string key)
        {
            Preference.backend.DeleteKey(key);
        }
        public static float GetFloat(string key, float defaultValue = 0)
        {
            return Preference.backend.GetFloat(key, defaultValue);
        }
        public static int GetInt(string key, int defaultValue = 0)
        {
            return Preference.backend.GetInt(key, defaultValue);
        }
        public static string GetString(string key, string defaultValue = "")
        {
            return Preference.backend.GetString(key, defaultValue);
        }
        public static bool HasKey(string key)
        {
            return Preference.backend.HasKey(key);
        }
        public static bool Save()
        {
            return Preference.backend.Save();
        }
        public static void SetFloat(string key, float value)
        {
            Preference.backend.SetFloat(key, value);
        }
        public static void SetInt(string key, int value)
        {
            Preference.backend.SetInt(key, value);
        }
        public static void SetString(string key, string value)
        {
            Preference.backend.SetString(key, value);
        }
    }
}

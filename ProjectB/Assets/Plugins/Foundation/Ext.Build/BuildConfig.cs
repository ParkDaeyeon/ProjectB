using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif// UNITY_EDITOR

using System.IO;

using Newtonsoft.Json;

namespace Ext.Build
{
    public class BuildConfig
    {
        public int buildNumber = 0;
        public int versionBuiltIn = 1;
        public int versionApi = 1;
        public string manifestBaseUrl = "";
        public int channel = 0;
        public string channelString = "0";
        
        public static BuildConfig Load()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Data/build");
            if (textAsset)
            {
                return JsonConvert.DeserializeObject<BuildConfig>(textAsset.text);
            }

            var config = new BuildConfig();
#if UNITY_EDITOR
            BuildConfig.Save(config);
#endif// UNITY_EDITOR

            return config;
        }

#if UNITY_EDITOR
        public static void Save(BuildConfig config)
        {
            string name = "/Resources/Data/build.json";
            string filename = string.Format("{0}{1}", Application.dataPath, name);
            string foldername = Path.GetDirectoryName(filename);

            if (!Directory.Exists(foldername))
                Directory.CreateDirectory(foldername);

            File.WriteAllText(filename, JsonConvert.SerializeObject(config, Formatting.Indented, NoErrorSettings.Settings));
            
            AssetDatabase.ImportAsset(string.Format("Assets{0}", name), ImportAssetOptions.ForceUpdate);
        }
#endif// UNITY_EDITOR
    }
}

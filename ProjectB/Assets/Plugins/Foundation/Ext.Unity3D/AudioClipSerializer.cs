using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Ext.String;

namespace Ext.Unity3D
{
    public class AudioClipSerializer : ManagedComponent
    {
        [Serializable]
        public struct Data
        {
            public string key;
            public AudioClip clip;

            public Data(string key, AudioClip clip)
            {
                this.key = key;
                this.clip = clip;
            }
            public bool Valid()
            {
                return this.clip;
            }
        }

        [SerializeField]
        List<Data> imported;
        public void Build(List<Data> datas)
        {
            this.imported = null != datas ? datas : new List<Data>();
            this.Rebuild();
        }
        void Awake()
        {
            if (null == this.map)
                this.Rebuild();
        }

        Dictionary<string, Data> map;
        void Rebuild()
        {
            var count = this.imported.Count;
            if (null == this.map)
                this.map = new Dictionary<string, Data>(count);
            else
                this.map.Clear();

            for (int n = 0; n < count; ++n)
            {
                var data = this.imported[n];
                if (string.IsNullOrEmpty(data.key))
                    continue;
                if (!data.clip)
                    continue;

                if (this.map.ContainsKey(data.key))
                    this.map[data.key] = data;
                else
                    this.map.Add(data.key, data);
            }
        }
        public IEnumerable<Data> Clips
        {
            get { return this.imported; }
        }
        public int Count
        {
            get { return this.imported.Count; }
        }
        public Data GetAt(int index)
        {
            return -1 < index && index < this.Count ? this.imported[index] : default(Data);
        }
        public AudioClip Get(string key)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                this.Rebuild();
#endif// UNITY_EDITOR
            if (null == this.map)
                this.Rebuild();

            var data = default(Data);
            if (this.map.TryGetValue(key, out data))
                return data.clip;

            return null;
        }

#if UNITY_EDITOR
        [SerializeField]
        UnityEngine.Object[] editorImports;
        [SerializeField]
        bool editorRebuild;
        [SerializeField]
        bool editorRebuildAddMode;
        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorRebuild)
            {
                this.editorRebuild = false;
                if (!this.editorRebuildAddMode)
                    this.imported.Clear();

                foreach (var import in this.editorImports)
                {
                    if (!import)
                        continue;

                    if (import is AudioClip)
                    {
                        var filename = UnityEditor.AssetDatabase.GetAssetPath(import);
                        var clip = (AudioClip)import;
                        this.EditorAddClip(filename, clip);
                    }
                    else
                    {
                        var dirnameOrigin = UnityEditor.AssetDatabase.GetAssetPath(import);
                        if (string.IsNullOrEmpty(dirnameOrigin))
                            continue;
                        
                        var dirname = dirnameOrigin.ToSeparatedPath().Replace("Assets/", "");
                        if ('/' != dirname[0])
                            dirname = dirname.Insert(0, "/");
                        if ('/' != dirname[dirname.Length - 1])
                            dirname += '/';
                        dirname = Application.dataPath.Replace('\\', '/') + dirname;

                        if (!Directory.Exists(dirname))
                            continue;

                        var guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", new string[] { dirnameOrigin, });
                        foreach (string guid in guids)
                        {
                            var filename = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                            var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(filename);
                            this.EditorAddClip(filename, clip);
                        }
                    }
                }
            }
        }

        void EditorAddClip(string filename, AudioClip clip)
        {
            if (string.IsNullOrEmpty(filename))
                return;

            filename = filename.ToWithoutExtPath().Replace("Assets/Resources/", "");
            if ('/' == filename[0])
                filename = filename.Remove(0, 1);

            var index = this.imported.FindIndex((data) => data.key == filename);
            if (-1 != index)
                this.imported[index] = new Data(filename, clip);
            else
                this.imported.Add(new Data(filename, clip));
        }
#endif// UNITY_EDITOR
    }
}

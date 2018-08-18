using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using Ext.Unity3D;
public class ScanMissingAssets : ScriptableObject
{
    [MenuItem("Assets/Find Missing Assets")]
    public static void FindAll()
    {
        ScanMissingAssets.Run(false);
    }

    [MenuItem("Assets/Remove Missing Assets")]
    public static void RemoveAll()
    {
        ScanMissingAssets.Run(true);
    }

    public static void Run(bool deleteMissings)
    {
        var reports = ScanAssets.Report<MissingData>(CollectAssets.Collect(CollectAssets.Target.Hierarchy), ScanMissingAssets.Scan);
        if (0 < reports.Count)
        {
            var sb = new StringBuilder(1024);
            foreach (var missing in reports)
                sb.Append(missing.ToString()).Append('\n');

            sb.Append("MISSING COUNT:").Append(reports.Count);

            if (deleteMissings)
            {
                foreach (var missing in reports)
                {
                    var go = missing.owner;
                    var indexes = missing.indexes;

                    var so = new SerializedObject(go);
                    var prop = so.FindProperty("m_Component");

                    for (int n = indexes.Count - 1; n >= 0; --n)
                        prop.DeleteArrayElementAtIndex(indexes[n]);

                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(go);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(go.scene);
                }
            }

            Debug.Log(sb.ToString());
        }
        else
        {
            Debug.Log("MISSING NOT FOUND!");
        }
    }


    public struct MissingData
    {
        public string assetPath;
        public string hierarchyPath;
        public GameObject owner;
        public List<int> indexes;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.assetPath).Append('/').Append(this.hierarchyPath).Append(':').Append('[');
            for (int n = 0, cnt = this.indexes.Count; n < cnt; ++n)
            {
                if (0 != n)
                    sb.Append(',').Append(' ');
                sb.Append(this.indexes[n]);
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
    static void Scan(GameObject go,
                     string assetPath,
                     string hierarchyPath,
                     List<MissingData> reports)
    {
        var comps = go.GetComponents<Component>();
        var indexes = default(List<int>);
        for (int n = 0; n < comps.Length; ++n)
        {
            var component = comps[n];
            if (component)
                continue;

            if (null == indexes)
                indexes = new List<int>(comps.Length);

            indexes.Add(n);
        }

        if (null != indexes)
        {
            reports.Add(new MissingData
            {
                assetPath = assetPath,
                hierarchyPath = hierarchyPath,
                owner = go,
                indexes = indexes,
            });
        }
    }
}

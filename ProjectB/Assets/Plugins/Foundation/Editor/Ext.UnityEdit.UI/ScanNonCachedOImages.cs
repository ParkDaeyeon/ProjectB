using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using Ext.Unity3D;
using Ext.Unity3D.UI;
namespace Ext.UnityEdit.UI
{
    public class ScanNonCachedOImages : ScriptableObject
    {
        [MenuItem("Assets/Scan None Cached Optimized Images")]
        public static void Run()
        {
            var reports = ScanAssets.Report<string>(CollectAssets.Collect(CollectAssets.Target.Hierarchy), ScanNonCachedOImages.Scan);
            if (0 < reports.Count)
            {
                var sb = new StringBuilder(1024);
                foreach (string log in reports)
                    sb.Append(log).Append('\n');

                sb.Append("NON-CACHED COUNT:").Append(reports.Count);

                Debug.Log(sb.ToString());
            }
            else
            {
                Debug.Log("NON-CACHED NOT FOUND!");
            }
        }

        static void Scan(GameObject go,
                         string assetPath,
                         string hierarchyPath,
                         List<string> reports)
        {
            var comps = go.GetComponents<OptimizedImage>();
            for (int n = 0; n < comps.Length; ++n)
            {
                var component = comps[n];
                if (!component)
                    continue;

                if (!component.Cacheable)
                    continue;

                if (component.EditorCheckCached())
                    continue;
                
                reports.Add(string.Format("{0}/{1}, component idx:{1}", assetPath, hierarchyPath, n));
            }
        }
    }
}
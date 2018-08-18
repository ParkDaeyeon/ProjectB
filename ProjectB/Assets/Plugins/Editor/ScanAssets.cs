using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using Ext.Unity3D;
public class ScanAssets : ScriptableObject
{
    
    public static List<T> Report<T>(List<GameObject> checkList,
                                    Action<GameObject, string, string, List<T>> scanFunc)
    {
        List<T> reports = new List<T>();
        for (int n = 0; n < checkList.Count; ++n)
        {
            try
            {
                ScanAssets.Traversal<T>(checkList[n],
                                        reports,
                                        scanFunc);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        return reports;
    }

    static void Traversal<T>(GameObject go,
                             List<T> reports,
                             Action<GameObject, string, string, List<T>> scanFunc)
    {
        if (!go)
            return;

        try
        {
            var assetPath = AssetDatabase.GetAssetPath(go);
            var hierarchyPath = go.GetHirarchyPath(1);
            scanFunc(go, assetPath, hierarchyPath, reports);

            var trans = go.transform;
            for (int n = 0, cnt = trans.childCount; n < cnt; ++n)
            {
                var child = trans.GetChild(n);
                if (!child)
                    continue;

                ScanAssets.Traversal(child.gameObject,
                                     reports,
                                     scanFunc);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}

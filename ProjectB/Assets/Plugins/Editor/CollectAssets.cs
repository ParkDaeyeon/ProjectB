using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Text;
using Ext.Unity3D;
public class CollectAssets : ScriptableObject
{
    public enum Target
    {
        Project,
        Hierarchy,
    }
    public static List<GameObject> Collect(Target target)
    {
        var objs = Target.Project == target ? Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets | SelectionMode.Editable | SelectionMode.Assets)
                                            : Selection.gameObjects;
        var checkList = new List<GameObject>(objs.Length);
        foreach (UnityEngine.Object obj in objs)
        {
            try
            {
                var go = obj as GameObject;
                if (go)
                    checkList.Add(go);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        return checkList;
    }
}

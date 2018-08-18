using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ext.Unity3D.UI;
using Ext.Unity3D;

namespace Ext.UnityEdit.UI
{
    public class UICacheRefresher : ScriptableObject
    {
        [MenuItem("Assets/Refresh All Serializable UICaches")]
        public static void RefreshAll()
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                var dict = UnityExtension.FindAllSerializableInstancesInHierarchy<Component, UICache>(root);
                foreach (var pair in dict)
                {
                    var component = pair.Key;
                    if (!component)
                        continue;

                    var founds = pair.Value;
                    var order = -1;
                    foreach (var cache in founds)
                    {
                        ++order;
                        if (null == cache)
                            continue;

                        if (cache.GameObject)
                        {
                            cache.Rebuild();
                        }
                        else
                        {
                            Debug.Log(string.Format("FOUND NULL GO CACHE:{0}, TYPE:{1}, ORDER:{2}",
                                                     component.name, cache.GetType().Name, order));
                            
                            cache.Assign(component.gameObject);
                        }
                    }
                }
            }
        }
    }
}

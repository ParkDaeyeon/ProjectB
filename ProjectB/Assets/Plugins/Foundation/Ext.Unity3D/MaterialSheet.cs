using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public class MaterialSheet : ScriptableObject
    {
        public const string MatSheetname = "Material/materialSheet";
        public const string MatSheetFilename = "Assets/Resources/" + MaterialSheet.MatSheetname + ".prefab";

        [SerializeField]
        protected List<Material> _list = new List<Material>();
        public List<Material> List { get { return this._list; } }

        protected Dictionary<string, Material> _dist = new Dictionary<string, Material>();
        public Dictionary<string, Material> Dist { get { return this._dist; } }

        protected static MaterialSheet _inst;
        static public MaterialSheet Inst
        {
            get
            {
                if (MaterialSheet._inst == null)
                {
                    UnityEngine.Object obj = Resources.Load(MaterialSheet.MatSheetname);
                    if (obj)
                        MaterialSheet._inst = GameObject.Instantiate(obj as MaterialSheet);
                    else
                        MaterialSheet._inst = null;
                }
                return MaterialSheet._inst;
            }
        }

        static public Material Get(string name)
        {
            return MaterialSheet.Inst.GetMaterial(name);
        }

        // TODO: Sheet 에서는 Get 만 수행하고 children 을 set 하는건 다른 곳에서 일괄 처리하도록 하자
        static public void Set(Transform trans, string name, Action<Transform, string> algorithm)
        {
            if (algorithm != null)
                algorithm(trans, name);

            for (int i = 0, cnt = trans.childCount; i < cnt; ++i)
                MaterialSheet.Set(trans.GetChild(i), name, algorithm);
        }

        public Material GetMaterial(string name)
        {
            if (this._dist.ContainsKey(name))
            {
                return this._dist[name];
            }
            else
            {
                this._dist = new Dictionary<string, Material>(this._list.Count);
                Material mat;
                for (int i = 0, cnt = this._list.Count; i < cnt; ++i)
                {
                    mat = this._list[i];
                    this._dist.Add(mat.name, mat);
                }
                if (this._dist.ContainsKey(name))
                {
                    return this._dist[name];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
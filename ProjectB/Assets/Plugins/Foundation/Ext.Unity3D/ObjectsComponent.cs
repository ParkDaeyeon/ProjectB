using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Ext.Unity3D
{
    public class ObjectsComponent : ManagedComponent
    {
        [SerializeField]
        protected GameObject[] objects;
        public GameObject[] Array { get { return this.objects; } }
        public void Rebuild(GameObject[] newArray)
        {
            this.objects = newArray;
        }


        public GameObject this[int index]
        {
            get
            {
                if (null != this.objects && 0 <= index && index < this.objects.Length)
                    return this.objects[index];

                return null;
            }
        }

        public bool IsValid { get { return 0 < this.Count; } }


        public int Count { get { return null != this.objects ? this.objects.Length : 0; } }

        public int Last { get { return null != this.objects && 0 < this.objects.Length ? this.objects.Length - 1 : 0; } }




#if UNITY_EDITOR
        [SerializeField]
        bool editorRebuild;
        [SerializeField]
        Transform[] editorGroupBases;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            if (this.editorRebuild && null != this.editorGroupBases)
            {
                var list = new List<GameObject>();
                foreach (Transform group in this.editorGroupBases)
                {
                    foreach (Transform t in group)
                    {
                        list.Add(t.gameObject);
                    }
                }

                this.objects = list.ToArray();
            }
        }
#endif// UNITY_EDITOR
    }
}
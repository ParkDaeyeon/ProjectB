using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;
namespace Program
{
    public class ForceAwakeComponent : MonoBehaviour
    {
        protected void Awake()
        {
            Transform t = this.transform;
            Component[] components = t.GetComponentsInChildren<Component>(true);
            for (int n = 0, count = components.Length; n < count; ++n)
            {
                var c = components[n];
                if (c)
                    c.gameObject.ForceAwake();
            }

            this.OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
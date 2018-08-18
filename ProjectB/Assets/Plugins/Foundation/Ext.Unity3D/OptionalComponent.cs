using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Ext.Unity3D
{
    public class OptionalComponent : MonoBehaviour
    {
        [TextArea(1, 10)]
        [SerializeField]
        string options = "";
        public string Options
        {
            set { this.options = value; }
            get { return this.options; }
        }

        [SerializeField]
        List<Component> specificComponents;
        public List<Component> SpecificComponents
        {
            set { this.specificComponents = value; }
            get { return this.specificComponents; }
        }

        public bool HasOption(string keyword)
        {
            if (string.IsNullOrEmpty(this.options))
                return false;

            return this.options.Contains(keyword);
        }
    }
}

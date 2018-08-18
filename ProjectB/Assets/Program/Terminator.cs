using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEngine.EventSystems;

using Ext.Unity3D;

using ReadWriteCsv;
using Newtonsoft.Json;

using Program.Core;
using Program.Model.Service;
using Program.Model.Service.Implement;
//using Program.Presents;
using Program.View;
using Program.View.Common;

namespace Program
{
    public class Terminator : MonoBehaviour
    {
        [SerializeField]
        GameObject appObject;

        void OnDestroy()
        {
            AppImpl.Close();
            App.Close();

            if (this.appObject)
                GameObject.Destroy(this.appObject);
            this.appObject = null;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;
using Ext.Unity3D.UI;
using Ext.Event;
using Program.Core;
using Ext.IO;

namespace Program
{
    public class StorageComponent : ManagedComponent
    {
#if UNITY_EDITOR
        [SerializeField]
        public bool editorTestDeletePlayerPrefs = false;
        [SerializeField]
        public bool editorTestUninstallAsset = false;
        protected override void OnEditorTesting()
        {
            base.OnEditorTesting();

            if (this.editorTestDeletePlayerPrefs)
            {
                this.editorTestDeletePlayerPrefs = false;

                PlayerPrefs.DeleteAll();
            }

            if (this.editorTestUninstallAsset)
            {
                this.editorTestUninstallAsset = false;

                Preference.DeleteAll();
            }
        }
#endif// UNITY_EDITOR
    }
}

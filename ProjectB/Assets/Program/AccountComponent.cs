//using UnityEngine;
//using UnityEngine.UI;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Ext.Unity3D;
//using Ext.Unity3D.UI;
//using Ext.Event;
//using Program.Model.Service.Content;

//namespace Program
//{
//    public class AccountComponent : ManagedComponent
//    {
//#if UNITY_EDITOR
//        [SerializeField]
//        Model.Domain.Type.AuthProviderType editorTestAuth;
//        [SerializeField]
//        string editorTestUserId;
//        [SerializeField]
//        string editorTestPassword;
//        [SerializeField]
//        bool editorTestRestore = false;
//        [SerializeField]
//        bool editorTestChange = false;

//        protected override void OnEditorTesting()
//        {
//            base.OnEditorTesting();

//            if (this.editorTestRestore)
//            {
//                this.editorTestRestore = false;

//                var requireClose = false;
//                if (!Account.IsOpened)
//                {
//                    Account.Open();
//                    requireClose = true;
//                }

//                this.editorTestAuth = Account.AuthProvider;
//                this.editorTestUserId = Account.UserId;
//                this.editorTestPassword = Account.Password;

//                if (requireClose)
//                    Account.Close();
//            }

//            if (this.editorTestChange)
//            {
//                this.editorTestChange = false;

//                var requireClose = false;
//                if (!Account.IsOpened)
//                {
//                    Account.Open();
//                    requireClose = true;
//                }

//                Account.SetAccount(this.editorTestAuth, this.editorTestUserId, this.editorTestPassword);

//                if (requireClose)
//                    Account.Close();
//            }
//        }
//#endif// UNITY_EDITOR
//    }
//}

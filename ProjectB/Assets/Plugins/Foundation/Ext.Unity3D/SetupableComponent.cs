using UnityEngine;
using System;
using System.Collections;
namespace Ext.Unity3D
{
    public abstract class SetupableComponent : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        bool editorAutoSetup = false;
        [SerializeField]
        bool editorAutoSetupInChildrens = false;
        [SerializeField]
        bool editorTesting = false;
        [SerializeField]
        bool editorTestingLooped = false;
        void OnDrawGizmos()
        {
            if (this.editorAutoSetup || this.editorAutoSetupInChildrens)
            {
                this.EditorTestingLoopedStop();

                if (this.editorAutoSetup)
                {
                    this.editorAutoSetup = false;
                    this.EditorSetting();
                }

                if (this.editorAutoSetupInChildrens)
                {
                    this.editorAutoSetupInChildrens = false;
                    this.EditorSetting(true);
                }

                if (this.editorTesting)
                {
                    this.EditorTesting();
                }

                if (this.editorTestingLooped)
                {
                    this.EditorTestingLoopedPlay();
                }
            }
            
            this.OnEditorUpdateAndDrawGizmos();
        }


        public void EditorSetting(bool children = false)
        {
            if (!this)
                return;

            var trans = this.transform;
            if (!trans)
            {
                Debug.LogWarning(string.Format("SETUPABLE_COMPONENT:BAD_TRANSFORM:{0}", this.name));
                return;
            }

            if (children)
            {
                SetupableComponent.OnEditorSettingInChildren(trans);
            }
            else
            {
                this.OnEditorSetting();
                this.OnEditorPostSetting();
            }
        }

        protected virtual void OnEditorSetting() {}
        protected virtual void OnEditorPostSetting()
        {
            UnityExtension.SetDirtyAll(this.transform);
        }

        
        static void OnEditorSettingInChildren(Transform trans)
        {
            if (!trans)
                return;

            var components = trans.GetComponents<SetupableComponent>();
            foreach (var component in components)
            {
                if (component)
                    component.OnEditorSetting();
            }

            for (int n = 0; n < trans.childCount; ++n)
            {
                var child = trans.GetChild(n);
                if (!child)
                    continue;

                SetupableComponent.OnEditorSettingInChildren(child);
            }

            // NOTE: Post 는 역순으로 칠드런 부터 돌아야 한다.
            components = trans.GetComponents<SetupableComponent>();
            foreach (var component in components)
            {
                if (component)
                    component.OnEditorPostSetting();
            }
        }

        protected virtual void OnEditorUpdateAndDrawGizmos() { }


        public void EditorTesting()
        {
            this.OnEditorTesting();
        }
        protected virtual void OnEditorTesting()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }


        bool editorTestingLoopedStatus = false;
        public void EditorTestingLoopedPlay()
        {
            if (this.editorTestingLoopedStatus)
                return;

            var notPlaying = false == UnityEditor.EditorApplication.isPlaying;
            if (notPlaying)
            {
                this.editorTestingLoopedStatus = true;

                UnityEditor.EditorApplication.update -= this.EditorTestingLoopedCheckForUpdate;
                UnityEditor.EditorApplication.update += this.EditorTestingLoopedCheckForUpdate;
            }
            else
                Debug.LogWarning("SETUPABLE_COMPONENT:CAN_NOT_PLAY_TESTING_LOOPED:FOR_ONLY_NON_PLAY_MODE");
        }

        public void EditorTestingLoopedStop()
        {
            if (!this.editorTestingLoopedStatus)
                return;

            this.editorTestingLoopedStatus = false;
            UnityEditor.EditorApplication.update -= this.EditorTestingLoopedCheckForUpdate;
        }

        void EditorTestingLoopedCheckForUpdate()
        {
            var notPlaying = false == UnityEditor.EditorApplication.isPlaying;
            if (notPlaying && this.editorTestingLooped && this.editorTestingLoopedStatus)
            {
                this.OnEditorTestingLooped();
                return;
            }

            this.EditorTestingLoopedStop();
        }

        protected virtual void OnEditorTestingLooped()
        {
            if (this)
                UnityEditor.EditorUtility.SetDirty(this);
            else
                UnityEditor.EditorApplication.update -= this.EditorTestingLoopedCheckForUpdate;
        }
#endif// UNITY_EDITOR
    }
}

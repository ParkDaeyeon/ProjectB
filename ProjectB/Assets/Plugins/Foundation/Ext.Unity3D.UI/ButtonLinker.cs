using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;

namespace Ext.Unity3D.UI
{
    [Serializable]
    public abstract class ButtonLinker<ButtonType, FollowerType> : ManagedUIComponent
        where ButtonType : Button
        where FollowerType : MonoBehaviour
    {
        [SerializeField]
        protected ButtonType target;
        protected virtual void ChangeTo(ButtonType newTarget)
        {
            this.target = newTarget;
        }

        [SerializeField]
        protected List<FollowerType> followers;
        public IEnumerable<FollowerType> Followers { get { return this.followers; } }
        public int Count { get { return this.followers.Count; } }
        public FollowerType this[int index] { get { return -1 < index && index < this.Count ? this.followers[index] : null; } }

        public void ResetFollowers(IEnumerable<FollowerType> followers)
        {
            this.followers.Clear();
            this.followers.AddRange(followers);
            this.ApplyFollowers();
        }

        void Awake()
        {
            this.ApplyFollowers();
        }

        public virtual void ApplyFollowers()
        {
#if UNITY_EDITOR && LOG_DEBUG
            this.EditorDebug();
#endif// UNITY_EDITOR && LOG_DEBUG
        }

        public virtual void DisableFollowers()
        {
        }

        void OnDestroy()
        {
            this.DisableFollowers();
        }

#if UNITY_EDITOR
        //TODO : delete editorRebuildFollowers 
        [SerializeField]
        bool editorRebuildFollowers = false;
        [SerializeField]
        Transform[] editorRebuildIgnoreTranses;

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            this.target = this.FindComponent<ButtonType>();
        }

        protected override void OnEditorPostSetting()
        {
            base.OnEditorPostSetting();

            if (this.editorRebuildFollowers)
            {
                this.editorRebuildFollowers = false;
                this.EditorCollectFollowers();
            }
        }

        protected virtual void EditorCollectFollowers()
        {
            if (!this.target)
                return;

            var foundedFollowers = new List<FollowerType>();
            foundedFollowers.AddRange(this.FindComponentsInChildren<FollowerType>());

            for (int n = foundedFollowers.Count - 1; n >= 0; --n)
            {
                var f = foundedFollowers[n];
                if (f == this.target.targetGraphic)
                {
                    foundedFollowers.RemoveAt(n);
                    continue;
                }

                if (null != this.editorRebuildIgnoreTranses)
                {
                    var checkNext = true;
                    var transFollower = f.transform;
                    foreach (var transIgnore in this.editorRebuildIgnoreTranses)
                    {
                        do
                        {
                            if (transFollower == transIgnore)
                            {
                                foundedFollowers.RemoveAt(n);
                                checkNext = false;
                                break;
                            }

                            transFollower = transFollower.parent;
                        }
                        while (transFollower);

                        if (!checkNext)
                            break;
                    }
                }
            }
            this.ResetFollowers(foundedFollowers);
        }

#if LOG_DEBUG
        //Note : Button Linker 컴포넌트의 target, followers중 null값이 있는지 체크하기 위해 추가됨
        //       null값이 있을 경우 Debug.LogError 호출
        //                없을 경우 Debug.Log 호출
        [SerializeField]
        bool editorDebugActive = true;
        bool editorDebugIsError = false;
        protected bool EditorDebugIsError
        {
            set { this.editorDebugIsError = value; }
            get { return this.editorDebugIsError; }
        }

        //TODO : 만약 EditorDebug 기능이 다른 컴포넌트들에도 계속 추가된다면 
        //       SetupableComponent.EditorSetting 아래로 가상 함수를 추가해주세요
        void EditorDebug()
        {
            if (!this.editorDebugActive)
                return;

            this.EditorDebugInit();

            var log = this.EditorDebugGetLog();

            var sb = new StringBuilder();
            sb.AppendLine(log.ToString());

            if (this.EditorDebugIsError)
                Debug.LogError(sb);
            else
                Debug.Log(sb);
        }


        protected virtual void EditorDebugInit()
        {
            this.EditorDebugIsError = false;
        }

        protected virtual StringBuilder EditorDebugGetLog()
        {
            var sb = new StringBuilder();

            var currentStateOfComponent = this.EditorDebugGetButtonLinkerStateLog();

            sb.AppendLine("[" + this.GetType() + ".EditorDebug" + "]")
              .AppendLine(currentStateOfComponent.ToString());

            return sb;
        }

        string EditorDebugGetButtonLinkerStateLog()
        {
            var sb = new StringBuilder();

            if (null == this.target)
                this.editorDebugIsError = true;

            var targetResult        = null != this.target ? this.target.name : "null";

            var followersResult = new StringBuilder();
            if (null != this.followers)
            {
                for (int n = 0, cnt = this.followers.Count; n < cnt; ++n)
                {
                    var f = this.followers[n];

                    if (null == f)
                        this.editorDebugIsError = true;

                    var fResult = null != f ? f.name : "null";

                    followersResult.AppendLine("index :" + n + "\t name :" + fResult);
                }
            }

            var thisGameObjectPath = this.EditorDebugGetPathOfGameObject(this.gameObject);

            sb.AppendLine("Path is\t"      + "\n{\n" + thisGameObjectPath         + "\n}\n")
              .AppendLine("Target is\t"    + "\n{\n" + targetResult               + "\n}\n")
              .AppendLine("Followers is\t" + "\n{\n" + followersResult.ToString() + "\n}\n");

            return sb.ToString();
        }

        //TODO : 아래 함수를 전역함수로 만들거나 or SetupableComponent Debug용 인터페이스 함수로 추가하세요
        //NOTE : ComponentSearchTool 에디터에서 가져온 코드
        string EditorDebugGetPathOfGameObject(GameObject go)
        {
            string name = go.name;

            while (null != go.transform.parent)
            {
                go = go.transform.parent.gameObject;
                name = go.name + "/" + name;
            }
            return name;
        }
#endif// LOG_DEBUG
#endif// UNITY_EDITOR
    }
}

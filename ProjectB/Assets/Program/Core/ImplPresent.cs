using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using Ext;
using Ext.Unity3D;

using Program.View;
using Ext.Async;

namespace Program.Core
{
    public abstract class ImplPresent<T> : Present
        where T : PresentView
    {
        protected override void OnOpen(object openArg)
        {
            base.OnOpen(openArg);

            this.view = this.AbstractView as T;
            this.view.ResetLayout();
        }

        protected override void OnClose(Type nextPresent)
        {
            this.view = null;

            base.OnClose(nextPresent);
        }

        T view;
        public T View
        {
            get { return this.view; }
        }


        public bool IsStarted
        {
            get { return this.view && this.view.IsStarted; }
        }



        protected void SetStateGotoNextPresentWithFO(Type nextPresentType,
                                                     float delay = 0.125f,
                                                     object openArg = null,
                                                     Action<Action> onWait = null,
                                                     Action onComplete = null,
                                                     AsyncOperation preloaded = null)
        {
            this.SetState(99999);

            var h = Wait.Active(9999999, true, null);
            if (null == preloaded)
            {
                preloaded = SceneManager.LoadSceneAsync(nextPresentType.Name, LoadSceneMode.Additive);
                preloaded.allowSceneActivation = false;
            }

            var start = Time.time;
            var view = this.View;

            this.AddStateTask(() =>
            {
                var alpha = 1 - Mathf.Clamp01((Time.time - start) / delay);
                view.GroupAlpha = alpha;
                if (0 < alpha)
                    return;

                if (0.9f <= preloaded.progress)
                {
                    Wait.Stop(h, false, true);

                    if (null != onWait)
                    {
                        onWait(() =>
                        {
                            Present.OpenNextPresent(nextPresentType, new Present.LoadRuleData(preloaded), openArg, onComplete);
                        });

                        this.ClearStateTask();
                    }
                    else
                        Present.OpenNextPresent(nextPresentType, new Present.LoadRuleData(preloaded), openArg, onComplete);
                }
            });
        }
    }
}

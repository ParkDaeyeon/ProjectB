using Program.Core;
using Program.Model.Service;
using Program.View;
using Program.View.Content;
using UnityEngine;

namespace Program.Presents
{
    public class Title : ImplPresent<TitleView>
    {
        public enum State
        {
            LoadAssets,
            ReadyToStart,
            Out,
        }


        protected override void OnOpen(object openArg)
        {
            base.OnOpen(openArg);

            // TODO: go to state load assets
            this.SetStateLoadAssets();
        }

        void SetStateLoadAssets()
        {
            this.SetState(State.LoadAssets);

            Asset.Start();

            // TODO: LoadAssets and set state to ready
            this.SetStateReadyToStart();
        }

        void SetStateReadyToStart()
        {
            this.SetState(State.ReadyToStart);

            this.AddStateTask(() =>
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    this.SetStateOut();
            });
        }

        void SetStateOut()
        {
            this.SetState(State.Out);

            Fade.Out(Color.black, 0.5f, () =>
            {
                // TODO: 임시로 ingame으로 보냄 나중에 아웃게임에 씬 생기면 그 때 변경
                Present.OpenNextPresent(typeof(Ingame));
            });
        }
    }
}

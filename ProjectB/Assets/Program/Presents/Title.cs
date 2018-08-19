using Program.Core;
using Program.View.Content;

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

            // TODO: LoadAssets and set state to ready
        }

        void SetStateReadyToStart()
        {
            this.SetState(State.ReadyToStart);

            // TODO: go to state out
            this.SetStateOut();
        }

        void SetStateOut()
        {
            this.SetState(State.Out);

            // TODO: 임시로 ingame으로 보냄 나중에 아웃게임에 씬 생기면 그 때 변경
            // Present.OpenNextPresent(typeof(Ingame));
        }
    }
}

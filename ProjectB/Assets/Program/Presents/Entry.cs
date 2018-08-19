using Program.Core;
using Program.View;

namespace Program.Presents
{
    public class Entry : ImplPresent<EntryView>
    {
        public enum State
        {
            Logo,
            Out,
        }


        protected override void OnOpen(object openArg)
        {
            base.OnOpen(openArg);

            // TODO: set state to logo
            this.SetStateLogo();
        }

        void SetStateLogo()
        {
            this.SetState(State.Logo);

            // TODO: set state to out
            this.SetStateOut();
        }

        void SetStateOut()
        {
            this.SetState(State.Out);

            Present.OpenNextPresent(typeof(Title));
        }
    }
}

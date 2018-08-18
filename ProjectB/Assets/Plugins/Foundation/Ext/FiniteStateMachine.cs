using System;
using System.Text;

namespace Ext
{
    public class FiniteStateMachine : IDisposable
    {
        // NOTE: A modified version of the State Pattern. A functor is not always needed
        int state = -1;
        public void SetState(int state)
        {
            this.ClearState();
            this.state = state;
        }
        public void SetState<T>(T state)
        {
            this.SetState(state.GetHashCode());
        }
        public int GetState()
        {
            return this.state;
        }
        public void ClearState()
        {
            this.state = -1;
            this.ClearStateTask();
            var closeTasks = this.stateCloseEvents;
            this.ClearStateCloseEvent();
            if (null != closeTasks)
                closeTasks();
        }
        public bool CompareState(int state)
        {
            return this.state == state;
        }
        public bool CompareState<T>(T state)
        {
            return this.CompareState(state.GetHashCode());
        }

        public void Dispose()
        {
            this.ClearState();
            this.ClearTask();
        }

        event Action stateTasks;
        public void AddStateTask(Action task)
        {
            this.stateTasks += task;
        }
        public void RemoveStateTask(Action task)
        {
            this.stateTasks -= task;
        }
        public void ResetStateTask(Action task)
        {
            this.stateTasks = task;
        }
        public void ClearStateTask()
        {
            this.stateTasks = null;
        }
        public bool HasStateTasks()
        {
            return null != this.stateTasks;
        }
        public void RunStateTasks()
        {
            if (null != this.stateTasks)
                this.stateTasks();
        }

        event Action stateCloseEvents;
        public void AddStateCloseEvent(Action @event)
        {
            this.stateCloseEvents += @event;
        }
        public void RemoveStateCloseEvent(Action @event)
        {
            this.stateCloseEvents -= @event;
        }
        public void ResetStateCloseEvent(Action @event)
        {
            this.stateCloseEvents = @event;
        }
        public void ClearStateCloseEvent()
        {
            this.stateCloseEvents = null;
        }
        public bool HasStateCloseEvents()
        {
            return null != this.stateCloseEvents;
        }
        
        event Action tasks;
        public void AddTask(Action task)
        {
            this.tasks += task;
        }
        public void RemoveTask(Action task)
        {
            this.tasks -= task;
        }
        public void ResetTask(Action task)
        {
            this.tasks = task;
        }
        public void ClearTask()
        {
            this.tasks = null;
        }
        public bool HasTasks()
        {
            return null != this.tasks;
        }
        public void RunTasks()
        {
            if (null != this.tasks)
                this.tasks();
        }

        static string ToEventString(Action events)
        {
            if (null == events)
                return "";

            var actions = events.GetInvocationList();
            var sb = new StringBuilder();
            sb.Append('[');
            for (int n = 0, cnt = actions.Length; n < cnt; ++n)
            {
                if (0 < n)
                    sb.Append(", ");
                var action = actions[n];
                sb.Append('\"').Append(null != action ? action.Method.Name : "null").Append('\"');
            }
            sb.Append(']');
            return sb.ToString();
        }
        public override string ToString()
        {
            return string.Format("{{\"state\": {0}, \"stateTasks\": {1}, \"stateCloseEvents\": {2}, \"tasks\": {3}}}",
                                 this.state,
                                 FiniteStateMachine.ToEventString(this.stateTasks),
                                 FiniteStateMachine.ToEventString(this.stateCloseEvents),
                                 FiniteStateMachine.ToEventString(this.tasks));
        }
    }
}

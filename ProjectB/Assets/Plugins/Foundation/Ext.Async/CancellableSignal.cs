using System;
using System.Diagnostics;
namespace Ext.Async
{
    public class CancellableSignal
    {
        public delegate bool BooleanFunction();
        BooleanFunction func;
        bool forceCancel = false;

        string comment;

        public CancellableSignal(BooleanFunction func = null, string comment = null, int skipStackFrame = 1)
        {
            this.func = func;

            if (string.IsNullOrEmpty(comment))
            {
#if LOG_DEBUG && UNITY_EDITOR
                StackFrame frame = new StackTrace(skipStackFrame, true).GetFrame(0);
                this.comment = string.Format("{0}.{1}() in {2}, line:{3}", frame.GetMethod().DeclaringType, frame.GetMethod().Name, frame.GetFileName(), frame.GetFileLineNumber());
#else// LOG_DEBUG && UNITY_EDITOR
                this.comment = "";
#endif// LOG_DEBUG && UNITY_EDITOR
            }
            else
            {
                this.comment = comment;
            }
        }

        public static bool IsCancelled(CancellableSignal signal)
        {
            if (null == signal)
                return false;

            bool ret = false;
            if (signal.forceCancel)
                ret = true;
            else if (null != signal.func)
                ret = signal.func();

#if LOG_SIGNAL
            if (ret)
                Debug.Log("CANCELED:" + signal);
#endif// LOG_SIGNAL

            return ret;
        }

        public static void Cancel(CancellableSignal signal)
        {
            if (null == signal)
                return;

            signal.forceCancel = true;
        }

        public static void Reset(CancellableSignal signal)
        {
            if (null == signal)
                return;

            signal.forceCancel = false;
        }

        public override string ToString()
        {
            return null == this.comment ? "" : this.comment;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Ext.Debugging
{
#if LOG_DEBUG
    public static class ClosableDebugger
    {
        static List<string> openList = new List<string>(1024);

        public static void Open(string tag)
        {
            ClosableDebugger.openList.Add(tag);
        }

        public static bool Close(string tag)
        {
            int idx = ClosableDebugger.openList.IndexOf(tag);
            if (-1 != idx)
            {
                ClosableDebugger.openList.RemoveAt(idx);
                return true;
            }

            return false;
        }

        public static void Clear()
        {
            ClosableDebugger.openList.Clear();
        }

        public static string Openeds
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                sb.Append("open:").Append(ClosableDebugger.openList.Count).Append(" tags\n");

                for (int n = 0; n < ClosableDebugger.openList.Count; ++n)
                    sb.Append(n).Append('.').Append(' ').Append(ClosableDebugger.openList[n]).Append('\n');

                return sb.ToString();
            }
        }
    }
#endif// LOG_DEBUG
}

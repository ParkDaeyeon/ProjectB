using System;
using System.Reflection;
using System.Diagnostics;
namespace Ext.Debugging
{
#if LOG_DEBUG && UNITY_EDITOR
    public static class LogHelper
    {
        public class SourceCode
        {
            public SourceCode(int skipStackFrame = 1)
            {
                this.frame = new StackTrace(skipStackFrame, true).GetFrame(0);
                this.method = this.frame.GetMethod();
            }

            StackFrame frame;
            MethodBase method;

            public string Method
            {
                get
                {
                    return string.Format("{0}.{1}()", this.method.DeclaringType, this.method.Name);
                }
            }

            public string FileName
            {
                get
                {
                    return this.frame.GetFileName();
                }
            }

            public int LineNumber
            {
                get
                {
                    return this.frame.GetFileLineNumber();
                }
            }

            public override string ToString()
            {
                return string.Format("{0} in {1}, line:{2}", this.Method, this.FileName, this.LineNumber);
            }
        }

        public static SourceCode CurrentSourceCode
        {
            get
            {
                return new SourceCode(2);
            }
        }

        public static string CurrentMethod
        {
            get
            {
                return new SourceCode(2).Method;
            }
        }
    }
#endif// LOG_DEBUG && UNITY_EDITOR
}
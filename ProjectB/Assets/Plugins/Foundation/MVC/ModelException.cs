using System;
namespace MVC
{
    public class ModelException : Exception
    {
        public static Action<ModelException> ExceptHandler { set; get; }

        public string Clazz { private set; get; }
        public string Function { private set; get; }
        public string Arguments { private set; get; }
        public Exception Except { private set; get; }

        public ModelException(string clazz, string func, string args, Exception except)
        {
            this.Clazz = clazz;
            this.Function = func;
            this.Arguments = args;
            this.Except = except;
        }

        public override string ToString()
        {
            return string.Format("METHOD:{0}.{1}, EXCEPT:{2},\nSOURCE:{3}",
                                 this.Clazz,
                                 this.Function,
                                 this.Except.ToString(),
                                 string.IsNullOrEmpty(this.Arguments) ? "" : this.Arguments);
        }


        public static string ToStringOrNull<T>(T value)
        {
            Type type = typeof(T);
            if (type.IsValueType)
                return value.ToString();
            else if (null != value)
                return value.ToString();

            return "null";
        }
    }
}
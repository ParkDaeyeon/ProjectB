using System;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json
{
    public class NoErrorSettings
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Error = NoErrorSettings.OnError,
        };
        public static JsonSerializerSettings Settings
        {
            get { return NoErrorSettings.settings; }
        }

        public static void OnError(object sender, ErrorEventArgs args)
        {
            NoErrorSettings.lastError = args.ErrorContext.Error;
            args.ErrorContext.Handled = true;
        }

        static Exception lastError = null;
        public static Exception LastError
        {
            get { return NoErrorSettings.lastError; }
        }
    }
}

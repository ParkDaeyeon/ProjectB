using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Newtonsoft.Json
{
    public class ToStringSettings
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Error = ToStringSettings.OnError,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
            },
        };
        public static JsonSerializerSettings Settings
        {
            get { return ToStringSettings.settings; }
        }

        public static void OnError(object sender, ErrorEventArgs args)
        {
            ToStringSettings.lastError = args.ErrorContext.Error;
            args.ErrorContext.Handled = true;
        }

        static Exception lastError = null;
        public static Exception LastError
        {
            get { return ToStringSettings.lastError; }
        }
    }
}

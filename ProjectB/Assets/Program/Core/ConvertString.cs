using Newtonsoft.Json;

namespace Program.Core
{
    public static class ConvertString
    {
        public static string Execute(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, ToStringSettings.Settings);
        }
    }
}

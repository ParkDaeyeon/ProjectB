#if LOG_DEBUG || LOG_MEMORY || LOG_NET || LOG_ASSET || LOG_STATUS
using System;
using UnityEngine.Profiling;

using THIS = Program.Model.Service.Implement.DebugImpl;
namespace Program.Model.Service.Implement
{
    public static class DebugImpl
    {
        public static string NormalizedString(string source, int maximumLength, char padChar, bool padRight)
        {
            if (string.IsNullOrEmpty(source))
                return "";

            if (maximumLength < source.Length)
            {
                source = source.Substring(maximumLength - 1, source.Length - maximumLength);
                return source;
            }
            else if (maximumLength > source.Length)
            {
                if (padRight)
                    return source.PadRight(maximumLength, padChar);
                else
                    return source.PadLeft(maximumLength, padChar);
            }
            else
                return source;
        }

        public static string CreatePresentChunk(string prefix, Type presentType, int state)
        {
            var format = "{0}:[P:{1}, S:0x{2:X8}]";
            return string.Format(format,
                                 prefix,
                                 THIS.NormalizedString(null != presentType ? presentType.Name : "_", 20, '_', true),
                                 state);
        }

        static DateTime dateTime = DateTime.Now;
        public static TimeSpan GetUptime()
        {
            return DateTime.Now - DebugImpl.dateTime;
        }
        public static string CreateUptimeChunk(string prefix = "NOW")
        {
            var format = "{0}:[{1:D2}d {2:D2}:{3:D2}:{4:D2}.{5:D3}]";
            var uptime = DebugImpl.GetUptime();
            return string.Format(format,
                                 prefix,
                                 uptime.Days,
                                 uptime.Hours,
                                 uptime.Minutes,
                                 uptime.Seconds,
                                 uptime.Milliseconds);
        }

        public static string CreateMemoryChunk(string prefix)
        {
            var format = "{0}:[HEAP:{1:0000.00}mb, FREE:{2:0000.00}mb, MONO_HEAP:{3:0000.00}mb, MONO_FREE:{4:0000.00}mb]";
            return string.Format(format,
                                 prefix,
                                 Convert.ToDouble((double)Profiler.GetTotalReservedMemoryLong() / 1024 / 1024),
                                 Convert.ToDouble((double)Profiler.GetTotalUnusedReservedMemoryLong() / 1024 / 1024),
                                 Convert.ToDouble((double)Profiler.GetMonoHeapSizeLong() / 1024 / 1024),
                                 Convert.ToDouble((double)(Profiler.GetMonoHeapSizeLong() - Profiler.GetMonoUsedSizeLong()) / 1024 / 1024));
        }
    }
}
#endif// LOG_DEBUG || LOG_MEMORY || LOG_NET || LOG_ASSET || LOG_STATUS
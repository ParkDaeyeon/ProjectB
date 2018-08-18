using System;
using System.Globalization;
using System.Text;
namespace Ext
{
    public static class TimeExtension
    {
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1, 9, 0, 0, DateTimeKind.Utc);
        public static DateTime BaseTime = TimeExtension.UnixBaseTime;

        public static readonly long MinMs = 0;
        public static readonly long MaxMs = DateTime.MaxValue.DateTimeToUnixTimeMs();
        public static readonly DateTime MinDate = TimeExtension.MinMs.MsToDateTime();
        public static readonly DateTime MaxDate = TimeExtension.MaxMs.MsToDateTime();

        public static long DateTimeToUnixTimeSec(this DateTime thiz)
        {
            return TimeExtension.DateTimeTickToSec((thiz - TimeExtension.BaseTime).Ticks);
        }
        public static long DateTimeToUnixTimeMs(this DateTime thiz)
        {
            return TimeExtension.DateTimeTickToMs((thiz - TimeExtension.BaseTime).Ticks);
        }
        public static long DateTimeToUnixTimeUs(this DateTime thiz)
        {
            return TimeExtension.DateTimeTickToUs((thiz - TimeExtension.BaseTime).Ticks);
        }

        public static DateTime UsToDateTime(this long us)
        {
            return TimeExtension.BaseTime.AddTicks(TimeExtension.UsToDateTimeTick(us));
        }
        public static DateTime MsToDateTime(this long ms)
        {
            return TimeExtension.BaseTime.AddTicks(TimeExtension.MsToDateTimeTick(ms));
        }
        public static DateTime SecToDateTime(this long sec)
        {
            return TimeExtension.BaseTime.AddTicks(TimeExtension.SecToDateTimeTick(sec));
        }


        public static long DateTimeTickToUs(long tick)
        {
            return tick / 10L;
        }
        public static long DateTimeTickToMs(long tick)
        {
            return tick / 10000L;
        }
        public static long DateTimeTickToSec(long tick)
        {
            return tick / 10000000L;
        }

        public static long UsToDateTimeTick(long us)
        {
            return us * 10L;
        }
        public static long UsToMs(long us)
        {
            return us / 1000L;
        }
        public static long UsToSec(long us)
        {
            return us / 1000000L;
        }

        public static long MsToDateTimeTick(long ms)
        {
            return ms * 10000L;
        }
        public static long MsToUs(long ms)
        {
            return ms * 1000L;
        }
        public static long MsToSec(long ms)
        {
            return ms / 1000L;
        }

        public static long SecToDateTimeTick(long sec)
        {
            return sec * 10000000L;
        }
        public static long SecToUs(long sec)
        {
            return sec * 1000000L;
        }
        public static long SecToMs(long sec)
        {
            return sec * 1000L;
        }


        static StringBuilder sbDate = new StringBuilder(64);

        public static string ToDateString(this int dateNum)
        {
            int __yy = (dateNum / 10000) % 100;
            int mm = (dateNum % 10000) / 100;
            int dd = dateNum % 100;

            TimeExtension.sbDate.Length = 0;

            if (10 > __yy)
                TimeExtension.sbDate.Append('0');
            TimeExtension.sbDate.Append(__yy).Append(". ");

            if (10 > mm)
                TimeExtension.sbDate.Append('0');
            TimeExtension.sbDate.Append(mm).Append(". ");

            if (10 > dd)
                TimeExtension.sbDate.Append('0');
            TimeExtension.sbDate.Append(dd);

            return TimeExtension.sbDate.ToString();
        }


        public static DateTime ToDate(this string thiz, string format)
        {
            DateTime date = default(DateTime);

            if (!string.IsNullOrEmpty(thiz))
                date = DateTime.ParseExact(thiz, format, CultureInfo.InvariantCulture);

            return date;
        }

        public static DateTime ToDateFromDashStyle120(this string thiz)
        {
            return thiz.ToDate("yyyy-MM-dd HH':'mm':'ss");
        }
    }
}
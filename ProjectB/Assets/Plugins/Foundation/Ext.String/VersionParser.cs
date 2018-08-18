using System;
namespace Ext.String
{
    public static class VersionParser
    {
        public static int[] Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
                return new int[0];

            var strings = version.Split('.');
            var values = new int[strings.Length];
            for (int n = 0, cnt = values.Length; n < cnt; ++n)
                if (!int.TryParse(strings[n], out values[n]))
                    values[n] = 0;

            return values;
        }

        public static int ParseAt(string version, int at)
        {
            if (string.IsNullOrEmpty(version) || 0 > at) // NOTE: INVALID_PARAM
                return 0;

            int n = 0;
            int startIndex = 0;
            int index = 0;
            do
            {
                startIndex = index;
                if (-1 == (index = version.IndexOf('.', startIndex + 1)))
                    break;
            }
            while (n++ < at);

            string str = -1 != index ? version.Substring(startIndex, index - startIndex) : 0 != startIndex ? version.Substring(startIndex) : version;
            if (string.IsNullOrEmpty(str))
                return 0;

            int value;
            if (!int.TryParse(str, out value))
                value = 0;

            return value;
        }

        public static int ParseMajor(string version)
        {
            return VersionParser.ParseAt(version, 0);
        }

        public static int ParseMinor(string version)
        {
            return VersionParser.ParseAt(version, 1);
        }
    }
}
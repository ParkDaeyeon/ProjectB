using System;
using System.Text;
using System.Collections.Generic;
namespace Ext.String
{
    public static class UriQuery
    {
        public static string ExtractHeadText(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                int idx = uri.IndexOf('?');
                if (-1 != idx)
                    return uri.Substring(0, idx);
            }

            return uri;
        }

        public static string ExtractQueryText(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                int idx = uri.IndexOf('?');
                if (-1 != idx)
                    return uri.Substring(idx + 1);
            }

            return null;
        }

        public static Dictionary<string, string> ExtractKeyValues(string uri)
        {
            ParseInfo parseInfo = UriQuery.Parse(uri);
            return parseInfo.keyValues;
        }

        public class ParseInfo
        {
            public string headText = "";
            public string queryText = "";
            public Dictionary<string, string> keyValues = new Dictionary<string, string>();
        }

        public static ParseInfo Parse(string uri)
        {
            ParseInfo parseInfo = new ParseInfo();

            int queryIndex = uri.IndexOf('?');

            if (-1 != queryIndex)
            {
                parseInfo.headText = uri.Substring(0, queryIndex);
                parseInfo.queryText = uri.Substring(queryIndex + 1);
                UriQuery.ParseQueryText(parseInfo.queryText, parseInfo.keyValues);
            }
            else
            {
                parseInfo.headText = uri;
            }

            return parseInfo;
        }

        public static void ParseQueryText(string queryText, Dictionary<string, string> getKeyValues)
        {
            if (null == queryText)
                return;

            if (null == getKeyValues)
                return;

            string[] s = queryText.Split('&');
            string[] s2;
            for (int n = 0, count = s.Length; n < count; ++n)
            {
                s2 = s[n].Split('=');

                if (2 == s2.Length)
                {
                    getKeyValues.Add(s2[0].TrimChar('"'), s2[1].TrimChar('"'));
                }
            }
        }

        public static ParseInfo Parse(string uri, bool isEncoded)
        {
            ParseInfo parseInfo = new ParseInfo();

            int queryIndex = uri.IndexOf('?');

            if (-1 != queryIndex)
            {
                parseInfo.headText = uri.Substring(0, queryIndex);
                parseInfo.queryText = uri.Substring(queryIndex + 1);
                UriQuery.ParseQueryText(parseInfo.queryText, parseInfo.keyValues, isEncoded);
            }
            else
            {
                parseInfo.headText = uri;
            }

            return parseInfo;
        }

        public static void ParseQueryText(string queryText, Dictionary<string, string> getKeyValues, bool isEncoded)
        {
            if (null == queryText)
                return;

            if (null == getKeyValues)
                return;

            string[] s = queryText.Split('&');
            for (int n = 0, count = s.Length; n < count; ++n)
            {
                int idx = s[n].IndexOf('=');
                if (-1 != idx)
                {
                    string key = s[n].Substring(0, idx).TrimChar('"');
                    string value = s[n].Substring(idx + 1).TrimChar('"');

                    if (isEncoded || value.Contains("%"))
                        value = Uri.UnescapeDataString(value);

                    getKeyValues.Add(key, value);
                }
            }
        }
    }
}
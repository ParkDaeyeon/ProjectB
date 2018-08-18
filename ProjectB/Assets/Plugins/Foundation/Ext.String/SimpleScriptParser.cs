using System;
using System.Text;
using System.Collections.Generic;
namespace Ext.String
{
    public static class SimpleScriptParser
    {
        public struct Data
        {
            public string keyword;
            public string value;
        }

        public static char[] DEFAULT_TRIM_CHARS = new char[] { ' ', };
        public static bool TryParseLine(string line, out Data data, string operatorKeyword = "=", char[] trimChars = null)
        {
            if (null == trimChars)
                trimChars = SimpleScriptParser.DEFAULT_TRIM_CHARS;
            line = line.Trim(trimChars);

            int idx = line.IndexOf(operatorKeyword);
            if (-1 != idx)
            {
                data.keyword = line.Substring(0, idx);
                data.value = line.Substring(idx + 1);
                return true;
            }

            data.keyword = line;
            data.value = null;
            return false;
        }

        public static List<Data> Parse(string[] lines, string operatorKeyword = "=", char[] trimChars = null)
        {
            List<Data> datas = new List<Data>(lines.Length);

            Data data;
            for (int n = 0; n < lines.Length; ++n)
            {
                if (SimpleScriptParser.TryParseLine(lines[n], out data, operatorKeyword, trimChars))
                    datas.Add(data);
            }

            return datas;
        }

        public static char[] DEFAULT_SEPARATORS = new char[] { '\n', };
        public static List<Data> Parse(string script, string operatorKeyword = "=", char[] trimChars = null, char[] separators = null)
        {
            if (null == separators)
                separators = SimpleScriptParser.DEFAULT_SEPARATORS;

            return SimpleScriptParser.Parse(script.Split(separators, StringSplitOptions.RemoveEmptyEntries), operatorKeyword, trimChars);
        }
    }
}
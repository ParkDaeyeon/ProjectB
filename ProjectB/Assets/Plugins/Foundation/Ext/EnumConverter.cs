using System;
using System.Text;

namespace Ext
{
    public static class EnumConverter
    {
        public enum Style
        {
            CSharp = 0,
            Java,
        }

        public static string ToString<T>(T enumValue, Style style = Style.CSharp)
        {
            return EnumConverter.ToString(enumValue.ToString(), style);
        }
        
        public static bool TryValue<T>(string stringValue, out T enumValue, Style style = Style.CSharp)
        {
            stringValue = EnumConverter.ToString(stringValue, style);
            try
            {
                var stringValues = Enum.GetNames(typeof(T));
                for (int n = 0, cnt = stringValue.Length; n < cnt; ++n)
                {
                    if (stringValues[n] == stringValue)
                    {
                        enumValue = (T)Enum.Parse(typeof(T), stringValue);
                        return true;
                    }
                }
            }
            catch (Exception) { }

            enumValue = default(T);
            return false;
        }



            static StringBuilder sb = new StringBuilder(128);
        public static string ToString(string stringValue, Style style = Style.CSharp)
        {
            if (string.IsNullOrEmpty(stringValue))
                return stringValue;

            var sb = EnumConverter.sb;
            sb.Length = 0;

            switch (style)
            {
            case Style.CSharp:
                {
                    bool isSkipUnderbar = false;
                    bool isFirstUpperChar = true;
                    for (int n = 0, cnt = stringValue.Length; n < cnt; ++n)
                    {
                        var c = stringValue[n];
                        if ('_' == c)
                        {
                            isFirstUpperChar = true;

                            if (isSkipUnderbar)
                                continue;
                        }

                        isSkipUnderbar = true;

                        if (char.IsUpper(c))
                        {
                            if (isFirstUpperChar)
                                isFirstUpperChar = false;
                            else
                                c = char.ToLower(c);
                        }

                        sb.Append(c);
                    }
                    return sb.ToString();
                }

            case Style.Java:
                {
                    for (int n = 0, cnt = stringValue.Length; n < cnt; ++n)
                    {
                        var c = stringValue[n];
                        if (char.IsUpper(c))
                        {
                            if (0 != n)
                                sb.Append('_');
                        }
                        else if (char.IsDigit(c))
                        {
                            sb.Append('_');
                        }
                        else if (char.IsLower(c))
                        {
                            c = char.ToUpper(c);
                        }

                        sb.Append(c);
                    }
                    return sb.ToString();
                }
                
            default:
                return stringValue;
            }
        }
    }
}

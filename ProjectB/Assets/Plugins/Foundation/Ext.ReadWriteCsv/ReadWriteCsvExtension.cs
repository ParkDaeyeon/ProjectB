using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ReadWriteCsv;
using Ext.String;
namespace ReadWriteCsv
{
    public static class ReadWriteCsvExtension
    {
        public struct CsvToJsonParseInfo
        {
            public string key;
            public enum TYPE
            {
                Plain,
                String,
                ArrayOpen,
                ArrayClose,
                ObjectOpen,
                ObjectClose,
            }
            public TYPE type;

            public CsvToJsonParseInfo(string key, TYPE type)
            {
                this.key = key;
                this.type = type;
            }
        }
        public static string CsvToJson(this string thiz, CsvToJsonParseInfo[] headers, bool useIndent = false, int skipRows = 0, int skipColumns = 0)
        {
            if (thiz == null)
                return null;

            StringBuilder sb = new StringBuilder(thiz.Length);
            sb.Append('[');

            int indent = 0;
            if (useIndent)
            {
                sb.Append('\n');
                ++indent;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(thiz);
                    writer.Flush();
                    stream.Position = 0;

                    using (CsvFileReader reader = new CsvFileReader(stream))
                    {
                        CsvRow row = new CsvRow();

                        for (int n = 0; n < skipRows; ++n)
                            reader.ReadRow(row);

                        if (null == headers)
                        {
                            reader.ReadRow(row);
                            string[] stringHeaders = row.ToArray();
                            headers = new CsvToJsonParseInfo[headers.Length];
                            for (int n = 0; n < stringHeaders.Length; ++n)
                                headers[n] = new CsvToJsonParseInfo(stringHeaders[n], CsvToJsonParseInfo.TYPE.String);
                        }

                        while (reader.ReadRow(row))
                        {
                            if (useIndent)
                            {
                                sb.AppendIndent(indent);
                            }

                            sb.Append('{');

                            if (useIndent)
                            {
                                sb.Append('\n');
                                ++indent;
                            }


                            int dataColumn = skipColumns;
                            for (int n = 0; n < headers.Length; ++n)
                            {
                                switch (headers[n].type)
                                {
                                case CsvToJsonParseInfo.TYPE.Plain:
                                case CsvToJsonParseInfo.TYPE.String:
                                case CsvToJsonParseInfo.TYPE.ArrayOpen:
                                case CsvToJsonParseInfo.TYPE.ObjectOpen:
                                    {
                                        if (CsvToJsonParseInfo.TYPE.Plain == headers[n].type || CsvToJsonParseInfo.TYPE.String == headers[n].type)
                                        {
                                            if (string.IsNullOrEmpty(row[dataColumn]))
                                            {
                                                ++dataColumn;
                                                continue;
                                            }
                                        }

                                        if (useIndent)
                                        {
                                            sb.AppendIndent(indent);
                                        }

                                        if (!string.IsNullOrEmpty(headers[n].key))
                                            sb.Append('\"').Append(headers[n].key).Append('\"').Append(':').Append(' ');

                                        switch (headers[n].type)
                                        {
                                        case CsvToJsonParseInfo.TYPE.Plain:
                                            if (row.Count <= dataColumn)
                                                throw new Exception("Field count must match header count. row.Count:" + row.Count + ", dataColumn:" + dataColumn);
                                            sb.Append(row[dataColumn++]).Append(',');
                                            break;

                                        case CsvToJsonParseInfo.TYPE.String:
                                            if (row.Count <= dataColumn)
                                                throw new Exception("Field count must match header count. row.Count:" + row.Count + ", dataColumn:" + dataColumn);

                                            var value = row[dataColumn++];
                                            if (-1 != value.IndexOf('\"'))
                                                value = value.Replace("\"", "\\\"");
                                            sb.Append('\"').Append(value).Append('\"').Append(',');
                                            break;

                                        case CsvToJsonParseInfo.TYPE.ArrayOpen:
                                            if (useIndent)
                                            {
                                                sb.Append('\n');
                                                sb.AppendIndent(indent);
                                            }
                                            sb.Append('[');
                                            break;

                                        case CsvToJsonParseInfo.TYPE.ObjectOpen:
                                            if (useIndent)
                                            {
                                                sb.Append('\n');
                                                sb.AppendIndent(indent);
                                            }
                                            sb.Append('{');
                                            break;
                                        }

                                        if (useIndent)
                                        {
                                            sb.Append('\n');

                                            switch (headers[n].type)
                                            {
                                            case CsvToJsonParseInfo.TYPE.ArrayOpen:
                                            case CsvToJsonParseInfo.TYPE.ObjectOpen:
                                                ++indent;
                                                break;
                                            }
                                        }

                                        break;
                                    }

                                case CsvToJsonParseInfo.TYPE.ArrayClose:
                                case CsvToJsonParseInfo.TYPE.ObjectClose:
                                    {
                                        if (useIndent)
                                        {
                                            --indent;
                                            sb.AppendIndent(indent);
                                        }

                                        switch (headers[n].type)
                                        {
                                        case CsvToJsonParseInfo.TYPE.ArrayClose:
                                            sb.Append(']').Append(',');
                                            break;

                                        case CsvToJsonParseInfo.TYPE.ObjectClose:
                                            sb.Append('}').Append(',');
                                            break;
                                        }

                                        if (useIndent)
                                        {
                                            sb.Append('\n');
                                        }

                                        break;
                                    }
                                }
                            }

                            if (useIndent)
                            {
                                --indent;
                                sb.AppendIndent(indent);
                            }

                            sb.Append('}').Append(',');

                            if (useIndent)
                            {
                                sb.Append('\n');
                            }
                        }
                    }
                }
            }

            sb.Append(']');

            if (useIndent)
            {
                sb.Append('\n');
            }


            return sb.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWriteCsv;
using System.IO;

namespace Ext
{
    public class CurrencySymbol
    {
        // NOTE: iso4217
        string code;
        public string Code { get { return this.code; } }

        string symbol;
        public string Symbol { get { return this.symbol; } }

        string appleSymbol;
        public string AppleSymbol { get { return this.appleSymbol; } }

        public CurrencySymbol(string code, string symbol, string appleSym)
        {
            this.code = code;
            this.symbol = !string.IsNullOrEmpty(symbol) ? symbol : code;
            this.appleSymbol = !string.IsNullOrEmpty(appleSym) ? appleSym : code;
        }




        // NOTE: static interface

        static Dictionary<string, CurrencySymbol> map = new Dictionary<string, CurrencySymbol>();
        public static CurrencySymbol GetSymbol(string code)
        {
            var map = CurrencySymbol.map;
            CurrencySymbol sym;
            if (map.TryGetValue(code, out sym))
                return sym;

            return null;
        }

        public static IEnumerable<CurrencySymbol> Symbols
        {
            get { return CurrencySymbol.map.Values; }
        }

        public static int Count
        {
            get { return CurrencySymbol.map.Count; }
        }

        public static void Update(byte[] csvBin)
        {
            using (var stream = new MemoryStream(csvBin))
                CurrencySymbol.Update(stream);
        }
        public static void Update(Stream csv)
        {
            var map = CurrencySymbol.map;
            map.Clear();

            using (var reader = new CsvFileReader(csv))
            {
                // NOTE: HEADER
                reader.ReadLine();
                
                var row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    var code = row[0];
                    if (string.IsNullOrEmpty(code))
                        continue;

                    if (map.ContainsKey(code))
                        continue;

                    var symbol = row[1];
                    var appleSym = row[2];
                    map.Add(code, new CurrencySymbol(code, symbol, appleSym));
                }
            }
        }
        
        public static void Clear()
        {
            var map = CurrencySymbol.map;
            map.Clear();
        }
    }
}

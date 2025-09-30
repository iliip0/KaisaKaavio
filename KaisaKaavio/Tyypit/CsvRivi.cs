using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    public class CsvRivi
    {
        private Dictionary<string, string> arvot = new Dictionary<string, string>();

        public CsvRivi()
        {
        }

        public void Set(string key, string value)
        {
            arvot[key] = value != null ? value : string.Empty;
        }

        public string Get(string key, string defaultValue)
        {
            if (arvot.ContainsKey(key))
            {
                return arvot[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public int GetInt(string key, int defaultValue)
        {
            var s = Get(key, defaultValue.ToString());
            int i;
            if (Int32.TryParse(s, out i))
            {
                return i;
            }

            return defaultValue;
        }
    }
}

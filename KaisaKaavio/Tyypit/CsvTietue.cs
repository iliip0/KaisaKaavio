using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    public class CsvTietue
    {
        public List<string> Avaimet { get; private set; } = new List<string>();
        public List<CsvRivi> Rivit { get; private set; } = new List<CsvRivi>();

        public CsvTietue()
        { 
        }

        public void Lue(string teksti)
        {
            this.Avaimet.Clear();
            this.Rivit.Clear();

            if (string.IsNullOrEmpty(teksti))
            {
                return;
            }

            var rivit = teksti.Split('\n');
            if (rivit.Length > 0)
            {
                Avaimet.AddRange(rivit[0].Split(';').Select(x => CsvDecode(x)));
            }

            if (!Avaimet.Any())
            {
                return;
            }

            foreach (var rivi in rivit.Skip(1))
            {
                CsvRivi r = new CsvRivi();

                var arvot = rivi.Split(';').Select(x => CsvDecode(x)).ToArray();
                for (int i = 0; i < arvot.Length && i < this.Avaimet.Count; ++i)
                {
                    r.Set(Avaimet[i], arvot[i]);
                }

                this.Rivit.Add(r);
            }
        }

        private string CsvDecode(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return s
                .Replace("#c#", ";")
                .Replace("#r#", "\r")
                .Replace("#n#", "\n");
        }
    }
}

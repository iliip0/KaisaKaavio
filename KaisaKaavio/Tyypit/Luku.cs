using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    public class Luku
    {
        /// <summary>
        /// Parsii ensimmäisen tekstissä esiintyvän kokonaisluvun.
        /// Esim "Aikuiset 50€ / Juniorit ja Naiset 25€" palauttaisi 50
        /// </summary>
        public static bool ParsiKokonaisluku(string teksti, out int luku)
        {
            luku = 0;

            if (string.IsNullOrEmpty(teksti))
            {
                return false;
            }

            StringBuilder lukuString = new StringBuilder();

            bool lukuAlkanut = false;

            foreach (var c in teksti)
            {
                if (Char.IsDigit(c))
                {
                    lukuString.Append(c);
                    lukuAlkanut = true;
                }
                else 
                {
                    if (lukuAlkanut)
                    {
                        break;
                    }
                }
            }

            return Int32.TryParse(lukuString.ToString(), out luku);
        }
    }
}

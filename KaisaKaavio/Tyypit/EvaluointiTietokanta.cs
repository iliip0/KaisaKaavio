using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Tietorakenne, joka tallentaa evaluoidut kaaviotilanteet
    /// haun nopeuttamiseksi kaavion loppupäässä.
    /// 
    /// Hakuavaimet (kaavion tilanteet) koodataan 64 bittiin tilan säästämiseksi ja hakujen nopeuttamiseksi
    /// Haun tulos koodataan 16 bittiin tilan säästämiseksi
    /// </summary>
    public class EvaluointiTietokanta
    {
        public class ParasHaku
        {
            public int Hakija = -1;
            public int Vastustaja = -1;
            public int Pisteytys = int.MaxValue;
        }

        private static Dictionary<ulong, ushort> haut = new Dictionary<ulong, ushort>();
        private static int EvaluoitujaTilanteita = 0;
        private static int Osumia = 0;

        public static ParasHaku AnnaParasHakuTilanteessa(ulong avain)
        {
            if (haut.TryGetValue(avain, out ushort mask))
            {
                Osumia++;
                return new ParasHaku()
                {
                    Hakija = (mask & 0xE000) >> 13,
                    Vastustaja = (mask & 0x1C00) >> 10,
                    Pisteytys = (mask & 0x03FF)
                };
            }

            return null;
        }

        public static void TallennaHaku(ulong avain, int hakijanIndeksi, int vastustajanIndeksi, int pisteytys)
        {
            pisteytys = Math.Min(pisteytys, 1023);

            ushort mask = (ushort)((hakijanIndeksi << 13) | (vastustajanIndeksi << 10) | pisteytys);

            haut.Add(avain, mask);

            EvaluoitujaTilanteita++;

            //Debug.WriteLine(string.Format("# {0} = {1}, {2}", avain, vastustajanIndeksi, pisteytys));
        }

        public static void Tulosta()
        {
#if DEBUG
            Debug.WriteLine(string.Format("### Evaluoituja tilanteita {0}, osumia {1}",
                EvaluoitujaTilanteita,
                Osumia));
#endif
        }
    }
}

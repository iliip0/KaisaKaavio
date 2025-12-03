using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Tietorakenne, joka tallentaa evaluoidut kaaviotilanteet
    /// haun nopeuttamiseksi kaavion loppupäässä
    /// </summary>
    public class EvaluointiTietokanta
    {
        /*
        public class Evaluointi
        {
            /// <summary>
            /// Kaavion tilanteen evaluoitu arvo. Mitä pienempi, sitä parempi
            /// </summary>
            public int Pisteytys = int.MaxValue;

            /// <summary>
            /// Mahdolliseen hakuvirheeseen liittyvä pelaaja 1
            /// </summary>
            public Pelaaja Pelaaja1 = null;

            /// <summary>
            /// Mahdolliseen hakuvirheeseen liittyvä pelaaja 2
            /// </summary>
            public Pelaaja Pelaaja2 = null;

            /// <summary>
            /// Evaluoidun kaaviotilanteen kuvaus
            /// </summary>
            public string Kuvaus = string.Empty;

#if DEBUG
            public string DebugKuvaus = string.Empty;
#endif
        }
        */

        public class ParasHaku
        {
            public int Hakija = -1;
            public int Vastustaja = -1;
            public int Pisteytys = int.MaxValue;
        }

        public int AlkuKierros { get; private set; }

        //private Dictionary<string, Evaluointi> evaluoinnit = new Dictionary<string, Evaluointi>();
        private static Dictionary<ulong, ushort> haut = new Dictionary<ulong, ushort>();
        private static int EvaluoitujaTilanteita = 0;
        private static int Osumia = 0;

        public EvaluointiTietokanta(int kierros)
        {
            this.AlkuKierros = kierros;
        }

        /*
        public bool HaeEvaluointi(string avain, out Evaluointi evaluointi)
        {
            if (this.evaluoinnit.TryGetValue(avain, out evaluointi))
            {
                Osumia++;
                return true;
            }

            evaluointi = null;
            return false;
        }
        */
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

#if DEBUG
            int hakija = (mask & 0xE000) >> 13;
            if (hakija != hakijanIndeksi)
            {
                int iii = 0;
            }

            int vastustaja = (mask & 0x1C00) >> 10;
            if (vastustaja != vastustajanIndeksi)
            {
                int jjj = 0;
            }

            int pisteet = (mask & 0x03FF);
            if (pisteet != pisteytys)
            {
                int kkk = 0;
            }

            if (haut.ContainsKey(avain))
            {
                int aaa = 0;
            }
#endif
            haut.Add(avain, mask);
            /*
            {
                Hakija = hakijanIndeksi,
                Vastustaja = vastustajanIndeksi,
                Pisteytys = pisteytys
            });
            */
            EvaluoitujaTilanteita++;

            //Debug.WriteLine(string.Format("# {0} = {1}, {2}", avain, vastustajanIndeksi, pisteytys));
        }

        /*
        public void TallennaEvaluointi(string avain, Evaluointi evaluointi)
        {
            this.evaluoinnit.Add(avain, evaluointi);
            EvaluoitujaTilanteita++;
        }
        */
        public static void Tulosta()
        {
#if DEBUG
            Debug.WriteLine(string.Format("### Evaluoituja tilanteita {0}, osumia {1}",
                EvaluoitujaTilanteita,
                Osumia));
#endif
        }
        /*
        public void Tyhjenna()
        {
            this.evaluoinnit.Clear();
        }
        */
    }
}

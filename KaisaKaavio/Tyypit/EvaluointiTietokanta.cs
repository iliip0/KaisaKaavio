using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Tietorakenne, joka tallentaa evaluoidut kaaviotilanteet
    /// haun nopeuttamiseksi kaavion loppupäässä
    /// </summary>
    public class EvaluointiTietokanta
    {
        public class Evaluointi
        {
            /// <summary>
            /// Kaavion tilanteen evaluoitu arvo. Mitä pienempi, sitä parempi
            /// </summary>
            public float Pisteytys = float.MaxValue;

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
        }

        public int AlkuKierros { get; private set; }

        private Dictionary<string, Evaluointi> evaluoinnit = new Dictionary<string, Evaluointi>();
        private static Dictionary<string, int> haut = new Dictionary<string, int>();
        private static int EvaluoitujaTilanteita = 0;
        private static int Osumia = 0;

        public EvaluointiTietokanta(int kierros)
        {
            this.AlkuKierros = kierros;
        }

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

        public static bool HaeVastustaja(string avain, out int vastustajanIndeksi)
        {
            if (haut.TryGetValue(avain, out vastustajanIndeksi))
            {
                Osumia++;
                return true;
            }

            vastustajanIndeksi = -1;
            return false;
        }

        public static void TallennaHaku(string avain, int vastustajanIndeksi)
        {
            haut.Add(avain, vastustajanIndeksi);
            EvaluoitujaTilanteita++;
        }

        public void TallennaEvaluointi(string avain, Evaluointi evaluointi)
        {
            this.evaluoinnit.Add(avain, evaluointi);
            EvaluoitujaTilanteita++;
        }

        public static void Tulosta()
        {
#if DEBUG
            Debug.WriteLine(string.Format("### Evaluoituja tilanteita {0}, osumia {1}",
                EvaluoitujaTilanteita,
                Osumia));
#endif
        }

        public void Tyhjenna()
        {
            this.evaluoinnit.Clear();
        }
    }
}

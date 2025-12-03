using System;
using System.Collections.Generic;
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

#if DEBUG
            public string DebugKuvaus = string.Empty;
#endif
        }

        public class ParasHaku
        {
            public int Hakija = -1;
            public int Vastustaja = -1;
            public float Pisteytys = float.MaxValue;
        }

        public int AlkuKierros { get; private set; }

        private Dictionary<string, Evaluointi> evaluoinnit = new Dictionary<string, Evaluointi>();
        private static Dictionary<string, ParasHaku> haut = new Dictionary<string, ParasHaku>();
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

        public static ParasHaku AnnaParasHakuTilanteessa(string avain)
        {
            ParasHaku haku = null;
            if (haut.TryGetValue(avain, out haku))
            {
                Osumia++;
                return haku;
            }

            return null;
        }

        public static void TallennaHaku(string avain, int hakijanIndeksi, int vastustajanIndeksi, float pisteytys)
        {
            haut.Add(avain, new ParasHaku() 
            {
                Hakija = hakijanIndeksi,
                Vastustaja = vastustajanIndeksi,
                Pisteytys = pisteytys
            });

            EvaluoitujaTilanteita++;

            //Debug.WriteLine(string.Format("# {0} = {1}, {2}", avain, vastustajanIndeksi, pisteytys));
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

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
            /// Kaavion evaluoitu arvo. Mitä pienempi, sitä parempi
            /// </summary>
            public float Pisteytys;

            /// <summary>
            /// Evaluoidun kaaviotilanteen kuvaus
            /// </summary>
            public string Kuvaus;

            public Pelaaja Pelaaja1;
            public Pelaaja Pelaaja2;
        }

        public int AlkuKierros { get; private set; }

        private Dictionary<string, Evaluointi> evaluoinnit = new Dictionary<string, Evaluointi>();
        private int EvaluoitujaTilanteita = 0;
        private int Osumia = 0;

        public EvaluointiTietokanta(int kierros)
        {
            this.AlkuKierros = kierros;
        }

        public bool HaeEvaluointi(string avain, out Evaluointi evaluointi)
        {
            if (this.evaluoinnit.TryGetValue(avain, out evaluointi))
            {
                this.Osumia++;
                return true;
            }

            evaluointi = null;
            return false;
        }

        public void TallennaEvaluointi(string avain, Evaluointi evaluointi)
        {
            this.evaluoinnit.Add(avain, evaluointi);
            this.EvaluoitujaTilanteita++;
        }

        public void Tulosta()
        {
#if DEBUG
            Debug.WriteLine(string.Format("### Evaluoituja tilanteita {0}, osumia {1}",
                this.EvaluoitujaTilanteita,
                this.Osumia));
#endif
        }

        public void Tyhjenna()
        {
            this.evaluoinnit.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingPelaajaTietue
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue(-1)]
        public int Id { get; set; }

        [XmlAttribute]
        [DefaultValue(0)]
        public int RankingPisteet { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string RankingPisteString { get; set; }

        [XmlAttribute]
        [DefaultValue(-1)]
        public int Sijoitus { get; set; }

        [XmlAttribute]
        [DefaultValue(-1)]
        public int KumulatiivinenSijoitus { get; set; }

        [XmlAttribute]
        [DefaultValue(0)]
        public int Lyontivuoroja { get; set; }

        [XmlAttribute]
        [DefaultValue(0)]
        public int Karoja { get; set; }

        private Dictionary<int, int> pisteKertymat = new Dictionary<int, int>();
        private List<int> yksittaisetPisteet = new List<int>();

        public RankingPelaajaTietue()
        {
            this.Id = -1;
            this.Nimi = string.Empty;
            this.RankingPisteet = 0;
            this.RankingPisteString = string.Empty;
            this.Sijoitus = -1;
            this.KumulatiivinenSijoitus = -1;
            this.Lyontivuoroja = 0;
            this.Karoja = 0;
        }

        public void PoistaPisteet()
        {
            this.RankingPisteet = 0;
            this.RankingPisteString = string.Empty;
            this.pisteKertymat.Clear();
            this.yksittaisetPisteet.Clear();
            this.Sijoitus = -1;
            this.KumulatiivinenSijoitus = -1;
            this.Lyontivuoroja = 0;
            this.Karoja = 0;
        }

        private void PaivitaPisteString()
        {
            StringBuilder s = new StringBuilder();

            foreach (var p in yksittaisetPisteet)
            {
                if (s.Length > 0)
                {
                    s.Append("+");
                }

                s.Append(p);
            }

            foreach (var p in pisteKertymat.OrderBy(x => x.Key))
            {
                if (s.Length > 0)
                {
                    s.Append("+");
                }

                if (p.Value == 1)
                {
                    s.Append(p.Key);
                }
                else 
                {
                    s.Append(string.Format("{0}x{1}", p.Value, p.Key));
                }
            }

            if (this.RankingPisteet > 0)
            {
                if (s.ToString().Contains("+") ||
                    s.ToString().Contains("x") ||
                    s.ToString().Contains("X"))
                {
                    s.Append(string.Format("={0}", this.RankingPisteet));
                }
            }

            this.RankingPisteString = s.ToString();
        }

        public void AnnaPisteita(int p, bool kertymana)
        {
            if (p > 0)
            {
                this.RankingPisteet += p;

                if (kertymana)
                {
                    if (this.pisteKertymat.ContainsKey(p))
                    {
                        this.pisteKertymat[p]++;
                    }
                    else
                    {
                        this.pisteKertymat.Add(p, 1);
                    }
                }
                else
                {
                    this.yksittaisetPisteet.Add(p);
                }

                PaivitaPisteString();
            }
        }

        public void LisaaOsakilpailunPisteet(int pisteet, string teksti)
        {
            this.RankingPisteet += pisteet;

            string pisteString = string.IsNullOrEmpty(teksti) ? "xxx" : teksti;

            if (string.IsNullOrEmpty(this.RankingPisteString))
            {
                this.RankingPisteString = pisteString;
            }
            else
            {
                this.RankingPisteString += " / ";
                this.RankingPisteString += pisteString;
            }
        }
    }
}

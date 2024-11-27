﻿using System;
using System.Collections.Generic;
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
        public int Id { get; set; }

        [XmlAttribute]
        public string Seura { get; set; }

        [XmlAttribute]
        public int RankingPisteet { get; set; }

        [XmlAttribute]
        public string RankingPisteString { get; set; }

        private Dictionary<int, int> pisteKertymat = new Dictionary<int, int>();
        private List<int> yksittaisetPisteet = new List<int>();

        public RankingPelaajaTietue()
        {
            this.Id = -1;
            this.Nimi = string.Empty;
            this.Seura = string.Empty;
            this.RankingPisteet = 0;
            this.RankingPisteString = string.Empty;
        }

        public void PoistaPisteet()
        {
            this.RankingPisteet = 0;
            this.RankingPisteString = string.Empty;
            this.pisteKertymat.Clear();
            this.yksittaisetPisteet.Clear();
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
                s.Append(string.Format("={0}", this.RankingPisteet));
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

            if (string.IsNullOrEmpty(this.RankingPisteString))
            {
                this.RankingPisteString = teksti;
            }
            else
            {
                this.RankingPisteString += " / ";
                this.RankingPisteString += teksti;
            }
        }
    }
}

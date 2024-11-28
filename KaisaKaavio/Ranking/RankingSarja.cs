using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingSarja
    {
        public string Nimi { get; set; }
        public RankingSarjanPituus Pituus { get; set; }
        public int SarjanNumero { get; set; }

        [XmlIgnore]
        public string Tiedosto { get; private set; }

        public BindingList<RankingOsakilpailu> Osakilpailut { get; set; }
        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        private bool muokattu = true;

        public RankingSarja()
        {
            this.Nimi = string.Empty;
            this.Tiedosto = string.Empty;
            this.Pituus = RankingSarjanPituus.Kuukausi;
            this.SarjanNumero = 0;
            this.Osakilpailut = new BindingList<RankingOsakilpailu>();
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }

        private StringBuilder tilanne = new StringBuilder();

        /// <summary>
        /// Rankingsarjan tilanneteksti rtf muodossa
        /// </summary>
        public string TilanneRtf
        {
            get
            {
                return tilanne.ToString();
            }
        }

        /// <summary>
        /// Rankingsarjan tilanneteksti SBiL keskustelupalstamuodossa
        /// </summary>
        public string TilanneSbil
        {
            get
            {
                return tilanne.ToString();
            }
        }

        public void Avaa(Loki loki, string tiedosto)
        {
            if (loki != null)
            {
                loki.Kirjoita(string.Format("Avataan rankingsarja tiedostosta {0}", tiedosto));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

            RankingSarja sarja = null;

            using (TextReader reader = new StreamReader(tiedosto))
            {
                sarja = (RankingSarja)serializer.Deserialize(reader);
                reader.Close();
            }

            if (sarja != null)
            {
                this.Tiedosto = tiedosto;
                this.Nimi = sarja.Nimi;
                this.Pituus = sarja.Pituus;
                this.SarjanNumero = sarja.SarjanNumero;

                this.muokattu = false;

                this.Osakilpailut.Clear();

                foreach (var o in sarja.Osakilpailut.OrderByDescending(x => x.AlkamisAika))
                {
#if !DEBUG
                    if (!string.IsNullOrEmpty(o.Nimi) &&
                        o.Osallistujat.Count > 0)
                    {
#endif
                        o.PaivitaTilanneTeksti();
                        this.Osakilpailut.Add(o);
#if !DEBUG
                    }
#endif
                }
            }
        }

        public void Tallenna(Loki loki, string kansio)
        {
            try
            {
                if (!muokattu)
                {
                    return;
                }

                if (string.IsNullOrEmpty(this.Nimi))
                {
                    this.Nimi = string.Format("RankingSarja_{0}", this.SarjanNumero);
                }

                this.Tiedosto = Path.Combine(kansio, this.Nimi + ".xml");

                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Tallennetaan rankingsarja {0} tiedostoon {1}", this.Nimi, this.Tiedosto));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

                string nimiTmp = Path.GetTempFileName();

                using (TextWriter writer = new StreamWriter(nimiTmp))
                {
                    serializer.Serialize(writer, this);
                    writer.Close();
                }

                File.Copy(nimiTmp, this.Tiedosto, true);
                File.Delete(nimiTmp);
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita("Rankingsarjan tallennus epäonnistui!", e, false);
                }
            }
        }

        public void PaivitaTilanne()
        {
            this.Osallistujat.Clear();

            List<RankingPelaajaTietue> kaikkiPelaajat = new List<RankingPelaajaTietue>();
 
            foreach (var o in this.Osakilpailut)
            {
                foreach (var p in o.Osallistujat)
                {
                    if (!kaikkiPelaajat.Any(x => string.Equals(x.Nimi, p.Nimi, StringComparison.OrdinalIgnoreCase)))
                    {
                        kaikkiPelaajat.Add(new RankingPelaajaTietue() 
                        {
                            Nimi = p.Nimi,
                        });
                    }
                }
            }

            foreach (var o in this.Osakilpailut)
            {
                foreach (var p in kaikkiPelaajat)
                {
                    var pelaaja = o.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, p.Nimi, StringComparison.OrdinalIgnoreCase));
                    if (pelaaja == null)
                    {
                        p.LisaaOsakilpailunPisteet(0, "xxx");
                    }
                    else
                    {
                        p.LisaaOsakilpailunPisteet(pelaaja.RankingPisteet, pelaaja.RankingPisteString);
                    }
                }
            }

            int kumulatiivinenSijoitus = 1;
            int sijoitus = 1;
            int edellisetPisteet = 99999999;
            int edellinenSijoitus = 99999999;

            foreach (var p in kaikkiPelaajat.OrderByDescending(x => x.RankingPisteet))
            {
                p.Sijoitus = sijoitus;
                p.KumulatiivinenSijoitus = kumulatiivinenSijoitus;

                if (p.RankingPisteet != edellisetPisteet)
                {
                    kumulatiivinenSijoitus++;
                    edellisetPisteet = p.RankingPisteet;
                    p.Sijoitus = sijoitus;
                    edellinenSijoitus = sijoitus;
                }
                else
                {
                    p.Sijoitus = edellinenSijoitus;
                }

                this.Osallistujat.Add(p);

                sijoitus++;
            }

            PaivitaTilanneTeksti();
        }

        public void PaivitaTilanneTeksti()
        {
            this.tilanne.Clear();

            foreach (var p in this.Osallistujat.OrderByDescending(x => x.RankingPisteet))
            {
                string rivi = string.Format("{0}. {1} {2}", p.Sijoitus, p.Nimi, p.RankingPisteString);

                tilanne.AppendLine(rivi);
            }
        }

        public void LisaaKilpailu(Kilpailu kilpailu, RankingAsetukset asetukset)
        {
            if (this.Osakilpailut.Any(x => x.AlkamisAika > kilpailu.AlkamisAika))
            {
                return; // Rankingkilpailuja voidaan lisätä vain sarjan loppuun
            }

            var osakilpailu = this.Osakilpailut.FirstOrDefault(x => string.Equals(x.Nimi, kilpailu.Nimi, StringComparison.OrdinalIgnoreCase));
            if (osakilpailu == null)
            {
                osakilpailu = new RankingOsakilpailu() 
                {
                    Nimi = kilpailu.Nimi,
                    AlkamisAika = kilpailu.AlkamisAika
                };

                if (this.Osakilpailut.Count == 0)
                {
                    this.Osakilpailut.Add(osakilpailu);
                }
                else 
                {
                    this.Osakilpailut.Insert(0, osakilpailu);
                }
            }

            osakilpailu.PaivitaKilpailu(kilpailu, this, asetukset);

            PaivitaTilanne();

            muokattu = true;
        }

        public int PelaajanSijoitus(int id)
        {
            return 10; // TODO
        }

        public List<RankingPelaajaTietue> RankingEnnenOsakilpailua(DateTime aika)
        {
            List<RankingPelaajaTietue> pelaajat = new List<RankingPelaajaTietue>();

            foreach (var osakilpailu in this.Osakilpailut.Where(x => x.AlkamisAika < aika))
            {
                foreach (var pelaaja in osakilpailu.Osallistujat)
                {
                    var p = pelaajat.FirstOrDefault(x => string.Equals(x.Nimi, pelaaja.Nimi, StringComparison.OrdinalIgnoreCase));
                    if (p != null)
                    {
                        p.RankingPisteet += pelaaja.RankingPisteet;
                    }
                    else
                    {
                        pelaajat.Add(new RankingPelaajaTietue() 
                        {
                            Nimi = pelaaja.Nimi,
                            RankingPisteet = pelaaja.RankingPisteet
                        });
                    }
                }
            }

            var pt = pelaajat.OrderByDescending(x => x.RankingPisteet).ToArray();
            pelaajat.Clear();
            pelaajat.AddRange(pt);

            int sijoitus = 1;
            int edellisetPisteet = 99999;
            foreach (var pelaaja in pelaajat)
            {
                pelaaja.Sijoitus = sijoitus;

                if (pelaaja.RankingPisteet != edellisetPisteet)
                {
                    edellisetPisteet = pelaaja.RankingPisteet;
                    sijoitus++;
                }
            }

            return pelaajat;
        }
    }
}

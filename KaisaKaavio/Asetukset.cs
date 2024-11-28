using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Asetukset
    {
        /// <summary>
        /// Tämä asetus määrää milloin automaattinen haku käynnistyy
        /// </summary>
        public static int PelejaEnintaanKeskenHaettaessa = 8;

        /// <summary>
        /// Tämä asetus määrää minimimäärän pelaajia joille kaavio voidaan arpoa
        /// </summary>
        public static int PelaajiaVahintaanKaaviossa = 4;

        /// <summary>
        /// Tämä asetus määrää kuinka usein automaattinen tallennus tapahtuu (sekunteja)
        /// </summary>
        public static int AutomaattisenTallennuksenTaajuus = 5 * 60;

        /// <summary>
        /// Viimeisimmän kilpailutiedoston nimi
        /// </summary>
        public string ViimeisinKilpailu { get; set; }

        /// <summary>
        /// Salin/pelipaikan tiedot. Nämä tallennetaan asetuksiin sillä eivät muutu kilpailujen välillä suuresti
        /// </summary>
        public Sali Sali { get; set; }

        /// <summary>
        /// Kisoihin osallistuneet pelaajat
        /// </summary>
        public BindingList<PelaajaTietue> Pelaajat { get; set; }

        /// <summary>
        /// Rankingkisojen (viikkokisat) pisteytysasetukset
        /// </summary>
        public Ranking.RankingAsetukset RankingAsetuksetKaisa { get; set; }
        public Ranking.RankingAsetukset RankingAsetuksetPool { get; set; }
        public Ranking.RankingAsetukset RankingAsetuksetKara { get; set; }
        public Ranking.RankingAsetukset RankingAsetuksetPyramidi { get; set; }
        public Ranking.RankingAsetukset RankingAsetuksetSnooker { get; set; }
        public Ranking.RankingAsetukset RankingAsetuksetHeyball { get; set; }

        /// <summary>
        /// Päivä jolloin edellisen kerran tarkistettiin, onko ohjelmasta päivityksiä
        /// </summary>
        public int ViimeisimmanPaivityksenPaiva { get; set; }

        /// <summary>
        /// Määrää onko automaattiset ohjelman päivitykset käytössä
        /// </summary>
        public bool PaivitaAutomaattisesti { get; set; }

        private string tiedosto = null;

        public Asetukset()
        {
            this.ViimeisinKilpailu = string.Empty;
            this.ViimeisimmanPaivityksenPaiva = 0;
            this.PaivitaAutomaattisesti = true;

            this.Sali = new Sali();
            this.Pelaajat = new BindingList<PelaajaTietue>();

            this.RankingAsetuksetKaisa = new Ranking.RankingAsetukset(Laji.Kaisa);
            this.RankingAsetuksetPool = new Ranking.RankingAsetukset(Laji.Pool);
            this.RankingAsetuksetPyramidi = new Ranking.RankingAsetukset(Laji.Pyramidi);
            this.RankingAsetuksetKara = new Ranking.RankingAsetukset(Laji.Kara);
            this.RankingAsetuksetSnooker = new Ranking.RankingAsetukset(Laji.Snooker);
            this.RankingAsetuksetHeyball = new Ranking.RankingAsetukset(Laji.Heyball);

            this.tiedosto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaavioAsetukset.xml");
        }

        public void Tallenna()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Asetukset));

            using (TextWriter writer = new StreamWriter(this.tiedosto))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }
        }

        public void Lataa()
        {
            if (File.Exists(tiedosto))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Asetukset));

                Asetukset asetukset = null;

                using (TextReader reader = new StreamReader(tiedosto))
                {
                    asetukset = (Asetukset)serializer.Deserialize(reader);
                    reader.Close();
                }

                if (asetukset != null)
                {
                    this.ViimeisinKilpailu = asetukset.ViimeisinKilpailu;
                    this.ViimeisimmanPaivityksenPaiva = asetukset.ViimeisimmanPaivityksenPaiva;
                    this.PaivitaAutomaattisesti = asetukset.PaivitaAutomaattisesti;

                    this.Sali.Nimi = asetukset.Sali.Nimi;
                    this.Sali.Osoite = asetukset.Sali.Osoite;
                    this.Sali.PuhelinNumero = asetukset.Sali.PuhelinNumero;

                    this.Sali.Linkit.Clear();
                    foreach (var linkki in asetukset.Sali.Linkit)
                    {
                        if (!string.IsNullOrEmpty(linkki.Osoite))
                        {
                            this.Sali.Linkit.Add(linkki);
                        }
                    }

                    this.Sali.Poydat.Clear();
                    foreach (var poyta in asetukset.Sali.Poydat)
                    {
                        if (!string.IsNullOrEmpty(poyta.Numero))
                        {
                            this.Sali.Poydat.Add(poyta);
                        }
                    }

                    this.Pelaajat.Clear();
                    foreach (var pelaaja in asetukset.Pelaajat)
                    {
                        if (!string.IsNullOrEmpty(pelaaja.Nimi))
                        {
                            this.Pelaajat.Add(pelaaja);
                        }
                    }

                    LataaRankingAsetukset(this.RankingAsetuksetKaisa, asetukset.RankingAsetuksetKaisa, Laji.Kaisa);
                    LataaRankingAsetukset(this.RankingAsetuksetPool, asetukset.RankingAsetuksetPool, Laji.Pool);
                    LataaRankingAsetukset(this.RankingAsetuksetKara, asetukset.RankingAsetuksetKara, Laji.Kara);
                    LataaRankingAsetukset(this.RankingAsetuksetSnooker, asetukset.RankingAsetuksetSnooker, Laji.Snooker);
                    LataaRankingAsetukset(this.RankingAsetuksetPyramidi, asetukset.RankingAsetuksetPyramidi, Laji.Pyramidi);
                    LataaRankingAsetukset(this.RankingAsetuksetHeyball, asetukset.RankingAsetuksetHeyball, Laji.Heyball);
                }
            }
        }

        private static void LataaRankingAsetukset(Ranking.RankingAsetukset omatAsetukset, Ranking.RankingAsetukset ladatutAsetukset, Laji laji)
        {
            omatAsetukset.PistetytysPeleista.Clear();
            foreach (var p in ladatutAsetukset.PistetytysPeleista)
            {
                if (p.Pisteet > 0)
                {
                    omatAsetukset.PistetytysPeleista.Add(p);
                }
            }

            omatAsetukset.PisteytysSijoituksista.Clear();
            foreach (var p in ladatutAsetukset.PisteytysSijoituksista)
            {
                if (p.Pisteet > 0)
                {
                    omatAsetukset.PisteytysSijoituksista.Add(p);
                }
            }

            if (omatAsetukset.PistetytysPeleista.Count == 0 &&
                omatAsetukset.PisteytysSijoituksista.Count == 0)
            {
                omatAsetukset.AsetaOletusasetukset(laji);
            }
        }

        private string MuotoileNimi(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return nimi;
            }

            var nimet = nimi.Split(',');
            if (nimet == null || nimet.Count() == 1)
            {
                return KapiteeliksiEkaKirjain(nimi);
            }

            List<string> kapiteeliNimet = new List<string>();
            foreach (var n in nimet)
            {
                kapiteeliNimet.Add(KapiteeliksiEkaKirjain(n.Trim()));
            }

            return string.Join(" ", kapiteeliNimet);
        }

        private string KapiteeliksiEkaKirjain(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return nimi;
            }

            if (Char.IsUpper(nimi[0]))
            {
                return nimi;
            }

            StringBuilder s = new StringBuilder();
            s.Append(Char.ToUpper(nimi[0]));

            if (nimi.Length > 1)
            {
                s.Append(nimi.Substring(1));
            }

            return s.ToString();
        }

        public void TallennaPelaajat(Kilpailu kilpailu)
        {
            foreach (var osallistuja in kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                // Tallenna vain pelaajat joilla on vähintään kaksi nimeä (etu & suku)
                if (osallistuja.Nimi.Split(' ').Count() > 1)
                {
                    string nimi = MuotoileNimi(osallistuja.Nimi);

                    PelaajaTietue vanhaPelaaja = this.Pelaajat.FirstOrDefault(x => string.Equals(x.Nimi, nimi, StringComparison.OrdinalIgnoreCase));
                    if (vanhaPelaaja != null)
                    {
                        vanhaPelaaja.Seura = osallistuja.Seura;
                    }
                    else
                    {
                        PelaajaTietue p =
                        new PelaajaTietue()
                        {
                            Nimi = nimi,
                            Seura = osallistuja.Seura
                        };

                        Pelaajat.Add(p);
                    }
                }
            }

            var pelaajatJarjestyksessa = this.Pelaajat.OrderBy(x => x.Nimi).ToArray();
            this.Pelaajat.Clear();
            foreach (var p in pelaajatJarjestyksessa)
            {
                this.Pelaajat.Add(p);
            }
        }
    }
}

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
        public Ranking.RankingAsetukset RankingAsetukset { get; set; }

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
            this.RankingAsetukset = new Ranking.RankingAsetukset();
            this.RankingAsetukset.AsetaOletusasetukset();

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

                    this.RankingAsetukset.PistetytysPeleista.Clear();
                    foreach (var p in asetukset.RankingAsetukset.PistetytysPeleista)
                    {
                        if (p.Pisteet > 0)
                        {
                            this.RankingAsetukset.PistetytysPeleista.Add(p);
                        }
                    }

                    this.RankingAsetukset.PisteytysSijoituksista.Clear();
                    foreach (var p in asetukset.RankingAsetukset.PisteytysSijoituksista)
                    {
                        if (p.Pisteet > 0)
                        {
                            this.RankingAsetukset.PisteytysSijoituksista.Add(p);
                        }
                    }

                    if (this.RankingAsetukset.PistetytysPeleista.Count == 0 &&
                        this.RankingAsetukset.PisteytysSijoituksista.Count == 0)
                    {
                        this.RankingAsetukset.AsetaOletusasetukset();
                    }
                }
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

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Asetukset
    {
        /// <summary>
        /// Tämä asetus määrää milloin automaattinen haku käynnistyy
        /// </summary>
        public static readonly int PelejaEnintaanKeskenHaettaessa = 8;

        /// <summary>
        /// Tämä asetus määrää minimimäärän pelaajia joille kaavio voidaan arpoa
        /// </summary>
        public static readonly int PelaajiaVahintaanKaaviossa = 4;

        /// <summary>
        /// Tämä asetus määrää, missä vaiheessa kisaa ruvetaan hakemaan pelejä paremmalla, mutta hitaammalla
        /// algoritmilla hakuvirheiden minimoimiseksi
        /// </summary>
        public static readonly int HuolellisenHaunPelaajamaara = 6;

        /// <summary>
        /// Tämä asetus määrää kuinka usein automaattinen tallennus tapahtuu (sekunteja)
        /// </summary>
        public static readonly int AutomaattisenTallennuksenTaajuus = 5 * 60;

        /// <summary>
        /// Tämä asetus määrää montako viimeksi avattua kilpailua listataan 'Viimeisimmät kilpailut' valikossa
        /// </summary>
        public static readonly int ViimeisimpiaKilpailuja = 15;

#if DEBUG
        public static readonly string KaisaKaavioServeri = "https://localhost:7071";
#else
        public static readonly string KaisaKaavioServeri = "https://kaisakaavio.fi";
#endif

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
        /// Rankingkisojen (viikkokisat) viimeisimmät pisteytysasetukset.
        /// Jokaisella sarjalla on omat asetuksensa, jotka tallentuvat sarjatiedostoihin.
        /// Näitä oletusasetuksia käytetään uusille alkaville sarjoille.
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


        /// <summary>
        /// X viimeksi aukaistua kilpailutiedostoa
        /// </summary>
        public BindingList<Tyypit.Tiedosto> ViimeisimmatKilpailut { get; set; }

        private string tiedosto = null;

        public class KisaOletusasetukset
        {
            [XmlAttribute]
            [DefaultValue(0)]
            public int Peliaika { get; set; }

            [XmlAttribute]
            [DefaultValue(0)]
            public int Tavoite { get; set; }

            [XmlAttribute]
            [DefaultValue(false)]
            public bool PeliaikaRajattu { get; set; }

            [XmlAttribute]
            [DefaultValue("")]
            public string Alalaji { get; set; }

            [XmlAttribute]
            [DefaultValue(false)]
            public bool RankingSarja { get; set; }

            [XmlAttribute]
            public Ranking.RankingSarjanPituus RankingSarjanTyyppi { get; set; }

            [XmlAttribute]
            public KaavioTyyppi KaavioTyyppi { get; set; }

            public KisaOletusasetukset()
            {
                this.Tavoite = 0;
                this.Peliaika = 0;
                this.PeliaikaRajattu = false;
                this.Alalaji = string.Empty;
                this.RankingSarja = false;
                this.RankingSarjanTyyppi = Ranking.RankingSarjanPituus.Kuukausi;
                this.KaavioTyyppi = KaisaKaavio.KaavioTyyppi.Pudari3Kierros;
            }
        }

        public KisaOletusasetukset OletusAsetuksetKaisa { get; set; }
        public KisaOletusasetukset OletusAsetuksetKara { get; set; }
        public KisaOletusasetukset OletusAsetuksetPyramidi { get; set; }
        public KisaOletusasetukset OletusAsetuksetPool { get; set; }
        public KisaOletusasetukset OletusAsetuksetHeyball { get; set; }
        public KisaOletusasetukset OletusAsetuksetSnooker { get; set; }

        public Asetukset()
        {
            this.ViimeisinKilpailu = string.Empty;
            this.ViimeisimmanPaivityksenPaiva = 0;
            this.PaivitaAutomaattisesti = true;

            this.Sali = new Sali();
            this.Pelaajat = new BindingList<PelaajaTietue>();
            this.ViimeisimmatKilpailut = new BindingList<Tyypit.Tiedosto>();

            this.RankingAsetuksetKaisa = new Ranking.RankingAsetukset(Laji.Kaisa);
            this.RankingAsetuksetPool = new Ranking.RankingAsetukset(Laji.Pool);
            this.RankingAsetuksetPyramidi = new Ranking.RankingAsetukset(Laji.Pyramidi);
            this.RankingAsetuksetKara = new Ranking.RankingAsetukset(Laji.Kara);
            this.RankingAsetuksetSnooker = new Ranking.RankingAsetukset(Laji.Snooker);
            this.RankingAsetuksetHeyball = new Ranking.RankingAsetukset(Laji.Heyball);

            this.OletusAsetuksetKaisa = new KisaOletusasetukset()
            {
                Peliaika = 40,
                RankingSarja = true,
                PeliaikaRajattu = true,
                Tavoite = 60
            };

            this.OletusAsetuksetKara = new KisaOletusasetukset()
            {
                Peliaika = 40,
                RankingSarja = true,
                PeliaikaRajattu = true,
                Tavoite = 20,
                Alalaji = "Kolmen vallin kara"
            };

            this.OletusAsetuksetPyramidi = new KisaOletusasetukset()
            {
                Peliaika = 40,
                RankingSarja = true,
                PeliaikaRajattu = true,
                Tavoite = 3,
                Alalaji = "Amerikanka"
            };

            this.OletusAsetuksetPool = new KisaOletusasetukset()
            {
                Tavoite = 4,
                Alalaji = "9-ball",
            };

            this.OletusAsetuksetHeyball = new KisaOletusasetukset()
            {
                Tavoite = 4,
            };

            this.OletusAsetuksetSnooker = new KisaOletusasetukset()
            {
                Tavoite = 2,
                Alalaji = "Snooker"
            };

            this.tiedosto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaavioAsetukset.xml");
        }

        public void Tallenna()
        {
            // Asetuksia ei tallenneta kun useita KaisaKaavioita voi olla auki samanaikaisesti
            if (!Program.UseampiKaisaKaavioAvoinna)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Asetukset));

                using (TextWriter writer = new StreamWriter(this.tiedosto))
                {
                    serializer.Serialize(writer, this);
                    writer.Close();
                }
            }
        }

        public void Lataa()
        {
            try
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

                        this.Sali.KopioiSalista(asetukset.Sali);

                        this.Pelaajat.Clear();
                        foreach (var pelaaja in asetukset.Pelaajat)
                        {
                            if (!string.IsNullOrEmpty(pelaaja.Nimi) &&
                                !pelaaja.Nimi.Contains("&") &&
                                !pelaaja.Nimi.Contains(" ja "))
                            {
                                string nimi =
                                    Tyypit.Nimi.MuotoileNimi(
                                    Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(pelaaja.Nimi));

                                if (!this.Pelaajat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi)))
                                {
                                    this.Pelaajat.Add(new PelaajaTietue()
                                    {
                                        Nimi = nimi,
                                        Seura = pelaaja.Seura,
                                        TasoitusHeyball = pelaaja.TasoitusHeyball,
                                        TasoitusKaisa = pelaaja.TasoitusKaisa,
                                        TasoitusKara = pelaaja.TasoitusKara,
                                        TasoitusPool = pelaaja.TasoitusPool,
                                        TasoitusPyramidi = pelaaja.TasoitusPyramidi,
                                        TasoitusSnooker = pelaaja.TasoitusSnooker
                                    });
                                }
                            }
                        }

                        this.ViimeisimmatKilpailut.Clear();
                        foreach (var v in asetukset.ViimeisimmatKilpailut)
                        {
                            if (!string.IsNullOrEmpty(v.Nimi) &&
                                !string.IsNullOrEmpty(v.Polku) &&
                                File.Exists(v.Polku))
                            {
                                this.ViimeisimmatKilpailut.Add(v);
                            }
                        }

                        LataaRankingAsetukset(this.RankingAsetuksetKaisa, asetukset.RankingAsetuksetKaisa, Laji.Kaisa);
                        LataaRankingAsetukset(this.RankingAsetuksetPool, asetukset.RankingAsetuksetPool, Laji.Pool);
                        LataaRankingAsetukset(this.RankingAsetuksetKara, asetukset.RankingAsetuksetKara, Laji.Kara);
                        LataaRankingAsetukset(this.RankingAsetuksetSnooker, asetukset.RankingAsetuksetSnooker, Laji.Snooker);
                        LataaRankingAsetukset(this.RankingAsetuksetPyramidi, asetukset.RankingAsetuksetPyramidi, Laji.Pyramidi);
                        LataaRankingAsetukset(this.RankingAsetuksetHeyball, asetukset.RankingAsetuksetHeyball, Laji.Heyball);

                        LataaKisaAsetukset(this.OletusAsetuksetKaisa, asetukset.OletusAsetuksetKaisa);
                        LataaKisaAsetukset(this.OletusAsetuksetKara, asetukset.OletusAsetuksetKara);
                        LataaKisaAsetukset(this.OletusAsetuksetPyramidi, asetukset.OletusAsetuksetPyramidi);
                        LataaKisaAsetukset(this.OletusAsetuksetPool, asetukset.OletusAsetuksetPool);
                        LataaKisaAsetukset(this.OletusAsetuksetHeyball, asetukset.OletusAsetuksetHeyball);
                        LataaKisaAsetukset(this.OletusAsetuksetSnooker, asetukset.OletusAsetuksetSnooker);
                    }
                }
            }
            catch
            {
                // TODO!!!! Asetusten varmuuskopiointi ja palautus

            }
        }

        public void LisaaViimeisimpiinKilpailuihin(Kilpailu kilpailu)
        {
            if (!Program.UseampiKaisaKaavioAvoinna)
            {
                if (kilpailu.TestiKilpailu)
                {
                    return; // Ei tallenneta höpöhöpödataa
                }

                try
                {
                    if (kilpailu != null &&
                        !string.IsNullOrEmpty(kilpailu.Nimi) &&
                        !string.IsNullOrEmpty(kilpailu.Tiedosto))
                    {
                        var vanha = this.ViimeisimmatKilpailut.FirstOrDefault(x => string.Equals(x.Polku, kilpailu.Tiedosto, StringComparison.OrdinalIgnoreCase));
                        if (vanha != null)
                        {
                            this.ViimeisimmatKilpailut.Remove(vanha);
                        }

                        Tyypit.Tiedosto tiedosto = new Tyypit.Tiedosto() { Nimi = kilpailu.Nimi, Polku = kilpailu.Tiedosto };

                        if (this.ViimeisimmatKilpailut.Count == 0)
                        {
                            this.ViimeisimmatKilpailut.Add(tiedosto);
                        }
                        else
                        {
                            this.ViimeisimmatKilpailut.Insert(0, tiedosto);
                        }

                        while (this.ViimeisimmatKilpailut.Count > ViimeisimpiaKilpailuja)
                        {
                            this.ViimeisimmatKilpailut.Remove(this.ViimeisimmatKilpailut.Last());
                        }
                    }
                }
                catch
                { 
                }
            }
        }

        public void PoistaViimeisimmistaKilpailuista(string tiedosto)
        {
            if (!Program.UseampiKaisaKaavioAvoinna)
            {
                while (true)
                {
                    var kilpa = this.ViimeisimmatKilpailut.FirstOrDefault(x => string.Equals(x.Polku, tiedosto, StringComparison.OrdinalIgnoreCase));
                    if (kilpa != null)
                    {
                        this.ViimeisimmatKilpailut.Remove(kilpa);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private static void LataaKisaAsetukset(KisaOletusasetukset omatAsetukset, KisaOletusasetukset ladatutAsetukset)
        {
            omatAsetukset.Alalaji = ladatutAsetukset.Alalaji;
            omatAsetukset.Peliaika = ladatutAsetukset.Peliaika;
            omatAsetukset.PeliaikaRajattu = ladatutAsetukset.PeliaikaRajattu;
            omatAsetukset.RankingSarja = ladatutAsetukset.RankingSarja;
            omatAsetukset.Tavoite = ladatutAsetukset.Tavoite;
            omatAsetukset.RankingSarjanTyyppi = ladatutAsetukset.RankingSarjanTyyppi;
            omatAsetukset.KaavioTyyppi = ladatutAsetukset.KaavioTyyppi;
        }

        private static void LataaRankingAsetukset(Ranking.RankingAsetukset omatAsetukset, Ranking.RankingAsetukset ladatutAsetukset, Laji laji)
        {
            omatAsetukset.KopioiAsetuksista(ladatutAsetukset);
            omatAsetukset.Laji = laji;
            if (omatAsetukset.Tyhja)
            {
                omatAsetukset.AsetaOletusasetukset(laji);
            }
        }

        public void TallennaPelaajat(Kilpailu kilpailu)
        {
            if (!Program.UseampiKaisaKaavioAvoinna)
            {
                if (kilpailu.TestiKilpailu)
                {
                    return; // Ei tallenneta höpöhöpödataa
                }

                foreach (var osallistuja in kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
                {
                    string nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(osallistuja.Nimi);

                    // Tallenna vain pelaajat joilla on vähintään kaksi nimeä (etu & suku)
                    if (nimi.Split(' ').Count() > 1 &&
                        nimi.Split(' ').Count() <= 3 &&
                        !nimi.Contains("&") &&
                        !nimi.Contains(" ja "))
                    {
                        string muotoiltuNimi = Tyypit.Nimi.MuotoileNimi(osallistuja.Nimi);

                        PelaajaTietue vanhaPelaaja = this.Pelaajat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, muotoiltuNimi));
                        if (vanhaPelaaja != null)
                        {
                            vanhaPelaaja.Seura = osallistuja.Seura;

                            if (kilpailu.KilpailuOnViikkokisa)
                            {
                                switch (kilpailu.Laji)
                                {
                                    case Laji.Heyball: vanhaPelaaja.TasoitusHeyball = osallistuja.Tasoitus; break;
                                    case Laji.Kaisa: vanhaPelaaja.TasoitusKaisa = osallistuja.Tasoitus; break;
                                    case Laji.Pool: vanhaPelaaja.TasoitusPool = osallistuja.Tasoitus; break;
                                    case Laji.Snooker: vanhaPelaaja.TasoitusSnooker = osallistuja.Tasoitus; break;
                                    case Laji.Kara: vanhaPelaaja.TasoitusKara = osallistuja.Tasoitus; break;
                                    case Laji.Pyramidi: vanhaPelaaja.TasoitusPyramidi = osallistuja.Tasoitus; break;
                                }
                            }
                        }
                        else
                        {
                            PelaajaTietue p =
                            new PelaajaTietue()
                            {
                                Nimi = muotoiltuNimi,
                                Seura = osallistuja.Seura
                            };

                            if (kilpailu.KilpailuOnViikkokisa)
                            {
                                switch (kilpailu.Laji)
                                {
                                    case Laji.Heyball: p.TasoitusHeyball = osallistuja.Tasoitus; break;
                                    case Laji.Kaisa: p.TasoitusKaisa = osallistuja.Tasoitus; break;
                                    case Laji.Pool: p.TasoitusPool = osallistuja.Tasoitus; break;
                                    case Laji.Snooker: p.TasoitusSnooker = osallistuja.Tasoitus; break;
                                    case Laji.Kara: p.TasoitusKara = osallistuja.Tasoitus; break;
                                    case Laji.Pyramidi: p.TasoitusPyramidi = osallistuja.Tasoitus; break;
                                }
                            }

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

        public Ranking.RankingAsetukset RankingPisteytys(Laji laji)
        {
            switch (laji)
            {
                case Laji.Snooker: return this.RankingAsetuksetSnooker;
                case Laji.Pyramidi: return this.RankingAsetuksetPyramidi;
                case Laji.Pool: return this.RankingAsetuksetPool;
                case Laji.Kara: return this.RankingAsetuksetKara;
                case Laji.Heyball: return this.RankingAsetuksetHeyball;
                case Laji.Kaisa: return this.RankingAsetuksetKaisa;

                default: throw new NotImplementedException(string.Format("Laji {0}", laji));
            }
        }

        public KisaOletusasetukset OletusAsetukset(Laji laji)
        {
            switch (laji)
            {
                case Laji.Heyball: return this.OletusAsetuksetHeyball;
                case Laji.Kaisa: return this.OletusAsetuksetKaisa;
                case Laji.Kara: return this.OletusAsetuksetKara;
                case Laji.Pool: return this.OletusAsetuksetPool;
                case Laji.Pyramidi: return this.OletusAsetuksetPyramidi;
                case Laji.Snooker: return this.OletusAsetuksetSnooker;

                default: throw new NotImplementedException(string.Format("Laji {0}", laji));
            }
        }
    }
}

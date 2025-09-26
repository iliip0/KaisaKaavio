using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    /// <summary>
    /// Testauksessa käytettävä työkalu, joka ottaa parametriksi
    /// oikean, oikein haetun kilpailun, ja testaa, että 
    /// KaisaKaavio ohjelma tuottaa samat haut uudella kilpailulla
    /// kun osallistujat ja pelien tulokset laitetaan samoiksi.
    /// </summary>
    public class TestiKilpailu
    {
        public Kilpailu OikeaKilpailu { get; private set; }
        public Kilpailu TestattavaKilpailu { get; private set; }
        private int PoytienMaara = 1;
        private bool SatunnainenPelienJarjestys = false;
        private Random Arpa = null;

        private Loki loki = null;

        public TestiKilpailu(int poytienMaara, bool satunnainenPelienJarjestys, Loki loki, Kilpailu oikeaKilpailu)
        {
            this.PoytienMaara = Math.Max(1, poytienMaara);
            this.SatunnainenPelienJarjestys = satunnainenPelienJarjestys;
            this.loki = loki;
            this.OikeaKilpailu = oikeaKilpailu;
            this.TestattavaKilpailu = new Kilpailu();
            this.TestattavaKilpailu.TestiKilpailu = true;
            this.TestattavaKilpailu.Loki = loki;
            this.Arpa = new Random();

            // Kopioidaan testauksen kannalta relevantit parametrit testattaavaan kilpailuun:
            this.TestattavaKilpailu.KaavioTyyppi = this.OikeaKilpailu.KaavioTyyppi;
            this.TestattavaKilpailu.KilpailunTyyppi = this.OikeaKilpailu.KilpailunTyyppi;
            this.TestattavaKilpailu.Nimi = string.Format("_TESTI_{0}_{1}", this.OikeaKilpailu.Nimi, DateTime.Now.ToString());
            this.TestattavaKilpailu.PelaajiaEnintaan = this.OikeaKilpailu.PelaajiaEnintaan;
            this.TestattavaKilpailu.PeliAika = this.OikeaKilpailu.PeliAika;
            this.TestattavaKilpailu.RankingKisa = this.OikeaKilpailu.RankingKisa;
            this.TestattavaKilpailu.RankkareidenMaara = this.OikeaKilpailu.RankkareidenMaara;
            this.TestattavaKilpailu.TavoitePistemaara = this.OikeaKilpailu.TavoitePistemaara;
            this.TestattavaKilpailu.Yksipaivainen = this.OikeaKilpailu.Yksipaivainen;
        }

        public void PelaaKilpailu(IStatusRivi status, int vaihe, int vaiheMax)
        {
            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Arvotaan kaavio", this.OikeaKilpailu.Nimi), true, vaihe, vaiheMax);
            ArvoKaavio();

            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Pelataan pelit", this.OikeaKilpailu.Nimi), true, vaihe + 2, vaiheMax);
            PelaaKaavio();

            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Tarkistetaan pelit", this.OikeaKilpailu.Nimi), true, vaihe + 3, vaiheMax);
            TarkistaPelit();
        }

        /// <summary>
        /// Kopioi osallistujalistan oikeasta kilpailusta ja tarkistaa että arvotut 1. ja 2. kierros ovat identtiset
        /// </summary>
        private void ArvoKaavio()
        {
            foreach (var osallistuja in this.OikeaKilpailu.Osallistujat.Where(x => x.Id >= 0))
            {
                this.TestattavaKilpailu.Osallistujat.Add(new Pelaaja() 
                {
                    Nimi = osallistuja.Nimi,
                    Id = osallistuja.Id,
                    Joukkue = osallistuja.Joukkue,
                    Pelaajan1Nimi = osallistuja.Pelaajan1Nimi,
                    Pelaajan1Seura = osallistuja.Pelaajan1Seura,
                    Pelaajan2Nimi = osallistuja.Pelaajan2Nimi,
                    Pelaajan2Seura = osallistuja.Pelaajan2Seura
                });
            }

            string virhe = string.Empty;
            if (!this.TestattavaKilpailu.ArvoKaavio(out virhe))
            {
                throw new Exception(string.Format("Kaavion arpominen epäonnistui: {0}", virhe));
            }

            // Tarkistetaan että 1. ja 2. kierroksen pelit on samat
            var testattavatAlkupelit = this.TestattavaKilpailu.Pelit.Where(x => x.Kierros < 3);
            var oikeatAlkupelit = this.OikeaKilpailu.Pelit.Where(x => x.Kierros < 3);

            if (testattavatAlkupelit.Count() != oikeatAlkupelit.Count())
            {
                throw new Exception(string.Format("Väärä määrä alkupelejä. Pitäisi olla {0}, oli {1}", 
                    oikeatAlkupelit.Count(), 
                    testattavatAlkupelit.Count()));
            }

            foreach (var peli in oikeatAlkupelit)
            {
                if (!testattavatAlkupelit.Any(x => 
                    x.Kierros == peli.Kierros && 
                    x.Id1 == peli.Id1 &&
                    x.Id2 == peli.Id2))
                {
                    throw new Exception(string.Format("Väärä haku alkukierroksille. {0} kierroksen peliä {1} - {2} ei löydy",
                        peli.Kierros,
                        peli.Pelaaja1,
                        peli.Pelaaja2));
                }
            }
        }

        private void PelaaKaavio()
        {
            while (PelaaSeuraavaPeli())
            {
                var haku = this.TestattavaKilpailu.Haku(null);
                if (haku != null)
                {
                    haku.AutomaattinenTestausMenossa = true;
                    haku.Hae();

                    foreach (var peli in haku.UudetPelit)
                    {
                        this.TestattavaKilpailu.LisaaPeli(peli.Pelaaja1, peli.Pelaaja2);
                    }
                }

                TarkistaPelaamattomatPelit();
            }
        }

        private void TarkistaPelaamattomatPelit()
        {
            foreach (var peli in this.TestattavaKilpailu.Pelit)
            {
                var oikeaPeli = this.OikeaKilpailu.Pelit.FirstOrDefault(x =>
                    x.Kierros == peli.Kierros &&
                    x.Id1 == peli.Id1 &&
                    x.Id2 == peli.Id2 &&
                    x.KierrosPelaaja1 == peli.KierrosPelaaja1 &&
                    x.KierrosPelaaja2 == peli.KierrosPelaaja2);

                if (oikeaPeli == null)
                {
                    throw new Exception(string.Format("Hakuvirhe kaaviossa {3}: {0}. kierroksen peliä {1} - {2} ei löytynyt oikeasta kaaviosta",
                        peli.Kierros,
                        peli.Pelaaja1,
                        peli.Pelaaja2,
                        TestattavaKilpailu.Nimi));
                }

                int tappiot1 = 0;
                int kesken1 = 0;
                int tappiot2 = 0;
                int kesken2 = 0;

                foreach (var aiempiPeli in this.TestattavaKilpailu.Pelit.Where(x => x.PeliNumero < peli.PeliNumero))
                {
                    if (aiempiPeli.Havisi(peli.Id1))
                    {
                        if (aiempiPeli.OnPudotusPeli(peli.Id1))
                        {
                            throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja1 on pudonnut aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                        }

                        tappiot1++;
                        if (tappiot1 > 1)
                        {
                            throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja1 on pudonnut aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                        }
                    }

                    if (aiempiPeli.Havisi(peli.Id2))
                    {
                        if (aiempiPeli.OnPudotusPeli(peli.Id2))
                        {
                            throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja2 on pudonnut aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                        }

                        tappiot2++;
                        if (tappiot2 > 1)
                        {
                            throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja2 on pudonnut aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                        }
                    }

                    if (aiempiPeli.Tilanne != PelinTilanne.Pelattu)
                    {
                        if (aiempiPeli.SisaltaaPelaajan(peli.Id1))
                        {
                            if (aiempiPeli.OnPudotusPeli(peli.Id1))
                            {
                                throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaajalla 1 on pudaripeli kesken aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                            }

                            kesken1++;
                            if ((tappiot1 + kesken1) > 1)
                            {
                                throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja 1 saattaa pudota aiemalla kierroksella (peli kesken)", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                            }
                        }

                        if (aiempiPeli.SisaltaaPelaajan(peli.Id2))
                        {
                            if (aiempiPeli.OnPudotusPeli(peli.Id2))
                            {
                                throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaajalla 2 on pudaripeli kesken aiemmalla kierroksella", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                            }

                            kesken2++;
                            if ((tappiot2 + kesken2) > 1)
                            {
                                throw new Exception(string.Format("{0}. kierroksen peli {1} - {2} haettu vaikka pelaaja 2 saattaa pudota aiemalla kierroksella (peli kesken)", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                            }
                        }
                    }
                }
            }
        }

        private void TarkistaPelit()
        {
            if (this.TestattavaKilpailu.Pelit.Count != this.OikeaKilpailu.Pelit.Count)
            {
                throw new Exception("Testattavassa kilpailussa väärä määrä pelejä");
            }

            foreach (var peli in this.OikeaKilpailu.Pelit)
            { 
                var testattavaPeli = this.TestattavaKilpailu.Pelit.FirstOrDefault(x => 
                    x.Kierros == peli.Kierros &&
                    x.Id1 == peli.Id1 &&
                    x.Id2 == peli.Id2 &&
                    x.KierrosPelaaja1 == peli.KierrosPelaaja1 &&
                    x.KierrosPelaaja2 == peli.KierrosPelaaja2);

                if (testattavaPeli == null)
                {
                    throw new Exception(string.Format("{0}. kierroksen peliä {1} - {2} ei löytynyt testattavasta kaaviosta", 
                        peli.Kierros,
                        peli.Pelaaja1,
                        peli.Pelaaja2));
                }
            }
        }

        private bool KaynnistaPeleja()
        {
            this.TestattavaKilpailu.PaivitaPelitValmiinaAlkamaan();

            int kaynnissa = this.TestattavaKilpailu.Pelit.Count(x => x.Tilanne == PelinTilanne.Kaynnissa);
            if (kaynnissa < this.PoytienMaara)
            {
                var valmiina = this.TestattavaKilpailu.Pelit.Where(x => x.Tilanne == PelinTilanne.ValmiinaAlkamaan);
                if (valmiina.Count() > 0)
                {
                    Peli peli = null;

                    if (this.SatunnainenPelienJarjestys && valmiina.Count() > 1)
                    {
                        peli = valmiina.ElementAt(this.Arpa.Next(valmiina.Count()));
                    }
                    else
                    {
                        peli = valmiina.First();
                    }

                    if (peli != null)
                    {
                        this.loki.Kirjoita(string.Format("Käynnistettiin {0}. kierroksen peli {1} - {2}", peli.Kierros, peli.Pelaaja1, peli.Pelaaja2));
                        peli.Tilanne = PelinTilanne.Kaynnissa;
                        peli.Pisteet1 = "0";
                        peli.Pisteet2 = "0";

                        return true;
                    }
                }
            }

            return false;
        }

        private bool PelaaSeuraavaPeli()
        {
            while (KaynnistaPeleja())
            { 
            }

            Peli peli = null;

            var kaynnissa = this.TestattavaKilpailu.Pelit.Where(x => x.Tilanne == PelinTilanne.Kaynnissa);
            if (kaynnissa.Count() > 0)
            {
                if (this.SatunnainenPelienJarjestys && kaynnissa.Count() > 1)
                {
                    peli = kaynnissa.ElementAt(this.Arpa.Next(kaynnissa.Count()));
                }
                else 
                {
                    peli = kaynnissa.First();
                }
            }

            if (peli == null)
            {
                if (this.TestattavaKilpailu.Pelit.Count() == this.OikeaKilpailu.Pelit.Count())
                {
                    return false;
                }
                else
                {
                    throw new Exception("Kaaviossa ei ollut pelattavia pelejä ja pelejä on vähemmän kuin oikeassa kilpailussa");
                }
            }

            var oikeaPeli = this.OikeaKilpailu.Pelit.FirstOrDefault(x => 
                x.Kierros == peli.Kierros &&
                x.Id1 == peli.Id1 &&
                x.Id2 == peli.Id2);

            if (oikeaPeli == null)
            {
                throw new Exception(string.Format("Hakuvirhe! Oikeassa kaaviossa ei löydy {0}. kierroksen peliä {1} - {2}",
                    peli.Kierros,
                    peli.Pelaaja1,
                    peli.Pelaaja2));
            }

            peli.Pisteet1 = oikeaPeli.Pisteet1;
            peli.Pisteet2 = oikeaPeli.Pisteet2;

            if (peli.Tilanne != PelinTilanne.Pelattu)
            {
                throw new Exception(string.Format("{0}. kierroksen pelin {1} - {2} tilanne ei ole 'Pelattu'", 
                    peli.Kierros,
                    peli.Pelaaja1,
                    peli.Pelaaja2));
            }

            this.loki.Kirjoita(string.Format("Pelattu {0}. kierroksen peli {1} - {2} ({3})", 
                peli.Kierros, peli.Pelaaja1, peli.Pelaaja2, peli.Tulos));

            return true;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    public class MonteCarloTestiKilpailu
    {
        public Kilpailu TestattavaKilpailu { get; private set; }
        private int PoytienMaara = 1;
        private int Pelaajia = 4;
        private bool SatunnainenPelienJarjestys = false;
        private Random Arpa = null;

        private Loki loki = null;

        public MonteCarloTestiKilpailu(string nimi, int poytienMaara, bool satunnainenPelienJarjestys, int pelaajia, Loki loki)
        {
            this.PoytienMaara = Math.Max(1, poytienMaara);
            this.Pelaajia = Math.Max(4, pelaajia);
            this.SatunnainenPelienJarjestys = satunnainenPelienJarjestys;
            this.loki = loki;
            this.TestattavaKilpailu = new Kilpailu() 
            {
                AlkamisAikaDt = DateTime.Today,
                KellonAika = "18:00",
                KaavioTyyppi = KaavioTyyppi.TuplaKaavio,
                KilpailunTyyppi = KilpailunTyyppi.KaisanRGKilpailu,
                Laji = Laji.Kaisa,
                TavoitePistemaara = 60,
                PelaajiaEnintaan = pelaajia + 1,
                RankingKisa = false,
                Nimi = nimi
            };

            this.TestattavaKilpailu.Loki = loki;
            this.Arpa = new Random();

            for (int p = 0; p < pelaajia; ++p)
            {
                this.TestattavaKilpailu.LisaaPelaaja(Tyypit.Nimi.KeksiNimi(this.Arpa));
            }
        }

        public void PelaaKilpailu(IStatusRivi status, int vaihe, int vaiheMax)
        {
            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Arvotaan kaavio", this.TestattavaKilpailu.Nimi), true, vaihe, vaiheMax);
            ArvoKaavio();

            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Pelataan pelit", this.TestattavaKilpailu.Nimi), true, vaihe + 2, vaiheMax);
            PelaaKaavio();

            status.PaivitaStatusRivi(string.Format("Testataan kilpailua {0} - Tarkistetaan pelit", this.TestattavaKilpailu.Nimi), true, vaihe + 3, vaiheMax);
            TarkistaPelit();
        }

        /// <summary>
        /// Kopioi osallistujalistan oikeasta kilpailusta ja tarkistaa että arvotut 1. ja 2. kierros ovat identtiset
        /// </summary>
        private void ArvoKaavio()
        {
            string virhe = string.Empty;
            if (!this.TestattavaKilpailu.ArvoKaavio(out virhe))
            {
                throw new Exception(string.Format("Kaavion arpominen epäonnistui: {0}", virhe));
            }

            // Tarkistetaan että 1. ja 2. kierroksen pelit on samat
            /*
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
             */
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
                        var uusiPeli = this.TestattavaKilpailu.Pelit.Last();
                        if (this.TestattavaKilpailu.MukanaOlevatPelaajatEnnenPelia(uusiPeli).Count() > 2)
                        {
                            if (this.TestattavaKilpailu.Pelit.Any(x => 
                                x.PeliNumero < uusiPeli.PeliNumero &&
                                x.SisaltaaPelaajat(uusiPeli.Id1, uusiPeli.Id2)))
                            {
                                throw new Exception(string.Format("Hakuvirhe! Pelaajat {0} ja {1} pelaavat uudestaan vastakkain ennen finaalia", 
                                    uusiPeli.PelaajanNimi1,
                                    uusiPeli.PelaajanNimi2));
                            }
                        }
                    }
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
                if (this.TestattavaKilpailu.KilpailuOnPaattynyt)
                {
                    return false;
                }
                else
                {
                    throw new Exception("Kaaviossa ei ollut pelattavia pelejä");
                }
            }

            if (this.Arpa.Next(100) < 50)
            {
                peli.Pisteet1 = "60";
                peli.Pisteet2 = this.Arpa.Next(0, 59).ToString();
            }
            else
            {
                peli.Pisteet2 = "60";
                peli.Pisteet1 = this.Arpa.Next(0, 59).ToString();
            }

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

        public void TarkistaPelit()
        { 
        }
    }
}

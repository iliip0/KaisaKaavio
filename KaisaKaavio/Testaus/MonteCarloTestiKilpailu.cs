using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public int UusintaHakuvirheita { get; private set; }    // < Hakuvirheitä, joissa tulee sama pelipari uudestaan ennen finaalia
        public Dictionary<int, int> UusintaHakuvirheitaPelaajaMaaranMukaan { get; private set; }
        public int EdellaHakuvirheita { get; private set; }     // < Hakuvirheitä, joissa toinen pelaaja on yli kierroksen vastustajaa edellä kaaviossa

        public MonteCarloTestiKilpailu(string nimi, int poytienMaara, bool satunnainenPelienJarjestys, int pelaajia, Loki loki)
        {
            this.UusintaHakuvirheita = 0;
            this.UusintaHakuvirheitaPelaajaMaaranMukaan = new Dictionary<int, int>();
            this.EdellaHakuvirheita = 0;

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
                Nimi = nimi,
                TestiKilpailu = true
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

        private void ArvoKaavio()
        {
            string virhe = string.Empty;
            if (!this.TestattavaKilpailu.ArvoKaavio(out virhe))
            {
                throw new Exception(string.Format("Kaavion arpominen epäonnistui: {0}", virhe));
            }

            double i = 1.0;
            foreach (var p in this.TestattavaKilpailu.Osallistujat.ToArray().OrderBy(x => x.Nimi))
            {
                p.Taso = (float)Math.Max(0.1, Math.Pow(0.8, i)) + (float)(this.Arpa.NextDouble() * 0.2);
                i += 1.0;
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
                        var uusiPeli = this.TestattavaKilpailu.Pelit.Last();
                        if (this.TestattavaKilpailu.MukanaOlevatPelaajatEnnenPelia(uusiPeli).Count() > 2)
                        {
                            if (this.TestattavaKilpailu.Pelit.Any(x => 
                                x.PeliNumero < uusiPeli.PeliNumero &&
                                x.SisaltaaPelaajat(uusiPeli.Id1, uusiPeli.Id2)))
                            {
                                int mukana = this.TestattavaKilpailu.MukanaOlevatPelaajatEnnenPelia(uusiPeli).Count();

                                if (this.UusintaHakuvirheitaPelaajaMaaranMukaan.ContainsKey(mukana))
                                {
                                    this.UusintaHakuvirheitaPelaajaMaaranMukaan[mukana]++;
                                }
                                else 
                                {
                                    this.UusintaHakuvirheitaPelaajaMaaranMukaan.Add(mukana, 1);
                                }

                                this.UusintaHakuvirheita++;
                            }
                        }

                        if (Math.Abs(uusiPeli.KierrosPelaaja1 - uusiPeli.KierrosPelaaja2) > 1)
                        {
                            this.EdellaHakuvirheita++;
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

            Pelaaja p1 = this.TestattavaKilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id1);
            Pelaaja p2 = this.TestattavaKilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id2);

            double pisteet1 = this.Arpa.NextDouble() * p1.Taso;
            double pisteet2 = this.Arpa.NextDouble() * p2.Taso;

            if (pisteet1 > pisteet2)
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
            var voittaja = TestattavaKilpailu.Voittaja();

            if (voittaja == null)
            {
                throw new Exception("Testikilpailua ei pelattu loppuun asti!!! Voittaja ei ole selvillä");
            }

            Debug.WriteLine(string.Format("#### Kilpailun voitti {0}, taso {1}", voittaja.Nimi, voittaja.Taso));
        }
    }
}

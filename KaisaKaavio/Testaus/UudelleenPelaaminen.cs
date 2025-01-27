using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    /// <summary>
    /// Apuluokka, joka pelaa uudelleen jo pelatun kilpailun samassa ottelujärjestyksessä
    /// </summary>
    public class UudelleenPelaaminen
    {
#if DEBUG
        private Kilpailu kilpailu = null;
        private Kilpailu vanhaKilpailu = null;
        private Loki loki = null;
#endif

        public UudelleenPelaaminen(Kilpailu kilpailu, Loki loki)
        {
#if DEBUG
            this.kilpailu = kilpailu;
            this.loki = loki;
#endif
        }

        public bool AvaaKilpailu(string tiedosto)
        {
#if DEBUG
            try
            {
                this.vanhaKilpailu = new Kilpailu();
                this.vanhaKilpailu.Avaa(tiedosto, false);

                this.kilpailu.Nimi = this.vanhaKilpailu.Nimi + "(Uusinta)";
                this.kilpailu.Tiedosto = this.vanhaKilpailu.Tiedosto.Replace(".xml", "(Uusinta).xml");

                this.kilpailu.AlkamisAika = this.vanhaKilpailu.AlkamisAika;
                //this.kilpailu.PaattymisAika = this.kilpailu.AlkamisAika;

                this.kilpailu.Ilmoittautuminen = this.vanhaKilpailu.Ilmoittautuminen;
                this.kilpailu.JarjestavaSeura = this.vanhaKilpailu.JarjestavaSeura;
                this.kilpailu.KaavioTyyppi = this.vanhaKilpailu.KaavioTyyppi;
                this.kilpailu.KellonAika = this.vanhaKilpailu.KellonAika;
                this.kilpailu.KilpailunJohtaja = this.vanhaKilpailu.KilpailunJohtaja;
                this.kilpailu.KilpailunTyyppi = this.vanhaKilpailu.KilpailunTyyppi;
                this.kilpailu.Sijoittaminen = this.vanhaKilpailu.Sijoittaminen;
                this.kilpailu.Laji = this.vanhaKilpailu.Laji;
                this.kilpailu.LisaTietoa = this.vanhaKilpailu.LisaTietoa;
                this.kilpailu.LisenssiVaatimus = this.vanhaKilpailu.LisenssiVaatimus;
                this.kilpailu.MaksuTapa = this.vanhaKilpailu.MaksuTapa;
                this.kilpailu.OsallistumisMaksu = this.vanhaKilpailu.OsallistumisMaksu;
                this.kilpailu.OsallistumisOikeus = this.vanhaKilpailu.OsallistumisOikeus;
                this.kilpailu.Palkinnot = this.vanhaKilpailu.Palkinnot;
                this.kilpailu.PelaajiaEnintaan = this.vanhaKilpailu.PelaajiaEnintaan;
                this.kilpailu.PeliAika = this.vanhaKilpailu.PeliAika;
                this.kilpailu.PuhelinNumero = this.vanhaKilpailu.PuhelinNumero;
                this.kilpailu.Pukeutuminen = this.vanhaKilpailu.Pukeutuminen;
                this.kilpailu.RankingKisaTyyppi = this.vanhaKilpailu.RankingKisaTyyppi;
                this.kilpailu.RankkareidenMaara = this.vanhaKilpailu.RankkareidenMaara;
                this.kilpailu.SijoitustenMaaraytyminen = this.vanhaKilpailu.SijoitustenMaaraytyminen;
                this.kilpailu.TavoitePistemaara = this.vanhaKilpailu.TavoitePistemaara;
                this.kilpailu.Yksipaivainen = this.vanhaKilpailu.Yksipaivainen;

                this.kilpailu.Pelit.Clear();
                this.kilpailu.Osallistujat.Clear();
                this.kilpailu.OsallistujatJarjestyksessa.Clear();
                this.kilpailu.JalkiIlmoittautuneet.Clear();

                this.kilpailu.RankingKisa = false;
                this.kilpailu.PelienTilannePaivitysTarvitaan = false;
                this.kilpailu.PelinTulosMuuttunutNumerolla = Int32.MaxValue;
                this.kilpailu.KilpailuPaattyiJuuri = false;
                this.kilpailu.MaxKierros = 0;
                this.kilpailu.TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
                this.kilpailu.TallennusTarvitaan = false;
                this.kilpailu.HakuTarvitaan = false;

                foreach (var osallistuja in this.vanhaKilpailu.Osallistujat)
                {
                    this.kilpailu.Osallistujat.Add(new Pelaaja() 
                    {
                        Id = osallistuja.Id,
                        Nimi = osallistuja.Nimi,
                        IlmoittautumisNumero = osallistuja.IlmoittautumisNumero,
                        Seura = osallistuja.Seura,
                        SeuranJasenMaksu = osallistuja.SeuranJasenMaksu,
                        Sijoitettu = osallistuja.Sijoitettu,
                        KabikeMaksu = osallistuja.KabikeMaksu,
                        LisenssiMaksu = osallistuja.LisenssiMaksu,
                        OsMaksu = osallistuja.OsMaksu,
                        Veloitettu = osallistuja.Veloitettu,
                    });
                }

                foreach (var peli in this.vanhaKilpailu.Pelit.Where(x => x.Kierros < 3))
                {
                    this.kilpailu.LisaaPeli(
                        this.kilpailu.Osallistujat.First(x => x.Id == peli.Id1),
                        this.kilpailu.Osallistujat.First(x => x.Id == peli.Id2));
                    
                }

                if (this.loki != null)
                {
                    this.loki.Kirjoita(string.Format("Avattiin kilpailu {0} uudelleen pelattavaksi (testausta varten)", tiedosto));
                }

                return true;
            }
            catch (Exception e)
            {
                if (this.loki != null)
                {
                    this.loki.Kirjoita(
                        string.Format("Uudelleenpelattavan kilpailun avaaminen epäonnistui tiedostosta {0}", tiedosto),
                        e,
                        true);
                }
            }
#endif
            return false;
        }

        private bool AloitaPeli(Peli peli)
        {
#if DEBUG
            var vanhaPeli = this.vanhaKilpailu.Pelit.FirstOrDefault(x => 
                x.Kierros == peli.Kierros &&
                x.PeliNumero == peli.PeliNumero &&
                x.SisaltaaPelaajat(peli.Id1, peli.Id2));

            if (vanhaPeli != null)
            {
                if (!string.IsNullOrEmpty(vanhaPeli.Poyta))
                {
                    peli.Poyta = vanhaPeli.Poyta;
                    return peli.Tilanne == PelinTilanne.Kaynnissa;
                }
                else
                {
                    return peli.KaynnistaPeli(null, true);
                }
            }
#endif
            return false;
        }

        private bool LopetaPeli(Peli peli)
        {
#if DEBUG
            var vanhaPeli = this.vanhaKilpailu.Pelit.FirstOrDefault(x =>
                x.Kierros == peli.Kierros &&
                x.PeliNumero == peli.PeliNumero &&
                x.SisaltaaPelaajat(peli.Id1, peli.Id2));

            if (vanhaPeli != null)
            {
                peli.Pisteet1 = vanhaPeli.Pisteet1;
                peli.Pisteet2 = vanhaPeli.Pisteet2;
                return peli.Tilanne == PelinTilanne.Pelattu;
            }
#endif
            return false;
        }

        public bool SeuraavaPeli()
        {
#if DEBUG
            if (this.vanhaKilpailu == null)
            {
                return false;
            }

            string ekaPeliValmiinaAlkamaanAika = null;
            Peli ekaPeliValmiinaAlkamaan = null;

            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Tilanne == PelinTilanne.ValmiinaAlkamaan))
            {
                var vanhaPeli = this.vanhaKilpailu.Pelit.FirstOrDefault(x => 
                    x.Kierros == peli.Kierros &&
                    x.PeliNumero == peli.PeliNumero &&
                    x.SisaltaaPelaajat(peli.Id1, peli.Id2) &&
                    !string.IsNullOrEmpty(x.Alkoi));

                if (vanhaPeli != null)
                {
                    if (ekaPeliValmiinaAlkamaan == null ||
                        vanhaPeli.Alkoi.CompareTo(ekaPeliValmiinaAlkamaanAika) < 0)
                    {
                        if (!this.kilpailu.Pelit.Any(x => 
                            x.Tilanne == PelinTilanne.Kaynnissa &&
                            string.Equals(vanhaPeli.Poyta, x.Poyta)))
                        {
                            ekaPeliValmiinaAlkamaan = peli;
                            ekaPeliValmiinaAlkamaanAika = vanhaPeli.Alkoi;
                        }
                    }
                }
            }

            string ekaPeliValmiinaLoppumaanAika = null;
            Peli ekaPeliValmiinaLoppumaan = null;

            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Tilanne == PelinTilanne.Kaynnissa))
            {
                var vanhaPeli = this.vanhaKilpailu.Pelit.FirstOrDefault(x =>
                    x.Kierros == peli.Kierros &&
                    x.PeliNumero == peli.PeliNumero &&
                    x.SisaltaaPelaajat(peli.Id1, peli.Id2) &&
                    !string.IsNullOrEmpty(x.Paattyi));

                if (vanhaPeli != null)
                {
                    if (ekaPeliValmiinaLoppumaan == null ||
                        vanhaPeli.Paattyi.CompareTo(ekaPeliValmiinaLoppumaanAika) < 0)
                    {
                        ekaPeliValmiinaLoppumaan = peli;
                        ekaPeliValmiinaLoppumaanAika = vanhaPeli.Paattyi;
                    }
                }
            }

            if (ekaPeliValmiinaAlkamaan != null && ekaPeliValmiinaLoppumaan != null)
            {
                if (ekaPeliValmiinaAlkamaanAika.CompareTo(ekaPeliValmiinaLoppumaanAika) <= 0)
                {
                    return AloitaPeli(ekaPeliValmiinaAlkamaan);
                }
                else 
                {
                    return LopetaPeli(ekaPeliValmiinaLoppumaan);
                }
            }
            else if (ekaPeliValmiinaAlkamaan != null)
            {
                return AloitaPeli(ekaPeliValmiinaAlkamaan);
            }
            else if (ekaPeliValmiinaLoppumaan != null)
            {
                return LopetaPeli(ekaPeliValmiinaLoppumaan);
            }
#endif
            return false;
        }
    }
}

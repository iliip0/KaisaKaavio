using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Kilpailu
        : NotifyPropertyChanged
    {
        [DefaultValue("")]
        public string Nimi { get; set; }

        [DefaultValue("")]
        public string KilpailunJohtaja { get; set; }

        [DefaultValue("")]
        public string JarjestavaSeura { get; set; }

        [DefaultValue("")]
        public string PuhelinNumero { get; set; }
        
        public DateTime AlkamisAika { get; set; }

        [DefaultValue("")]
        public string KellonAika { get; set; }
        
        public DateTime PaattymisAika { get; set; }

        public bool Yksipaivainen { get; set; }

        public bool RankingKisa { get; set; }

        public Ranking.RankingSarjanPituus RankingKisaTyyppi { get; set; }

        public SijoitustenMaaraytyminen SijoitustenMaaraytyminen { get; set; }

        public decimal PeliAika { get; set; }
        public decimal RankkareidenMaara { get; set; }
        public decimal TavoitePistemaara { get; set; }
        public decimal PelaajiaEnintaan { get; set; }

        [DefaultValue("")]
        public string LisenssiVaatimus { get; set; }

        public KaavioTyyppi KaavioTyyppi { get; set; }

        [DefaultValue("")]
        public string LisaTietoa { get; set; }

        [DefaultValue("")]
        public string OsallistumisMaksu { get; set; }

        [DefaultValue("")]
        public string OsallistumisOikeus { get; set; }

        [DefaultValue("")]
        public string MaksuTapa { get; set; }

        [DefaultValue("")]
        public string Pukeutuminen { get; set; }

        [DefaultValue("")]
        public string Palkinnot { get; set; }

        [DefaultValue("")]
        public string Ilmoittautuminen { get; set; }
        
        public bool KilpailuOnViikkokisa { get; set; }
        public Laji Laji { get; set; }

        public BindingList<Pelaaja> Osallistujat { get; set; }
        public BindingList<Pelaaja> JalkiIlmoittautuneet { get; set; }
        public BindingList<Peli> Pelit { get; set; }
        
        [XmlIgnore]
        public BindingList<Pelaaja> OsallistujatJarjestyksessa { get; set; }

        [XmlIgnore]
        public string Tiedosto { get; set; }

        [XmlIgnore]
        public bool HakuTarvitaan = false;

        [XmlIgnore]
        public int PelinTulosMuuttunutNumerolla = Int32.MaxValue;

        [XmlIgnore]
        public int TallennusAjastin = 0;

        [XmlIgnore]
        public bool TallennusTarvitaan = false;

        [XmlIgnore]
        public bool PelienTilannePaivitysTarvitaan = false;

        [XmlIgnore]
        public Loki Loki = null;

        [XmlIgnore]
        public int MaxKierros = 0;

        [XmlIgnore]
        public bool KilpailuPaattyiJuuri = false;

        public Kilpailu()
        {
            Osallistujat = new BindingList<Pelaaja>();
            Osallistujat.ListChanged += Osallistujat_ListChanged;

            OsallistujatJarjestyksessa = new BindingList<Pelaaja>();

            JalkiIlmoittautuneet = new BindingList<Pelaaja>();

            Pelit = new BindingList<Peli>();
            Pelit.ListChanged += Pelit_ListChanged;

            Nimi = string.Format("Kaisan viikkokilpailu {0}.{1}.{2}", 
                DateTime.Now.Day, 
                DateTime.Now.Month, 
                DateTime.Now.Year);

            AlkamisAika = DateTime.Today;
            KellonAika = "18:00";
            Yksipaivainen = true;

            RankingKisa = false;
            RankingKisaTyyppi = Ranking.RankingSarjanPituus.Kuukausi;

            OsallistumisMaksu = "10€";
            OsallistumisOikeus = "Avoin kaikille";
            KaavioTyyppi = KaavioTyyppi.Pudari3Kierros;
            JarjestavaSeura = string.Empty;
            PaattymisAika = DateTime.Today;
            PeliAika = 40;
            TavoitePistemaara = 60;
            RankkareidenMaara = 3;
            Tiedosto = string.Empty;
            LisaTietoa = string.Empty;
            KilpailunJohtaja = string.Empty;
            LisenssiVaatimus = string.Empty;
            MaksuTapa = string.Empty;
            Palkinnot = string.Empty;
            Pukeutuminen = "Vapaa";
            Ilmoittautuminen = string.Empty;
            PelaajiaEnintaan = 32;
            KilpailuOnViikkokisa = true;
            Laji = KaisaKaavio.Laji.Kaisa;
            SijoitustenMaaraytyminen = KaisaKaavio.SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista;

            TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
            TallennusTarvitaan = false;
        }

#region Pelit

        void LisaaPeli(Peli peli)
        {
            peli.PropertyChanged += Kilpailu_PropertyChanged;
            peli.Kilpailu = this;
            peli.PaivitaRivinUlkoasu = true;

            Pelit.Add(peli);
        }

        public void PoistaPeli(Peli peli)
        {
            Pelit.Remove(peli);

            peli.PropertyChanged -= Kilpailu_PropertyChanged;
            peli.Kilpailu = null;
        }

        public void PoistaKaikkiPelit()
        {
            while (Pelit.Count() > 0)
            {
                PoistaPeli(Pelit.First());
            }
        }

        void Pelit_ListChanged(object sender, ListChangedEventArgs e)
        {
            this.TallennusTarvitaan = true;
            this.PelienTilannePaivitysTarvitaan = true;

            RaisePropertyChanged("ArvonnanTilanneTeksti");
            RaisePropertyChanged("ArvontaNappiPainettavissa");
            RaisePropertyChanged("ArvontaNapinTeksti");
            RaisePropertyChanged("VoiLisataPelaajia");
            RaisePropertyChanged("VoiPoistaaPelaajia");
            RaisePropertyChanged("KilpailuAlkanut");
        }

        private void Kilpailu_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Pyydetään uusi haku aina kun jonkun pelin tulos muuttuu
            if (sender is Peli)
            {
                Peli peli = (Peli)sender;
                if (string.Equals(e.PropertyName, "Tulos"))
                {
                    this.PelienTilannePaivitysTarvitaan = true;

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (Voittaja() != null)
                        {
                            this.KilpailuPaattyiJuuri = true;
                        }
                    }
                }
                else if (string.Equals(e.PropertyName, "Tilanne"))
                {
                    this.PelienTilannePaivitysTarvitaan = true;

                    RaisePropertyChanged("ArvonnanTilanneTeksti");
                    RaisePropertyChanged("ArvontaNappiPainettavissa");
                    RaisePropertyChanged("ArvontaNapinTeksti");
                    RaisePropertyChanged("VoiLisataPelaajia");
                    RaisePropertyChanged("VoiPoistaaPelaajia");
                    RaisePropertyChanged("PelienTilanneTeksti");

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (Voittaja() != null)
                        {
                            this.KilpailuPaattyiJuuri = true;
                        }
                    }
                }
            }

            this.TallennusTarvitaan = true;
        }

        private void PoistaTyhjatPelit(int kierroksestaAlkaen)
        {
            bool poistettiinJotain = false;

            while (true)
            {
                var tyhja = Pelit.FirstOrDefault(x =>
                    (x.Kierros >= kierroksestaAlkaen) &&
                    (x.Tilanne != PelinTilanne.Kaynnissa && x.Tilanne != PelinTilanne.Pelattu));

                if (tyhja != null)
                {
#if DEBUG
                    Debug.WriteLine("Poistetaan alkamaton peli {0} - {1} kierrokselta {2}", tyhja.Pelaaja1, tyhja.Pelaaja2, tyhja.Kierros);
#endif
                    PoistaPeli(tyhja);
                    poistettiinJotain = true;
                }
                else
                {
                    break;
                }
            }

            if (poistettiinJotain)
            {
                PaivitaPelinumerot();
            }
        }

        public void PoistaTyhjatPelitAlkaenNumerosta(int numerostaAlkaen)
        {
            bool poistettiinJotain = false;

            while (true)
            {
                var tyhja = Pelit.FirstOrDefault(x =>
                    (x.Kierros > 2) &&
                    (x.PeliNumero > numerostaAlkaen) &&
                    (x.Tilanne != PelinTilanne.Kaynnissa && x.Tilanne != PelinTilanne.Pelattu));

                if (tyhja != null)
                {
#if DEBUG
                    this.Loki.Kirjoita(string.Format("Poistetaan alkamaton peli {0} - {1} kierrokselta {2}", tyhja.Pelaaja1, tyhja.Pelaaja2, tyhja.Kierros));
#endif
                    PoistaPeli(tyhja);
                    poistettiinJotain = true;
                }
                else
                {
                    break;
                }
            }

            if (poistettiinJotain)
            {
                PaivitaPelinumerot();
            }
        }

        private PelinTilanne LisaaPeli(Pelaaja pelaaja, int kierros1, Pelaaja vastustaja, int kierros2)
        {
            Peli peli = new Peli()
            {
                Kierros = Math.Max(kierros1, kierros2),
                KierrosPelaaja1 = kierros1,
                KierrosPelaaja2 = kierros2,
                Kilpailu = this,
                Alkoi = string.Empty,
                Paattyi = string.Empty,
                PelaajaId1 = pelaaja == null ? string.Empty : pelaaja.Id.ToString(),
                PelaajaId2 = vastustaja == null ? string.Empty : vastustaja.Id.ToString(),
                PeliNumero = Pelit.Count() + 1,
                Pisteet1 = string.Empty,
                Pisteet2 = string.Empty,
                Poyta = string.Empty,
                Tilanne = PelinTilanne.Tyhja,
                PaivitaRivinUlkoasu = true
            };

            if (TarkistaVoikoPeliAlkaa(peli))
            {
                peli.Tilanne = PelinTilanne.ValmiinaAlkamaan;
            }

#if DEBUG
            int peleja = Pelit.Count;
#endif

            // Lajitellaan pelit lennosta
            if ((Pelit.Count == 0) || (Pelit.Last().LajitteluNumero <= peli.LajitteluNumero))
            {
                LisaaPeli(peli);
            }
            else
            {
                Loki.Kirjoita("Lajitellaan pelit", null, false);

                List<Peli> lajittelemattomat = new List<Peli>();
                lajittelemattomat.Add(peli);

                while ((Pelit.Count > 0) && (Pelit.Last().LajitteluNumero > peli.LajitteluNumero))
                {
                    lajittelemattomat.Add(Pelit.Last());
                    PoistaPeli(Pelit.Last());
                }

                foreach (var lajiteltuPeli in lajittelemattomat.OrderBy(x => x.LajitteluNumero))
                {
                    lajiteltuPeli.PeliNumero = Pelit.Count + 1;
                    LisaaPeli(lajiteltuPeli);
                }
            }

#if DEBUG // Tarkista että pelit on lajiteltu oikein
            if ((peleja + 1) != Pelit.Count)
            {
                Debug.WriteLine("# VIRHE: Pelien määrä ei täsmää!");
            }

            {
                int numero = 1;
                int lajittelu = -1000000;
                foreach (var p in Pelit)
                {
                    if (p.Kilpailu != this)
                    {
                        Debug.WriteLine("# VIRHE: Väärä kilpailu pelissä!");
                    }

                    if (p.LajitteluNumero < lajittelu)
                    {
                        Debug.WriteLine("# VIRHE: Pelit järjestetty väärin!");
                    }

                    if (p.PeliNumero != numero)
                    {
                        Debug.WriteLine("# VIRHE: Pelinumerot väärin!");
                    }

                    lajittelu = p.LajitteluNumero;
                    numero++;
                }
            }
#endif
            Loki.Kirjoita(string.Format("Haettiin peli kierrokselle {0}. {1} vs {2}",
                peli.Kierros,
                pelaaja != null ? pelaaja.Nimi : string.Empty,
                vastustaja != null ? vastustaja.Nimi : string.Empty));

            PelienTilannePaivitysTarvitaan = true;

            return peli.Tilanne;
        }

        public PelinTilanne LisaaPeli(Pelaaja pelaaja, Pelaaja vastustaja)
        {
            int kierros1 = pelaaja == null ? 0 : Pelit.Count(x =>
                x.SisaltaaPelaajan(pelaaja.Id)) + 1;

            int kierros2 = vastustaja == null ? 0 : Pelit.Count(x =>
                x.SisaltaaPelaajan(vastustaja.Id)) + 1;

            if (kierros2 < kierros1)
            {
                return LisaaPeli(vastustaja, kierros2, pelaaja, kierros1);
            }

            else if ((kierros1 == kierros2) && (vastustaja.Id < pelaaja.Id))
            {
                return LisaaPeli(vastustaja, kierros2, pelaaja, kierros1);
            }

            else
            {
                return LisaaPeli(pelaaja, kierros1, vastustaja, kierros2);
            }
        }

#endregion

#region Osallistujat
        public void PoistaEiMukanaOlevatPelaajat()
        {
            while (true)
            {
                var osallistuja = this.Osallistujat.FirstOrDefault(x => x.Id < 0);
                if (osallistuja != null)
                {
                    this.Osallistujat.Remove(osallistuja);
                }
                else
                {
                    break;
                }
            }
        }
#endregion

        public void PaivitaOsallistujatJarjestyksessa()
        {
            this.OsallistujatJarjestyksessa.Clear();

            foreach (var o in this.Osallistujat
                .Where(x => !string.IsNullOrEmpty(x.Nimi) && x.Id >= 0)
                .OrderBy(x => x.Id))
            {
                this.OsallistujatJarjestyksessa.Add(o);
            }
        }

        public void PaivitaKaavioData()
        {
            PaivitaOsallistujatJarjestyksessa();

            this.MaxKierros = 0;

            foreach (var osallistuja in this.OsallistujatJarjestyksessa)
            {
                osallistuja.Sijoitus = 0;
                osallistuja.Pudotettu = false;
                osallistuja.Tappiot = 0;
                osallistuja.Voitot = 0;
                osallistuja.Pisteet = 0;
                osallistuja.Pelit.Clear();
            }

            foreach (var peli in Pelit.Where(x => x.Id1 >= 0 && x.Id2 >= 0))
            {
                var osallistuja1 = this.OsallistujatJarjestyksessa.FirstOrDefault(x => x.Id == peli.Id1);
                var osallistuja2 = this.OsallistujatJarjestyksessa.FirstOrDefault(x => x.Id == peli.Id2);

                if (osallistuja1 != null && osallistuja2 != null)
                {
                    Pelaaja.PeliTietue p1 = new Pelaaja.PeliTietue() 
                    {
                        Vastustaja = peli.Id2,
                        Pisteet = peli.Pisteet(peli.Id1),
                        Pelattu = peli.Tilanne == PelinTilanne.Pelattu,
                        Pudari = peli.OnPudotusPeli(),
                        Tilanne = peli.Tilanne,
                        Kierros = peli.Kierros
                    };

                    osallistuja1.Pelit.Add(p1);

                    Pelaaja.PeliTietue p2 = new Pelaaja.PeliTietue()
                    {
                        Vastustaja = peli.Id1,
                        Pisteet = peli.Pisteet(peli.Id2),
                        Pelattu = peli.Tilanne == PelinTilanne.Pelattu,
                        Pudari = peli.OnPudotusPeli(),
                        Tilanne = peli.Tilanne,
                        Kierros = peli.Kierros
                    };

                    osallistuja2.Pelit.Add(p2);

                    osallistuja1.Pisteet += p1.Pisteet;
                    osallistuja2.Pisteet += p2.Pisteet;

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (!peli.Havisi(peli.Id1))
                        {
                            p1.Voitto = true;
                            osallistuja1.Voitot++;
                        }
                        else
                        {
                            osallistuja1.Tappiot++;
                            if (osallistuja1.Tappiot >= 2 || peli.OnPudotusPeli())
                            {
                                osallistuja1.Pudotettu = true;
                            }
                        }

                        if (!peli.Havisi(peli.Id2))
                        {
                            p2.Voitto = true;
                            osallistuja2.Voitot++;
                        }
                        else 
                        {
                            osallistuja2.Tappiot++;
                            if (osallistuja2.Tappiot >= 2 || peli.OnPudotusPeli())
                            {
                                osallistuja2.Pudotettu = true;
                            }
                        }
                    }
                }

                this.MaxKierros = Math.Max(this.MaxKierros, osallistuja1.Pelit.Count);
                this.MaxKierros = Math.Max(this.MaxKierros, osallistuja2.Pelit.Count);
            }

            if (this.KilpailuOnPaattynyt)
            {
                var tulokset = Tulokset();
                foreach (var tulos in tulokset)
                {
                    var p = this.OsallistujatJarjestyksessa.FirstOrDefault(x => x.Id == tulos.Id);
                    if (p != null)
                    {
                        p.Sijoitus = tulos.Sijoitus;
                    }
                }
            }
        }

        void Osallistujat_ListChanged(object sender, ListChangedEventArgs e)
        {
            RaisePropertyChanged("ArvonnanTilanneTeksti");
            RaisePropertyChanged("ArvontaNappiPainettavissa");
            RaisePropertyChanged("ArvontaNapinTeksti");
            RaisePropertyChanged("VoiLisataPelaajia");
            RaisePropertyChanged("VoiPoistaaPelaajia");

            TallennusTarvitaan = true;
        }

        /// <summary>
        /// Kertoo onko sallittua muokata peliä annetulla numerolla.
        /// </summary>
        public bool VoiMuokataPelia(Peli peli)
        {
            if (this.PelinTulosMuuttunutNumerolla != Int32.MaxValue)
            {
                return false; // Ei saa muokata jos pelien tuloksia on juuri muokattu
            }

            if (this.Pelit.Any(x => x.PeliNumero != peli.PeliNumero && x.Tulos == PelinTulos.Virheellinen))
            {
                return false; // Ei saa muokata jos peleissä on virheellisiä tuloksia (voittaja ei selvillä)
            }

            if (peli.Tilanne == PelinTilanne.ValmiinaAlkamaan || peli.Tilanne == PelinTilanne.Kaynnissa)
            {
                return true;
            }

            if (peli.Tilanne == PelinTilanne.Pelattu)
            {
                if (this.Pelit.Any(x =>
                    (x.PeliNumero > peli.PeliNumero) &&
                    (x.Tilanne == PelinTilanne.Kaynnissa || x.Tilanne == PelinTilanne.Pelattu) &&
                    x.SisaltaaJommanKummanPelaajan(peli.Id1, peli.Id2)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void TallennaNimella(string nimi)
        {
            Loki.Kirjoita(string.Format("Tallennetaan kilpailu tiedostoon {0}", nimi));

            XmlSerializer serializer = new XmlSerializer(typeof(Kilpailu));

            string nimiTmp = Path.GetTempFileName();

            using (TextWriter writer = new StreamWriter(nimiTmp))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }

            File.Copy(nimiTmp, nimi, true);
            File.Delete(nimiTmp);

            Tiedosto = nimi;

            this.TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
            this.TallennusTarvitaan = false;
        }

        public void Tallenna()
        {
            if (!string.IsNullOrEmpty(Tiedosto))
            {
                TallennaNimella(Tiedosto);
            }
        }

        public void Avaa(string tiedosto)
        {
            if (Loki != null)
            {
                Loki.Kirjoita(string.Format("Avataan kilpailu tiedostosta {0}", tiedosto));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Kilpailu));

            Kilpailu kilpailu = null;

            using (TextReader reader = new StreamReader(tiedosto))
            {
                kilpailu = (Kilpailu)serializer.Deserialize(reader);
                reader.Close();
            }

            if (kilpailu != null)
            {
                this.Tiedosto = tiedosto;
                this.AlkamisAika = kilpailu.AlkamisAika;
                this.PaattymisAika = kilpailu.PaattymisAika;
                this.KilpailunJohtaja = kilpailu.KilpailunJohtaja;
                this.JarjestavaSeura = kilpailu.JarjestavaSeura;
                this.Nimi = kilpailu.Nimi;
                this.PeliAika = kilpailu.PeliAika;
                this.PuhelinNumero = kilpailu.PuhelinNumero;
                this.RankkareidenMaara = kilpailu.RankkareidenMaara;
                this.KaavioTyyppi = kilpailu.KaavioTyyppi;
                this.OsallistumisMaksu = kilpailu.OsallistumisMaksu;
                this.OsallistumisOikeus = kilpailu.OsallistumisOikeus;
                this.KellonAika = kilpailu.KellonAika;
                this.LisaTietoa = kilpailu.LisaTietoa;
                this.Yksipaivainen = kilpailu.Yksipaivainen;
                this.RankingKisa = kilpailu.RankingKisa;
                this.RankingKisaTyyppi = kilpailu.RankingKisaTyyppi;
                this.LisenssiVaatimus = kilpailu.LisenssiVaatimus;
                this.MaksuTapa = kilpailu.MaksuTapa;
                this.Pukeutuminen = kilpailu.Pukeutuminen;
                this.Palkinnot = kilpailu.Palkinnot;
                this.Ilmoittautuminen = kilpailu.Ilmoittautuminen;
                this.PelaajiaEnintaan = kilpailu.PelaajiaEnintaan;
                this.TavoitePistemaara = kilpailu.TavoitePistemaara;
                this.KilpailuOnViikkokisa = kilpailu.KilpailuOnViikkokisa;
                this.Laji = kilpailu.Laji;
                this.SijoitustenMaaraytyminen = kilpailu.SijoitustenMaaraytyminen;

                this.JalkiIlmoittautuneet.Clear();

                foreach (var j in kilpailu.JalkiIlmoittautuneet)
                {
                    if (!string.IsNullOrEmpty(j.Nimi))
                    {
                        this.JalkiIlmoittautuneet.Add(j);
                    }
                }

                this.Osallistujat.Clear();
                this.OsallistujatJarjestyksessa.Clear();

                foreach (var o in kilpailu.Osallistujat)
                {
                    if (!string.IsNullOrEmpty(o.Nimi))
                    {
                        this.Osallistujat.Add(o);
                    }
                }

                PoistaKaikkiPelit();

                int pelinNumero = 1;

                foreach (var p in kilpailu.Pelit)
                {
                    if (p.Kierros < 3 ||
                        (p.Tilanne == PelinTilanne.Pelattu || p.Tilanne == PelinTilanne.Kaynnissa))
                    {
                        p.PeliNumero = pelinNumero++;
                        LisaaPeli(p);
                    }
                }

                PaivitaOsallistujatJarjestyksessa();

                this.TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
            }
        }

        public string PelaajanNimiKaaviossa(string idString)
        {
            int id = -1;
            if (!Int32.TryParse(idString, out id))
            {
                return idString;
            }

            var pelaaja = this.Osallistujat.FirstOrDefault(x => x.Id == id);
            if (pelaaja == null || string.IsNullOrEmpty(pelaaja.Nimi))
            {
                return idString;
            }

            string nimi = pelaaja.Nimi;
            
            if (!string.IsNullOrEmpty(pelaaja.Seura))
            {
                nimi += " " + pelaaja.Seura.Trim();
            }

            if (!string.IsNullOrEmpty(pelaaja.Sijoitettu))
            {
                nimi += " (" + pelaaja.Sijoitettu.Trim() + ")"; 
            }

            return nimi;
        }

        public void SiirraJalkiIlmoittautuneetOsallistujiin(Asetukset asetukset)
        {
            foreach (var j in this.JalkiIlmoittautuneet)
            {
                if (!string.IsNullOrEmpty(j.Nimi))
                {
                    var pelaaja = asetukset.Pelaajat.FirstOrDefault(x => string.Equals(j.Nimi, x.Nimi));
                    if (pelaaja != null)
                    {
                        j.Seura = pelaaja.Seura;
                    }

                    this.Osallistujat.Add(j);
                }
            }

            this.JalkiIlmoittautuneet.Clear();
        }

        public bool ArvoKaavio(out string virhe)
        {
            virhe = string.Empty;

            if (Pelit.Any(x => (x.Kierros > 1) && (x.Tilanne == PelinTilanne.Pelattu || x.Tilanne == PelinTilanne.Kaynnissa)))
            {
                this.Loki.Kirjoita("Kaavion arpominen ei ole mahdollista enää toisen kierroksen alettua");
                return false;
            }

            foreach (var osallistuja in this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                if (this.Osallistujat.Count(x => string.Equals(x.Nimi, osallistuja.Nimi, StringComparison.OrdinalIgnoreCase)) > 1)
                {
                    virhe = string.Format("Pelaaja {0} on kahdesti osallistujalistalla", osallistuja.Nimi);
                    return false;
                }
            }

            PoistaTyhjatOsallistujat();
            PoistaTyhjatPelit(1);

            if (!KilpailuOnViikkokisa && 
                this.Osallistujat.Any(x => !string.IsNullOrEmpty(x.Sijoitettu)) &&
                !this.Osallistujat.Any(x => x.Id > 0))
            {
                ArvoPelaajienIdtSijoituksilla();
            }
            else
            {
                ArvoPelaajienIdt();
            }

            PaivitaOsallistujatJarjestyksessa();

            return HaeAlkukierrokset(out virhe);
        }

        private void ArvoPelaajienIdt()
        {
            var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
            int maxId = 0;
            if (osallistujat.Count() > 0)
            {
                maxId = osallistujat.Count(x => x.Id >= 0) + 1;
            }

            Random r = new Random(DateTime.Now.Millisecond);

            foreach (var o in osallistujat.Where(x => x.Id <= 0))
            {
                o.Id = maxId + 1 + r.Next();
            }

            int id = 1;
            foreach (var o in osallistujat.OrderBy(x => x.Id))
            {
                o.Id = id++;
            }
        }

        private void ArvoPelaajienIdtSijoituksilla()
        {
            var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));

            List<int> vapaatIdt = new List<int>();
            int i = 1;
            foreach (var o in osallistujat)
            {
                vapaatIdt.Add(i);
                i++;
            }

            var sijoitetut = osallistujat
                .Where(x => !string.IsNullOrEmpty(x.Sijoitettu))
                .OrderBy(x => x.Sijoitettu)
                .Take(8);

            int sijoitus = 0;
            foreach (var sijoitettu in sijoitetut)
            {
                switch (sijoitus)
                {
                    case 0:
                        sijoitettu.Id = osallistujat.Count() - 1;
                        break;

                    case 1:
                        sijoitettu.Id = (int)Math.Floor(osallistujat.Count() * 0.5f);
                        break;

                    case 2:
                        sijoitettu.Id = (int)Math.Floor(sijoitetut.ElementAt(1).Id * 0.5f);
                        break;

                    case 3:
                        sijoitettu.Id = (int)Math.Round((sijoitetut.ElementAt(1).Id + sijoitetut.ElementAt(0).Id) * 0.5f);
                        break;

                    case 4:
                        sijoitettu.Id = (int)Math.Round((sijoitetut.ElementAt(3).Id + sijoitetut.ElementAt(1).Id) * 0.5f);
                        break;

                    case 5:
                        sijoitettu.Id = (int)Math.Floor(sijoitetut.ElementAt(2).Id * 0.5f);
                        break;

                    case 6:
                        sijoitettu.Id = (int)Math.Round((sijoitetut.ElementAt(1).Id + sijoitetut.ElementAt(2).Id) * 0.5f);
                        break;

                    case 7:
                        sijoitettu.Id = (int)Math.Ceiling((sijoitetut.ElementAt(0).Id + sijoitetut.ElementAt(3).Id) * 0.5f);
                        break;

                    default:
                        break;
                }
                sijoitus++;
            }

            foreach (var sijoitettu in sijoitetut)
            {
                if (vapaatIdt.Contains(sijoitettu.Id))
                {
                    vapaatIdt.Remove(sijoitettu.Id);
                }
            }

            Random r = new Random(DateTime.Now.Millisecond);

            foreach (var pelaaja in osallistujat.Where(x => x.Id <= 0))
            {
                if (vapaatIdt.Count > 0)
                {
                    pelaaja.Id = vapaatIdt.ElementAt(r.Next(vapaatIdt.Count));
                }
                else
                {
                    pelaaja.Id = Osallistujat.Count + 1 + r.Next();
                }

                if (vapaatIdt.Contains(pelaaja.Id))
                {
                    vapaatIdt.Remove(pelaaja.Id);
                }
            }

            int id = 1;
            foreach (var o in osallistujat.OrderBy(x => x.Id))
            {
                o.Id = id++;
            }
        }

        public int LaskePelit(int pelaaja, int kierros)
        {
            return Pelit.Count(x => x.Kierros < kierros && x.SisaltaaPelaajan(pelaaja));
        }

        public int LaskeTappiot(int pelaaja, int kierros)
        {
            int tappiot = Pelit.Count(x => 
                x.Kierros < kierros && 
                x.Tilanne == PelinTilanne.Pelattu &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Havisi(pelaaja));

            if (tappiot < 2)
            {
                if (Pelit.Any(x =>
                    x.Kierros < kierros &&
                    x.Tilanne == PelinTilanne.Pelattu &&
                    x.SisaltaaPelaajan(pelaaja) &&
                    x.Havisi(pelaaja) &&
                    x.OnPudotusPeli()))
                {
                    tappiot = 2;
                }
            }
            return tappiot;
        }

        public int LaskeVoitot(int pelaaja, int kierros)
        {
            return Pelit.Count(x =>
                x.Kierros < kierros &&
                x.Tilanne == PelinTilanne.Pelattu &&
                x.SisaltaaPelaajan(pelaaja) &&
                !x.Havisi(pelaaja));
        }

        public int LaskePisteet(int pelaaja, int kierros)
        {
            int pisteet = 0;

            foreach (var peli in Pelit.Where(x => x.Tilanne == PelinTilanne.Pelattu && x.SisaltaaPelaajan(pelaaja)))
            {
                pisteet += peli.Pisteet(pelaaja);
            }

            return pisteet;
        }

        public bool KierrosTaynna(int kierros)
        {
            int huilaajia = 0;

            foreach (var o in Osallistujat)
            {
                if (LaskeTappiot(o.Id, kierros) < 2)
                {
                    if (!Pelit.Any(x => x.Kierros == kierros && x.SisaltaaPelaajan(o.Id)))
                    {
                        huilaajia++;
                    }
                }
            }

            return huilaajia < 2;
        }

        public int LaskeKeskinaiset(int pelaaja, IEnumerable<Pelaaja> mukana)
        {
            int i = 0;

            foreach (var p in mukana)
            {
                if (p.Id != pelaaja)
                { 
                    i += this.Pelit.Count(x => x.SisaltaaPelaajat(pelaaja, p.Id));
                }
            }

            return i;
        }

        public void PoistaTyhjatOsallistujat()
        {
            while (true)
            {
                var osallistuja = this.Osallistujat.FirstOrDefault(x => x.Id < 0 && string.IsNullOrEmpty(x.Nimi));
                if (osallistuja != null)
                {
                    this.Osallistujat.Remove(osallistuja);
                }
                else
                {
                    return;
                }
            }
        }

        private bool HaeAlkukierrokset(out string virhe)
        {
            virhe = string.Empty;

#if DEBUG // Invariantit:
            if (this.OsallistujatJarjestyksessa.Count != this.Osallistujat.Count)
            {
                this.Loki.Kirjoita("BUGI!!! Osallistujalistat epäsynkassa arvottaessa alkukierroksia!", null, true);
                return false;
            }

            if (this.Osallistujat.Any(x => x.Id < 0))
            {
                this.Loki.Kirjoita("BUGI!!! Osallistujissa on id:ttömiä pelaajia arvottaessa alkukierroksia!", null, true);
                return false;
            }

            if (this.Osallistujat.Count < Asetukset.PelaajiaVahintaanKaaviossa)
            {
                this.Loki.Kirjoita("BUGI!!! Liian vähän osallistujia arvottaessa alkukierroksia!", null, true);
                return false;
            }

            if (this.Pelit.Any(x => x.Kierros > 1))
            {
                this.Loki.Kirjoita("BUGI!!! Kaaviossa on toisen kierroksen pelejä arvottaessa ekoja kierroksia!", null, true);
                return false;
            }
#endif

            Loki.Kirjoita("Haetaan pelit kierrokselle 1");

            while (true)
            {
                var mukana = this.Osallistujat.Where(x => Pelit.Count(y => y.SisaltaaPelaajan(x.Id)) == 0);

                var hakijat = mukana
                    .OrderBy(x => x.Id)
                    .OrderBy(x => Pelit.Count(y => (y.Kierros == 1) && y.SisaltaaPelaajan(x.Id)));

                if (hakijat.Count() < 2)
                {
                    break;
                }

                var hakija = hakijat.First();

                var vastustajat = hakijat
                    .Where(x => (x.Id != hakija.Id) && !Pelit.Any(y => y.SisaltaaPelaajat(hakija.Id, x.Id)))
                    .OrderBy(x => x.Id)
                    .OrderBy(x => Pelit.Count(y => (y.Kierros == 1) && y.SisaltaaPelaajan(x.Id)));

                if (vastustajat.Count() < 1)
                {
#if DEBUG
                    this.Loki.Kirjoita("BUGI!!! Hakijalle ei löytynyt vastustajaa alkukierrokselle", null, true);
#endif
                    return false;
                }

                LisaaPeli(hakija, vastustajat.First());
            }

            Loki.Kirjoita("Haetaan pelit kierrokselle 2");

            bool parillinen = (this.Osallistujat.Count() % 2) == 0;
            bool jaollinenNeljalla = (this.Osallistujat.Count() % 4) == 0;

            // Jos pelaajia on parillinen, mutta ei neljällä jaollinen määrä niin tokan kierroksen lopussa tarvitaan "kieppi"
            if (parillinen && !jaollinenNeljalla)
            { 
                var a = this.OsallistujatJarjestyksessa[this.Osallistujat.Count - 5];
                var b = this.OsallistujatJarjestyksessa[this.Osallistujat.Count - 2];
                LisaaPeli(a, b);

                var c = this.OsallistujatJarjestyksessa[this.Osallistujat.Count - 3];
                var d = this.OsallistujatJarjestyksessa[this.Osallistujat.Count - 1];
                LisaaPeli(c, d);
            }

            while (true)
            {
                var mukana = this.Osallistujat.Where(x => x.Id >= 0 && Pelit.Count(y => y.SisaltaaPelaajan(x.Id)) < 2);

                var hakijat = mukana
                    .OrderBy(x => x.Id)
                    .OrderBy(x => Pelit.Count(y => y.SisaltaaPelaajan(x.Id)));

#if DEBUG
                Debug.WriteLine(string.Format("# Hakijat {0}", string.Join(",", hakijat.Select(x => ("(" + x.Id + " " + x.Nimi + ")")))));
#endif

                if (hakijat.Count() < 2)
                {
                    break;
                }

                var hakija = hakijat.First();

                var vastustajat = hakijat
                    .Where(x => (x.Id != hakija.Id) && !Pelit.Any(y => y.SisaltaaPelaajat(hakija.Id, x.Id)))
                    .OrderBy(x => x.Id)
                    .OrderBy(x => Pelit.Count(y => y.SisaltaaPelaajan(x.Id)));

                if (vastustajat.Count() < 1)
                {
#if DEBUG
                    this.Loki.Kirjoita("BUGI!!! Hakijalle ei löytynyt vastustajaa alkukierrokselle", null, true);
#endif
                    return false;
                }

#if DEBUG
                Debug.WriteLine(string.Format("# Vastustajat {0}", string.Join(",", vastustajat.Select(x => ("(" + x.Id + " " + x.Nimi + ")")))));
#endif

                LisaaPeli(hakija, vastustajat.First());
            }

            return true;
        }

        public HakuAlgoritmi Haku(IStatusRivi status)
        {
            int kierros = 1;
            int pelejaKesken = 0;
            int vajaitaPeleja = 0;

            int viimeinenPelattuKierros = 0;

            for (; kierros < 999; ++kierros)
            {
                if (Pelit.Any(x => x.Kierros == kierros))
                {
                    pelejaKesken = Pelit.Count(x => (x.Kierros == kierros) && x.Tilanne != PelinTilanne.Pelattu);
                    if (pelejaKesken == 0 && KierrosTaynna(kierros))
                    {
                        viimeinenPelattuKierros = kierros;
                    }

                    vajaitaPeleja = Pelit.Count(x => (x.Kierros == kierros) && (x.Id1 < 0 || x.Id2 < 0));
                    if (pelejaKesken > 0 || vajaitaPeleja > 0)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            int hakuKierros = Math.Max(3, viimeinenPelattuKierros + 1);

#if DEBUG
            Debug.WriteLine("=======================( H A K U )=============================");
#endif

            // Käynnissä oleva kierros on hyvällä mallilla, hae pelejä seuraavalle kierrokselle
            if (KierrosTaynna(hakuKierros))
            {
                if (Pelit.Count(x =>
                      x.Kierros <= hakuKierros &&
                      x.Tilanne != PelinTilanne.Pelattu) <= Asetukset.PelejaEnintaanKeskenHaettaessa)
                {
#if DEBUG
                    Debug.WriteLine("Haetaan pelejä seuraavalle kierrokselle {0}", hakuKierros + 1);
#endif
                    return Haku(hakuKierros + 1, status);
                }
                else 
                {
#if DEBUG
                    Debug.WriteLine("Liikaa pelejä kesken kierroksella {0}. Lykätään hakua", hakuKierros);
#endif
                }
            }

            // Hae lisää pelejä käynnissä olevalle vajaalle kierrokselle
            else
            {
                if (Pelit.Count(x =>
                      x.Kierros < hakuKierros &&
                      x.Tilanne != PelinTilanne.Pelattu) <= Asetukset.PelejaEnintaanKeskenHaettaessa)
                {
#if DEBUG
                    Debug.WriteLine("Haetaan pelejä käynnissä olevalle kierrokselle {0}", hakuKierros);
#endif
                    return Haku(hakuKierros, status);
                }
                else
                {
#if DEBUG
                    Debug.WriteLine("Liikaa pelejä kesken kierroksella {0}. Lykätään hakua", hakuKierros);
#endif
                }
            }

            return null;
        }

        public void PaivitaPelienTulokset()
        {
            foreach (var peli in this.Pelit)
            {
                peli.PaivitaTulos();
            }
        }

        public void PaivitaPelitValmiinaAlkamaan()
        {
            this.PelienTilannePaivitysTarvitaan = false;

            foreach (var peli in this.Pelit)
            {
                if (peli.Tilanne != PelinTilanne.Pelattu && peli.Tilanne != PelinTilanne.Kaynnissa)
                {
                    if (peli.Id1 >= 0 && peli.Id2 >= 0)
                    {
                        if (peli.Kierros > 2 &&
                            this.Pelit.Any(x =>
                            x.PeliNumero < peli.PeliNumero &&
                            x.Tilanne != PelinTilanne.Pelattu &&
                            x.SisaltaaJommanKummanPelaajan(peli.Id1, peli.Id2)))
                        {
                            // Pelaamattomia pelejä aikaisemmalla kierroksella => Ei voi aloittaa
                            peli.Tilanne = PelinTilanne.Tyhja;
                        }

                        else if (this.Pelit.Any(x =>
                            x != peli &&
                            x.Tilanne == PelinTilanne.Kaynnissa &&
                            x.SisaltaaJommanKummanPelaajan(peli.Id1, peli.Id2)))
                        {
                            // Toisella pelaajista on käynnissä peli
                            peli.Tilanne = PelinTilanne.Tyhja;
                        }

                        else if (this.Pelit.Any(x => x.Tulos == PelinTulos.Virheellinen))
                        {
                            // Jos kaaviossa on virhe, niin uusia pelejä ei voida aloittaa
                            peli.Tilanne = PelinTilanne.Tyhja;
                        }
                        else
                        {
                            peli.Tilanne = PelinTilanne.ValmiinaAlkamaan;
                        }
                    }
                    else
                    {
                        peli.Tilanne = PelinTilanne.Tyhja;
                    }
                }
            }
        }
        
        public int LaskeTappiotPelille(int pelaaja, int peliNumero)
        {
            int tappiot = Pelit.Count(x => 
                (x.Tilanne == PelinTilanne.Pelattu) && 
                x.SisaltaaPelaajan(pelaaja) && 
                x.Havisi(pelaaja) &&
                (x.PeliNumero <= peliNumero));

            if (tappiot < 2)
            {
                if (Pelit.Any(x => 
                    (x.Tilanne == PelinTilanne.Pelattu) && 
                    x.SisaltaaPelaajan(pelaaja) && 
                    x.Havisi(pelaaja) &&
                    (x.PeliNumero <= peliNumero &&
                    x.OnPudotusPeli())))
                {
                    tappiot = 2;
                }
            }

            return tappiot;
        }

        /// <summary>
        /// 
        /// </summary>
        public HakuAlgoritmi Haku(int kierros, IStatusRivi status)
        {
            return new HakuAlgoritmi(this, Loki, kierros, status);
        }

        private void PaivitaPelinumerot()
        {
            int numero = 1;
            foreach (var p in Pelit)
            {
                p.PeliNumero = numero++;
            }
        }

        private bool TarkistaVoikoPeliAlkaa(Peli peli)
        {
            if (peli.Id1 <= 0 || peli.Id2 <= 0)
            {
                return false;
            }

            if (Pelit.Any(x => x.SisaltaaPelaajan(peli.Id1) && (x.Kierros < peli.Kierros) && (x.Tilanne != PelinTilanne.Pelattu)))
            {
                return false;
            }

            if (Pelit.Any(x => x.SisaltaaPelaajan(peli.Id2) && (x.Kierros < peli.Kierros) && (x.Tilanne != PelinTilanne.Pelattu)))
            {
                return false;
            }

            return true;
        }

        public bool Mukana(Pelaaja pelaaja)
        {
            int tappiot = Pelit.Count(x => x.SisaltaaPelaajan(pelaaja.Id) && x.Havisi(pelaaja.Id));
            if (tappiot > 1)
            {
                return false;
            }

            if (Pelit.Any(x => x.SisaltaaPelaajan(pelaaja.Id) && x.Havisi(pelaaja.Id) && x.OnPudotusPeli()))
            {
                return false;
            }

            return true;
        }

        public bool MukanaEnnenPelia(Pelaaja pelaaja, Peli peli)
        {
            int tappiot = Pelit.Count(x => 
                (x.PeliNumero < peli.PeliNumero) && 
                x.SisaltaaPelaajan(pelaaja.Id) && 
                x.Havisi(pelaaja.Id));
            
            if (tappiot > 1)
            {
                return false;
            }

            if (Pelit.Any(x => 
                (x.PeliNumero < peli.PeliNumero) && 
                x.SisaltaaPelaajan(pelaaja.Id) && 
                x.Havisi(pelaaja.Id) && 
                x.OnPudotusPeli()))
            {
                return false;
            }

            return true;
        }

        [XmlIgnore]
        public bool VoiLisataPelaajia
        {
            get
            {
                if (Pelit.Any(x => 
                    x.Kierros > 1 && 
                    (x.Tilanne == PelinTilanne.Kaynnissa || 
                    x.Tilanne == PelinTilanne.Pelattu)))
                {
                    return false;
                }

                return true;
            }
        }

        [XmlIgnore]
        public bool VoiPoistaaPelaajia
        {
            get
            {
                return !KilpailuAlkanut;
            }
        }

        [XmlIgnore]
        public bool KilpailuAlkanut
        {
            get
            {
                return Pelit.Any(x => x.Tilanne == PelinTilanne.Kaynnissa || x.Tilanne == PelinTilanne.Pelattu);
            }
        }

        [XmlIgnore]
        public bool ArvontaNappiPainettavissa
        {
            get
            {
                var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
                if (osallistujat.Count() < 4)
                {
                    return false;
                }

                if (Pelit.Any(x => 
                    (x.Kierros > 1) &&
                    (x.Tilanne == PelinTilanne.Kaynnissa || x.Tilanne == PelinTilanne.Pelattu)))
                {
                    return false;
                }

                if (this.Pelit.Count == 0)
                {
                    return true;
                }

                if (!Osallistujat.Any(x => (!string.IsNullOrEmpty(x.Nimi)) && x.Id < 0))
                {
                    return false;
                }

                return true;
            }
        }

        [XmlIgnore]
        public string ArvontaNapinTeksti
        {
            get
            {
                var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
                if (osallistujat.Count() < 4)
                {
                    return "Arvo kaavio";
                }

                if (osallistujat.Any(x => x.Id < 0))
                {
                    if (osallistujat.Any(x => x.Id >= 0))
                    {
                        return "Arvo jälki-ilmoittautuneet kaavioon";
                    }
                    else
                    {
                        if (this.JalkiIlmoittautuneet.Any(x => !string.IsNullOrEmpty(x.Nimi)))
                        {
                            return "Arvo alakaavio";
                        }
                        else
                        {
                            return "Arvo kaavio";
                        }
                    }
                }

                return "Arvo kaavio";
            }
        }

        [XmlIgnore]
        public KilpailunTilanne Tilanne
        {
            get
            {
                try
                {
                    if (Pelit.Count == 0)
                    {
                        return KilpailunTilanne.Arvonta;
                    }

                    var mukana = this.Osallistujat.Where(x => x.Id >= 0 && Mukana(x));

                    if (mukana.Count() == 1)
                    {
                        return KilpailunTilanne.Paattynyt;
                    }

                    return KilpailunTilanne.Kaynnissa;
                }
                catch
                {
                    return KilpailunTilanne.Arvonta;
                }
            }
        }

        public Pelaaja Voittaja()
        {
            var mukana = this.Osallistujat.Where(x => x.Id >= 0 && Mukana(x));
            if (mukana.Count() == 1)
            {
                return mukana.First();
            }

            return null;
        }

        [XmlIgnore]
        public bool KilpailuOnPaattynyt
        {
            get 
            {
                return Voittaja() != null;
            }
        }

        public IEnumerable<Pelaaja> Tulokset()
        {
            foreach (var o in this.Osallistujat.Where(x => x.Id >= 0))
            {
                o.Pisteet = LaskePisteet(o.Id, 999);
                o.Voitot = LaskeVoitot(o.Id, 999);
                o.Tappiot = LaskeTappiot(o.Id, 999);
                o.Pudotettu = !Mukana(o);

                if (o.Pudotettu)
                {
                    o.PudonnutKierroksella = this.Pelit.Count(x => x.SisaltaaPelaajan(o.Id));
                }
                else
                {
                    o.PudonnutKierroksella = 0;
                }

                // Sijoituspisteet määräävät pelaajien sijoituksen kisan päätyttyä.
                o.SijoitusPisteet = o.Pudotettu ? 0 : 1000000000; // Laittaa voittajan ykköseksi ja mukana olevat listan kärkeen
                o.SijoitusPisteet += o.Voitot * 100000;
                o.SijoitusPisteet += o.Pisteet;
            }

            var tulokset = this.Osallistujat
                .Where(x => x.Id >= 0)
                .ToArray()
                .OrderByDescending(x => x.SijoitusPisteet);

            // Huomioidaan sijoituksissa kuinka pitkälle pelaaja pääsi (jos näin on asetettu) 
            if (this.SijoitustenMaaraytyminen != KaisaKaavio.SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista && KilpailuOnPaattynyt)
            {
                var finaali = this.Pelit.LastOrDefault();
                if (finaali != null)
                {
                    var finalisti = finaali.Haviaja();
                    if (finalisti != null)
                    {
                        finalisti.SijoitusPisteet += 500000000;

                        if (this.SijoitustenMaaraytyminen == KaisaKaavio.SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista)
                        {
                            var t = tulokset
                                .Where(x => !finaali.SisaltaaPelaajan(x.Id))
                                .OrderByDescending(x => x.PudonnutKierroksella);

                            if (t.Count() > 0)
                            {
                                int pronssiKierros = t.FirstOrDefault().PudonnutKierroksella;
                                foreach (var tt in t.Where(x => x.PudonnutKierroksella == pronssiKierros))
                                {
                                    tt.SijoitusPisteet = 250000000;
                                }
                            }
                        }
                    }
                }

                tulokset = tulokset.ToArray()
                    .OrderByDescending(x => x.Pisteet)
                    .OrderByDescending(x => x.Voitot)
                    .OrderByDescending(x => x.SijoitusPisteet);
            }

            int sijoitus = 1;

            int edellinenSijoitus = 999999;
            int edellisetPisteet = 9999999;

            foreach (var t in tulokset)
            {
                if (t.Pudotettu && (t.SijoitusPisteet == edellisetPisteet))
                {
                    t.Sijoitus = edellinenSijoitus;
                }
                else
                {
                    edellinenSijoitus = sijoitus;
                    edellisetPisteet = t.SijoitusPisteet;

                    t.Sijoitus = sijoitus;
                }

                sijoitus++;
            }

            return tulokset;
        }

        [XmlIgnore]
        public bool ToinenKierrosAlkanut
        {
            get 
            {
                return this.Pelit.Any(x =>
                        (x.Kierros > 1) &&
                        (x.Tilanne == PelinTilanne.Pelattu || x.Tilanne == PelinTilanne.Kaynnissa));
            }
        }

        [XmlIgnore]
        public string ArvonnanTilanneTeksti
        {
            get
            {
                try
                {
                    if (ToinenKierrosAlkanut)
                    {
                        var mukana = this.Osallistujat.Where(x => (x.Id >= 0) && Mukana(x));
                        if (mukana.Count() == 1)
                        {
                            return string.Format("Kilpailu on päättynyt. {0} voitti", mukana.First().Nimi);
                        }
                        else
                        {
                            return string.Format("Ilmoittautuminen on päättynyt. {0} pelaajaa kaaviossa", this.Osallistujat.Count(x => x.Id >= 0));
                        }
                    }

                    var osallistujat = Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));

                    int ilmoittautuneita = osallistujat.Count();
                    int arvottuja = osallistujat.Where(x => x.Id >= 0).Count();
                    int eiArvottuja = osallistujat.Where(x => x.Id < 0).Count();

                    bool kaavioTaynna = ilmoittautuneita >= PelaajiaEnintaan;
                    if (kaavioTaynna)
                    {
                        if (ilmoittautuneita == PelaajiaEnintaan)
                        {
                            return string.Format("Kaavio on täynnä. {0} pelaajaa mukana", ilmoittautuneita);
                        }
                        else
                        {
                            return string.Format("Kaavio on täynnä. {0} pelaajaa mukana. {1} varalla", ilmoittautuneita, ilmoittautuneita - PelaajiaEnintaan);
                        }
                    }
                    else
                    {
                        if (arvottuja == 0)
                        {
                            return string.Format("Ilmoittautuminen käynnissä. {0} pelaajaa ilmoittautunut", ilmoittautuneita);
                        }
                        else
                        {
                            if (eiArvottuja > 0)
                            {
                                return string.Format("Kaavio arvottu. {0} arvottu kaavioon, {1} jälki-ilmoittautunutta", arvottuja, eiArvottuja);
                            }
                            else
                            {
                                return string.Format("Kaavio arvottu. {0} pelaajaa mukana. Jälki-ilmoittautuneita voidaan lisätä", arvottuja);
                            }
                        }
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public IEnumerable<Pelaaja> MukanaOlevatPelaajatEnnenPelia(Peli peli)
        {
            return this.Osallistujat.Where(x => x.Id >= 0 && MukanaEnnenPelia(x, peli));
        }

        [XmlIgnore]
        public string PelienTilanneTeksti
        {
            get
            {
                var mukana = this.Osallistujat.Where(x => x.Id >= 0 && Mukana(x));
                if (mukana.Count() == 1)
                {
                    return string.Format("Kilpailu on päättynyt. {0} voitti!", mukana.First().Nimi);
                }

                if (mukana.Count() == 2)
                {
                    return string.Format("Pelataan finaalia {0} vs {1}.", mukana.First().Nimi, mukana.Last().Nimi);
                }

                int minKierros = Int32.MaxValue;
                int maxKierros = Int32.MinValue;
                int pelejaKaynnissa = 0;

                foreach (var peli in this.Pelit)
                {
                    if (peli.Tilanne == PelinTilanne.Kaynnissa || peli.Tilanne == PelinTilanne.ValmiinaAlkamaan)
                    {
                        pelejaKaynnissa++;
                        minKierros = Math.Min(minKierros, peli.Kierros);
                        maxKierros = Math.Max(maxKierros, peli.Kierros);
                    }
                }

                string mukanaString = string.Format("Mukana {0} pelaajaa.", mukana.Count());
                if (mukana.Count() > 1 && mukana.Count() < 6)
                {
                    mukanaString = string.Format("Mukana {0} pelaajaa ({1}).", mukana.Count(), string.Join(", ", mukana.Select(x => x.Nimi).ToArray()));
                }

                if (pelejaKaynnissa > 0)
                {
                    if (minKierros == maxKierros)
                    {
                        return string.Format("Pelataan kierrosta {0}. {1}", minKierros, mukanaString);
                    }
                    else
                    {
                        return string.Format("Pelataan kierroksia {0}-{1}. {2}", minKierros, maxKierros, mukanaString);
                    }
                }
                else
                {
                    return mukanaString;
                }
            }
        }

        public string AlkamisAikaString()
        {
            if (string.IsNullOrEmpty(this.KellonAika))
            {
                return string.Format("{0}.{1}.{2}", this.AlkamisAika.Day, this.AlkamisAika.Month, this.AlkamisAika.Year);
            }
            else
            {
                return string.Format("{0}.{1}.{2} klo:{3}", this.AlkamisAika.Day, this.AlkamisAika.Month, this.AlkamisAika.Year, this.KellonAika);
            }
        }

        public int RankingSarjanNumero()
        {
            switch (this.RankingKisaTyyppi)
            {
                case Ranking.RankingSarjanPituus.Kuukausi: return this.AlkamisAika.Month;
                case Ranking.RankingSarjanPituus.Vuodenaika: return (this.AlkamisAika.Month - 1) / 3;
                case Ranking.RankingSarjanPituus.Puolivuotta: return (this.AlkamisAika.Month - 1) / 6;
                case Ranking.RankingSarjanPituus.Vuosi: return 0;
                default: return 0;
            }
        }
    }
}

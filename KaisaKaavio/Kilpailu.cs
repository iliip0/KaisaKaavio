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
        /// <summary>
        /// Kilpailun ID joka identifioi sen yksiselitteisesti
        /// </summary>
        [DefaultValue("")]
        public string Id { get; set; }

        [DefaultValue("")]
        public string Nimi { get; set; }

        [DefaultValue("")]
        public string KilpailunJohtaja { get; set; }

        [DefaultValue("")]
        public string JarjestavaSeura { get; set; }

        [DefaultValue("")]
        public string PuhelinNumero { get; set; }

        [DefaultValue("")]
        public string AlkamisAika { get; set; }

        [XmlIgnore]
        public DateTime AlkamisAikaDt 
        {
            set
            {
                this.AlkamisAika = Tyypit.Aika.DateTimeToString(value);
            }
            get 
            {
                return Tyypit.Aika.ParseDateTime(this.AlkamisAika);
            }
        }

        [DefaultValue("")]
        public string KellonAika { get; set; }
        
        [DefaultValue(true)]
        public bool Yksipaivainen { get; set; }

        [DefaultValue(false)]
        public bool TestiKilpailu { get; set; }

        [DefaultValue("")]
        public string KaavioidenYhdistaminenKierroksesta { get; set; }

        [DefaultValue("")]
        public string BiljardiOrgId { get; set; }

        [DefaultValue("")]
        public string EdellisenBiljardiOrgHaunPvm { get; set; }

        [XmlIgnore]
        public int KaavioidenYhdistaminenKierroksestaInt 
        {
            get
            {
                int k = 0;
                Int32.TryParse(this.KaavioidenYhdistaminenKierroksesta, out k);
                return k;
            }
        }

        [XmlIgnore]
        public Sali Sali { get; set; }

        [XmlIgnore]
        private Kilpailu JoukkueKilpailu = null;

        [XmlIgnore]
        private Kilpailu JoukkueKilpailunVarsinainenKilpailu = null;

        [XmlIgnore]
        public bool OnJoukkuekilpailunJoukkueKilpailu
        {
            get
            {
                return this.JoukkueKilpailunVarsinainenKilpailu != null;
            }
        }

        [XmlIgnore]
        public Ranking.RankingOsakilpailuTietue RankingOsakilpailu { get; set; }

        [XmlIgnore]
        public bool RankingKisa 
        {
            get
            {
                if (this.KilpailuOnViikkokisa)
                {
                    return this.RankingOsakilpailu != null ? this.RankingOsakilpailu.OnRankingOsakilpailu : false;
                }
                else 
                {
                    return false;
                }
            }
            set
            {
                if (this.RankingOsakilpailu != null)
                {
                    this.RankingOsakilpailu.OnRankingOsakilpailu = value;
                }
            }
        }

        [XmlIgnore]
        public Ranking.RankingSarjanPituus RankingKisaTyyppi
        {
            get
            {
                return this.RankingOsakilpailu != null ? this.RankingOsakilpailu.SarjanPituus : Ranking.RankingSarjanPituus.Kuukausi;
            }
            set 
            {
                if (this.RankingOsakilpailu != null)
                {
                    this.RankingOsakilpailu.SarjanPituus = value;
                }
            }
        }

        [XmlIgnore]
        public Laji RankingKisaLaji 
        {
            get
            {
                return this.RankingOsakilpailu != null ? this.RankingOsakilpailu.Laji : this.Laji;
            }
            set
            {
                if (this.RankingOsakilpailu != null)
                {
                    this.RankingOsakilpailu.Laji = value;
                }
            }
        }

        public SijoitustenMaaraytyminen SijoitustenMaaraytyminen { get; set; }

        [DefaultValue(true)]
        public bool PeliaikaOnRajattu { get; set; }

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
        
        [XmlIgnore]
        public bool KilpailuOnViikkokisa { get { return this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa; } }

        [XmlIgnore]
        public bool KilpailuOnTasurikisa 
        { 
            get 
            { 
                return 
                    (this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.Viikkokisa ||
                    this.KilpailunTyyppi == KaisaKaavio.KilpailunTyyppi.AvoinKilpailu) &&
                    this.Laji != KaisaKaavio.Laji.Kaisa; 
            } 
        }

        [XmlIgnore]
        public bool Tyhja { get { return string.IsNullOrEmpty(this.Id) && string.IsNullOrEmpty(this.Nimi); } }

        public Laji Laji { get; set; }
        public KilpailunTyyppi KilpailunTyyppi { get; set; }
        public KilpaSarja KilpaSarja { get; set; }
        public Sijoittaminen Sijoittaminen { get; set; }

        public BindingList<Pelaaja> Osallistujat { get; set; }
        public BindingList<Pelaaja> JalkiIlmoittautuneet { get; set; }
        public BindingList<Sali> PeliPaikat { get; set; }
        public BindingList<Peli> Pelit { get; set; }
        
        [XmlIgnore]
        public BindingList<Pelaaja> OsallistujatJarjestyksessa { get; set; }

        [XmlIgnore]
        public BindingList<Pelaaja> OsallistujatJarjestyksessaKaavioon 
        {
            get 
            {
                if (this.JoukkueKilpailu != null)
                {
                    return this.JoukkueKilpailu.OsallistujatJarjestyksessa;
                }
                else
                {
                    return this.OsallistujatJarjestyksessa;
                }
            } 
        }

        [XmlIgnore]
        public Kilpailu KilpailuKaavioon
        {
            get 
            {
                if (this.JoukkueKilpailu != null)
                {
                    return this.JoukkueKilpailu;
                }
                else
                {
                    return this;
                }
            }
        }

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

            PeliPaikat = new BindingList<Sali>();

            Pelit = new BindingList<Peli>();
            Pelit.ListChanged += Pelit_ListChanged;

            Id = string.Empty;
            Nimi = string.Empty;

            AlkamisAikaDt = DateTime.Today;
            KellonAika = string.Empty;
            Yksipaivainen = true;

            this.RankingOsakilpailu = null;

            OsallistumisMaksu = string.Empty;
            OsallistumisOikeus = string.Empty;
            KaavioTyyppi = KaavioTyyppi.Pudari3Kierros;
            JarjestavaSeura = string.Empty;
            PeliAika = 40;
            PeliaikaOnRajattu = true;
            TavoitePistemaara = 60;
            RankkareidenMaara = 3;
            Tiedosto = string.Empty;
            LisaTietoa = string.Empty;
            KilpailunJohtaja = string.Empty;
            LisenssiVaatimus = string.Empty;
            MaksuTapa = string.Empty;
            Palkinnot = string.Empty;
            Pukeutuminen = string.Empty;
            Ilmoittautuminen = string.Empty;
            PelaajiaEnintaan = 32;
            Laji = KaisaKaavio.Laji.Kaisa;
            KilpailunTyyppi = KaisaKaavio.KilpailunTyyppi.Viikkokisa;
            Sijoittaminen = KaisaKaavio.Sijoittaminen.EiSijoittamista;
            KilpaSarja = KaisaKaavio.KilpaSarja.Yleinen;
            SijoitustenMaaraytyminen = KaisaKaavio.SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista;
            BiljardiOrgId = string.Empty;
            EdellisenBiljardiOrgHaunPvm = string.Empty;

            TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
            TallennusTarvitaan = false;

            TestiKilpailu = false;

            this.Sali = null;
            this.KaavioidenYhdistaminenKierroksesta = "5";
        }

#region Pelit

        void LisaaPeli(Peli peli)
        {
            peli.PropertyChanged += Kilpailu_PropertyChanged;
            peli.Kilpailu = this;
            peli.PaivitaRivinUlkoasu = true;

            Pelit.Add(peli);
        }

        public void PoistaPeli(Peli peli, bool nollaaKilpailu)
        {
            Pelit.Remove(peli);

            peli.PropertyChanged -= Kilpailu_PropertyChanged;

            if (nollaaKilpailu)
            {
                peli.Kilpailu = null;
            }
        }

        public void PoistaKaikkiPelit()
        {
            if (Pelit.Any())
            {
#if PROFILE
                using (new Testaus.Profileri("Kilpailu.PoistaKaikkiPelit"))
#endif
                {
                    while (Pelit.Any())
                    {
                        PoistaPeli(Pelit.First(), true);
                    }
                }
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

        public void PaivitaKilpailuUi()
        {
            try
            {
                RaisePropertyChanged("PelienTilanneTeksti");

                if (this.JoukkueKilpailu != null)
                {
                    this.JoukkueKilpailu.RaisePropertyChanged("PelienTilanneTeksti");
                }

                if (this.JoukkueKilpailunVarsinainenKilpailu != null)
                {
                    this.JoukkueKilpailunVarsinainenKilpailu.RaisePropertyChanged("PelienTilanneTeksti");
                }
            }
            catch
            { 
            }
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

                    if (this.JoukkueKilpailu != null)
                    {
                        PaivitaPelitJoukkueKisaan();
                        PaivitaKilpailuUi();
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
                    PoistaPeli(tyhja, true);
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
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita(string.Format("Poistetaan alkamaton peli {0} - {1} kierrokselta {2}", tyhja.Pelaaja1, tyhja.Pelaaja2, tyhja.Kierros));
                    }
#endif
                    PoistaPeli(tyhja, true);
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
            int kierros = Math.Max(kierros1, kierros2);
            string paikka = string.Empty;

            if (OnUseanPelipaikanKilpailu)
            {
                if (kierros < this.KaavioidenYhdistaminenKierroksestaInt)
                {
                    paikka = pelaaja.PeliPaikka;

#if DEBUG
                    if (!string.Equals(pelaaja.PeliPaikka, vastustaja.PeliPaikka))
                    {
                        paikka = string.Format("{0}/{1}", pelaaja.PeliPaikka, vastustaja.PeliPaikka);
                    }
#endif
                }
                else 
                {
                    paikka = this.Sali != null ? this.Sali.LyhytNimi : string.Empty;
                }
            }

            if (OnUseanPelipaikanKilpailu && kierros >= this.KaavioidenYhdistaminenKierroksestaInt)
            {
                paikka = Salit().First().LyhytNimi;
            }

            Peli peli = new Peli()
            {
                Kierros = kierros,
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
                PaivitaRivinUlkoasu = true,
                Paikka = paikka
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
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita("Lajitellaan pelit", null, false);
                }

                List<Peli> lajittelemattomat = new List<Peli>();
                lajittelemattomat.Add(peli);

                while ((Pelit.Count > 0) && (Pelit.Last().LajitteluNumero > peli.LajitteluNumero))
                {
                    lajittelemattomat.Add(Pelit.Last());
                    PoistaPeli(Pelit.Last(), false);
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
            if (this.Loki != null)
            {
                Loki.Kirjoita(string.Format("Haettiin peli kierrokselle {0}. {1} vs {2}",
                    peli.Kierros,
                    pelaaja != null ? pelaaja.Nimi : string.Empty,
                    vastustaja != null ? vastustaja.Nimi : string.Empty));
            }

            PelienTilannePaivitysTarvitaan = true;

            return peli.Tilanne;
        }

        private PelinTilanne LisaaWO(Pelaaja pelaaja, int kierros)
        {
            string paikka = string.Empty;

            if (OnUseanPelipaikanKilpailu)
            {
                if (kierros < this.KaavioidenYhdistaminenKierroksestaInt)
                {
                    paikka = pelaaja.PeliPaikka;
                }
                else
                {
                    paikka = this.Sali != null ? this.Sali.LyhytNimi : string.Empty;
                }
            }

            if (OnUseanPelipaikanKilpailu && kierros >= this.KaavioidenYhdistaminenKierroksestaInt)
            {
                paikka = Salit().First().LyhytNimi;
            }

            Peli peli = new Peli()
            {
                Kierros = kierros,
                KierrosPelaaja1 = kierros,
                KierrosPelaaja2 = kierros,
                Kilpailu = this,
                Alkoi = string.Empty,
                Paattyi = string.Empty,
                PelaajaId1 = pelaaja == null ? string.Empty : pelaaja.Id.ToString(),
                PelaajaId2 = string.Empty,
                PeliNumero = Pelit.Count() + 1,
                Pisteet1 = this.TavoitePistemaara.ToString(),
                Pisteet2 = "0",
                Poyta = string.Empty,
                Tilanne = PelinTilanne.Pelattu,
                Tulos = PelinTulos.Pelaaja1Voitti,
                PaivitaRivinUlkoasu = true,
                Paikka = paikka
            };

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
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita("Lajitellaan pelit", null, false);
                }

                List<Peli> lajittelemattomat = new List<Peli>();
                lajittelemattomat.Add(peli);

                while ((Pelit.Count > 0) && (Pelit.Last().LajitteluNumero > peli.LajitteluNumero))
                {
                    lajittelemattomat.Add(Pelit.Last());
                    PoistaPeli(Pelit.Last(), false);
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
            if (this.Loki != null)
            {
                Loki.Kirjoita(string.Format("Lisättiin w.o. peli kierrokselle {0} pelaajalle {1}",
                    peli.Kierros,
                    pelaaja != null ? pelaaja.Nimi : string.Empty));
            }

            PelienTilannePaivitysTarvitaan = true;

            return peli.Tilanne;
        }

        public PelinTilanne LisaaWO(Pelaaja pelaaja)
        {
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.LisaaWO"))
#endif
            {
                int kierros = pelaaja == null ? 0 : Pelit.Count(x =>
                    x.SisaltaaPelaajan(pelaaja.Id)) + 1;

                return LisaaWO(pelaaja, kierros);
            }
        }

        public PelinTilanne LisaaPeli(Pelaaja pelaaja, Pelaaja vastustaja)
        {
            if (this.JoukkueKilpailu != null)
            {
                if (this.JoukkueKilpailu.Osallistujat.Any(x => string.Equals(x.Nimi, pelaaja.Nimi)) &&
                    this.JoukkueKilpailu.Osallistujat.Any(x => string.Equals(x.Nimi, vastustaja.Nimi)))
                {
                    var tilanne = this.JoukkueKilpailu.LisaaPeli(pelaaja, vastustaja);

                    PaivitaPelitJoukkueKisasta();

                    return tilanne;
                }
                else
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("Bugi!, tuntematon peli lisätty joukkuekisaan", null, true);
                    }
                }
            }

#if PROFILE
            using (new Testaus.Profileri("Kilpailu.LisaaPeli"))
#endif
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
        }

#endregion

#region Osallistujat

        public bool LisaaPelaaja(string nimi)
        {
            if (this.Osallistujat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi)))
            {
                return false;
            }

            Pelaaja pelaaja = new Pelaaja()
            {
                Nimi = nimi
            };

            this.Osallistujat.Add(pelaaja);

            return true;
        }

        // Vain testikilpailussa
        public bool LisaaPari(string nimi1, string nimi2, string seura)
        {
            while (this.Osallistujat.Any(x => 
                Tyypit.Nimi.Equals(x.Pelaajan1Nimi, nimi1) ||
                Tyypit.Nimi.Equals(x.Pelaajan2Nimi, nimi1)))
            {
                nimi1 += "a";
            }

            while (this.Osallistujat.Any(x =>
                Tyypit.Nimi.Equals(x.Pelaajan1Nimi, nimi2) ||
                Tyypit.Nimi.Equals(x.Pelaajan2Nimi, nimi2)))
            {
                nimi2 += "a";
            }

            Pelaaja pelaaja = new Pelaaja()
            {
                Nimi = Tyypit.Nimi.NimiParikisassa(nimi1) + " & " + Tyypit.Nimi.NimiParikisassa(nimi2),
                Seura = seura,
                Pelaajan1Nimi = nimi1,
                Pelaajan1Seura = seura,
                Pelaajan2Nimi = nimi2,
                Pelaajan2Seura = seura,
            };

            this.Osallistujat.Add(pelaaja);

            return true;
        }

        // Vain testikilpailussa
        public bool LisaaJoukkue(string nimi1, string nimi2, string nimi3, string seura)
        {
            while (this.Osallistujat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi1)))
            {
                nimi1 += "a";
            }

            while (this.Osallistujat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi2)))
            {
                nimi2 += "a";
            }

            while (this.Osallistujat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi3)))
            {
                nimi3 += "a";
            }

            int i = 1;

            while (this.Osallistujat.Any(x => string.Equals(x.Joukkue, seura + " " + i)))
            { 
                i++;
            }

            string joukkue = seura + " " + i;

            this.Osallistujat.Add(new Pelaaja() 
            {
                Nimi = nimi1,
                Seura = seura,
                Joukkue = joukkue
            });

            this.Osallistujat.Add(new Pelaaja()
            {
                Nimi = nimi2,
                Seura = seura,
                Joukkue = joukkue
            });

            this.Osallistujat.Add(new Pelaaja()
            {
                Nimi = nimi3,
                Seura = seura,
                Joukkue = joukkue
            });
            
            return true;
        }

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
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.PaivitaOsallistujatJarjestyksessa"))
#endif
            {
                this.OsallistujatJarjestyksessa.Clear();

                foreach (var o in this.Osallistujat
                    .Where(x => !string.IsNullOrEmpty(x.Nimi) && x.Id >= 0)
                    .OrderBy(x => x.Id))
                {
                    this.OsallistujatJarjestyksessa.Add(o);
                }
            }
        }

        public void PaivitaKaavioData()
        {
            PaivitaOsallistujatJarjestyksessa();

            this.MaxKierros = 0;

            foreach (var osallistuja in this.OsallistujatJarjestyksessa)
            {
                osallistuja.Sijoitus.Sijoitus = 0;
                osallistuja.Sijoitus.Pudotettu = false;
                osallistuja.Sijoitus.Tappiot = 0;
                osallistuja.Sijoitus.Voitot = 0;
                osallistuja.Sijoitus.Pisteet = 0;
                osallistuja.Pelit.Clear();
            }

            foreach (var peli in Pelit.Where(x => x.Id1 >= 0))
            {
                var osallistuja1 = this.OsallistujatJarjestyksessa.FirstOrDefault(x => x.Id == peli.Id1);
                var osallistuja2 = this.OsallistujatJarjestyksessa.FirstOrDefault(x => x.Id == peli.Id2);

                if (osallistuja1 != null)
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
                    osallistuja1.Sijoitus.Pisteet += p1.Pisteet;

                    this.MaxKierros = Math.Max(this.MaxKierros, osallistuja1.Pelit.Count);

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (!peli.Havisi(peli.Id1))
                        {
                            p1.Voitto = true;
                            osallistuja1.Sijoitus.Voitot++;
                        }
                        else
                        {
                            osallistuja1.Sijoitus.Tappiot++;
                            if (osallistuja1.Sijoitus.Tappiot >= 2 || peli.OnPudotusPeli())
                            {
                                osallistuja1.Sijoitus.Pudotettu = true;
                            }
                        }
                    }
                }

                if (osallistuja2 != null)
                {
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
                    osallistuja2.Sijoitus.Pisteet += p2.Pisteet;

                    this.MaxKierros = Math.Max(this.MaxKierros, osallistuja2.Pelit.Count);

                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (!peli.Havisi(peli.Id2))
                        {
                            p2.Voitto = true;
                            osallistuja2.Sijoitus.Voitot++;
                        }
                        else
                        {
                            osallistuja2.Sijoitus.Tappiot++;
                            if (osallistuja2.Sijoitus.Tappiot >= 2 || peli.OnPudotusPeli())
                            {
                                osallistuja2.Sijoitus.Pudotettu = true;
                            }
                        }
                    }
                }
            }

            var tulokset = Tulokset();

            if (this.JoukkueKilpailu != null)
            {
                this.JoukkueKilpailu.PaivitaKaavioData();
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

        public void TallennaNimella(string nimi, bool muutaKilpailunSijainti)
        {
            if (this.Loki != null)
            {
                Loki.Kirjoita(string.Format("Tallennetaan kilpailu tiedostoon {0}", nimi));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Kilpailu));

            string nimiTmp = Path.GetTempFileName();

            using (TextWriter writer = new StreamWriter(nimiTmp))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }

            File.Copy(nimiTmp, nimi, true);
            File.Delete(nimiTmp);

            if (muutaKilpailunSijainti)
            {
                Tiedosto = nimi;

                this.TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
                this.TallennusTarvitaan = false;
            }
        }

        public void Tallenna()
        {
            if (!string.IsNullOrEmpty(Tiedosto))
            {
                TallennaNimella(Tiedosto, true);
            }
        }

        public void VarmistaEttaKilpailullaOnId()
        {
            if (string.IsNullOrEmpty(this.Id) ||
                this.Id.Contains('{') ||
                this.Id.Contains('}') ||
                this.Id.Contains(','))
            {
                this.Id = Guid.NewGuid().GetHashCode().ToString("X").ToUpper();

                if (this.Loki != null)
                {
                    this.Loki.Kirjoita(string.Format("Luotiin kilpailulle {0} Id {1}", this.Nimi, this.Id), null, false);
                }
            }
        }

        public void Avaa(string tiedosto, bool editoitavaksi)
        {
            this.Osallistujat.RaiseListChangedEvents = false;
            this.OsallistujatJarjestyksessa.RaiseListChangedEvents = false;
            this.Pelit.RaiseListChangedEvents = false;

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

                this.Id = kilpailu.Id;
                this.AlkamisAika = kilpailu.AlkamisAika;
                this.KilpailunJohtaja = kilpailu.KilpailunJohtaja;
                this.JarjestavaSeura = kilpailu.JarjestavaSeura;
                this.Nimi = kilpailu.Nimi;
                this.PeliAika = kilpailu.PeliAika;
                this.PeliaikaOnRajattu = kilpailu.PeliaikaOnRajattu;
                this.PuhelinNumero = kilpailu.PuhelinNumero;
                this.RankkareidenMaara = kilpailu.RankkareidenMaara;
                this.KaavioTyyppi = kilpailu.KaavioTyyppi;
                this.OsallistumisMaksu = kilpailu.OsallistumisMaksu;
                this.OsallistumisOikeus = kilpailu.OsallistumisOikeus;
                this.KellonAika = kilpailu.KellonAika;
                this.LisaTietoa = kilpailu.LisaTietoa;
                this.Yksipaivainen = kilpailu.Yksipaivainen;
                this.LisenssiVaatimus = kilpailu.LisenssiVaatimus;
                this.MaksuTapa = kilpailu.MaksuTapa;
                this.Pukeutuminen = kilpailu.Pukeutuminen;
                this.Palkinnot = kilpailu.Palkinnot;
                this.Ilmoittautuminen = kilpailu.Ilmoittautuminen;
                this.PelaajiaEnintaan = kilpailu.PelaajiaEnintaan;
                this.TavoitePistemaara = kilpailu.TavoitePistemaara;
                this.KilpailunTyyppi = kilpailu.KilpailunTyyppi;
                this.Laji = kilpailu.Laji;
                this.Sijoittaminen = kilpailu.Sijoittaminen;
                this.SijoitustenMaaraytyminen = kilpailu.SijoitustenMaaraytyminen;
                this.KilpaSarja = kilpailu.KilpaSarja;
                this.TestiKilpailu = kilpailu.TestiKilpailu;
                this.KaavioidenYhdistaminenKierroksesta = kilpailu.KaavioidenYhdistaminenKierroksesta;
                this.BiljardiOrgId = kilpailu.BiljardiOrgId;
                this.EdellisenBiljardiOrgHaunPvm = kilpailu.EdellisenBiljardiOrgHaunPvm;

                this.RankingOsakilpailu = null;

                VarmistaEttaKilpailullaOnId();

                this.PeliPaikat.Clear();
                foreach (var p in kilpailu.PeliPaikat)
                {
                    if (!string.IsNullOrEmpty(p.Nimi) &&
                        !this.PeliPaikat.Any(x => string.Equals(x.Nimi, p.Nimi, StringComparison.OrdinalIgnoreCase)))
                    {
                        p.VarmistaAinakinYksiPoyta();
                        this.PeliPaikat.Add(p);
                    }
                }

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

                if (editoitavaksi)
                {
                    PaivitaOsallistujatJarjestyksessa();
                }

                if (this.KilpaSarja == KaisaKaavio.KilpaSarja.Joukkuekilpailu)
                {
                    PaivitaJoukkueKisa();
                }

                this.TallennusAjastin = Asetukset.AutomaattisenTallennuksenTaajuus;
            }
        }

        public string PelaajanNimiKaaviossa(string idString, bool detaljit, int kierros)
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
 
            if (!detaljit &&
                (this.KilpaSarja == KaisaKaavio.KilpaSarja.Parikilpailu ||
                this.KilpaSarja == KaisaKaavio.KilpaSarja.MixedDoubles))
            {
                var osat = nimi.Split(' ');
                List<string> nimiosat = new List<string>();

                foreach (var o in osat)
                {
                    if ((o.Length == 1 && o[0] == '&') ||
                        (o.Count(x => Char.IsLetter(x)) > 2))
                    {
                        nimiosat.Add(o.Trim());
                    }
                }

                nimi = string.Join(" ", nimiosat);
            }
            else
            {
                if (!detaljit)
                {
                    nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(pelaaja.Nimi);
                }
            }
            
            if (detaljit && !string.IsNullOrEmpty(pelaaja.Seura))
            {
                nimi += " " + pelaaja.Seura.Trim();
            }

            if (this.KilpailuOnViikkokisa)
            {
                if (detaljit && !string.IsNullOrEmpty(pelaaja.Sijoitettu))
                {
                    if (pelaaja.Sijoitettu.Contains('(') || pelaaja.Sijoitettu.Contains('['))
                    {
                        nimi += " " + pelaaja.Sijoitettu.Trim();
                    }
                    else
                    {
                        nimi += " (" + pelaaja.Sijoitettu.Trim() + ")";
                    }
                }
            }
            else
            {
                if (detaljit && !string.IsNullOrEmpty(pelaaja.Sijoitettu) && kierros < 4)
                {
                    if (pelaaja.Sijoitettu.Contains('(') || pelaaja.Sijoitettu.Contains('['))
                    {
                        nimi = pelaaja.Sijoitettu.Trim() + " " + nimi;
                    }
                    else
                    {
                        nimi = "[" + pelaaja.Sijoitettu.Trim() + "] " + nimi;
                    }
                }
            }

            if (this.KilpailuOnTasurikisa)
            {
                if (detaljit && !string.IsNullOrEmpty(pelaaja.Tasoitus))
                {
                    if (pelaaja.Tasoitus.Contains('(') || pelaaja.Tasoitus.Contains('['))
                    {
                        nimi += " " + pelaaja.Tasoitus.Trim();
                    }
                    else
                    {
                        nimi += " (" + pelaaja.Tasoitus.Trim() + ")";
                    }
                }
            }

            return nimi;
        }

        public string PelaajanNimiTulosluettelossa(string idString)
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

            if (this.KilpaSarja == KaisaKaavio.KilpaSarja.MixedDoubles ||
                this.KilpaSarja == KaisaKaavio.KilpaSarja.Parikilpailu)
            {
                if (!string.IsNullOrEmpty(pelaaja.Pelaajan1Nimi) &&
                    !string.IsNullOrEmpty(pelaaja.Pelaajan2Nimi) &&
                    !string.IsNullOrEmpty(pelaaja.Pelaajan1Seura) &&
                    !string.IsNullOrEmpty(pelaaja.Pelaajan2Seura))
                {
                    return string.Format("{0} {1} & {2} {3}",
                        pelaaja.Pelaajan1Nimi,
                        pelaaja.Pelaajan1Seura,
                        pelaaja.Pelaajan2Nimi,
                        pelaaja.Pelaajan2Seura);
                }
            }


            string nimi = pelaaja.Nimi;

            if (this.JoukkueKilpailunVarsinainenKilpailu != null)
            {
                var pelaajat = this.JoukkueKilpailunVarsinainenKilpailu.Osallistujat.Where(x => string.Equals(x.Joukkue, pelaaja.Nimi));
                if (pelaajat != null && pelaajat.Any())
                {
                    nimi += " (" + string.Join(", ", pelaajat.Select(x => x.Nimi)) + ")";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pelaaja.Seura))
                {
                    nimi += " " + pelaaja.Seura.Trim();
                }
            }

            return nimi;
        }

        public void SiirraJalkiIlmoittautuneetOsallistujiin(Asetukset asetukset)
        {
            foreach (var j in this.JalkiIlmoittautuneet)
            {
                if (!string.IsNullOrEmpty(j.Nimi))
                {
                    var pelaaja = asetukset.Pelaajat.FirstOrDefault(x => Tyypit.Nimi.Equals(j.Nimi, x.Nimi));
                    if (pelaaja != null)
                    {
                        j.Seura = pelaaja.Seura;

                        if (this.KilpailuOnTasurikisa)
                        {
                            j.Tasoitus = pelaaja.Tasoitus(this.Laji);
                        }
                    }

                    this.Osallistujat.Add(j);
                }
            }

            this.JalkiIlmoittautuneet.Clear();
        }

        public void PeruutaArvonta()
        {
            this.Pelit.Clear();

            foreach (var o in this.Osallistujat)
            {
                o.Id = -1;
            }

            PaivitaOsallistujatJarjestyksessa();
        }

        public bool ArvoKaavio(out string virhe)
        {
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.ArvoKaavio"))
#endif
            {
                virhe = string.Empty;

                if (Pelit.Any(x => (x.Kierros > 1) && (x.Tilanne == PelinTilanne.Pelattu || x.Tilanne == PelinTilanne.Kaynnissa)))
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("Kaavion arpominen ei ole mahdollista enää toisen kierroksen alettua");
                    }

                    virhe = "Kaavion arpominen ei ole mahdollista enää toisen kierroksen alettua";
                    return false;
                }

                foreach (var osallistuja in this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
                {
                    if (this.JoukkueKilpailunVarsinainenKilpailu != null)
                    {
                        if (this.Osallistujat.Count(x => string.Equals(x.Nimi, osallistuja.Nimi)) > 1)
                        {
                            virhe = string.Format("Joukkue {0} on kahdesti osallistujalistalla", osallistuja.Nimi);
                            return false;
                        }
                    }
                    else
                    {
                        if (this.Osallistujat.Count(x => Tyypit.Nimi.Equals(x.Nimi, osallistuja.Nimi)) > 1)
                        {
                            virhe = string.Format("Pelaaja {0} on kahdesti osallistujalistalla", osallistuja.Nimi);
                            return false;
                        }
                    }
                }

                if (this.KilpaSarja == KaisaKaavio.KilpaSarja.Joukkuekilpailu)
                {
                    var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
                    if (osallistujat.Any(x => string.IsNullOrEmpty(x.Joukkue)))
                    {
                        virhe = "Ilmoittautuneissa on pelaaja/pelaajia ilman joukkuetta";
                        return false;
                    }

                    var joukkueet = osallistujat.GroupBy(x => x.Joukkue);
                    if (joukkueet.Count() < 4)
                    {
                        virhe = "Joukkueita on alle neljä. Kisoja ei voida pelata";
                        return false;
                    }

                    foreach (var joukkue in joukkueet)
                    {
                        if (joukkue.Count() < 3)
                        {
                            virhe = string.Format("Joukkueessa {0} on alle 3 pelaaja", joukkue.Key);
                            return false;
                        }

                        if (joukkue.Count() > 4)
                        {
                            virhe = string.Format("Joukkueessa {0} on yli 4 pelaaja", joukkue.Key);
                            return false;
                        }
                    }
                }

                PoistaTyhjatOsallistujat();
                PoistaTyhjatPelit(1);

                int osallistujia = this.Osallistujat.Count(x => !string.IsNullOrEmpty(x.Nimi));
                int sijoitettuja = this.Osallistujat.Count(x => !string.IsNullOrEmpty(x.Sijoitettu));

                if (this.KilpaSarja == KaisaKaavio.KilpaSarja.Joukkuekilpailu)
                {
                    // Pelaajien id:llä ei ole väliä joukkuekisassa, haku tehdään joukkueilla
                    int id = 1;
                    foreach (var pelaaja in this.Osallistujat.OrderBy(x => x.Joukkue))
                    {
                        pelaaja.Id = id;
                        id++;
                    }

                    PaivitaOsallistujatJarjestyksessa();

                    PaivitaJoukkueKisa();
                    if (this.JoukkueKilpailu.ArvoKaavio(out virhe))
                    {
                        // Tallenetaan joukkue id pelaajille
                        foreach (var pelaaja in this.Osallistujat)
                        {
                            var joukkue = this.JoukkueKilpailu.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, pelaaja.Joukkue));
                            if (joukkue != null)
                            {
                                pelaaja.JoukkueId = joukkue.Id.ToString(); // !!!
                            }
                        }

                        // Laitetaan pelaajien idt samaan järjestykseen joukkueiden kanssa
                        int pelaajaId = 1;
                        foreach (var joukkue in this.JoukkueKilpailu.Osallistujat.OrderBy(x => x.Id))
                        {
                            foreach (var pelaaja in this.Osallistujat.Where(x => string.Equals(x.Joukkue, joukkue.Nimi)))
                            {
                                pelaaja.Id = pelaajaId;
                                pelaajaId++;
                            }
                        }

                        PaivitaPelitJoukkueKisasta();
                        return true;
                    }
                    else 
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.Sijoittaminen != KaisaKaavio.Sijoittaminen.EiSijoittamista &&
                        sijoitettuja > 0 &&
                        !this.Osallistujat.Any(x => x.Id > 0))
                    {
                        if (this.Sijoittaminen == KaisaKaavio.Sijoittaminen.Sijoitetaan8Pelaajaa)
                        {
                            ArvoPelaajienIdtSijoituksilla8();
                        }
                        else if (osallistujia >= 24 && this.Sijoittaminen == KaisaKaavio.Sijoittaminen.Sijoitetaan24Pelaajaa)
                        {
                            ArvoPelaajienIdtSijoituksilla24();
                        }
                        else
                        {
                            ArvoPelaajienIdt();
                        }
                    }
                    else
                    {
                        ArvoPelaajienIdt();
                    }
                }

                if (this.OnUseanPelipaikanKilpailu)
                {
                    if (!AsetaPelipaikatPelaajille(out virhe))
                    {
                        return false;
                    }
                }
                
                PaivitaOsallistujatJarjestyksessa();

                if (this.OnUseanPelipaikanKilpailu)
                {
                    return HaeAlkukierroksetUseanPelipaikanKisaan(out virhe);
                }
                else
                {
                    return HaeAlkukierroksetYhdenPelipaikanKisaan(out virhe);
                }
            }
        }

        public int PelipaikanIndeksi(string pelipaikka)
        {
            if (string.IsNullOrEmpty(pelipaikka))
            {
                return 0;
            }

            int indeksi = 0;

            var salit = Salit();

            foreach (var p in salit)
            {
                if (string.Equals(pelipaikka, p.LyhytNimi, StringComparison.OrdinalIgnoreCase))
                {
                    return indeksi;
                }
                indeksi++;
            }

            return indeksi;
        }

        private int PelaajanPaikkaOsakaaviossa(Pelaaja pelaaja, List<Sali> salit)
        {
            var sali = salit.FirstOrDefault(x => pelaaja.Id >= x.MinPelaajaId && pelaaja.Id <= x.MaxPelaajaId);
            if (sali == null)
            {
                return pelaaja.Id;
            }

            return pelaaja.Id - sali.MinPelaajaId;
        }

        private bool VaarassaPaikassa(Pelaaja pelaaja, List<Sali> salit)
        {
            if (string.IsNullOrEmpty(pelaaja.PeliPaikka))
            {
                return false;
            }

            var s = salit.FirstOrDefault(x => string.Equals(pelaaja.PeliPaikka, x.LyhytNimi, StringComparison.OrdinalIgnoreCase));
            if (s == null)
            {
                return false;
            }

            return pelaaja.Id < s.MinPelaajaId || pelaaja.Id > s.MaxPelaajaId;
        }

        private bool AsetaPelipaikatPelaajille(out string virhe)
        {
            int max = 0;

            if (this.Sali != null)
            {
                this.Sali.MinPelaajaId = 1;
                this.Sali.MaxPelaajaId = this.Sali.Pelaajia;
                max = this.Sali.MaxPelaajaId;
            }

            foreach (var s in this.PeliPaikat.Where(x => x.Kaytossa))
            {
                s.MinPelaajaId = max + 1;
                s.MaxPelaajaId = s.MinPelaajaId + s.Pelaajia;
                max = s.MaxPelaajaId;
            }

            List<Sali> salit = new List<Sali>();

            if (this.Sali != null)
            {
                salit.Add(this.Sali);
            }

            salit.AddRange(this.PeliPaikat.Where(x => x.Kaytossa));

            Dictionary<Pelaaja, Sali> pelaajatVaarallaSalilla = new Dictionary<Pelaaja, Sali>();

            // Varmista, että pelaajat joilla on ennalta määrätty pelipaikka päätyvät oikealle salille
            foreach (var pelaaja in this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.PeliPaikka)))
            {
                var s = salit.FirstOrDefault(x => string.Equals(pelaaja.PeliPaikka, x.LyhytNimi, StringComparison.OrdinalIgnoreCase));
                if (s == null)
                {
                    if (this.PeliPaikat.Any(x => string.Equals(x.LyhytNimi, pelaaja.PeliPaikka, StringComparison.OrdinalIgnoreCase)
                        && !x.Kaytossa))
                    {
                        pelaaja.PeliPaikka = string.Empty;
                    }
                    else
                    {
                        virhe = string.Format("Pelaajalle ennaltamäärättyä pelipaikkaa '{0}' ei löydy pelipaikoista", pelaaja.PeliPaikka);
                        return false;
                    }
                }
                else if (pelaaja.Id < s.MinPelaajaId || pelaaja.Id > s.MaxPelaajaId)
                {
                    pelaajatVaarallaSalilla.Add(pelaaja, s);
                }
            }

            while (pelaajatVaarallaSalilla.Any())
            {
                var pelaaja = pelaajatVaarallaSalilla.OrderBy(x => string.IsNullOrEmpty(x.Key.Sijoitettu) ? 1 : 0).First();
                pelaajatVaarallaSalilla.Remove(pelaaja.Key);
                if (!VaihdaPelaajaOikealleSalille(pelaaja.Key, pelaaja.Value, pelaajatVaarallaSalilla, salit, out virhe))
                {
                    return false;
                }
            }

            // Aseta lopulliset salipaikat
            foreach (var s in salit)
            {
                foreach (var p in this.Osallistujat.Where(x => x.Id >= s.MinPelaajaId && x.Id <= s.MaxPelaajaId))
                {
                    p.PeliPaikka = s.LyhytNimi;
                }
            }

            virhe = string.Empty;
            return true;
        }

        private bool VaihdaPelaajaOikealleSalille(
            Pelaaja pelaaja, 
            Sali sali,
            Dictionary<Pelaaja, Sali> pelaajatVaarallaPaikalla, 
            List<Sali> salit,
            out string virhe)
        {
            List<Pelaaja> vaihtopelaajat = new List<Pelaaja>();

            foreach (var o in this.Osallistujat)
            {
                if (o.Id >= sali.MinPelaajaId && o.Id <= sali.MaxPelaajaId)
                {
                    if (!string.IsNullOrEmpty(o.PeliPaikka) && string.Equals(o.PeliPaikka, sali.LyhytNimi, StringComparison.OrdinalIgnoreCase))
                    {
                    }
                    else 
                    {
                        vaihtopelaajat.Add(o);
                    }
                }
            }

            Pelaaja parasVaihto = null;
            int parhaatPisteet = 0;

            int pelaajanPaikka = PelaajanPaikkaOsakaaviossa(pelaaja, salit);

            foreach (var vaihtopelaaja in vaihtopelaajat)
            {
                int pisteet = 0;

                int vaihtopelaajanPaikka = PelaajanPaikkaOsakaaviossa(vaihtopelaaja, salit);

                int paikkaEtaisyys = Math.Abs(pelaajanPaikka - vaihtopelaajanPaikka);

                if (paikkaEtaisyys < 5)
                {
                    pisteet += 5 - paikkaEtaisyys; // Vaihda mielellään samasta kohtaa kaavioita pelaajat keskenään
                }

                if (!string.IsNullOrEmpty(pelaaja.Sijoitettu) && !string.IsNullOrEmpty(vaihtopelaaja.Sijoitettu))
                {
                    pisteet += 1000; // Vaihda sijoitettu pelaaja mielellään toisen sijoitetun pelaajan kanssa
                }

                if (string.IsNullOrEmpty(pelaaja.Sijoitettu) != string.IsNullOrEmpty(vaihtopelaaja.Sijoitettu))
                {
                    pisteet -= 100000; // Älä ikinä vaihda sijoitetun ja ei-sijoitetun pelaajan paikkaa
                }

                if (pisteet > parhaatPisteet)
                {
                    parasVaihto = vaihtopelaaja;
                    parhaatPisteet = pisteet;
                }
            }

            if (parasVaihto != null)
            {
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita(string.Format("Vaihdettiin pelaajien {0} ja {1} paikat kaaviossa jotta aiempi saadaan oikealle salille", 
                        pelaaja.Nimi,
                        parasVaihto.Nimi));
                }

                int temp = parasVaihto.Id;
                parasVaihto.Id = pelaaja.Id;
                pelaaja.Id = temp;

                virhe = string.Empty;
                return true;
            }
            else
            {
                virhe = string.Format("Pelaajien vaihto pelipaikan varmistamiseksi ei onnistunut pelaajalle {0}", pelaaja.Nimi);
                return false;
            }
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

        private void ArvoPelaajienIdtSijoituksilla8()
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

        private void ArvoPelaajienIdtSijoituksilla24()
        {
            int[] paikat = new int[]{ 16, 1, 24, 9, 17, 8, 20, 4, 13, 12, 21, 5, 15, 2, 23, 10, 18, 7, 14, 3, 22, 11, 19, 6 };
            List<int> paikkalista = new List<int>();
            paikkalista.AddRange(paikat);

            var osallistujat = this.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi));
            int osallistujia = osallistujat.Count();

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
                .Take(24);

            int sijoitus = 1;
            foreach (var sijoitettu in sijoitetut)
            {
                sijoitettu.Sijoitettu = sijoitus.ToString();

                int paikka = paikkalista.IndexOf(sijoitus);
                float d = (float)paikka / 23.0f;
                int id = 1 + (int)Math.Floor(d * ((float)(osallistujia - 2)));

                if (!vapaatIdt.Contains(id))
                {
#if DEBUG
                    Debug.WriteLine(string.Format("Sijoitettu id {0} ei vapaa. Valitaan lähin", id));
#endif
                    id = vapaatIdt.OrderBy(x => Math.Abs(x - id)).FirstOrDefault();

#if DEBUG
                    Debug.WriteLine(string.Format("Valittu id {0}", id));
#endif
                }

                sijoitettu.Id = id;
                vapaatIdt.Remove(id);

                sijoitus++;
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

            // Varmistetaan vielä että id:issä ei ole "koloja"
            int sid = 1;
            foreach (var o in osallistujat.OrderBy(x => x.Id))
            {
                o.Id = sid++;
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

        public int LaskeJoukkuePisteet(int pelaaja, int kierros)
        {
            int pisteet = 0;

            if (this.JoukkueKilpailunVarsinainenKilpailu != null)
            {
                Pelaaja joukkue = this.Osallistujat.FirstOrDefault(x => x.Id == pelaaja);

                var pelaajat = this.JoukkueKilpailunVarsinainenKilpailu.Osallistujat.Where(x => string.Equals(x.Joukkue, joukkue.Nimi));

                foreach (var p in pelaajat)
                {
                    pisteet += this.JoukkueKilpailunVarsinainenKilpailu.LaskePisteet(p.Id, kierros);
                }
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

        private bool HaeAlkukierroksetYhdenPelipaikanKisaan(out string virhe)
        {
            return HaeAlkukierrokset(out virhe);
        }

        private bool HaeAlkukierroksetUseanPelipaikanKisaan(out string virhe)
        {
            var osakilpailut = Osakilpailut();

            foreach (var osakilpailu in osakilpailut)
            {
                if (!osakilpailu.HaeAlkukierroksetYhdenPelipaikanKisaan(out virhe))
                {
                    return false;
                }
            }

            PaivitaOsakilpailujenHaut(osakilpailut);

            virhe = string.Empty;
            return true;
        }

        private bool HaeAlkukierrokset(out string virhe)
        {
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.HaeAlkukierrokset"))
#endif
            {
                virhe = string.Empty;

#if DEBUG // Invariantit:
                if (this.OsallistujatJarjestyksessa.Count != this.Osallistujat.Count)
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("BUGI!!! Osallistujalistat epäsynkassa arvottaessa alkukierroksia!", null, true);
                    }
                    return false;
                }

                if (this.Osallistujat.Any(x => x.Id < 0))
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("BUGI!!! Osallistujissa on id:ttömiä pelaajia arvottaessa alkukierroksia!", null, true);
                    }
                    return false;
                }

                if (this.Osallistujat.Count < Asetukset.PelaajiaVahintaanKaaviossa)
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("BUGI!!! Liian vähän osallistujia arvottaessa alkukierroksia!", null, true);
                    }
                    return false;
                }

                if (this.Pelit.Any(x => x.Kierros > 1))
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita("BUGI!!! Kaaviossa on toisen kierroksen pelejä arvottaessa ekoja kierroksia!", null, true);
                    }
                    return false;
                }
#endif
                bool pelaajiaSijoitettu = false;

                if (this.Sijoittaminen == KaisaKaavio.Sijoittaminen.Sijoitetaan24Pelaajaa)
                {
                    if (this.Loki != null)
                    {
                        Loki.Kirjoita("Laitetaan sijoitetuille pelaajille W.O. pelit");
                    }

                    foreach (var sijoitettu in this.Osallistujat.Where(x => x.SijoitusNumero <= 12))
                    {
                        LisaaWO(sijoitettu);
                        pelaajiaSijoitettu = true;
                    }

                    foreach (var sijoitettu in this.Osallistujat.Where(x => x.SijoitusNumero <= 24))
                    {
                        LisaaWO(sijoitettu);
                        pelaajiaSijoitettu = true;
                    }
                }

                if (this.Loki != null)
                {
                    this.Loki.Kirjoita("Haetaan pelit kierrokselle 1");
                }

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
                        if (this.Loki != null)
                        {
                            this.Loki.Kirjoita("BUGI!!! Hakijalle ei löytynyt vastustajaa alkukierrokselle", null, true);
                        }
#endif
                        return false;
                    }

                    LisaaPeli(hakija, vastustajat.First());
                }

                if (this.Loki != null)
                {
                    Loki.Kirjoita("Haetaan pelit kierrokselle 2");
                }

                if (!pelaajiaSijoitettu)
                {
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
                }

                while (true)
                {
                    var mukana = this.Osallistujat.Where(x => Pelit.Count(y => y.SisaltaaPelaajan(x.Id)) < 2);

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
                        if (this.Loki != null)
                        {
                            this.Loki.Kirjoita("BUGI!!! Hakijalle ei löytynyt vastustajaa alkukierrokselle", null, true);
                        }
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
        }

        public IHakuAlgoritmi Haku(IStatusRivi status)
        {
            if (this.JoukkueKilpailu != null)
            {
                PaivitaPelitJoukkueKisaan();
                return this.JoukkueKilpailu.Haku(status);
            }

            if (this.OnUseanPelipaikanKilpailu)
            {
                var pelaamattomatPelit = this.Pelit.Where(x => x.Tilanne != PelinTilanne.Pelattu);
                if (pelaamattomatPelit.Any())
                {
                    int kierros = pelaamattomatPelit
                        .Select(x => x.Kierros)
                        .Min();

                    if (kierros >= this.KaavioidenYhdistaminenKierroksestaInt)
                    {
                        return YhdenPelipaikanHaku(status);
                    }
                }
                else 
                {
                    var pelatutPelit = this.Pelit.Where(x => x.Tilanne == PelinTilanne.Pelattu);
                    if (pelatutPelit.Any())
                    {
                        int kierros = pelatutPelit
                            .Select(x => x.Kierros)
                            .Max();

                        if (kierros >= this.KaavioidenYhdistaminenKierroksestaInt - 1)
                        {
                            return YhdenPelipaikanHaku(status);
                        }
                    }
                }

                int minKierros = 0;

                var haku = UseanPelipaikanHaku(status, out minKierros);

                if (haku != null)
                {
                    return haku;
                }
                else
                {
                    return null;
                }
            }
            else 
            {
                return YhdenPelipaikanHaku(status);
            }
        }

        private IHakuAlgoritmi UseanPelipaikanHaku(IStatusRivi status, out int minKierros)
        {
            minKierros = 0;

            var osakilpailut = Osakilpailut();

            List<IHakuAlgoritmi> haut = new List<IHakuAlgoritmi>();

            foreach (var kilpailu in osakilpailut)
            {
                var haku = kilpailu.Haku(status);
                if (haku != null)
                {
                    if (haku.Kierros <= this.KaavioidenYhdistaminenKierroksestaInt - 1)
                    {
                        haut.Add(haku);
                    }

                    if (minKierros == 0)
                    {
                        minKierros = haku.Kierros;
                    }
                    else
                    {
                        minKierros = Math.Min(minKierros, haku.Kierros);
                    }
                }
            }

            if (haut.Any())
            {
                return new UseanPelipaikanHakuAlgoritmi(haut, this.KaavioidenYhdistaminenKierroksestaInt - 1); 
            }

            return null;
        }

        private IHakuAlgoritmi YhdenPelipaikanHaku(IStatusRivi status)
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

                    vajaitaPeleja = Pelit.Count(x => 
                        (x.Kierros == kierros) && 
                        (x.Tilanne != PelinTilanne.Pelattu) &&
                        (x.Id1 < 0 || x.Id2 < 0));

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
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.PaivitaPelienTulokset"))
#endif
            {
                foreach (var peli in this.Pelit)
                {
                    peli.PaivitaTulos();
                }
            }
        }

        public void PaivitaPelitValmiinaAlkamaan()
        {
#if PROFILE
            using (new Testaus.Profileri("Kilpailu.PaivitaPelitValmiinaAlkamaan"))
#endif
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

        private IHakuAlgoritmi Haku(int kierros, IStatusRivi status)
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
        public bool KaavioArvottu
        {
            get { return this.Pelit.Any(); }
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
        public Pelaaja KilpailunVoittaja
        {
            get
            {
                if (this.JoukkueKilpailu != null)
                {
                    return this.JoukkueKilpailu.Voittaja();
                }
                else
                {
                    return this.Voittaja();
                }
            }
        }

        public IEnumerable<string> Poydat(Sali sali)
        {
            List<string> poydat = new List<string>();

            if (sali != null)
            {
                poydat.AddRange(sali.Poydat
                    .Where(x => !string.IsNullOrEmpty(x.Numero))
                    .OrderBy(x => x.Numero)
                    .Select(x => x.Numero));
            }

            if (!OnUseanPelipaikanKilpailu)
            {
                foreach (var peli in this.Pelit.Where(x => !string.IsNullOrEmpty(x.Poyta)))
                {
                    if (!poydat.Contains(peli.Poyta))
                    {
                        poydat.Add(peli.Poyta);
                    }
                }
            }

            return poydat.OrderBy(x => x);
        }

        public IEnumerable<string> VapaatPoydat(Peli peli, Asetukset asetukset)
        {
            Sali sali = null;

            if (this.OnUseanPelipaikanKilpailu) // TODO!!! Kaavioiden yhdistäminen
            {
                var salit = Salit();

                if (string.IsNullOrEmpty(peli.Paikka))
                {
                    sali = salit.FirstOrDefault();
                }
                else
                {
                    sali = salit.FirstOrDefault(x => string.Equals(peli.Paikka, x.LyhytNimi));
                }
            }
            else
            {
                sali = asetukset.Sali;
            }

            var poydat = Poydat(sali);

            if (this.OnUseanPelipaikanKilpailu) // TODO!!! Kaavioiden yhdistäminen
            {
                return poydat.Where(x => !this.Pelit.Any(y => 
                    y.OnSalilla(sali) &&
                    y.Tilanne == PelinTilanne.Kaynnissa && 
                    string.Equals(y.Poyta, x)));
            }
            else
            {
                return poydat.Where(x => !this.Pelit.Any(y => y.Tilanne == PelinTilanne.Kaynnissa && string.Equals(y.Poyta, x)));
            }
        }

        [XmlIgnore]
        public bool OnUseanPelipaikanKilpailu
        {
            get
            {
                return this.PeliPaikat.Any(x => !x.Tyhja);
            }
        }

        [XmlIgnore]
        public bool KilpailuOnPaattynyt
        {
            get 
            {
                return Voittaja() != null;
            }
        }

        public IEnumerable<Pelaaja.TulosTietue> Tulokset()
        {
            List<Pelaaja.TulosTietue> tulostietueet = new List<Pelaaja.TulosTietue>();
            
            foreach (var o in this.Osallistujat.Where(x => x.Id >= 0))
            {
                o.Sijoitus.Lyontivuoroja = 0;
                o.Sijoitus.Karoja = 0;
                o.Sijoitus.Pisteet = LaskePisteet(o.Id, 999);
                o.Sijoitus.Voitot = LaskeVoitot(o.Id, 999);
                o.Sijoitus.Tappiot = LaskeTappiot(o.Id, 999);
                o.Sijoitus.JoukkuePisteet = LaskeJoukkuePisteet(o.Id, 999);
                o.Sijoitus.Pudotettu = !Mukana(o);
                o.Sijoitus.SijoitusOnVarma = true;

                if (o.Sijoitus.Pudotettu)
                {
                    o.Sijoitus.PudonnutKierroksella = this.Pelit.Count(x => x.SisaltaaPelaajan(o.Id));
                }
                else
                {
                    o.Sijoitus.PudonnutKierroksella = 0;
                }

                // Sijoituspisteet määräävät pelaajien sijoituksen kisan päätyttyä.
                if (this.JoukkueKilpailunVarsinainenKilpailu != null)
                {
                    o.Sijoitus.SijoitusPisteet = o.Sijoitus.Pudotettu ? 0 : 1000000000; // Laittaa voittajan ykköseksi ja mukana olevat listan kärkeen
                    o.Sijoitus.SijoitusPisteet += o.Sijoitus.Voitot * 1000000;
                    o.Sijoitus.SijoitusPisteet += o.Sijoitus.Pisteet * 4000;
                    o.Sijoitus.SijoitusPisteet += o.Sijoitus.JoukkuePisteet;
                }
                else 
                {
                    o.Sijoitus.SijoitusPisteet = o.Sijoitus.Pudotettu ? 0 : 1000000000; // Laittaa voittajan ykköseksi ja mukana olevat listan kärkeen
                    o.Sijoitus.SijoitusPisteet += o.Sijoitus.Voitot * 100000;
                    o.Sijoitus.SijoitusPisteet += o.Sijoitus.Pisteet;
                }

                tulostietueet.Add(o.Sijoitus);
            }

            if (this.Laji == KaisaKaavio.Laji.Kara)
            {
                foreach (var p in this.Pelit)
                {
                    var p1 = this.Osallistujat.FirstOrDefault(x => x.Id == p.Id1);
                    if (p1 != null)
                    {
                        p1.Sijoitus.Lyontivuoroja += p.LyontivuorojaInt;
                        bool v = false, h = false;
                        int pisteet = Peli.Pisteet(p.Pisteet1, out v, out h) - p1.TasoitusInt;
                        pisteet = Math.Max(0, pisteet);
                        p1.Sijoitus.Karoja += pisteet;
                    }

                    var p2 = this.Osallistujat.FirstOrDefault(x => x.Id == p.Id2);
                    if (p2 != null)
                    {
                        p2.Sijoitus.Lyontivuoroja += p.LyontivuorojaInt;
                        bool v = false, h = false;
                        int pisteet = Peli.Pisteet(p.Pisteet2, out v, out h) - p2.TasoitusInt;
                        pisteet = Math.Max(0, pisteet);
                        p2.Sijoitus.Karoja += pisteet;
                    }
                }
            }

            var tulokset = tulostietueet.OrderByDescending(x => x.SijoitusPisteet);

            int mukana = tulokset.Count(x => !x.Pudotettu);

            // Huomioidaan sijoituksissa kuinka pitkälle pelaaja pääsi (jos näin on asetettu) 
            if (this.SijoitustenMaaraytyminen != KaisaKaavio.SijoitustenMaaraytyminen.VoittajaKierroksistaLoputPisteista && mukana <= 2)
            {
                var finaali = this.Pelit.LastOrDefault();
                if (finaali != null)
                {
                    if (KilpailuOnPaattynyt)
                    {
                        var finalisti = tulokset.FirstOrDefault(x => x.Pelaaja == finaali.Haviaja());
                        if (finalisti != null)
                        {
                            finalisti.SijoitusPisteet += 500000000;
                        }
                    }

                    if (this.SijoitustenMaaraytyminen == KaisaKaavio.SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista)
                    {
                        var t = tulokset
                            .Where(x => !finaali.SisaltaaPelaajan(x.Pelaaja.Id))
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

            // Tarkistetaan onko tulokset varmasti lopullisia
            if (mukana > 1)
            {
                foreach (var t in tulokset)
                {
                    t.SijoitusOnVarma = t.Pudotettu;

                    if (tulokset.Where(x => x.Pelaaja != t.Pelaaja && !x.Pudotettu).Any(y =>
                        y.Voitot < t.Voitot ||
                        y.Voitot == t.Voitot && y.Pisteet <= t.Pisteet))
                    {
                        if (SijoitustenMaaraytyminen == KaisaKaavio.SijoitustenMaaraytyminen.KaksiParastaKierroksistaLoputPisteista &&
                            mukana <= 2 &&
                            t.Sijoitus >= 3)
                        {
                        }
                        else
                        {
                            t.SijoitusOnVarma = false; // Joku mukana olevista voi vielä jäädä huonommalle sijalle
                        }
                    }

                    if (SijoitustenMaaraytyminen == KaisaKaavio.SijoitustenMaaraytyminen.KolmeParastaKierroksistaLoputPisteista &&
                        mukana > 2 &&
                        t.Sijoitus <= 3)
                    {
                        t.SijoitusOnVarma = false;
                    }

                    if (SijoitustenMaaraytyminen == KaisaKaavio.SijoitustenMaaraytyminen.KaksiParastaKierroksistaLoputPisteista &&
                        mukana > 2 &&
                        t.Sijoitus < 3)
                    {
                        t.SijoitusOnVarma = false;
                    }

                    if (mukana == 1 && t.Sijoitus == 1)
                    {
                        t.SijoitusOnVarma = true;
                    }
                }
            }

            // Poistetaan jaetut sijat tulosten alkupäästä, kun nimet ovat tyhjiä
            int sija = 1;
            foreach (var t in tulokset.Where(x => !x.SijoitusOnVarma))
            {
                t.Sijoitus = sija++;
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
                if (this.JoukkueKilpailu != null)
                {
                    return this.JoukkueKilpailu.PelienTilanneTeksti;
                }

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
                return this.AlkamisAika;
            }
            else
            {
                return string.Format("{0} klo:{1}", this.AlkamisAika, this.KellonAika);
            }
        }

        public int RankingSarjanNumero()
        {
            var aika = this.AlkamisAikaDt;

            switch (this.RankingKisaTyyppi)
            {
                case Ranking.RankingSarjanPituus.Kuukausi: return aika.Month;
                case Ranking.RankingSarjanPituus.Vuodenaika: return (aika.Month - 1) / 3;
                case Ranking.RankingSarjanPituus.Puolivuotta: return (aika.Month - 1) / 6;
                case Ranking.RankingSarjanPituus.Vuosi: return 0;
                default: return 0;
            }
        }

        public void PeruutaKaynnissaOlevaPeli(Peli peli)
        {
            if (this.Loki != null)
            {
                this.Loki.Kirjoita(string.Format("Peruutetaan ottelun {0} käynnistys (manuaalinen muutos kisanvetäjältä).", peli.Kuvaus()));
            }

            peli.Tilanne = PelinTilanne.Tyhja;
            peli.Tulos = PelinTulos.EiTiedossa;
            peli.Pisteet1 = string.Empty;
            peli.Pisteet2 = string.Empty;
            peli.Poyta = string.Empty;
            peli.Alkoi = string.Empty;
            peli.Paattyi = string.Empty;
        }

        /// <summary>
        /// Muuttaa jo pelatun pelin tulosta kaaviossa, sekä tarvittaessa mitätöi tämän pelin jälkeisiä
        /// pelejä kaaviossa
        /// </summary>
        public void PaivitaPelatunPelinTulos(Peli peli, PelinTulos uusiTulos, PelinTilanne uusiTilanne)
        {
            if (uusiTilanne == PelinTilanne.Tyhja && uusiTulos == PelinTulos.EiTiedossa)
            {
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita(string.Format("Peruutetaan ottelun {0} käynnistys (manuaalinen muutos kisanvetäjältä).", peli.Kuvaus()));
                }

                peli.Tilanne = PelinTilanne.Tyhja;
                peli.Tulos = PelinTulos.EiTiedossa;
                peli.Pisteet1 = string.Empty;
                peli.Pisteet2 = string.Empty;
                peli.Poyta = string.Empty;
                peli.Alkoi = string.Empty;
                peli.Paattyi = string.Empty;
                return;
            }

            if (this.Loki != null)
            {
                this.Loki.Kirjoita(string.Format("Päivitetään pelatun pelin {0} tulos (manuaalinen muutos kisanvetäjältä).", peli.Kuvaus()));
            }

            int id1 = peli.Id1;
            int id2 = peli.Id2;
            int pelinNumero = peli.PeliNumero;

            int tappiot1 = LaskeTappiotPelille(id1, pelinNumero - 1);
            int tappiot2 = LaskeTappiotPelille(id2, pelinNumero - 1);

            if (uusiTilanne == PelinTilanne.Tyhja)
            {
                if (this.Loki != null)
                {
                    this.Loki.Kirjoita(string.Format("  -Poistetaan peli {0}", peli.Kuvaus()));
                }
                this.PoistaPeli(peli, true);
            }
            else if (uusiTulos == PelinTulos.Pelaaja1Voitti)
            {
                tappiot2++;
            }
            else if (uusiTulos == PelinTulos.Pelaaja2Voitti)
            {
                tappiot1++;
            }
            else if (uusiTulos == PelinTulos.MolemmatHavisi)
            {
                tappiot1++;
                tappiot2++;
            }

            // Poista pelit jotka eivät ole "sallittuja" muutoksen jälkeen
            var poistettavatPelit = this.Pelit.Where(x =>
                    x.PeliNumero > pelinNumero &&
                    x.SisaltaaJommanKummanPelaajan(id1, id2)).ToArray();

            foreach (var poistettavaPeli in poistettavatPelit)
            {
                bool poista = false;

                if (poistettavaPeli.SisaltaaPelaajan(id1))
                {
                    if (tappiot1 >= 2)
                    {
                        poista = true;
                    }
                    tappiot1++;
                }

                if (poistettavaPeli.SisaltaaPelaajan(id2))
                {
                    if (tappiot2 >= 2)
                    {
                        poista = true;
                    }
                    tappiot2++;
                }

                if (poista)
                {
                    if (this.Loki != null)
                    {
                        this.Loki.Kirjoita(string.Format("  -Poistetaan peli {0}", poistettavaPeli.Kuvaus()));
                    }
                    this.PoistaPeli(poistettavaPeli, true);
                }
            }

            PaivitaPelinumerot();

            this.HakuTarvitaan = true;
        }

        public List<Sali> Salit()
        {
            List<Sali> salit = new List<Sali>();

            if (this.Sali != null)
            {
                salit.Add(this.Sali);
            }

            salit.AddRange(this.PeliPaikat.Where(x => x.Kaytossa));

            return salit;
        }

        /// <summary>
        /// Luo jokaiselle pelipaikalle oman osakilpailunsa kilpailun tämänhetkisen tilanteen perusteella
        /// </summary>
        public List<Kilpailu> Osakilpailut()
        {
            List<Kilpailu> osakilpailut = new List<Kilpailu>();

            var salit = Salit();

            bool oletusSali = true;

            foreach (var sali in salit)
            {
                Kilpailu osakilpailu = new Kilpailu() 
                {
                   KaavioTyyppi = this.KaavioTyyppi,
                   KilpailunTyyppi = this.KilpailunTyyppi,
                   KilpaSarja = this.KilpaSarja,
                   Laji = this.Laji,
                   Nimi = this.Nimi,
                   TavoitePistemaara = this.TavoitePistemaara,
                   TestiKilpailu = this.TestiKilpailu,
                   Loki = this.Loki,
                   Sijoittaminen = this.Sijoittaminen,
                };

                foreach (var pelaaja in this.Osallistujat.Where(x => x.Id >= 0))
                {
                    if (oletusSali)
                    {
                        if (string.IsNullOrEmpty(pelaaja.PeliPaikka) || string.Equals(pelaaja.PeliPaikka, sali.LyhytNimi))
                        {
                            osakilpailu.Osallistujat.Add(new Pelaaja() 
                            {
                                Id = pelaaja.Id,
                                Nimi = pelaaja.Nimi,
                                PeliPaikka = sali.LyhytNimi,
                                Sijoitettu = pelaaja.Sijoitettu,
                                Joukkue = pelaaja.Joukkue,
                                Pelaajan1Nimi = pelaaja.Pelaajan1Nimi,
                                Pelaajan1Seura = pelaaja.Pelaajan1Seura,
                                Pelaajan2Nimi = pelaaja.Pelaajan2Nimi,
                                Pelaajan2Seura = pelaaja.Pelaajan2Seura
                            });
                        }
                    }
                    else 
                    {
                        if (string.Equals(pelaaja.PeliPaikka, sali.LyhytNimi))
                        {
                            osakilpailu.Osallistujat.Add(new Pelaaja()
                            {
                                Id = pelaaja.Id,
                                Nimi = pelaaja.Nimi,
                                PeliPaikka = pelaaja.PeliPaikka,
                                Sijoitettu = pelaaja.Sijoitettu,
                                Joukkue = pelaaja.Joukkue,
                                Pelaajan1Nimi = pelaaja.Pelaajan1Nimi,
                                Pelaajan1Seura = pelaaja.Pelaajan1Seura,
                                Pelaajan2Nimi = pelaaja.Pelaajan2Nimi,
                                Pelaajan2Seura = pelaaja.Pelaajan2Seura
                            });
                        }
                    }
                }

                osakilpailu.PaivitaOsallistujatJarjestyksessa();

                foreach (var peli in this.Pelit)
                {
                    if (oletusSali)
                    {
                        if (string.IsNullOrEmpty(peli.Paikka) || string.Equals(peli.Paikka, sali.LyhytNimi))
                        {
                            osakilpailu.Pelit.Add(new Peli() 
                            {
                                PelaajaId1 = peli.PelaajaId1,
                                PelaajaId2 = peli.PelaajaId2,
                                KierrosPelaaja1 = peli.KierrosPelaaja1,
                                KierrosPelaaja2 = peli.KierrosPelaaja2,
                                Kierros = peli.Kierros,
                                Kilpailu = osakilpailu,
                                PeliNumero = peli.PeliNumero,
                                Pisteet1 = peli.Pisteet1,
                                Pisteet2 = peli.Pisteet2,
                                Tilanne = peli.Tilanne,
                                Tulos = peli.Tulos,
                                Paikka = sali.LyhytNimi
                            });
                        }
                    }
                    else
                    {
                        if (string.Equals(peli.Paikka, sali.LyhytNimi))
                        {
                            osakilpailu.Pelit.Add(new Peli()
                            {
                                PelaajaId1 = peli.PelaajaId1,
                                PelaajaId2 = peli.PelaajaId2,
                                KierrosPelaaja1 = peli.KierrosPelaaja1,
                                KierrosPelaaja2 = peli.KierrosPelaaja2,
                                Kierros = peli.Kierros,
                                Kilpailu = osakilpailu,
                                PeliNumero = peli.PeliNumero,
                                Pisteet1 = peli.Pisteet1,
                                Pisteet2 = peli.Pisteet2,
                                Tilanne = peli.Tilanne,
                                Tulos = peli.Tulos,
                                Paikka = sali.LyhytNimi
                            });
                        }
                    }
                }

                osakilpailut.Add(osakilpailu);

                oletusSali = false;
            }

            return osakilpailut;
        }

        public void PaivitaOsakilpailujenHaut(List<Kilpailu> osakilpailut)
        {
            foreach (var osakilpailu in osakilpailut)
            {
                foreach (var peli in osakilpailu.Pelit)
                {
                    if (!this.Pelit.Any(x => 
                        x.Id1 == peli.Id1 &&
                        x.Id2 == peli.Id2 &&
                        x.Kierros == peli.Kierros &&
                        x.KierrosPelaaja1 == peli.KierrosPelaaja1 &&
                        x.KierrosPelaaja2 == peli.KierrosPelaaja2))
                    {
                        if (peli.Id2 < 0)
                        {
                            this.LisaaWO(this.Osallistujat.First(x => x.Id == peli.Id1));
                        }
                        else
                        {
                            this.LisaaPeli(
                                this.Osallistujat.First(x => x.Id == peli.Id1),
                                peli.KierrosPelaaja1,
                                this.Osallistujat.First(x => x.Id == peli.Id2),
                                peli.KierrosPelaaja2);
                        }
                    }
                }
            }
        }

        public void PaivitaJoukkueKisa()
        {
            if (JoukkueKilpailu == null)
            {
                JoukkueKilpailu = new Kilpailu();
                JoukkueKilpailu.JoukkueKilpailunVarsinainenKilpailu = this;
            }

            JoukkueKilpailu.KaavioidenYhdistaminenKierroksesta = this.KaavioidenYhdistaminenKierroksesta;
            JoukkueKilpailu.KaavioTyyppi = this.KaavioTyyppi;
            JoukkueKilpailu.KilpailunTyyppi = this.KilpailunTyyppi;
            JoukkueKilpailu.KilpaSarja = KaisaKaavio.KilpaSarja.Yleinen;
            JoukkueKilpailu.Laji = this.Laji;
            JoukkueKilpailu.Nimi = this.Nimi;
            JoukkueKilpailu.PeliAika = this.PeliAika;
            JoukkueKilpailu.PeliaikaOnRajattu = this.PeliaikaOnRajattu;
            JoukkueKilpailu.RankkareidenMaara = this.RankkareidenMaara;
            JoukkueKilpailu.Sijoittaminen = KaisaKaavio.Sijoittaminen.EiSijoittamista;
            JoukkueKilpailu.SijoitustenMaaraytyminen = this.SijoitustenMaaraytyminen;
            JoukkueKilpailu.TavoitePistemaara = 3; // Oikeasti 2 riittää, mutta tämä estää pelin "päättymisen" 2-0 tilanteessa, sekä pistemäärän rajoittamisen kahteen
            JoukkueKilpailu.Yksipaivainen = this.Yksipaivainen;
            JoukkueKilpailu.Loki = this.Loki;

            JoukkueKilpailu.Sali = this.Sali;

            JoukkueKilpailu.PeliPaikat.Clear();
            foreach (var p in this.PeliPaikat)
            {
                JoukkueKilpailu.PeliPaikat.Add(p);
            }

            JoukkueKilpailu.Osallistujat.Clear();

            foreach (var o in this.Osallistujat)
            {
                var joukkue = JoukkueKilpailu.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, o.Joukkue));
                if (joukkue == null)
                {
                    int id = -1;
                    Int32.TryParse(o.JoukkueId, out id);

                    joukkue = new Pelaaja()
                    {
                        Nimi = o.Joukkue,
                        Id = id,
                        OsMaksu = this.OsallistumisMaksu,
                    };

                    JoukkueKilpailu.Osallistujat.Add(joukkue);
                }

                if (string.IsNullOrEmpty(joukkue.PeliPaikka) && !string.IsNullOrEmpty(o.PeliPaikka))
                {
                    joukkue.PeliPaikka = o.PeliPaikka;
                }
            }

            if (JoukkueKilpailu.Pelit.Any())
            {
                foreach (var joukkuePeli in JoukkueKilpailu.Pelit)
                {
                    joukkuePeli.PropertyChanged -= joukkuePeli_PropertyChanged;
                }

                JoukkueKilpailu.Pelit.Clear();
            }

            foreach (var peli in this.Pelit)
            {
                int peliNumero = 1;

                var p = JoukkueKilpailu.Pelit.FirstOrDefault(x => 
                    string.Equals(x.Pelaaja1, peli.Joukkue1) &&
                    string.Equals(x.Pelaaja2, peli.Joukkue2) &&
                    x.KierrosPelaaja1 == peli.KierrosPelaaja1 &&
                    x.KierrosPelaaja2 == peli.KierrosPelaaja2);

                if (p == null)
                {
                    Peli joukkuePeli = new Peli() 
                    {
                        Kilpailu = JoukkueKilpailu,
                        Paikka = peli.Paikka,
                        PeliNumero = peliNumero++,
                        PelaajaId1 = JoukkueKilpailu.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, peli.Joukkue1)).Id.ToString(),
                        PelaajaId2 = JoukkueKilpailu.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, peli.Joukkue2)).Id.ToString(),
                        KierrosPelaaja1 = peli.KierrosPelaaja1,
                        KierrosPelaaja2 = peli.KierrosPelaaja2,
                        Kierros = peli.Kierros,
                    };

                    joukkuePeli.PropertyChanged += joukkuePeli_PropertyChanged;
                    this.JoukkueKilpailu.Pelit.Add(joukkuePeli);
                }
            }

            PaivitaPelitJoukkueKisaan();
        }

        private void joukkuePeli_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "Tulos"))
            {
                if (this.JoukkueKilpailu != null)
                {
                    if (this.JoukkueKilpailu.Voittaja() != null)
                    {
                        this.KilpailuPaattyiJuuri = true;
                    }
                }
            }
        }

        public void PaivitaPelitJoukkueKisaan()
        {
            //JoukkueKilpailu.Pelit.Clear();

            int i = 1;

            foreach (var p in JoukkueKilpailu.Pelit)
            {
                p.PeliNumero = i++;

                //if (p.Tilanne != PelinTilanne.Pelattu)
                {
                    int p1 = 0;
                    int p2 = 0;

                    bool kaynnissa = false;
                    bool paattunut = true;
                    bool valmiinaAlkamaan = true;
                    string alkoi = string.Empty;
                    string paattyi = string.Empty;

                    foreach (var peli in this.Pelit.Where(x =>
                        x.KierrosPelaaja1 == p.KierrosPelaaja1 &&
                        x.KierrosPelaaja2 == p.KierrosPelaaja2 &&
                        string.Equals(x.Joukkue1, p.Pelaaja1) &&
                        string.Equals(x.Joukkue2, p.Pelaaja2)))
                    {
                        if (!string.IsNullOrEmpty(peli.Alkoi))
                        {
                            if (string.IsNullOrEmpty(alkoi) || alkoi.CompareTo(peli.Alkoi) > 0)
                            {
                                alkoi = peli.Alkoi;
                            }
                        }

                        if (!string.IsNullOrEmpty(peli.Paattyi))
                        {
                            if (string.IsNullOrEmpty(paattyi) || paattyi.CompareTo(peli.Paattyi) < 0)
                            {
                                paattyi = peli.Paattyi;
                            }
                        }

                        if (peli.Tilanne == PelinTilanne.Pelattu)
                        {
                            if (peli.Tulos == PelinTulos.Pelaaja1Voitti)
                            {
                                p1++;
                            }
                            else if (peli.Tulos == PelinTulos.Pelaaja2Voitti)
                            {
                                p2++;
                            }
                        }
                        else
                        {
                            if (peli.Tilanne == PelinTilanne.Kaynnissa)
                            {
                                kaynnissa = true;
                            }
                            else if (peli.Tilanne == PelinTilanne.ValmiinaAlkamaan)
                            {
                            }
                            else
                            {
                                valmiinaAlkamaan = false;
                            }

                            paattunut = false;
                        }
                    }

                    string pisteet1 = p1.ToString();
                    string pisteet2 = p2.ToString();
                    PelinTilanne tilanne = PelinTilanne.Tyhja;
                    //string alkoi = string.Empty;
                    //string paattyi = string.Empty;

                    if (paattunut)
                    {
                        pisteet1 = p1.ToString();
                        pisteet2 = p2.ToString();

                        if (p1 > p2)
                        {
                            pisteet1 += "v";
                        }
                        else if (p2 > p1)
                        {
                            pisteet2 += "v";
                        }

                        tilanne = PelinTilanne.Pelattu;
                        p.Alkoi = alkoi;
                        p.Paattyi = paattyi;
                    }
                    else if (kaynnissa)
                    {
                        pisteet1 = p1.ToString();
                        pisteet2 = p2.ToString();
                        tilanne = PelinTilanne.Kaynnissa;
                        p.Alkoi = alkoi;
                        p.Paattyi = string.Empty;
                    }
                    else if (valmiinaAlkamaan)
                    {
                        pisteet1 = string.Empty;
                        pisteet2 = string.Empty;
                        tilanne = PelinTilanne.ValmiinaAlkamaan;
                        p.Alkoi = string.Empty;
                        p.Paattyi = string.Empty;
                    }
                    else
                    {
                        pisteet1 = string.Empty;
                        pisteet2 = string.Empty;
                        tilanne = PelinTilanne.Tyhja;
                        p.Alkoi = string.Empty;
                        p.Paattyi = string.Empty;
                    }

                    p.Tilanne = tilanne;
                    p.Pisteet1 = pisteet1;
                    p.Pisteet2 = pisteet2;
                }
            }
        }

        public void PaivitaPelitJoukkueKisasta()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            foreach (var p in JoukkueKilpailu.Pelit)
            {
                var peli = this.Pelit.FirstOrDefault(x => 
                    x.KierrosPelaaja1 == p.KierrosPelaaja1 &&
                    x.KierrosPelaaja2 == p.KierrosPelaaja2 &&
                    string.Equals(x.Joukkue1, p.Pelaaja1) &&
                    string.Equals(x.Joukkue2, p.Pelaaja2));

                if (peli == null)
                {
                    List<Pelaaja> pelaajat1 = new List<Pelaaja>();
                    pelaajat1.AddRange(this.Osallistujat.Where(x => string.Equals(x.Joukkue, p.Pelaaja1)));

                    List<Pelaaja> pelaajat2 = new List<Pelaaja>();
                    pelaajat2.AddRange(this.Osallistujat.Where(x => string.Equals(x.Joukkue, p.Pelaaja2)));

                    if (pelaajat1.Count < 3 || pelaajat2.Count < 3)
                    {
                        if (this.Loki != null)
                        {
                            this.Loki.Kirjoita("Bugi!!! Joukkueessa liian vähän pelaajia", null, true);
                        }

                        continue;
                    }

                    for (int i = 0; i < 3; ++i)
                    {
                        Pelaaja pelaaja1 = pelaajat1.ElementAt(r.Next(pelaajat1.Count));
                        pelaajat1.Remove(pelaaja1);

                        Pelaaja pelaaja2 = pelaajat2.ElementAt(r.Next(pelaajat2.Count));
                        pelaajat2.Remove(pelaaja2);

                        Peli osaottelu = new Peli() 
                        {
                            PelaajaId1 = pelaaja1.Id.ToString(),
                            PelaajaId2 = pelaaja2.Id.ToString(),
                            KierrosPelaaja1 = p.KierrosPelaaja1,
                            KierrosPelaaja2 = p.KierrosPelaaja2,
                            Kierros = p.Kierros,
                            Kilpailu = this,
                            Paikka = p.Paikka,
                            PeliNumero = p.PeliNumero,
                        };

                        LisaaPeli(osaottelu);
                    }
                }
            }
        }

        public int TappiotPelaajalleEnnenPelia(int id, Peli p)
        {
            if (this.JoukkueKilpailu != null)
            {
                var pelaaja = this.Osallistujat.FirstOrDefault(x => x.Id == id);
                if (pelaaja != null && !string.IsNullOrEmpty(pelaaja.Joukkue))
                {
                    var joukkue = this.JoukkueKilpailu.Osallistujat.FirstOrDefault(x => string.Equals(x.Nimi, pelaaja.Joukkue));
                    if (joukkue != null)
                    {
                        var p1 = this.Osallistujat.FirstOrDefault(x => x.Id == p.Id1);
                        var p2 = this.Osallistujat.FirstOrDefault(x => x.Id == p.Id2);
                        var joukkuePeli = this.JoukkueKilpailu.Pelit.FirstOrDefault(x => 
                            x.KierrosPelaaja1 == p.KierrosPelaaja1 &&
                            x.KierrosPelaaja2 == p.KierrosPelaaja2 &&
                            string.Equals(x.Pelaaja1, p1.Joukkue) &&
                            string.Equals(x.Pelaaja2, p2.Joukkue));

                        if (joukkuePeli != null)
                        {
                            return this.JoukkueKilpailu.TappiotPelaajalleEnnenPelia(joukkue.Id, joukkuePeli);
                        }
                    }
                }
            }

            int tappiot = 0;

            foreach (var peli in this.Pelit.Where(x => x.PeliNumero <= p.PeliNumero))
            {
                tappiot += peli.Tappiot(id);
            }

            return tappiot;
        }
    }
}

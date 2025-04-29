using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Peli
        : NotifyPropertyChanged
    {
        [XmlIgnore]
        public Kilpailu Kilpailu { get; set; }

        [XmlAttribute]
        public int Kierros { get; set; }

        [XmlIgnore]
        public string KierrosTeksti 
        { 
            get 
            {
                return OnPudotusPeli() ? string.Format("{0} (CUP)", Kierros) : Kierros.ToString();
            }    
        }

        [XmlAttribute]
        public int KierrosPelaaja1 { get; set; }

        [XmlAttribute]
        public int KierrosPelaaja2 { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string LuovutusPelaaja1 { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string LuovutusPelaaja2 { get; set; }

        [XmlIgnore]
        public int PeliNumero { get; set; }

        [XmlIgnore]
        public int JoukkuePeliNumero { get; set; }

        private string pelaajaId1 = string.Empty;

        [XmlAttribute]
        public string PelaajaId1 
        {
            get { return this.pelaajaId1; }
            set 
            {
                if (!string.Equals(this.pelaajaId1, value))
                {
                    this.pelaajaId1 = value;
                    RaisePropertyChanged("PelaajaId1");
                    RaisePropertyChanged("Pelaaja1");
                    RaisePropertyChanged("Id1");
                }
            }
        }

        private string pelaajaId2 = string.Empty;

        [XmlAttribute]
        public string PelaajaId2 
        {
            get { return this.pelaajaId2; } 
            set 
            {
                if (!string.Equals(this.pelaajaId2, value))
                {
                    this.pelaajaId2 = value;
                    RaisePropertyChanged("PelaajaId2");
                    RaisePropertyChanged("Pelaaja2");
                    RaisePropertyChanged("Id2");
                }
            }
        }

        private string lyontivuoroja = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Lyontivuoroja
        {
            get
            {
                return this.lyontivuoroja;
            }

            set
            {
                if (!string.Equals(this.lyontivuoroja, value))
                {
                    this.lyontivuoroja = value;
                    RaisePropertyChanged("Lyontivuoroja");
                    RaisePropertyChanged("LyontivuorojaInt");
                    RaisePropertyChanged("Keskiarvo1");
                    RaisePropertyChanged("KeskiarvoTeksti1");
                    RaisePropertyChanged("Keskiarvo2");
                    RaisePropertyChanged("KeskiarvoTeksti2");
                }
            }
        }

        [XmlIgnore]
        public int LyontivuorojaInt
        {
            get
            {
                int v = 0;

                if (this.Kilpailu != null && this.Tilanne == PelinTilanne.Pelattu)
                {
                    v = (int)this.Kilpailu.PeliAika;
                }

                if (!string.IsNullOrEmpty(this.lyontivuoroja))
                {
                    Int32.TryParse(this.lyontivuoroja, out v);
                }

                return v;
            }
        }

        [XmlIgnore]
        public float Keskiarvo1
        {
            get
            {
                int vuoroja = this.LyontivuorojaInt;
                if (vuoroja <= 0)
                {
                    return 0.0f;
                }

                bool v = false, h = false;
                int pisteita = Pisteet(this.Pisteet1, out v, out h);

                int tasoitus = 0;
                if (this.Kilpailu != null)
                {
                    var pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id1);
                    if (pelaaja != null)
                    {
                        tasoitus = pelaaja.TasoitusInt;
                    }
                }

                pisteita -= tasoitus;
                pisteita = Math.Max(0, pisteita);

                return ((float)pisteita) / ((float)vuoroja);
            }
        }

        [XmlIgnore]
        public string KeskiarvoTeksti1
        {
            get
            {
                return this.Keskiarvo1.ToString("0.00").Replace(',', '.');
            }
        }

        [XmlIgnore]
        public float Keskiarvo2
        {
            get
            {
                int vuoroja = this.LyontivuorojaInt;
                if (vuoroja <= 0)
                {
                    return 0.0f;
                }

                bool v = false, h = false;
                int pisteita = Pisteet(this.Pisteet2, out v, out h);

                int tasoitus = 0;
                if (this.Kilpailu != null)
                {
                    var pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id2);
                    if (pelaaja != null)
                    {
                        tasoitus = pelaaja.TasoitusInt;
                    }
                }

                pisteita -= tasoitus;
                pisteita = Math.Max(0, pisteita);

                return ((float)pisteita) / ((float)vuoroja);
            }
        }

        [XmlIgnore]
        public string KeskiarvoTeksti2
        {
            get
            {
                return this.Keskiarvo2.ToString("0.00").Replace(',', '.');
            }
        }

        private string pisteet1 = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Pisteet1
        {
            get { return this.pisteet1; }
            set
            {
                var p = RajaaPisteet(value);
                if (!string.Equals(this.pisteet1, p))
                {
                    this.pisteet1 = p;
                    RaisePropertyChanged("Pisteet1");

                    PaivitaTulos();

                    if (this.Tilanne == PelinTilanne.ValmiinaAlkamaan && !string.IsNullOrEmpty(this.pisteet1))
                    {
                        KaynnistaPeli(null, false);
                    }

                    if (this.Kilpailu != null && this.Kilpailu.Laji == Laji.Kara)
                    {
                        RaisePropertyChanged("Keskiarvo1");
                        RaisePropertyChanged("KeskiarvoTeksti1");
                    }
                }
            }
        }

        private string pisteet2 = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Pisteet2
        {
            get { return this.pisteet2; }
            set
            {
                var p = RajaaPisteet(value);
                if (!string.Equals(this.pisteet2, p))
                {
                    this.pisteet2 = p;
                    RaisePropertyChanged("Pisteet2");

                    PaivitaTulos();

                    if (this.Tilanne == PelinTilanne.ValmiinaAlkamaan && !string.IsNullOrEmpty(this.pisteet2))
                    {
                        KaynnistaPeli(null, false);
                    }

                    if (this.Kilpailu != null && this.Kilpailu.Laji == Laji.Kara)
                    {
                        RaisePropertyChanged("Keskiarvo2");
                        RaisePropertyChanged("KeskiarvoTeksti2");
                    }
                }
            }
        }

        [XmlIgnore]
        public string Pisteet1Tuloksissa
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Pisteet1) && !string.IsNullOrEmpty(this.LuovutusPelaaja1))
                {
                    return string.Format("{0} ({1})", this.Pisteet1, this.LuovutusPelaaja1);
                }
                else if (!string.IsNullOrEmpty(this.LuovutusPelaaja1))
                {
                    return this.LuovutusPelaaja1;
                }
                else
                {
                    return this.Pisteet1;
                }
            }
        }

        [XmlIgnore]
        public string Pisteet2Tuloksissa
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Pisteet2) && !string.IsNullOrEmpty(this.LuovutusPelaaja2))
                {
                    return string.Format("{0} ({1})", this.Pisteet2, this.LuovutusPelaaja2);
                }
                else if (!string.IsNullOrEmpty(this.LuovutusPelaaja2))
                {
                    return this.LuovutusPelaaja2;
                }
                else
                {
                    return this.Pisteet2;
                }
            }
        }

        private string poyta = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Poyta
        {
            get { return this.poyta; }
            set
            {
                if (!string.Equals(this.poyta, value))
                {
                    this.poyta = value;
                    RaisePropertyChanged("Poyta");

                    // Käynnistä peli kun pöytä valitaan, mikäli peli on valmiina alkamaan
                    if (!string.IsNullOrEmpty(this.poyta) && this.Tilanne == PelinTilanne.ValmiinaAlkamaan)
                    {
                        KaynnistaPeli(null, false);
                    }
                }
            }
        }

        [XmlAttribute]
        [DefaultValue("")]
        public string Paikka { get; set; }

        [XmlIgnore]
        public string PaikkaTeksti 
        {
            get 
            {
                if (!string.IsNullOrEmpty(this.Paikka))
                {
                    return this.Paikka;
                }

                if (this.Kilpailu != null && this.Kilpailu.Sali != null)
                {
                    return this.Kilpailu.Sali.LyhytNimi;
                }

                return string.Empty;
            }
        }

        private PelinTilanne tilanne = PelinTilanne.Tyhja;

        [XmlAttribute]
        public PelinTilanne Tilanne
        {
            get { return this.tilanne; }
            set
            {
                if (this.tilanne != value)
                {
                    this.tilanne = value;
                    RaisePropertyChanged("Tilanne");
                    RaisePropertyChanged("TilanneTeksti");

                    // Laita tilanteeksi 0 - 0 kun peli käynnistyy (mikäli pisteitä ei ole jo merkitty)
                    if (this.tilanne == PelinTilanne.Kaynnissa ||
                        this.tilanne == PelinTilanne.Pelattu)
                    {
                        if (string.IsNullOrEmpty(this.pisteet1))
                        {
                            this.Pisteet1 = "0";
                        }

                        if (string.IsNullOrEmpty(this.pisteet2))
                        {
                            this.Pisteet2 = "0";
                        }

                        if (string.IsNullOrEmpty(this.Alkoi))
                        {
                            PaivitaPelinAlkamisAika();
                        }
                    }

                    if (this.tilanne == PelinTilanne.Pelattu)
                    {
                        if (string.IsNullOrEmpty(this.Paattyi))
                        {
                            this.Paattyi = DateTime.Now.ToShortTimeString();
                        }
                    }
                }
            }
        }

        private void PaivitaPelinAlkamisAika()
        {
            this.Alkoi = DateTime.Now.ToShortTimeString();

            // Jos peli on kilpailun ekoja pelejä, laita alkamisajaksi kilpailun alkamisaika
            try
            {
                if (this.Kilpailu != null &&
                    !string.IsNullOrEmpty(this.Kilpailu.KellonAika) &&
                    !string.IsNullOrEmpty(this.poyta))
                {
                    bool ekapeliPoydalla = this.Kilpailu.Pelit.Count(x => string.Equals(x.Poyta, this.Poyta)) <= 1;

                    if (ekapeliPoydalla)
                    {
                        int aikaero = 0;
                        if (Tyypit.Aika.AikaeroMinuutteina(this.Kilpailu.KellonAika, this.Alkoi, out aikaero))
                        {
                            if (aikaero < -75 || aikaero > 75)
                            {
                                this.Alkoi = this.Kilpailu.KellonAika;
                            }
                        }
                    }
                }
            }
            catch
            { 
            }
        }

        private PelinTulos tulos = PelinTulos.EiTiedossa;

        [XmlIgnore]
        public PelinTulos Tulos
        {
            get { return this.tulos; }
            set 
            {
                if (this.tulos != value)
                {
                    // Tärkeä! Jos jo pelatun ottelun tulos muuttuu, täytyy kaavio arpoa uudelleen pelistä eteenpäin
                    if (this.tulos == PelinTulos.MolemmatHavisi ||
                        this.tulos == PelinTulos.Pelaaja1Voitti ||
                        this.tulos == PelinTulos.Pelaaja2Voitti)
                    {
                        if (this.Kilpailu != null)
                        {
                            this.Kilpailu.PelinTulosMuuttunutNumerolla = Math.Min(this.Kilpailu.PelinTulosMuuttunutNumerolla, this.PeliNumero);
                        }
                    }

                    this.tulos = value;

                    if (this.tulos == PelinTulos.MolemmatHavisi ||
                        this.tulos == PelinTulos.Pelaaja1Voitti ||
                        this.tulos == PelinTulos.Pelaaja2Voitti)
                    {
                        if (this.Kilpailu != null)
                        {
                            this.Kilpailu.HakuTarvitaan = true;
                        }
                    }

                    RaisePropertyChanged("Tulos");

                    if (this.Kilpailu != null)
                    {
                        this.Kilpailu.PaivitaKilpailuUi();
                    }
                }
            }
        }

        [XmlIgnore]
        public string VirheTuloksessa { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Alkoi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Paattyi { get; set; }

        [XmlIgnore]
        public string Pelaaja1 
        { 
            get 
            { 
                return this.Kilpailu == null ? this.pelaajaId1 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId1, true, this.Kierros); 
            } 
        }

        [XmlIgnore]
        public string Pelaaja2
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId2 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId2, true, this.Kierros);
            }
        }

        [XmlIgnore]
        public string Seura1
        {
            get
            {
                if (this.Kilpailu != null)
                {
                    try
                    {
                        return this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == this.Id1).Seura;
                    }
                    catch
                    { 
                    }
                }

                return string.Empty;
            }
        }

        [XmlIgnore]
        public string Seura2
        {
            get
            {
                if (this.Kilpailu != null)
                {
                    try
                    {
                        return this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == this.Id2).Seura;
                    }
                    catch
                    {
                    }
                }

                return string.Empty;
            }
        }

        [XmlIgnore]
        public string PelaajanNimi1
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId1 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId1, false, this.Kierros);
            }
        }

        [XmlIgnore]
        public string PelaajanNimi2
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId2 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId2, false, this.Kierros);
            }
        }

        [XmlIgnore]
        public string PelaajanLyhytNimi1
        {
            get
            {
                if (this.Kilpailu != null &&
                    (this.Kilpailu.KilpaSarja == KilpaSarja.Parikilpailu || this.Kilpailu.KilpaSarja == KilpaSarja.MixedDoubles))
                {
                    return PelaajanNimi1;
                }
                else
                {
                    return Tyypit.Nimi.LyhytNimi(PelaajanNimi1);
                }
            }
        }

        [XmlIgnore]
        public string PelaajanLyhytNimi2
        {
            get
            {
                if (this.Kilpailu != null &&
                    (this.Kilpailu.KilpaSarja == KilpaSarja.Parikilpailu || this.Kilpailu.KilpaSarja == KilpaSarja.MixedDoubles))
                {
                    return PelaajanNimi2;
                }
                else
                {
                    return Tyypit.Nimi.LyhytNimi(PelaajanNimi2);
                }
            }
        }

        [XmlIgnore]
        public string Joukkue1
        {
            get
            {
                if (this.Kilpailu != null)
                {
                    var pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id1);
                    if (pelaaja != null)
                    {
                        return pelaaja.Joukkue;
                    }
                }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public string Joukkue2
        {
            get
            {
                if (this.Kilpailu != null)
                {
                    var pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id2);
                    if (pelaaja != null)
                    {
                        return pelaaja.Joukkue;
                    }
                }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public int Id1 
        {
            get 
            {
                int id = -1;
                if (Int32.TryParse(this.pelaajaId1, out id))
                {
                    return id;
                }
                return -1;
            } 
        }

        [XmlIgnore]
        public int Id2
        {
            get
            {
                int id = -1;
                if (Int32.TryParse(this.pelaajaId2, out id))
                {
                    return id;
                }
                return -1;
            }
        }

        [XmlIgnore]
        public string Viiva { get { return "-"; } }

        [XmlIgnore]
        public string TilanneTeksti 
        {
            get 
            {
                switch (this.tilanne)
                {
                    case PelinTilanne.Tyhja: return string.Empty;
                    case PelinTilanne.EiVastustajaa: return string.Empty;
                    case PelinTilanne.ValmiinaAlkamaan: return "Käynnistä peli";
                    case PelinTilanne.AiempiPeliMenossa: return string.Empty;
                    case PelinTilanne.Kaynnissa: return string.IsNullOrEmpty(this.Poyta) ? "Käynnissä" : string.Format("Pöytä {0}", this.Poyta);
                    case PelinTilanne.Pelattu: return "Pelattu";
                    default: return string.Empty;
                }
            }
        }

        [XmlIgnore]
        public string LisaTietoa
        {
            get
            {
                if (Id1 >= 0 && Id2 < 0)
                {
                    return string.Empty; // Walkover peli
                }

                StringBuilder s = new StringBuilder();

                switch (this.Tilanne)
                {
                    case PelinTilanne.Pelattu: s.Append(string.Format("{0} - {1}", this.Alkoi, this.Paattyi)); break;
                    case PelinTilanne.Kaynnissa:
                        if (this.Kilpailu != null && string.Equals(this.Kilpailu.KellonAika, this.Alkoi))
                        {
                            s.Append(string.Format("{0}", this.Alkoi));
                        }
                        else
                        {
                            s.Append(string.Format("Alkoi {0}", this.Alkoi));
                        }
                        break;
                    default: break;
                }

                if (this.KierrosPelaaja1 < this.Kierros && this.KierrosPelaaja2 < this.Kierros)
                {
                    s.Append(string.Format(" (Molempien {0}. kierros)", this.KierrosPelaaja1));
                }
                else if (this.KierrosPelaaja1 < this.Kierros)
                {
                    s.Append(string.Format(" ({0} {1}. kierros)", PelaajanLyhytNimi1, this.KierrosPelaaja1));
                }
                else if (this.KierrosPelaaja2 < this.Kierros)
                {
                    s.Append(string.Format(" ({0} {1}. kierros)", PelaajanLyhytNimi2, this.KierrosPelaaja2));
                }

                if (!string.IsNullOrEmpty(this.PisinSarja1) && !string.IsNullOrEmpty(this.ToiseksiPisinSarja1))
                {
                    s.Append(string.Format(" ({0} sarjat {1}p ja {2}p)", PelaajanLyhytNimi1, this.PisinSarja1, this.ToiseksiPisinSarja1));
                }
                else if (!string.IsNullOrEmpty(this.PisinSarja1))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", PelaajanLyhytNimi1, this.PisinSarja1));
                }
                else if (!string.IsNullOrEmpty(this.ToiseksiPisinSarja1))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", PelaajanLyhytNimi1, this.ToiseksiPisinSarja1));
                }

                if (!string.IsNullOrEmpty(this.PisinSarja2) && !string.IsNullOrEmpty(this.ToiseksiPisinSarja2))
                {
                    s.Append(string.Format(" ({0} sarjat {1}p ja {2}p)", PelaajanLyhytNimi2, this.PisinSarja2, this.ToiseksiPisinSarja2));
                }
                else if (!string.IsNullOrEmpty(this.PisinSarja2))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", PelaajanLyhytNimi2, this.PisinSarja2));
                }
                else if (!string.IsNullOrEmpty(this.ToiseksiPisinSarja2))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", PelaajanLyhytNimi2, this.ToiseksiPisinSarja2));
                }

                return s.ToString();
            }
        }

        [XmlIgnore]
        public bool PaivitaRivinUlkoasu = false;

        public Peli()
        {
            Kierros = 0;
            PeliNumero = 0;
            JoukkuePeliNumero = 0;
            Alkoi = string.Empty;
            Paattyi = string.Empty;
            Kilpailu = null;
            Tulos = PelinTulos.EiTiedossa;
            VirheTuloksessa = string.Empty;
            PisinSarja1 = string.Empty;
            ToiseksiPisinSarja1 = string.Empty;
            PisinSarja2 = string.Empty;
            ToiseksiPisinSarja2 = string.Empty;
            Paikka = string.Empty;
        }

        /// <summary>
        /// Numero, jonka perusteella pelit lajitellaan "pelit" taulukossa
        /// </summary>
        [XmlIgnore]
        public int LajitteluNumero
        {
            get 
            {
                if (this.Kilpailu != null && this.Kilpailu.OnUseanPelipaikanKilpailu)
                {
                    return Kierros * 100000000 + this.Kilpailu.PelipaikanIndeksi(this.Paikka) * 10000000 + KierrosPelaaja1 * 1000000 + Id1 * 1000 + Id2;
                }
                else
                {
                    return Kierros * 100000000 + KierrosPelaaja1 * 1000000 + Id1 * 1000 + Id2;
                }
            }
        }

        [XmlAttribute]
        [DefaultValue("")]
        public string PisinSarja1 { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string ToiseksiPisinSarja1 { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string PisinSarja2 { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string ToiseksiPisinSarja2 { get; set; }

        public bool SisaltaaPelaajan(int id)
        {
            return Id1 == id || Id2 == id;
        }

        public bool SisaltaaPelaajat(int id1, int id2)
        {
            return SisaltaaPelaajan(id1) && SisaltaaPelaajan(id2);
        }

        public bool SisaltaaJommanKummanPelaajan(int id1, int id2)
        {
            return SisaltaaPelaajan(id1) || SisaltaaPelaajan(id2);
        }

        public bool SisaltaaJoukkueen(string joukkue)
        {
            return string.Equals(Joukkue1, joukkue) || string.Equals(Joukkue2, joukkue);
        }

        public bool SisaltaaJommanKummanJoukkueen(string joukkue1, string joukkue2)
        {
            return SisaltaaJoukkueen(joukkue1) || SisaltaaJoukkueen(joukkue2);
        }

        public bool Tyhja()
        {
            return Id1 < 0 || Id2 < 0;
        }

        public string Kuvaus()
        {
            return string.Format("[{0}] {1} - {2} ({3}, {4})", 
                this.Kierros,
                this.Pelaaja1,
                this.Pelaaja2,
                this.Tilanne,
                this.Tulos);
        }

        public Pelaaja Haviaja()
        {
            if (this.Tilanne == PelinTilanne.Pelattu)
            {
                if (this.tulos == PelinTulos.Pelaaja1Voitti)
                {
                    return this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id2);
                }
                else if (this.tulos == PelinTulos.Pelaaja2Voitti)
                {
                    return this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id1);
                }
            }

            return null;
        }

        public int Pisteet(int id)
        {
            bool a = false, b = false;

            if (this.tilanne == PelinTilanne.Pelattu && SisaltaaPelaajan(id))
            {
                if (id == Id1)
                {
                    return Pisteet(pisteet1, out a, out b);
                }
                else 
                {
                    return Pisteet(pisteet2, out a, out b);
                }
            }

            return 0;
        }

        public int Tappiot(int id)
        {
            if (this.tilanne == PelinTilanne.Pelattu && SisaltaaPelaajan(id))
            {
                int tappioPisteet = OnPudotusPeli() ? 2 : 1;

                bool voitti1 = false;
                bool voitti2 = false;
                bool havisi1 = false;
                bool havisi2 = false;
                int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
                int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

                if (voitti1)
                {
                    return id == Id2 ? tappioPisteet : 0;
                }

                if (voitti2)
                {
                    return id == Id1 ? tappioPisteet : 0;
                }

                if ((id == Id1) && havisi1)
                {
                    return tappioPisteet;
                }

                if ((id == Id2) && havisi2)
                {
                    return tappioPisteet;
                }

                if (id == Id1)
                {
                    return p1 < p2 ? tappioPisteet : 0;
                }
                else
                {
                    return p1 > p2 ? tappioPisteet : 0;
                }
            }

            return 0;
        }

        public bool Voitti(int id)
        {
            if (this.tilanne == PelinTilanne.Pelattu && SisaltaaPelaajan(id))
            {
                bool voitti1 = false;
                bool voitti2 = false;
                bool havisi1 = false;
                bool havisi2 = false;
                int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
                int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

                if (voitti1)
                {
                    return id == Id1;
                }

                if (voitti2)
                {
                    return id == Id2;
                }

                if (havisi1 && id == Id1)
                {
                    return false;
                }

                if (havisi2 && id == Id2)
                {
                    return false;
                }

                if (id == Id1)
                {
                    return p1 > p2;
                }
                else
                {
                    return p1 < p2;
                }
            }

            return false;
        }

        public bool Havisi(int id)
        {
            if (this.tilanne == PelinTilanne.Pelattu && SisaltaaPelaajan(id))
            {
                bool voitti1 = false;
                bool voitti2 = false;
                bool havisi1 = false;
                bool havisi2 = false;
                int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
                int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

                if (voitti1)
                {
                    return id == Id2;
                }

                if (voitti2)
                {
                    return id == Id1;
                }

                if (havisi1)
                {
                    return id == Id1;
                }

                if (havisi2)
                {
                    return id == Id2;
                }

                if (id == Id1)
                {
                    return p1 < p2;
                }
                else 
                {
                    return p1 > p2;
                }
            }

            return false;
        }

        public bool KaynnistaPeli(Asetukset asetukset, bool valitsePoytaAutomaattisesti)
        {
            if (this.Kilpailu != null && this.Tilanne == PelinTilanne.ValmiinaAlkamaan)
            {
                if (this.Kilpailu.Pelit.Any(x => 
                    (x != this) &&
                    (x.Tilanne == PelinTilanne.Kaynnissa) &&
                    (x.SisaltaaJommanKummanPelaajan(Id1, Id2))))
                {
                    return false;
                }

                // Tarkista että kaavion peleissä ei ole virheitä
                if (this.Kilpailu.Pelit.Any(x => x.Tulos == PelinTulos.Virheellinen))
                {
                    return false;
                }

                // Automaattinen pöydän valinta
                if (valitsePoytaAutomaattisesti && string.IsNullOrEmpty(this.Poyta))
                {
                    var vapaaPoyta = this.Kilpailu.VapaatPoydat(this, asetukset).FirstOrDefault();
                    if (vapaaPoyta != null)
                    {
                        this.Poyta = vapaaPoyta;
                    }
                }

                this.Tilanne = PelinTilanne.Kaynnissa;
                return true;
            }

            return false;
        }

        public bool OnSalilla(Sali sali)
        {
            if (sali == null)
            {
                return true;
            }

            if (string.IsNullOrEmpty(this.Paikka) && this.Kilpailu != null && this.Kilpailu.Sali == sali)
            {
                return true;
            }

            return string.Equals(this.Paikka, sali.LyhytNimi);
        }

        static public int Pisteet(string teksti, out bool voitti, out bool havisi)
        {
            if (string.IsNullOrEmpty(teksti))
            {
                voitti = false;
                havisi = false;
                return 0;
            }

            int pisteet = 0;

            StringBuilder s = new StringBuilder();

            if (teksti.StartsWith("-"))
            {
                s.Append('-');
                teksti = teksti.Substring(1);
            }

            foreach (var c in teksti)
            {
                if (Char.IsDigit(c))
                {
                    s.Append(c);
                }
            }

            Int32.TryParse(s.ToString(), out pisteet);

            voitti = teksti.Contains("v") || teksti.Contains("V");
            havisi = teksti.Contains("h") || teksti.Contains("H");

            return pisteet;
        }

        /// <summary>
        /// Tarkistaa että rivillä(pelissä) ei ole virheitä. Mikäli virhe löytyy, palautetaan virheen kuvaus
        /// </summary>
        public string TarkistaRivi()
        {
            if (Tilanne == PelinTilanne.Pelattu)
            {
                bool voitti1 = false;
                bool voitti2 = false;
                bool havisi1 = false;
                bool havisi2 = false;
                int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
                int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

                if (voitti1 && voitti2)
                {
                    return "v molemmissa pisteissä";
                }

                if (voitti1 && havisi1)
                {
                    return "v ja h samassa pistesarakkeessa";
                }

                if (voitti2 && havisi2)
                {
                    return "v ja h samassa pistesarakkeessa";
                }

                if (!voitti1 && !voitti2 && !havisi1 && !havisi2)
                {
                    if (p1 == p2)
                    {
                        return "Pisteet tasan. Merkitse v tai h voiton tai häviön merkiksi";
                    }
                }
            }

            return string.Empty;
        }

        public bool PisteidenPerusteellaValmis()
        {
            if (string.IsNullOrEmpty(this.pisteet1) ||
                string.IsNullOrEmpty(this.pisteet2))
            {
                return false;
            }

            bool voitti1 = false;
            bool voitti2 = false;
            bool havisi1 = false;
            bool havisi2 = false;
            int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
            int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

            if (voitti1 || voitti2 || havisi1 || havisi2)
            {
                return true;
            }

            if (p1 != p2 && (p1 >= Kilpailu.TavoitePistemaara || p2 >= Kilpailu.TavoitePistemaara))
            {
                return true;
            }

            return false;
        }

        public int TappiotPeliRivilla1()
        {
            if (this.Kilpailu != null)
            {
                int tappiot = this.Kilpailu.TappiotPelaajalleEnnenPelia(Id1, this);
                return Math.Min(2, tappiot);
            }
            return 0;
        }

        public int TappiotPeliRivilla2()
        {
            if (this.Kilpailu != null)
            {
                int tappiot = this.Kilpailu.TappiotPelaajalleEnnenPelia(Id2, this);
                return Math.Min(2, tappiot);
            }
            return 0;
        }

        private void PelaajaRtf(Pelaaja pelaaja, int tappiot, string pisteet, Tyypit.Teksti teksti, bool tulostaPisteet, bool tulostaSeura, bool tulostaTasuri, bool tulostaSijoitus)
        {
            if (pelaaja == null)
            {
                return;
            }

            string nimi = pelaaja.Nimi;

            if (!tulostaPisteet &&
                this.Kilpailu != null &&
                (this.Kilpailu.KilpaSarja == KilpaSarja.MixedDoubles ||
                this.Kilpailu.KilpaSarja == KilpaSarja.Parikilpailu))
            {
                nimi = pelaaja.PitkaNimi;
            }

            // Joukkuekilpailussa joukkueiden nimet menee paksuksi/punaseksi jne..., mutta pelaajat on normaalilla tekstillä
            if (this.Kilpailu != null && this.Kilpailu.KilpaSarja == KilpaSarja.Joukkuekilpailu)
            {
                teksti.NormaaliTeksti(nimi);
            }
            else
            {
                if (tappiot == 0)
                {
                    if (tulostaSijoitus && pelaaja.SijoitusNumero <= 24)
                    {
                        teksti.PaksuTeksti(string.Format("[{0}] ", pelaaja.SijoitusNumero));
                    }

                    teksti.PaksuTeksti(nimi);
                }
                else if (tappiot == 1)
                {
                    if (tulostaSijoitus && pelaaja.SijoitusNumero <= 24)
                    {
                        teksti.NormaaliTeksti(string.Format("[{0}] ", pelaaja.SijoitusNumero));
                    }

                    teksti.NormaaliTeksti(nimi);
                }
                else
                {
                    if (tulostaSijoitus && pelaaja.SijoitusNumero <= 24)
                    {
                        teksti.PunainenTeksti(string.Format("[{0}] ", pelaaja.SijoitusNumero));
                    }

                    teksti.PunainenTeksti(nimi);
                }

                if (tulostaSeura && !string.IsNullOrEmpty(pelaaja.Seura))
                {
                    teksti.NormaaliTeksti(" " + pelaaja.Seura);
                }
            }

            if (tulostaTasuri && !string.IsNullOrEmpty(pelaaja.Tasoitus))
            {
                if (pelaaja.Tasoitus.Contains('(') || pelaaja.Tasoitus.Contains('['))
                {
                    teksti.NormaaliTeksti(" " + pelaaja.Tasoitus);
                }
                else
                {
                    teksti.NormaaliTeksti(" (" + pelaaja.Tasoitus + ")");
                }
            }

            if (tulostaPisteet && !string.IsNullOrEmpty(pisteet))
            {
                teksti.NormaaliTeksti(" " + pisteet);
            }

            if (tulostaPisteet)
            {
                if (this.Kilpailu != null && this.Kilpailu.Laji == Laji.Kara && this.Tilanne == PelinTilanne.Pelattu)
                {
                    teksti.NormaaliTeksti(" ");

                    if (pelaaja.Id == Id1)
                    {
                        teksti.PieniVihreaTeksti(string.Format("[{0}]", KeskiarvoTeksti1));
                    }
                    else if (pelaaja.Id == Id2)
                    {
                        teksti.PieniVihreaTeksti(string.Format("[{0}]", KeskiarvoTeksti2));
                    }
                }
            }
        }

        private void Pelaaja1Rtf(Tyypit.Teksti teksti, bool tulostaPisteet, bool tulostaSeura, bool tulostaTasoitus, bool tulostaSijoitus)
        {
            if (this.Kilpailu == null || this.Id1 < 0)
            {
                return;
            }

            Pelaaja pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id1);
            if (pelaaja == null)
            {
                return;
            }

            int tappiot = this.Kilpailu.LaskeTappiotPelille(Id1, PeliNumero);
            PelaajaRtf(pelaaja, tappiot, Pisteet1Tuloksissa, teksti, tulostaPisteet, tulostaSeura, tulostaTasoitus, tulostaSijoitus);
        }

        private void Pelaaja2Rtf(Tyypit.Teksti teksti, bool tulostaPisteet, bool tulostaSeura, bool tulostaTasoitus, bool tulostaSijoitus)
        {
            if (this.Kilpailu == null || this.Id2 < 0)
            {
                return;
            }

            Pelaaja pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id2);
            if (pelaaja == null)
            {
                return;
            }

            int tappiot = this.Kilpailu.LaskeTappiotPelille(Id2, PeliNumero);
            PelaajaRtf(pelaaja, tappiot, Pisteet2Tuloksissa, teksti, tulostaPisteet, tulostaSeura, tulostaTasoitus, tulostaSijoitus);
        }

        public void RichTextKuvausAlkaviinPeleihin(Tyypit.Teksti teksti, string poyta, string aika)
        {
            Pelaaja1Rtf(teksti, false, true, false, true);
            teksti.NormaaliTeksti(" - ");
            Pelaaja2Rtf(teksti, false, true, false, true);

            if (!string.IsNullOrEmpty(poyta))
            {
                teksti.NormaaliTeksti(" ");
                teksti.PieniTeksti(string.Format(" (pöytä {0} kello {1})", poyta, aika));
            }

            teksti.RivinVaihto();
        }

        public void RichTextKuvaus(Sali sali, Tyypit.Teksti teksti)
        {
            if (this.Kilpailu == null)
            {
                return;
            }

            if (this.Tilanne == PelinTilanne.Kaynnissa)
            {
                teksti.PelinTilanneTeksti(this.Poyta);
                teksti.NormaaliTeksti(" ");
            }

            Pelaaja1Rtf(teksti, true, true, true, Kierros < 4);

            teksti.NormaaliTeksti(" - ");

            Pelaaja2Rtf(teksti, true, true, true, Kierros < 4);

            // Striimi ja tulostaululinkit
            if (this.Tilanne == PelinTilanne.Kaynnissa)
            {
                var poyta = sali.Poydat.FirstOrDefault(x => x.Numero.Equals(this.poyta, StringComparison.OrdinalIgnoreCase));
                if (poyta != null)
                {
                    if (!string.IsNullOrEmpty(poyta.StriimiLinkki))
                    {
                        teksti.NormaaliTeksti(" (");
                        teksti.Linkki(null, "striimi", poyta.StriimiLinkki);
                        teksti.NormaaliTeksti(")");
                    }

                    if (!string.IsNullOrEmpty(poyta.TulosLinkki))
                    {
                        teksti.NormaaliTeksti(" (");
                        teksti.Linkki(null, "tulostaulu", poyta.TulosLinkki);
                        teksti.NormaaliTeksti(")");
                    }
                }
            }

            List<string> nootit1 = new List<string>();
            List<string> nootit2 = new List<string>();

            if (this.Kilpailu != null && this.Kilpailu.KilpaSarja == KilpaSarja.Joukkuekilpailu)
            {
                // Joukkuekisassa pelaajan kierrosilmoitus on joukkueen kohdalla, ei yksittäisissä peleissä
            }
            else
            {
                if (KierrosPelaaja1 < Kierros)
                {
                    nootit1.Add(string.Format("{0}. kierros", KierrosPelaaja1));
                }

                if (KierrosPelaaja2 < Kierros)
                {
                    nootit2.Add(string.Format("{0}. kierros", KierrosPelaaja2));
                }
            }

            if (!string.IsNullOrEmpty(PisinSarja1) && !string.IsNullOrEmpty(ToiseksiPisinSarja1))
            {
                nootit1.Add(string.Format("{0}p ja {1}p sarjat", PisinSarja1, ToiseksiPisinSarja1));
            }
            else if (!string.IsNullOrEmpty(PisinSarja1))
            {
                nootit1.Add(string.Format("{0}p sarja", PisinSarja1));
            }

            if (!string.IsNullOrEmpty(PisinSarja2) && !string.IsNullOrEmpty(ToiseksiPisinSarja2))
            {
                nootit2.Add(string.Format("{0}p ja {1}p sarjat", PisinSarja2, ToiseksiPisinSarja2));
            }
            else if (!string.IsNullOrEmpty(PisinSarja2))
            {
                nootit2.Add(string.Format("{0}p sarja", PisinSarja2));
            }

            if (nootit1.Any())
            {
                string lyhytNimi1 = PelaajanLyhytNimi1;
                teksti.NormaaliTeksti(" ");
                teksti.PieniTeksti(string.Format(" ({0} {1})", lyhytNimi1, string.Join(", ", nootit1)));
            }

            if (nootit2.Any())
            {
                string lyhytNimi2 = PelaajanLyhytNimi2;
                teksti.NormaaliTeksti(" ");
                teksti.PieniTeksti(string.Format(" ({0} {1})", lyhytNimi2, string.Join(", ", nootit2)));
            }

            if (this.Kilpailu != null && this.Kilpailu.OnJoukkuekilpailunJoukkueKilpailu)
            {
                // Joukkuepelin kestoa ei ilmoiteta, vain yksittäisten pelien
            }
            else
            {
                if (this.Tilanne == PelinTilanne.Kaynnissa)
                {
                    if (!string.IsNullOrEmpty(this.Alkoi))
                    {
                        teksti.NormaaliTeksti(" ");
                        teksti.PieniTeksti(string.Format(" (alkoi {0})", this.Alkoi));
                    }

                }
                else if (this.Tilanne == PelinTilanne.Pelattu)
                {
                    if (!string.IsNullOrEmpty(this.Alkoi) && !string.IsNullOrEmpty(this.Paattyi))
                    {
                        int aikaero = 0;
                        if (Tyypit.Aika.AikaeroMinuutteina(this.Alkoi, this.Paattyi, out aikaero) && aikaero > 0)
                        {
                            teksti.NormaaliTeksti(" ");
                            teksti.PieniTeksti(string.Format(" ({0}min)", aikaero));
                        }
                    }
                }
            }

            teksti.RivinVaihto();
        }

        public bool OnPudotusPeli()
        {
            if (this.Kilpailu != null)
            {
                switch (this.Kilpailu.KaavioTyyppi)
                {
                    case KaavioTyyppi.Pudari2Kierros: return this.Kierros >= 2;
                    case KaavioTyyppi.Pudari3Kierros: return this.Kierros >= 3;
                    default: return false;
                }
            }

            return false;
        }

        public static PelinTulos LaskePelinTulosJaTilannePisteista(string pisteet1, string pisteet2, int tavoite, out PelinTilanne tilanne, out string virhe)
        {
            bool voitti1 = false;
            bool voitti2 = false;
            bool havisi1 = false;
            bool havisi2 = false;
            int p1 = Pisteet(pisteet1, out voitti1, out havisi1);
            int p2 = Pisteet(pisteet2, out voitti2, out havisi2);

            virhe = string.Empty;
            tilanne = PelinTilanne.Pelattu;

            if (voitti1 && voitti2)
            {
                return PelinTulos.Virheellinen;
            }

            else if (havisi1 && havisi2)
            {
                return PelinTulos.MolemmatHavisi;
            }

            else if (voitti1 && havisi1)
            {
                virhe = "Virheellinen tulosrivi: v ja h samassa pistesarakkeessa";
                return PelinTulos.Virheellinen;
            }

            else if (voitti2 && havisi2)
            {
                virhe = "Virheellinen tulosrivi: v ja h samassa pistesarakkeessa";
                return PelinTulos.Virheellinen;
            }

            else if (voitti1 || havisi2)
            {
                return PelinTulos.Pelaaja1Voitti;
            }

            else if (voitti2 || havisi1)
            {
                return PelinTulos.Pelaaja2Voitti;
            }

            else if (p1 >= tavoite && p2 >= tavoite)
            {
                virhe = string.Format("Molemmilla pelaajilla {0} pistettä tai enemmän. Merkitse v tai h toiselle pelaajalle voiton tai häviön merkiksi", tavoite);
                return PelinTulos.Virheellinen;
            }

            else if (p1 >= tavoite)
            {
                return PelinTulos.Pelaaja1Voitti;
            }

            else if (p2 >= tavoite)
            {
                return PelinTulos.Pelaaja2Voitti;
            }

            else if (p1 > 0 || p2 > 0)
            {
                tilanne = PelinTilanne.Kaynnissa;
                return PelinTulos.EiTiedossa;
            }
            else 
            {
                if (string.IsNullOrEmpty(pisteet1) && string.IsNullOrEmpty(pisteet2))
                {
                    tilanne = PelinTilanne.Tyhja;
                }
                else
                {
                    tilanne = PelinTilanne.Kaynnissa;
                }
                return PelinTulos.EiTiedossa;
            }
        }

        private string RajaaPisteet(string p)
        {
            if (this.Kilpailu != null && this.Kilpailu.Laji == Laji.Kaisa)
            {
                bool voitti = false;
                bool havisi = false;
                int pisteet = Pisteet(p, out voitti, out havisi);

                if (pisteet >= 0 && pisteet <= this.Kilpailu.TavoitePistemaara)
                {
                    return p;
                }

                if (pisteet < 0)
                {
                    if (voitti)
                    {
                        return "0v";
                    }
                    else if (havisi)
                    {
                        return "0h";
                    }
                    else
                    {
                        return "0";
                    }
                }

                if (pisteet > this.Kilpailu.TavoitePistemaara)
                {
                    if (voitti)
                    {
                        return this.Kilpailu.TavoitePistemaara.ToString() + "v";
                    }
                    else if (havisi)
                    {
                        return this.Kilpailu.TavoitePistemaara.ToString() + "h";
                    }
                    else
                    {
                        return this.Kilpailu.TavoitePistemaara.ToString();
                    }
                }
            }

            return p;
        }

        [XmlIgnore]
        public int Kesto
        {
            get
            {
                int kesto = 0;
                if (!string.IsNullOrEmpty(this.Alkoi) && !string.IsNullOrEmpty(this.Paattyi))
                {
                    Tyypit.Aika.AikaeroMinuutteina(this.Alkoi, this.Paattyi, out kesto);
                }
                return kesto;
            }
        }

        public void PaivitaTulos()
        {
            bool voitti1 = false;
            bool voitti2 = false;
            bool havisi1 = false;
            bool havisi2 = false;
            int p1 = Pisteet(this.pisteet1, out voitti1, out havisi1);
            int p2 = Pisteet(this.pisteet2, out voitti2, out havisi2);

            int tavoite = 60;
            if (this.Kilpailu != null)
            {
                tavoite = (int)this.Kilpailu.TavoitePistemaara;
            }

            if (voitti1 && voitti2)
            {
                this.Tulos = PelinTulos.Virheellinen;
                this.VirheTuloksessa = "Virheellinen tulosrivi: v molemmissa pisteissä";
            }

            else if (havisi1 && havisi2)
            {
                this.Tilanne = PelinTilanne.Pelattu;
                this.Tulos = PelinTulos.MolemmatHavisi;
            }

            else if (voitti1 && havisi1)
            {
                this.Tulos = PelinTulos.Virheellinen;
                this.VirheTuloksessa = "Virheellinen tulosrivi: v ja h samassa pistesarakkeessa";
            }

            else if (voitti2 && havisi2)
            {
                this.Tulos = PelinTulos.Virheellinen;
                this.VirheTuloksessa = "Virheellinen tulosrivi: v ja h samassa pistesarakkeessa";
            }

            else if (voitti1 || havisi2)
            {
                this.Tilanne = PelinTilanne.Pelattu;
                this.Tulos = PelinTulos.Pelaaja1Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else if (voitti2 || havisi1)
            {
                this.Tilanne = PelinTilanne.Pelattu;
                this.Tulos = PelinTulos.Pelaaja2Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else if (p1 >= tavoite && p2 >= tavoite)
            {
                this.Tulos = PelinTulos.Virheellinen;

                if (this.Kilpailu != null)
                {
                    this.VirheTuloksessa = string.Format("Molemmilla pelaajilla {0} pistettä tai enemmän. Merkitse v tai h toiselle pelaajalle voiton tai häviön merkiksi", Kilpailu.TavoitePistemaara);
                }
                else
                {
                    this.VirheTuloksessa = string.Format("Molemmilla pelaajilla tavoitepistemäärä tai enemmän pisteitä. Merkitse v tai h toiselle pelaajalle voiton tai häviön merkiksi");
                }
            }

            else if (p1 >= tavoite)
            {
                this.Tilanne = PelinTilanne.Pelattu;
                this.Tulos = PelinTulos.Pelaaja1Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else if (p2 >= tavoite)
            {
                this.Tilanne = PelinTilanne.Pelattu;
                this.Tulos = PelinTulos.Pelaaja2Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else
            {
                this.Tulos = PelinTulos.EiTiedossa;
                this.VirheTuloksessa = string.Empty;
            }

            // Automaattinen pelin tilanteen päivitys tuloksen perusteella
            if (this.Tulos == PelinTulos.Virheellinen && 
                (this.Tilanne == PelinTilanne.Pelattu || this.Tilanne == PelinTilanne.ValmiinaAlkamaan))
            {
                this.Tilanne = PelinTilanne.Kaynnissa;
            }

            if ((this.Tulos == PelinTulos.Pelaaja1Voitti || this.Tulos == PelinTulos.Pelaaja2Voitti) &&
                (this.Tilanne == PelinTilanne.ValmiinaAlkamaan || this.Tilanne == PelinTilanne.Kaynnissa))
            {
                this.Tilanne = PelinTilanne.Pelattu;

                // Päivitä karan lyöntivuorot automaattisesti jos mahdollista
                if (this.Kilpailu != null && this.Kilpailu.Laji == Laji.Kara)
                {
                    if (string.IsNullOrEmpty(this.Lyontivuoroja))
                    {
                        if (p1 < (int)this.Kilpailu.TavoitePistemaara &&
                            p2 < (int)this.Kilpailu.TavoitePistemaara)
                        {
                            this.Lyontivuoroja = ((int)this.Kilpailu.PeliAika).ToString();
                        }
                    }
                }
            }

            if (this.Tilanne == PelinTilanne.Pelattu && (this.tulos == PelinTulos.EiTiedossa || this.tulos == PelinTulos.Virheellinen))
            {
                this.Tilanne = PelinTilanne.Kaynnissa;
            }
        }
    }
}

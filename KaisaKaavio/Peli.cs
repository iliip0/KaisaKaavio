﻿using System;
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

        [XmlAttribute]
        public int KierrosPelaaja1 { get; set; }

        [XmlAttribute]
        public int KierrosPelaaja2 { get; set; }

        [XmlIgnore]
        public int PeliNumero { get; set; }

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

        private string pisteet1 = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Pisteet1
        {
            get { return this.pisteet1; }
            set
            {
                if (!string.Equals(this.pisteet1, value))
                {
                    this.pisteet1 = value;
                    RaisePropertyChanged("Pisteet1");

                    PaivitaTulos();

                    if (this.Tilanne == PelinTilanne.ValmiinaAlkamaan && !string.IsNullOrEmpty(this.pisteet1))
                    {
                        KaynnistaPeli(null, false);
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
                if (!string.Equals(this.pisteet2, value))
                {
                    this.pisteet2 = value;
                    RaisePropertyChanged("Pisteet2");

                    PaivitaTulos();

                    if (this.Tilanne == PelinTilanne.ValmiinaAlkamaan && !string.IsNullOrEmpty(this.pisteet2))
                    {
                        KaynnistaPeli(null, false);
                    }
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
                return this.Kilpailu == null ? this.pelaajaId1 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId1, true); 
            } 
        }

        [XmlIgnore]
        public string Pelaaja2
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId2 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId2, true);
            }
        }

        [XmlIgnore]
        public string PelaajanNimi1
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId1 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId1, false);
            }
        }

        [XmlIgnore]
        public string PelaajanNimi2
        {
            get
            {
                return this.Kilpailu == null ? this.pelaajaId2 : this.Kilpailu.PelaajanNimiKaaviossa(this.pelaajaId2, false);
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
                    s.Append(string.Format(" ({0} {1}. kierros)", this.Pelaaja1, this.KierrosPelaaja1));
                }
                else if (this.KierrosPelaaja2 < this.Kierros)
                {
                    s.Append(string.Format(" ({0} {1}. kierros)", this.Pelaaja2, this.KierrosPelaaja2));
                }

                if (!string.IsNullOrEmpty(this.PisinSarja1) && !string.IsNullOrEmpty(this.ToiseksiPisinSarja1))
                {
                    s.Append(string.Format(" ({0} sarjat {1}p ja {2}p)", this.Pelaaja1, this.PisinSarja1, this.ToiseksiPisinSarja1));
                }
                else if (!string.IsNullOrEmpty(this.PisinSarja1))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", this.Pelaaja1, this.PisinSarja1));
                }
                else if (!string.IsNullOrEmpty(this.ToiseksiPisinSarja1))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", this.Pelaaja1, this.ToiseksiPisinSarja1));
                }

                if (!string.IsNullOrEmpty(this.PisinSarja2) && !string.IsNullOrEmpty(this.ToiseksiPisinSarja2))
                {
                    s.Append(string.Format(" ({0} sarjat {1}p ja {2}p)", this.Pelaaja2, this.PisinSarja2, this.ToiseksiPisinSarja2));
                }
                else if (!string.IsNullOrEmpty(this.PisinSarja2))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", this.Pelaaja2, this.PisinSarja2));
                }
                else if (!string.IsNullOrEmpty(this.ToiseksiPisinSarja2))
                {
                    s.Append(string.Format(" ({0} {1}p sarja)", this.Pelaaja2, this.ToiseksiPisinSarja2));
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
            Alkoi = string.Empty;
            Paattyi = string.Empty;
            Kilpailu = null;
            Tulos = PelinTulos.EiTiedossa;
            VirheTuloksessa = string.Empty;
            PisinSarja1 = string.Empty;
            ToiseksiPisinSarja1 = string.Empty;
            PisinSarja2 = string.Empty;
            ToiseksiPisinSarja2 = string.Empty;
        }

        /// <summary>
        /// Numero, jonka perusteella pelit lajitellaan "pelit" taulukossa
        /// </summary>
        [XmlIgnore]
        public int LajitteluNumero
        {
            get 
            {
                return Kierros * 100000000 + KierrosPelaaja1 * 1000000 + Id1 * 1000 + Id2;
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
                    var vapaaPoyta = this.Kilpailu.VapaatPoydat(asetukset).FirstOrDefault();
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

        static public int Pisteet(string teksti, out bool voitti, out bool havisi)
        {
            if (string.IsNullOrEmpty(teksti))
            {
                voitti = false;
                havisi = false;
                return 0;
            }

            int pisteet = 0;

            string s = string.Empty;
            foreach (var c in teksti)
            {
                if (Char.IsDigit(c))
                {
                    s += c;
                }
            }

            Int32.TryParse(s, out pisteet);

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
                int tappiot = 0;
                
                foreach (var peli in Kilpailu.Pelit.Where(x => x.PeliNumero <= this.PeliNumero))
                {
                    tappiot += peli.Tappiot(Id1);
                }

                return Math.Min(2, tappiot);
            }
            return 0;
        }

        public int TappiotPeliRivilla2()
        {
            if (this.Kilpailu != null)
            {
                int tappiot = 0;

                foreach (var peli in Kilpailu.Pelit.Where(x => x.PeliNumero <= this.PeliNumero))
                {
                    tappiot += peli.Tappiot(Id2);
                }

                return Math.Min(2, tappiot);
            }
            return 0;
        }

        private void PelaajaRtf(Pelaaja pelaaja, int tappiot, string pisteet, StringBuilder rtf, StringBuilder sbil, bool tulostaPisteet, bool tulostaSeura)
        {
            if (pelaaja == null)
            {
                return;
            }

            if (tappiot == 0)
            {
                rtf.Append(@"\b " + pelaaja.Nimi + @" \b0");
                sbil.Append("[b]" + pelaaja.Nimi + "[/b]");
            }
            else if (tappiot == 1)
            {
                rtf.Append(pelaaja.Nimi);
                sbil.Append(pelaaja.Nimi);
            }
            else
            {
                rtf.Append(@"\b \cf2 " + pelaaja.Nimi + @" \cf1 \b0");
                sbil.Append("[b][color=#FF0000]" + pelaaja.Nimi + "[/color][/b]");
            }

            if (tulostaSeura && !string.IsNullOrEmpty(pelaaja.Seura))
            {
                rtf.Append(" " + pelaaja.Seura);
                sbil.Append(" " + pelaaja.Seura);
            }

            if (tulostaPisteet && !string.IsNullOrEmpty(pisteet))
            {
                rtf.Append(" " + pisteet);
                sbil.Append(" " + pisteet);
            }
        }

        private void Pelaaja1Rtf(StringBuilder rtf, StringBuilder sbil, bool tulostaPisteet, bool tulostaSeura)
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
            PelaajaRtf(pelaaja, tappiot, Pisteet1, rtf, sbil, tulostaPisteet, tulostaSeura);
        }

        private void Pelaaja2Rtf(StringBuilder rtf, StringBuilder sbil, bool tulostaPisteet, bool tulostaSeura)
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
            PelaajaRtf(pelaaja, tappiot, Pisteet2, rtf, sbil, tulostaPisteet, tulostaSeura);
        }

        public void RichTextKuvaus(Sali sali, StringBuilder rtf, StringBuilder sbil, bool alkavienPelienTekstiin)
        {
            if (this.Kilpailu == null)
            {
                return;
            }

            Pelaaja1Rtf(rtf, sbil, !alkavienPelienTekstiin, true);

            rtf.Append(" - ");
            sbil.Append(" - ");

            Pelaaja2Rtf(rtf, sbil, !alkavienPelienTekstiin, true);

            string lyhytNimi1 = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(PelaajanNimi1);
            string lyhytNimi2 = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(PelaajanNimi2);

            if (KierrosPelaaja1 < Kierros && KierrosPelaaja2 < Kierros)
            {
                rtf.Append(string.Format(" - Molempien {0}. kierros", KierrosPelaaja1));
                sbil.Append(string.Format("[size=85][color=#115099] - Molempien {0}. kierros[/color][/size]", KierrosPelaaja1));
            }
            else if (KierrosPelaaja1 < Kierros)
            {
                Pelaaja pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id1);
                if (pelaaja != null)
                {
                    rtf.Append(string.Format(" - {0} {1}. kierros", lyhytNimi1, KierrosPelaaja1));
                    sbil.Append(string.Format("[size=85][color=#115099] - {0} {1}. kierros[/color][/size]", lyhytNimi1, KierrosPelaaja1));
                }
            }
            else if (KierrosPelaaja2 < Kierros)
            {
                Pelaaja pelaaja = this.Kilpailu.Osallistujat.FirstOrDefault(x => x.Id == Id2);
                if (pelaaja != null)
                {
                    rtf.Append(string.Format(" - {0} {1}. kierros", lyhytNimi2, KierrosPelaaja2));
                    sbil.Append(string.Format("[size=85][color=#115099] - {0} {1}. kierros[/color][/size]", lyhytNimi2, KierrosPelaaja2));
                }
            }

            if (alkavienPelienTekstiin)
            {
                if (!string.IsNullOrEmpty(this.Poyta))
                {
                    rtf.Append(string.Format(" - Pöytä {0}", this.Poyta));
                    sbil.Append(string.Format(" - Pöytä {0}", this.Poyta));
                }

                if (!string.IsNullOrEmpty(this.Alkoi))
                {
                    rtf.Append(string.Format(" ({0})", this.Alkoi));
                    sbil.Append(string.Format(" ({0})", this.Alkoi));
                }
            }
            else if (this.Tilanne == PelinTilanne.Kaynnissa)
            {
                rtf.Append(@"\cf3 ");
                sbil.Append("[color=#0000FF]");

                if (string.IsNullOrEmpty(this.Poyta))
                {
                    rtf.Append(" - Käynnissä");
                    sbil.Append(" - Käynnissä");
                }
                else 
                {
                    rtf.Append(string.Format(" - Pöytä {0}", this.Poyta));
                    sbil.Append(string.Format(" - Pöytä {0}", this.Poyta));
                }

                if (!string.IsNullOrEmpty(this.Alkoi))
                {
                    rtf.Append(string.Format(" ({0})", this.Alkoi));
                    sbil.Append(string.Format(" ({0})", this.Alkoi));
                }

                rtf.Append(@". \cf1 ");
                sbil.Append(".[/color]");

                var poyta = sali.Poydat.FirstOrDefault(x => x.Numero.Equals(this.poyta, StringComparison.OrdinalIgnoreCase));
                if (poyta != null)
                {
                    if (!string.IsNullOrEmpty(poyta.StriimiLinkki))
                    {
                        rtf.Append(@" (\cf3 \ul striimi\ul0 \cf1)");
                        sbil.Append(string.Format(" ([url={0}]striimi[/url])", poyta.StriimiLinkki));
                    }

                    if (!string.IsNullOrEmpty(poyta.TulosLinkki))
                    {
                        rtf.Append(@"(\cf3 \ul tilanne\ul0 \cf1)");
                        sbil.Append(string.Format(" ([url={0}]tilanne[/url])", poyta.TulosLinkki));
                    }
                }
            }
            else if (this.Tilanne == PelinTilanne.Pelattu)
            {
                if (!string.IsNullOrEmpty(this.Alkoi) && !string.IsNullOrEmpty(this.Paattyi))
                {
                    int aikaero = 0;
                    if (Tyypit.Aika.AikaeroMinuutteina(this.Alkoi, this.Paattyi, out aikaero) && aikaero > 0)
                    {
                        rtf.Append(string.Format(@"    \cf4 ({0}min) \cf1", aikaero));
                        sbil.Append(string.Format("    [size=85][color=#115099]({0}min)[/color][/size]", aikaero));
                    }
                }
            }

            rtf.Append(@" \line ");
            sbil.Append(Environment.NewLine);
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
                this.Tulos = PelinTulos.Pelaaja1Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else if (voitti2 || havisi1)
            {
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
                this.Tulos = PelinTulos.Pelaaja1Voitti;
                this.VirheTuloksessa = string.Empty;
            }

            else if (p2 >= tavoite)
            {
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
            }

            if (this.Tilanne == PelinTilanne.Pelattu && (this.tulos == PelinTulos.EiTiedossa || this.tulos == PelinTulos.Virheellinen))
            {
                this.Tilanne = PelinTilanne.Kaynnissa;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Pelaaja
        : NotifyPropertyChanged
    {
        private string nimi = string.Empty;

        [XmlAttribute]
        public string Nimi 
        {
            get
            {
                return this.nimi;
            }

            set
            {
                if (!string.Equals(this.nimi, value))
                {
                    this.nimi = value;
                    RaisePropertyChanged("Nimi");
                }
            }
        }

        [XmlIgnore]
        public string LyhytNimi
        {
            get
            {
                return Tyypit.Nimi.LyhytNimi(this.Nimi);
            }
        }

        public string KeskipitkaNimi(Kilpailu kilpailu)
        {
            string lyhytNimi = LyhytNimi;

            if (kilpailu != null)
            {
                if (kilpailu.Osallistujat.Any(x => x != this && string.Equals(x.LyhytNimi, lyhytNimi)))
                {
                    return Tyypit.Nimi.NimiParikisassa(this.Nimi);
                }
            }

            return lyhytNimi;
        }

        [XmlIgnore]
        public string PitkaNimi
        {
            get
            {
                if (!string.IsNullOrEmpty(Pelaajan1Nimi) &&
                    !string.IsNullOrEmpty(Pelaajan1Seura) &&
                    !string.IsNullOrEmpty(Pelaajan2Nimi) &&
                    !string.IsNullOrEmpty(Pelaajan2Seura))
                {
                    return string.Format("{0} {1} & {2} {3}",
                        Pelaajan1Nimi,
                        Pelaajan1Seura,
                        Pelaajan2Nimi,
                        Pelaajan2Seura);
                }

                return Nimi;
            }
        }

        private string seura = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Seura 
        {
            get
            {
                return this.seura;
            }

            set
            {
                if (!string.Equals(this.seura, value))
                {
                    this.seura = value;
                    RaisePropertyChanged("Seura");
                }
            }
        }

        private string sijoitettu = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Sijoitettu
        {
            get
            {
                return this.sijoitettu;
            }

            set
            {
                if (!string.Equals(this.sijoitettu, value))
                {
                    this.sijoitettu = value;
                    RaisePropertyChanged("Sijoitettu");
                }
            }
        }

        [XmlIgnore]
        public int SijoitusNumero
        {
            get
            {
                int numero = Int32.MaxValue;
                if (!string.IsNullOrEmpty(this.sijoitettu))
                {
                    Int32.TryParse(this.sijoitettu, out numero);
                }
                return numero;
            }
        }

        private string tasoitus = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Tasoitus
        {
            get
            {
                return this.tasoitus;
            }

            set
            {
                if (!string.Equals(this.tasoitus, value))
                {
                    this.tasoitus = value;
                    RaisePropertyChanged("Tasoitus");
                    RaisePropertyChanged("TasoitusInt");
                }
            }
        }

        [XmlIgnore]
        public int TasoitusInt
        {
            get
            {
                int t = 0;
                if (!string.IsNullOrEmpty(this.tasoitus))
                {
                    Int32.TryParse(this.tasoitus, out t);
                }
                return t;
            }
        }

        /// <summary>
        /// Pelaajan joukkue, mikäli pelataan joukkuekilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string Joukkue { get; set; }

        /// <summary>
        /// Pelaajan joukkueen id, mikäli pelataan joukkuekilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string JoukkueId { get; set; }

        /// <summary>
        /// Peliparin 1. pelaajan nimi, mikäli pelataan parikilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string Pelaajan1Nimi { get; set; }

        /// <summary>
        /// Peliparin 1. pelaajan seura, mikäli pelataan parikilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string Pelaajan1Seura { get; set; }

        /// <summary>
        /// Peliparin 2. pelaajan nimi, mikäli pelataan parikilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string Pelaajan2Nimi { get; set; }

        /// <summary>
        /// Peliparin 2. pelaajan seura, mikäli pelataan parikilpailua
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string Pelaajan2Seura { get; set; }
        
        [XmlAttribute]
        [DefaultValue("")]
        public string KabikeMaksu { get; set; }

        [XmlIgnore]
        [DefaultValue("")]
        public string LisenssiMaksu { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string OsMaksu { get; set; }

        [XmlIgnore]
        [DefaultValue("")]
        public string SeuranJasenMaksu { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Veloitettu { get; set; }

        [XmlIgnore]
        public string IlmoittautumisNumero { get; set; }

        private string peliPaikka = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string PeliPaikka
        {
            get
            {
                return this.peliPaikka;
            }

            set
            {
                if (!string.Equals(this.peliPaikka, value))
                {
                    this.peliPaikka = value;
                    RaisePropertyChanged("PeliPaikka");
                }
            }
        }

        private int id = 0;

        [XmlAttribute]
        public int Id 
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// Tietorakenne, jolla pelit esitetään 'Kaavio'-näkymässä
        /// </summary>
        public class PeliTietue
        {
            public int Vastustaja = -1;
            public bool Pelattu = false;
            public bool Voitto = false;
            public int Pisteet = 0;
            public bool Pudari = false;
            public PelinTilanne Tilanne = PelinTilanne.Tyhja;
            public int Kierros = -1;
        }

        /// <summary>
        /// Tietorakenne, jolla pelaaja esitetään 'Tulokset'-näkymässä
        /// </summary>
        public class TulosTietue
        {
            public Pelaaja Pelaaja = null;
            public int Voitot = 0;
            public int Tappiot = 0;
            public int Pisteet = 0;
            public int JoukkuePisteet = 0; // Tehtyjen pisteiden määrä Kaisan joukkuekilpailussa
            public bool Pudotettu = false;
            public int Sijoitus = 0;
            public int SijoitusPisteet = 0;
            public int PudonnutKierroksella = 0;
            public bool SijoitusOnVarma = false;
            public int Lyontivuoroja = 0;
            public int Karoja = 0;

            public string Nimi 
            {
                get 
                {
                    return (Pelaaja != null && SijoitusOnVarma) ? Pelaaja.Nimi : string.Empty;
                }
            }
        }

        [XmlIgnore]
        public TulosTietue Sijoitus = null;

        [XmlIgnore]
        public int Voitot { get { return this.Sijoitus != null ? this.Sijoitus.Voitot : 0; } }

        [XmlIgnore]
        public int Tappiot { get { return this.Sijoitus != null ? this.Sijoitus.Tappiot : 0; } }

        [XmlIgnore]
        public int Pisteet { get { return this.Sijoitus != null ? this.Sijoitus.Pisteet : 0; } }

        [XmlIgnore]
        public List<PeliTietue> Pelit = new List<PeliTietue>();

        /// <summary>
        /// Pelaajan taso. Käytössä ainoastaan ohjelman testauksessa
        /// </summary>
        [XmlIgnore]
        public float Taso { get; set; }

        public Pelaaja()
        {
            Nimi = string.Empty;
            Seura = string.Empty;
            Sijoitettu = string.Empty;
            Tasoitus = string.Empty;
            KabikeMaksu = string.Empty;
            LisenssiMaksu = string.Empty;
            OsMaksu = string.Empty;
            SeuranJasenMaksu = string.Empty;
            Veloitettu = string.Empty;
            IlmoittautumisNumero = string.Empty;

            Taso = 1.0f;

            Joukkue = string.Empty;
            JoukkueId = string.Empty;

            Pelaajan1Nimi = string.Empty;
            Pelaajan1Seura = string.Empty;
            Pelaajan2Nimi = string.Empty;
            Pelaajan2Seura = string.Empty;

            Id = -1;

            this.Sijoitus = new TulosTietue() { Pelaaja = this };
        }
    }
}

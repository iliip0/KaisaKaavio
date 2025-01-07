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
            public bool Pudotettu = false;
            public int Sijoitus = 0;
            public int SijoitusPisteet = 0;
            public int PudonnutKierroksella = 0;
            public bool SijoitusOnVarma = false;

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
        public List<PeliTietue> Pelit = new List<PeliTietue>();

        public Pelaaja()
        {
            Nimi = string.Empty;
            Seura = string.Empty;
            Sijoitettu = string.Empty;
            KabikeMaksu = string.Empty;
            LisenssiMaksu = string.Empty;
            OsMaksu = string.Empty;
            SeuranJasenMaksu = string.Empty;
            Veloitettu = string.Empty;
            IlmoittautumisNumero = string.Empty;
            Id = -1;

            this.Sijoitus = new TulosTietue() { Pelaaja = this };
        }
    }
}

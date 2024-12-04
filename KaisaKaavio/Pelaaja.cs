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

        [XmlAttribute]
        [DefaultValue("")]
        public string Sijoitettu { get; set; }

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

        [XmlIgnore]
        public int Voitot { get; set; }

        [XmlIgnore]
        public int Tappiot { get; set; }

        [XmlIgnore]
        public int Pisteet { get; set; }

        [XmlIgnore]
        public bool Pudotettu { get; set; }

        [XmlIgnore]
        public int Sijoitus { get; set; }

        [XmlIgnore]
        public int SijoitusPisteet { get; set; }

        [XmlIgnore]
        public int PudonnutKierroksella { get; set; }

        public class PeliTietue
        {
            public int Vastustaja = -1;
            public bool Pelattu = false;
            public bool Voitto = false;
            public int Pisteet = 0;
        }

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
            Voitot = 0;
            Tappiot = 0;
            Pisteet = 0;
            Pudotettu = false;
            Sijoitus = 0;
            SijoitusPisteet = 0;
            PudonnutKierroksella = 0;
        }
    }
}

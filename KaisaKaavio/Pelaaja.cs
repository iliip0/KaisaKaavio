using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Pelaaja
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        [XmlAttribute]
        public string Seura { get; set; }

        [XmlAttribute]
        public string Sijoitettu { get; set; }

        [XmlAttribute]
        public string KabikeMaksu { get; set; }

        [XmlAttribute]
        public string LisenssiMaksu { get; set; }

        [XmlAttribute]
        public string OsMaksu { get; set; }

        [XmlAttribute]
        public string SeuranJasenMaksu { get; set; }

        [XmlAttribute]
        public string Veloitettu { get; set; }

        [XmlIgnore]
        public string IlmoittautumisNumero { get; set; }

        [XmlAttribute]
        public int Id { get; set; }

        [XmlIgnore]
        public int Voitot { get; set; }

        [XmlIgnore]
        public int Tappiot { get; set; }

        [XmlIgnore]
        public int Pisteet { get; set; }

        [XmlIgnore]
        public bool Pudotettu { get; set; }

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
        }
    }
}

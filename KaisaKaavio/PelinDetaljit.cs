using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class PelinDetaljit
    {
        [XmlAttribute]
        public int Kierros { get; set; }

        [XmlAttribute]
        public int Pelaaja1 { get; set; }

        [XmlAttribute]
        public int Pelaaja2 { get; set; }

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

        public PelinDetaljit()
        {
            Kierros = 0;
            Pelaaja1 = -1;
            Pelaaja2 = -1;
            PisinSarja1 = string.Empty;
            PisinSarja2 = string.Empty;
            ToiseksiPisinSarja1 = string.Empty;
            ToiseksiPisinSarja2 = string.Empty;
        }

        [XmlIgnore]
        public bool Tyhja
        {
            get
            {
                if ((Kierros < 1) || (Pelaaja1 < 0) || (Pelaaja2 < 0))
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(PisinSarja1) ||
                    !string.IsNullOrEmpty(PisinSarja2) ||
                    !string.IsNullOrEmpty(ToiseksiPisinSarja1) ||
                    !string.IsNullOrEmpty(ToiseksiPisinSarja2))
                {
                    return false;
                }

                return true;
            }
        }
    }
}

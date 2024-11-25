using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Linkki
    {
        [XmlAttribute]
        public string Teksti { get; set; }

        [XmlAttribute]
        public string Osoite { get; set; }

        public Linkki()
        {
            this.Teksti = string.Empty;
            this.Osoite = string.Empty;
        }
    }
}

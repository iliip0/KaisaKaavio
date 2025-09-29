using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KaisaKaavio.Tyypit
{
    public class SaliTietue
    {
        [XmlAttribute]
        public string Lyhenne { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Alias { get; set; } = string.Empty;

        [XmlAttribute]
        public int KaisaPoytia { get; set; } = 0;
    }
}

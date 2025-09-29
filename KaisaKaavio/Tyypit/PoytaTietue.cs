using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Tyypit
{
    public class PoytaTietue
    {
        public string SalinLyhenne { get; set; } = string.Empty;
        public string Nimi { get; set; } = string.Empty;
        [XmlAttribute]
        [DefaultValue(false)]
        public bool OnStriimi { get; set; } = false;
    }
}

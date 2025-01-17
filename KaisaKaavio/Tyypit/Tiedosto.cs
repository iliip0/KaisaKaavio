using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Tyypit
{
    public class Tiedosto
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Polku { get; set; }

        public override string ToString()
        {
            return Nimi;
        }

        public static void PoistaVanhimmatTiedostotKansiosta(string kansio, int sailytaN)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(kansio);
            }
            catch
            { 
            }
        }
    }
}

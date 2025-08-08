using System.ComponentModel;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Toimitsija
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string PuhelinNumero { get; set; }

        [XmlAttribute]
        public ToimitsijanRooli Rooli { get; set; }

        public Toimitsija()
        {
            this.Nimi = string.Empty;
            this.PuhelinNumero = string.Empty;
            this.Rooli = ToimitsijanRooli.Apulainen;
        }
    }
}

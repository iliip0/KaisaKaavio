using System.ComponentModel;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Toimitsija
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string PuhelinNumero { get; set; } = string.Empty;

        [XmlAttribute]
        public ToimitsijanRooli Rooli { get; set; } = ToimitsijanRooli.Apulainen;
    }
}

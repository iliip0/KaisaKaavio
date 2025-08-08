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

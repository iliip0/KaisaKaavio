using System.ComponentModel;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class PelaajaTietue
    {
        [XmlAttribute]
        public string Nimi { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Seura { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusPool { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusKara { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusPyramidi { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusHeyball { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusSnooker { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusKaisa { get; set; } = string.Empty;

        public PelaajaTietue()
        {
        }

        public string Tasoitus(Laji laji)
        {
            switch (laji)
            {
                case Laji.Heyball: return this.TasoitusHeyball;
                case Laji.Kaisa: return this.TasoitusKaisa;
                case Laji.Kara: return this.TasoitusKara;
                case Laji.Pool: return this.TasoitusPool;
                case Laji.Snooker: return this.TasoitusSnooker;
                case Laji.Pyramidi: return this.TasoitusPyramidi;
                default: return string.Empty;
            }
        }
    }
}

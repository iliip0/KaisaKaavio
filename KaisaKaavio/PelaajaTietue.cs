using System.ComponentModel;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class PelaajaTietue
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Seura { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusPool { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusKara { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusPyramidi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusHeyball { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusSnooker { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TasoitusKaisa { get; set; }

        public PelaajaTietue()
        {
            this.Nimi = string.Empty;
            this.Seura = string.Empty;
            this.TasoitusHeyball = string.Empty;
            this.TasoitusPool = string.Empty;
            this.TasoitusKaisa = string.Empty;
            this.TasoitusKara = string.Empty;
            this.TasoitusSnooker = string.Empty;
            this.TasoitusPyramidi = string.Empty;
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

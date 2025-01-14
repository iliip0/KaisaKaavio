using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Sali
    {
        [XmlAttribute]
        [DefaultValue(true)]
        public bool Kaytossa { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Osoite { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string PuhelinNumero { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Seura { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Lyhenne { get; set; }

        public BindingList<Linkki> Linkit { get; set; }

        public BindingList<Poyta> Poydat { get; set; }

        public BindingList<Toimitsija> Toimitsijat { get; set; }

        [XmlIgnore]
        public int Poytia { get { return this.Poydat.Count; } }

        public Sali()
        {
            this.Kaytossa = true;
            this.Nimi = string.Empty;
            this.Osoite = string.Empty;
            this.PuhelinNumero = string.Empty;
            this.Seura = string.Empty;
            this.Lyhenne = string.Empty;
            this.Linkit = new BindingList<Linkki>();
            this.Poydat = new BindingList<Poyta>();
            this.Toimitsijat = new BindingList<Toimitsija>();
        }
    }
}

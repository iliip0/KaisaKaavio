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
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Osoite { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string PuhelinNumero { get; set; }

        public BindingList<Linkki> Linkit { get; set; }

        public BindingList<Poyta> Poydat { get; set; }

        public Sali()
        {
            this.Nimi = string.Empty;
            this.Osoite = string.Empty;
            this.PuhelinNumero = string.Empty;
            this.Linkit = new BindingList<Linkki>();
            this.Poydat = new BindingList<Poyta>();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Poyta
    {
        [XmlAttribute]
        [DefaultValue(true)]
        public bool Kaytossa { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Numero { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string StriimiLinkki { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string TulosLinkki { get; set; }

        public Poyta()
        {
            this.Kaytossa = true;
            Numero = string.Empty;
            StriimiLinkki = string.Empty;
            TulosLinkki = string.Empty;
        }
    }
}

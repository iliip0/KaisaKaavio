﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PelaajaTietue()
        {
            this.Nimi = string.Empty;
            this.Seura = string.Empty;
        }
    }
}

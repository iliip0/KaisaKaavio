using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingPelaajaTietue
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        [XmlAttribute]
        public string Seura { get; set; }

        [XmlAttribute]
        public int Voitot { get; set; }

        [XmlAttribute]
        public int Pisteet { get; set; }

        [XmlAttribute]
        public int RankingPisteet { get; set; }

        public RankingPelaajaTietue()
        {
            this.Nimi = string.Empty;
            this.Seura = string.Empty;
            this.Voitot = 0;
            this.Pisteet = 0;
            this.RankingPisteet = 0;
        }
    }
}

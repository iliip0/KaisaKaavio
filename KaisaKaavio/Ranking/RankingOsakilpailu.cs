using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingOsakilpailu
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        public RankingOsakilpailu()
        {
            this.Nimi = string.Empty;
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }
    }
}

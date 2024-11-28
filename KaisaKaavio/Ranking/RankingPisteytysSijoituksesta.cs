using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingPisteytysSijoituksesta
    {
        [XmlAttribute]
        public RankingPisteetSijoituksesta Ehto { get; set; }

        [XmlAttribute]
        public int Pisteet { get; set; }

        public RankingPisteytysSijoituksesta()
        {
            this.Ehto = RankingPisteetSijoituksesta.Voittajalle;
            this.Pisteet = 1;
        }

        public RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta ehto, int pisteet)
        {
            this.Ehto = ehto;
            this.Pisteet = pisteet;
        }
    }
}

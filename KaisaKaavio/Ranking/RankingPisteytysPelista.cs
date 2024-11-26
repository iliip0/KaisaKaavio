using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingPisteytysPelista
    {
        [XmlAttribute]
        public RankingPisteetPelista Ehto { get; set; }

        [XmlAttribute]
        public int Pisteet { get; set; }

        public RankingPisteytysPelista()
        {
            this.Ehto = RankingPisteetPelista.JokaisestaVoitosta;
            this.Pisteet = 1;
        }
    }
}

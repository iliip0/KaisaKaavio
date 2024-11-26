using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Ranking
{
    public enum RankingSarjanPituus
    {
        Kuukausi,       // Kuukausittainen rankingsarja
        Vuodenaika,     // Kolmen kuukauden pituinen rankingsarja (Kevät, Kesä, Syksy, Talvi)
        Puolivuotta,    // Kuuden kuukauden pituinen rankingsarja (Kevät, Syksy)
        Vuosi           // Vuoden mittainen rankingsarja
    }
}

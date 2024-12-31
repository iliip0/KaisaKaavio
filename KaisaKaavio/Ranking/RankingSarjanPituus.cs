using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Ranking
{
    public enum RankingSarjanPituus
    {
        [Description("Kuukauden välein vaihtuva sarja")]
        Kuukausi,                       // (Tammikuu, Helmikuu, Maaliskuu, ...)

        [Description("Kolmen kuukauden välein vaihtuva sarja")]
        Vuodenaika,                     // (Tammikuu-Maaliskuu, Huhtikuu-Kesäkuu, Heinäkuu-Syyskuu, Elokuu-Joulukuu)

        [Description("Puolen vuoden välein vaihtuva sarja")]
        Puolivuotta,                    // (Tammikuu-Kesäkuu, Heinäkuu-Joulukuu)

        [Description("Vuoden mittainen sarja")]
        Vuosi                           // Vuoden mittainen rankingsarja
    }
}

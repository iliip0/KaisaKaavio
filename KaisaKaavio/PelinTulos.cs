using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum PelinTulos
    {
        EiTiedossa,         // Peli on kesken tai ei ole vielä alkanut
        Pelaaja1Voitti,
        Pelaaja2Voitti,
        MolemmatHavisi,
        Virheellinen        // Pelin tulostiedoissa on virhe
    }
}

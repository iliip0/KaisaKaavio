using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum PelinTilanne
    {
        Tyhja,              // Tyhja peli rivi
        EiVastustajaa,      // Toinen pelaaja on tiedossa, vastustaja ei
        EiVoidaHakea,
        AiempiPeliMenossa,  // Molemmat pelaajat on tiedossa mutta vähintään toisella on aiempi peli kesken
        ValmiinaAlkamaan,
        Kaynnissa,
        Pelattu
    }
}

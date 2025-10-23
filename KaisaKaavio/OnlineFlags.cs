using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    [Flags]
    public enum OnlineFlags
    {
        Nakyvyys1       = 0x01, // Näkyvyys pakattu bitteihin 0-1
        Nakyvyys2       = 0x02, // 
        OnlineIlmo1     = 0x04, // OnlineIlmoittautuminen pakattu bitteihin 2-3
        OnlineIlmo2     = 0x08, //
        KaksiArvontaa   = 0x10, // Arvonta tehdään kahdessa osassa. Yläkaavio voi aloittaa pelit vaikka ilmo on kesken
        SalliKommentti  = 0x20, // Ilmoittautumisen yhteyteen saa laittaa kommentin kisanvetäjälle
        SalliKaveriIlmo = 0x40, // Kaverin puolesta saa ilmoittautua
        // Yksi bitti vielä käyttämättä

        NakyvyysMask = Nakyvyys1 | Nakyvyys2,
        OnlineIlmoMask = OnlineIlmo1 | OnlineIlmo2,
    }
}

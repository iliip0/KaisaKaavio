using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    [Flags]
    public enum IlmoittautumisFlags
    {
        None = 0,

        KaksiArvontaa   = 0x01,     // Arvonta tehdään kahdessa osassa. Yläkaavio voi aloittaa pelit vaikka ilmo on kesken
        SalliKommentti  = 0x02,     // Ilmoittautumisen yhteyteen saa laittaa kommentin kisanvetäjälle
        SalliKaveriIlmo = 0x04,     // Kaverin puolesta saa ilmoittautua
        OnlineIlmo      = 0x08,     // Kisaan voi ilmoittautua KaisaKaavio.fi sivustolla
        VainLinkilla    = 0x10,     // Online ilmo vain linkin saaneille

        ViikkokisaDefault = OnlineIlmo | SalliKommentti | SalliKaveriIlmo,
        OpenDefault = OnlineIlmo
    }
}

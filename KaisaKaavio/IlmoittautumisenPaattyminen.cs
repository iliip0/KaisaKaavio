using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum IlmoittautumisenPaattyminen
    {
        [Description("Kaksiosainen arvonta")]
        KaksiOsainenArvonta,

        [Description("Kilpailun merkittynä alkamisaikana")]
        KunKilpailuAlkaa,

        [Description("5 minuuttia ennen kilpailun alkua")]
        ViisiMinuutiaEnnenKisanAlkua,

        [Description("15 minuuttia ennen kilpailun alkua")]
        VarttiaEnnenKisanAlkua,

        [Description("30 minuuttia ennen kilpailun alkua")]
        PuoliTuntiaEnnenKisanAlkua,

        [Description("Edellisenä iltana 21:00")]
        EdellisenaIltana,

        [Description("Edeltävänä torstaina 21:00")]
        EdellisenaTorstaina,

        [Description("Edeltävänä keskiviikkona 21:00")]
        EdellisenaKeskiviikkona
    }
}

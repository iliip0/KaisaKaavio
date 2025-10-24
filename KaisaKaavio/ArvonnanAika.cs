using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum ArvonnanAika
    {
        [Description("Tuntia ennen kilpailun alkamisaikaa")]
        TuntiaEnnen,

        [Description("45 minuuttia ennen kilpailun alkamisaikaa")]
        KolmeVarttiaEnnen,

        [Description("30 minuuttia ennen kilpailun alkamisaikaa")]
        PuoliTuntiaEnnen,

        [Description("20 minuuttia ennen kilpailun alkamisaikaa")]
        KaksiKymmentaMinuuttiaEnnen,

        [Description("15 minuuttia ennen kilpailun alkamisaikaa")]
        VarttiaEnnen,

        [Description("10 minuuttia ennen kilpailun alkamisaikaa")]
        KymmenenMinuuttiaEnnen,

        [Description("5 minuuttia ennen kilpailun alkamisaikaa")]
        ViisiMinuuttiaEnnen,

        [Description("Kilpailun merkittynä alkamisaikana")]
        KilpailunAlkaessa
    }
}

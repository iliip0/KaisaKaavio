using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum IlmoittautumisenAlkaminen
    {
        [Description("Nyt")]
        Nyt,

        [Description("Kisapäivän aamuna")]
        KisaPaivanAamuna,

        [Description("Edellisenä aamuna")]
        EdellisenaAamuna,

        [Description("Kilpailuviikon maanantaiaamuna")]
        EdellisenaMaanantaina
    }
}

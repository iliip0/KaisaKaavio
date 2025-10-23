using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum Nakyvyys
    {
        [Description("Kilpailu näkyy kaikille KaisaKaavio.fi sivustolla")]
        Kaikille,

        [Description("Kilpailu näkyy vain linkin saaneille")]
        VainLinkinKautta,

        [Description("Kilpailu näkyy vain sivuston ylläpitäjille")]
        VainYllapitajille,

        [Description("Kilpailua ei tallenneta KaisaKaavio.fi sivustolle")]
        Offline
    }
}

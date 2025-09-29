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
        [Description("Kilpailua ei tallenneta KaisaKaavio.fi sivustolle")]
        Offline,            // < Kilpailua ei lähetetä KaisaKaavio.fi sivustolle

        [Description("Kilpailu näkyy vain sivuston ylläpitäjille")]
        VainYllapitajille,  // < Kilpailu näkyy vain KaisaKaavio.fi sivuston ylläpitäjille

        [Description("Kilpailu näkyy vain linkin saaneille")]
        VainLinkinKautta,   // < Kilpailu näkyy vain linkin kautta. Ei näy KaisaKaavio.fi sivuston linkkien kautta

        [Description("Kilpailu näkyy kaikille KaisaKaavio.fi sivustolla")]
        Kaikille            // < Kilpailu näkyy kaikille KaisaKaavio.fi sivuston käyttäjille
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum Nakyvyys
    {
        Offline,            // < Kilpailua ei lähetetä KaisaKaavio.fi sivustolle
        VainYllapitajille,  // < Kilpailu näkyy vain KaisaKaavio.fi sivuston ylläpitäjille
        VainLinkinKautta,   // < Kilpailu näkyy vain linkin kautta. Ei näy KaisaKaavio.fi sivuston linkkien kautta
        Kaikille            // < Kilpailu näkyy kaikille KaisaKaavio.fi sivuston käyttäjille
    }
}

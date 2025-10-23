using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum OnlineIlmoittautuminen
    {
        [Description("Kaikille avoin ilmoittautuminen")]
        Kaikille,

        [Description("Ilmoittautuminen vain linkin saaneille")]
        VainLinkinSaaneille,

        [Description("Online ilmoittautuminen ei käytössä")]
        EiKaytossa
    }
}

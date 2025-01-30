using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    interface ITestiAjo
    {
        int OnnistuneitaTesteja { get; set; }
        int EpaonnistuneitaTesteja { get; set; }
        string VirheKansio { get; set; }

        bool Aja();
    }
}

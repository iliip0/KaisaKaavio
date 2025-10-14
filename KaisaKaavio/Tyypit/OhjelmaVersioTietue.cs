using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    public class OhjelmaVersioTietue
    {
        public string Versio { get; set; } = string.Empty;
        public string Tiiseri { get; set; } = string.Empty;
        public string Muutokset { get; set; } = string.Empty;
        public string Bugit { get; set; } = string.Empty;

        public int VersioNumero 
        {
            get
            {
                int n = 0;
                Int32.TryParse(Versio, out n);
                return n;
            }
        }
    }
}

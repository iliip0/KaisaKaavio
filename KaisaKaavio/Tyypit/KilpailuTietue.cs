using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    public class KilpailuTietue
    {
        public string Id { get; set; } = string.Empty;
        public string Nimi { get; set; } = string.Empty;
        public int SalasanaHash { get; set; } = 0;
        public string Pvm { get; set; } = string.Empty; 
    }
}

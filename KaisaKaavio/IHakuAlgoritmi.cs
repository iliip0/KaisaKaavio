using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public interface IHakuAlgoritmi
    {
        int Kierros { get; }
        Exception HakuVirhe { get; }
        bool PeruutaHaku { get; set; }
        bool HakuValmis { get; }
        bool UusiHakuTarvitaan { get; }
        bool AutomaattinenTestausMenossa { get; set; }
        List<HakuAlgoritmi.Pelaajat> UudetPelit { get; }
        void Hae();
    }
}

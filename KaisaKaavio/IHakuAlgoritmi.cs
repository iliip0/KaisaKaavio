using System;
using System.Collections.Generic;

namespace KaisaKaavio
{
    public interface IHakuAlgoritmi
    {
        int MaxKierros { get; set; }
        Exception HakuVirhe { get; }
        bool PeruutaHaku { get; set; }
        bool HakuValmis { get; }
        bool UusiHakuTarvitaan { get; }
        bool AutomaattinenTestausMenossa { get; set; }
        List<HakuAlgoritmi.Pelaajat> UudetPelit { get; }
        void Hae();
    }
}

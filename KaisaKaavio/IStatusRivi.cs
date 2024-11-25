using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public interface IStatusRivi
    {
        void PaivitaStatusRivi(string teksti, bool mittariNakyvissa, int mittari, int mittariMax);
    }
}

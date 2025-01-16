using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum SijoitustenMaaraytyminen
    {
        [Description("Voittajaa lukuunottamatta kaikki voittojen ja pisteiden perusteella")]
        VoittajaKierroksistaLoputPisteista,

        [Description("Finalisteja lukuunottamatta kaikki voittojen ja pisteiden perusteella")]
        KaksiParastaKierroksistaLoputPisteista,

        [Description("Sijat 1-3 kierrosten perusteella, loput voittojen ja pisteiden")]
        KolmeParastaKierroksistaLoputPisteista,
    }
}

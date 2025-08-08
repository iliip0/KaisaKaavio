using System.ComponentModel;

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

using System.ComponentModel;

namespace KaisaKaavio
{
    public enum KaavioTyyppi
    {
        [Description("Tuplakaavio loppuun asti")]
        TuplaKaavio,

        [Description("Pudotuspelit 2.kierroksesta alkaen")]
        Pudari2Kierros,

        [Description("Pudotuspelit 3.kierroksesta alkaen")]
        Pudari3Kierros,
    }
}

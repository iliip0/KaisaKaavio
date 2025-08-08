namespace KaisaKaavio
{
    public enum Sijoittaminen
    {
        /// <summary>
        /// Pelaajia ei sijoiteta kaavioon
        /// </summary>
        EiSijoittamista,
        
        /// <summary>
        /// 1-8 pelaajaa sijoitetaan kaavioon. Pelaajat ovat kaaviossa mahdollisimman etäällä toisistaan
        /// </summary>
        Sijoitetaan8Pelaajaa,

        /// <summary>
        /// 1-24 pelaajaa sijoitetaan kaavioon. Sijat 1-12 aloittavat pelit kolmannella kierroksella, sijat 13-24 toisella.
        /// </summary>
        Sijoitetaan24Pelaajaa
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public enum KilpailunTyyppi
    {
        /// <summary>
        /// Kolmas kierros pudari, ei sijoitettuja pelaajia. Mahdollisuus viikkokisaranking sarjaan
        /// </summary>
        Viikkokisa,
        
        /// <summary>
        /// Tuplakaavio loppuun asti, ei sijoitettuja pelaajia.
        /// </summary>
        AvoinKilpailu,

        /// <summary>
        /// Tuplakaavio loppuun asti, 1-8 sijoitettua pelaajaa. Mahdollisuus usean pelipaikan käyttöön
        /// </summary>
        KaisanRGKilpailu,

        /// <summary>
        /// Tuplakaavio loppuun asti, 1-24 sijoitettua pelaajaa. Mahdollisuus usean pelipaikan käyttöön
        /// </summary>
        KaisanSMKilpailu
    }
}

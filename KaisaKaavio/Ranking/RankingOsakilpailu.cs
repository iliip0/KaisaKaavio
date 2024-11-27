using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingOsakilpailu
    {
        [XmlAttribute]
        public string Nimi { get; set; }

        [XmlAttribute]
        public DateTime AlkamisAika { get; set; }

        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        public RankingOsakilpailu()
        {
            this.Nimi = string.Empty;
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }

        private StringBuilder tilanne = new StringBuilder();

        /// <summary>
        /// Rankingosakilpailun tilanneteksti rtf muodossa
        /// </summary>
        public string TilanneRtf
        {
            get
            {
                return tilanne.ToString();
            }
        }

        /// <summary>
        /// Rankingosakilpailun tilanneteksti sbil keskustelupalstamuodossa
        /// </summary>
        public string TilanneSbil
        {
            get
            {
                return tilanne.ToString();
            }
        }

        private void AnnaPisteitaVoitosta(Kilpailu kilpailu, Peli peli, int voittajaId, int voitettuId, RankingSarja sarja, RankingAsetukset asetukset)
        {
            var voittaja = this.Osallistujat.FirstOrDefault(x => x.Id == voittajaId);
            if (voittaja == null)
            {
                return;
            }

            int pisteet = 0;

            foreach (var a in asetukset.PistetytysPeleista.Where(x => x.Pisteet > 0))
            {
                if (a.Ehto == RankingPisteetPelista.JokaisestaVoitosta)
                {
                    pisteet += a.Pisteet;
                }
                else if (a.Ehto == RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari)
                {
                    if (peli.Kierros == 1 && kilpailu.KaavioTyyppi == KaavioTyyppi.Pudari2Kierros)
                    {
                        pisteet += a.Pisteet;
                    }
                }
                else if (a.Ehto == RankingPisteetPelista.RankingYkkosenVoitosta)
                {
                    if (sarja.PelaajanSijoitus(voitettuId) == 1)
                    {
                        pisteet += a.Pisteet;
                    }
                }
                else if (a.Ehto == RankingPisteetPelista.RankingKakkosenVoitosta)
                {
                    if (sarja.PelaajanSijoitus(voitettuId) == 2)
                    {
                        pisteet += a.Pisteet;
                    }
                }
                else if (a.Ehto == RankingPisteetPelista.RankingKolmosenVoitosta)
                {
                    if (sarja.PelaajanSijoitus(voitettuId) == 3)
                    {
                        pisteet += a.Pisteet;
                    }
                }
            }

            if (pisteet > 0)
            {
                voittaja.AnnaPisteita(pisteet, true);
            }
        }

        public void PaivitaKilpailu(Kilpailu kilpailu, RankingSarja sarja, RankingAsetukset asetukset)
        {
            foreach (var pelaaja in kilpailu.Osallistujat.Where(x => x.Id >= 0))
            {
                if (!Osallistujat.Any(x => x.Id == pelaaja.Id))
                { 
                    Osallistujat.Add(new RankingPelaajaTietue()
                    {
                        Id = pelaaja.Id,
                        Nimi = pelaaja.Nimi,
                        Seura = pelaaja.Seura
                    });
                }
            }

            foreach (var o in this.Osallistujat)
            {
                o.PoistaPisteet();

                // Pisteet osallistumisesta
                foreach (var a in asetukset.PisteytysSijoituksista.Where(x => x.Ehto == RankingPisteetSijoituksesta.KaikilleOsallistujille))
                {
                    if (a.Pisteet > 0)
                    {
                        o.AnnaPisteita(a.Pisteet, false);
                    }
                }
            }

            // Pisteet peleistä
            foreach (var p in kilpailu.Pelit)
            {
                if (p.Tilanne == PelinTilanne.Pelattu)
                {
                    if (p.Tulos == PelinTulos.Pelaaja1Voitti)
                    {
                        AnnaPisteitaVoitosta(kilpailu, p, p.Id1, p.Id2, sarja, asetukset); 
                    }
                    else if (p.Tulos == PelinTulos.Pelaaja2Voitti)
                    {
                        AnnaPisteitaVoitosta(kilpailu, p, p.Id2, p.Id1, sarja, asetukset);
                    }
                }
            }


            // Pisteet sijoituksista TODO
            if (asetukset.PisteytysSijoituksista.Count > 0)
            {
                /*
                int sijoitus = 1;
                var tulokset = kilpailu.Tulokset();
                foreach (var p in tulokset)
                { 
                }
                 */
            }

            // Päivitä teksti
            this.tilanne.Clear();
            foreach (var o in this.Osallistujat.OrderByDescending(x => x.RankingPisteet))
            {
                tilanne.AppendLine(string.Format("{0} {1}", o.Nimi, o.RankingPisteString));
            }
        }
    }
}

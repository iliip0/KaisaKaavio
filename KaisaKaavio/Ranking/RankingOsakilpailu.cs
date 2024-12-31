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

        [XmlAttribute]
        public int KilpailunNumero { get; set; }

        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        public RankingOsakilpailu()
        {
            this.Nimi = string.Empty;
            this.KilpailunNumero = 0;
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }

        private StringBuilder tilanne = new StringBuilder();

        /// <summary>
        /// Rankingosakilpailun tilanneteksti rtf muodossa
        /// </summary>
        [XmlIgnore]
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
        [XmlIgnore]
        public string TilanneSbil
        {
            get
            {
                return tilanne.ToString();
            }
        }

        private void AnnaPisteitaVoitosta(Kilpailu kilpailu, Peli peli, List<RankingPelaajaTietue> ranking, int voittajaId, int voitettuId, RankingSarja sarja, RankingAsetukset asetukset)
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
                    var p = ranking.FirstOrDefault(x => x.Id == voitettuId);
                    if (p != null && p.KumulatiivinenSijoitus == 1)
                    {
                        pisteet += a.Pisteet;
                    }
                }
                else if (a.Ehto == RankingPisteetPelista.RankingKakkosenVoitosta)
                {
                    var p = ranking.FirstOrDefault(x => x.Id == voitettuId);
                    if (p != null && p.KumulatiivinenSijoitus == 2)
                    {
                        pisteet += a.Pisteet;
                    }
                }
                else if (a.Ehto == RankingPisteetPelista.RankingKolmosenVoitosta)
                {
                    var p = ranking.FirstOrDefault(x => x.Id == voitettuId);
                    if (p != null && p.KumulatiivinenSijoitus == 3)
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
            var ranking = sarja.RankingEnnenOsakilpailua(kilpailu.AlkamisAikaDt);

            bool paattynyt = kilpailu.KilpailuOnPaattynyt;
            var tulokset = kilpailu.Tulokset();

            foreach (var pelaaja in kilpailu.Osallistujat.Where(x => x.Id >= 0))
            {
                if (!Osallistujat.Any(x => x.Id == pelaaja.Id))
                { 
                    Osallistujat.Add(new RankingPelaajaTietue()
                    {
                        Id = pelaaja.Id,
                        Nimi = pelaaja.Nimi,
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

                if (paattynyt)
                {
                    var tulos = tulokset.FirstOrDefault(x => x.Pelaaja.Id == o.Id);
                    if (tulos != null)
                    {
                        o.Sijoitus = tulos.Sijoitus;

                        // Pisteet sijoituksesta
                        foreach (var a in asetukset.PisteytysSijoituksista)
                        {
                            if ((tulos.Sijoitus == 1 && a.Ehto == RankingPisteetSijoituksesta.Voittajalle) ||
                                (tulos.Sijoitus == 2 && a.Ehto == RankingPisteetSijoituksesta.Kakkoselle) ||
                                (tulos.Sijoitus == 3 && a.Ehto == RankingPisteetSijoituksesta.Kolmoselle) ||
                                (tulos.Sijoitus == 4 && a.Ehto == RankingPisteetSijoituksesta.Neloselle) ||
                                (tulos.Sijoitus == 5 && a.Ehto == RankingPisteetSijoituksesta.Vitoselle) ||
                                (tulos.Sijoitus == 6 && a.Ehto == RankingPisteetSijoituksesta.Kutoselle) ||
                                (tulos.Sijoitus == 7 && a.Ehto == RankingPisteetSijoituksesta.Seiskalle) ||
                                (tulos.Sijoitus == 8 && a.Ehto == RankingPisteetSijoituksesta.Kasille) ||
                                (tulos.Sijoitus == 9 && a.Ehto == RankingPisteetSijoituksesta.Ysille) ||
                                (tulos.Sijoitus == 10 && a.Ehto == RankingPisteetSijoituksesta.Kymmenennelle))
                            {
                                if (a.Pisteet > 0)
                                {
                                    o.AnnaPisteita(a.Pisteet, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        o.Sijoitus = 0;
                    }
                }
                else
                {
                    o.Sijoitus = 0;
                }
            }

            // Pisteet peleistä
            foreach (var p in kilpailu.Pelit)
            {
                if (p.Tilanne == PelinTilanne.Pelattu)
                {
                    if (p.Tulos == PelinTulos.Pelaaja1Voitti)
                    {
                        AnnaPisteitaVoitosta(kilpailu, p, ranking, p.Id1, p.Id2, sarja, asetukset); 
                    }
                    else if (p.Tulos == PelinTulos.Pelaaja2Voitti)
                    {
                        AnnaPisteitaVoitosta(kilpailu, p, ranking, p.Id2, p.Id1, sarja, asetukset);
                    }
                }
            }

            PaivitaTilanneTeksti();
        }

        public void PaivitaTilanneTeksti()
        {
            this.tilanne.Clear();

            this.tilanne.AppendLine(this.Nimi);
            this.tilanne.AppendLine(string.Format("Osallistui {0} pelaajaa", this.Osallistujat.Count));
            this.tilanne.AppendLine();

            foreach (var o in this.Osallistujat.OrderBy(x => x.Sijoitus))
            {
                if (o.Sijoitus > 0)
                {
                    tilanne.AppendLine(string.Format("{0}. {1} ({2})", o.Sijoitus, o.Nimi, o.RankingPisteString));
                }
                else
                {
                    tilanne.AppendLine(string.Format("{0} ({1})", o.Nimi, o.RankingPisteString));
                }
            }
        }
    }
}

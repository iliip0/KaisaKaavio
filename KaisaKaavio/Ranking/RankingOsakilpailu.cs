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
        [DefaultValue("")]
        public string Id { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string KilpailunTarkistusSumma { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string AlkamisAika { get; set; }

        [XmlIgnore]
        public DateTime AlkamisAikaDt { get { return Tyypit.Aika.ParseDateTime(this.AlkamisAika); } }

        [XmlAttribute]
        public int KilpailunNumero { get; set; }

        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        public RankingOsakilpailu()
        {
            this.Id = string.Empty;
            this.Nimi = string.Empty;
            this.AlkamisAika = string.Empty;
            this.KilpailunTarkistusSumma = string.Empty;
            this.KilpailunNumero = 0;
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }

        private Tyypit.Teksti tilanne = new Tyypit.Teksti();

        /// <summary>
        /// Rankingosakilpailun tilanneteksti rtf muodossa
        /// </summary>
        [XmlIgnore]
        public string TilanneRtf
        {
            get
            {
                return tilanne.Rtf;
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
                return tilanne.Sbil;
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

        public void PaivitaKilpailu(Ranking rankingit, Kilpailu kilpailu, RankingSarja sarja, RankingAsetukset asetukset)
        {
            var kokonaisRanking = sarja.RankingEnnenOsakilpailua(kilpailu.AlkamisAikaDt);

            if (asetukset.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta &&
                sarja.OnSarjanEnsimmainenKilpailu(kilpailu))
            {
                kokonaisRanking = rankingit.AvaaEdellinenSarja(kilpailu).RankingEnnenOsakilpailua(kilpailu.AlkamisAikaDt);
            }

            bool paattynyt = kilpailu.KilpailuOnPaattynyt;
            var tulokset = kilpailu.Tulokset();

            foreach (var pelaaja in kilpailu.Osallistujat.Where(x => x.Id >= 0))
            {
                if (!Osallistujat.Any(x => x.Id == pelaaja.Id))
                { 
                    Osallistujat.Add(new RankingPelaajaTietue()
                    {
                        Id = pelaaja.Id,
                        Nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(pelaaja.Nimi),
                    });
                }
            }

            // Laitetaan rankinglistalle samat id kuin osallistujille
            foreach (var r in kokonaisRanking)
            {
                var p = Osallistujat.FirstOrDefault(x => Tyypit.Nimi.Equals(r.Nimi, x.Nimi));
                if (p != null)
                {
                    r.Id = p.Id;
                }
            }

            var mukanaOlevienRanking = kokonaisRanking
                .Where(x => Osallistujat.Any(y => y.Id == x.Id))
                .OrderBy(x => x.KumulatiivinenSijoitus);

            List<RankingPelaajaTietue> ranking = new List<RankingPelaajaTietue>();

            if (asetukset.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista)
            {
                ranking.AddRange(mukanaOlevienRanking);
            }
            else 
            {
                ranking.AddRange(kokonaisRanking);
            }

            List<int> sijoitukset = new List<int>();
            sijoitukset.AddRange(mukanaOlevienRanking
                .Select(x => x.KumulatiivinenSijoitus)
                .Distinct()
                .OrderBy(x => x));

            foreach (var m in mukanaOlevienRanking)
            {
                m.KumulatiivinenSijoitus = sijoitukset.IndexOf(m.KumulatiivinenSijoitus) + 1;
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
            this.tilanne = new Tyypit.Teksti();

            this.tilanne.Otsikko(this.Nimi);
            this.tilanne.NormaaliRivi(string.Format("Osallistui {0} pelaajaa", this.Osallistujat.Count));
            this.tilanne.RivinVaihto();
        
            foreach (var o in this.Osallistujat.OrderBy(x => x.Sijoitus))
            {
                if (o.Sijoitus > 0)
                {
                    tilanne.NormaaliTeksti(string.Format("{0}. {1} ", o.Sijoitus, o.Nimi));
                }
                else
                {
                    tilanne.NormaaliTeksti(o.Nimi + " ");
                }

                tilanne.PaksuTeksti(string.Format("{0}p ", o.RankingPisteet));
                tilanne.PieniTeksti(" (" + o.RankingPisteString + ")");
                tilanne.RivinVaihto();
            }
        }
    }
}

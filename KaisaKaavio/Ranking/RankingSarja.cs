using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingSarja
    {
        public int Vuosi { get; set; }
        public RankingSarjanPituus Pituus { get; set; }
        public int SarjanNumero { get; set; }
        public Laji Laji { get; set; }

        public BindingList<RankingOsakilpailu> Osakilpailut { get; set; }
        public BindingList<RankingPelaajaTietue> Osallistujat { get; set; }

        private bool muokattu = true;

        [XmlIgnore]
        public string Nimi
        {
            get
            {
                string laji = Enum.GetName(typeof(Laji), this.Laji);
                switch (this.Laji)
                {
                    case KaisaKaavio.Laji.Pool:
                    case KaisaKaavio.Laji.Heyball:
                    case KaisaKaavio.Laji.Snooker:
                        laji += "in";
                        break;

                    default:
                        laji += "n";
                        break;
                }

                string sarja = Tyypit.Aika.RankingSarjanNimi(this.Pituus, this.SarjanNumero);

                return string.Format("{0} viikkokisaranking {1} {2}", laji, sarja, this.Vuosi);
            }
        }

        [XmlIgnore]
        public string Kuvaus
        {
            get
            {
                if (this.Osakilpailut.Count > 0 && this.Osallistujat.Count > 0)
                {
                    return string.Format("{0} osakilpailua, {1} osallistujaa",
                        this.Osakilpailut.Count,
                        this.Osallistujat.Count);
                }
                else
                {
                    return "Valitussa rankingsarjassa ei ole kilpailuja";
                }
            }
        }

        public RankingSarja()
        {
            this.Vuosi = 0;
            this.Pituus = RankingSarjanPituus.Kuukausi;
            this.SarjanNumero = 0;
            this.Laji = Laji.Kaisa;
            this.Osakilpailut = new BindingList<RankingOsakilpailu>();
            this.Osallistujat = new BindingList<RankingPelaajaTietue>();
        }

        //private StringBuilder viimeisinTulos = new StringBuilder();
        //private StringBuilder rankingTilanneRtf = new StringBuilder();
        //private StringBuilder rankingTilanneSbil = new StringBuilder();

        private Tyypit.Teksti tilanneLyhyt = new Tyypit.Teksti();
        private Tyypit.Teksti tilannePitka = new Tyypit.Teksti();

        /// <summary>
        /// Rankingsarjan tilanneteksti rtf muodossa
        /// </summary>
        [XmlIgnore]
        public string TilanneRtf
        {
            get
            {
                return this.tilannePitka.Rtf;
            }
        }

        /// <summary>
        /// Rankingsarjan tilanneteksti SBiL keskustelupalstamuodossa
        /// </summary>
        [XmlIgnore]
        public string TilanneSbil
        {
            get
            {
                return this.tilannePitka.Sbil;
            }
        }

        [XmlIgnore]
        public string TilanneRtfLyhyt
        {
            get
            {
                return this.tilanneLyhyt.Rtf;
            }
        }

        [XmlIgnore]
        public string TilanneSbilLyhyt
        {
            get 
            {
                return this.tilanneLyhyt.Sbil;
            }
        }

        public bool SisaltaaOsakilpailun(RankingOsakilpailuTietue tietue)
        {
            var aika = Tyypit.Aika.ParseDateTime(tietue.Pvm);

            if (aika.Year != this.Vuosi)
            {
                return false;
            }

            if (tietue.Laji != this.Laji)
            {
                return false;
            }

            if (tietue.SarjanPituus != this.Pituus)
            {
                return false;
            }

            if (Tyypit.Aika.RankingSarjanNumeroAjasta(this.Pituus, aika) != this.SarjanNumero)
            {
                return false;
            }

            return true;
        }

        [XmlIgnore]
        public string Kansio
        {
            get
            {
                return Tyypit.Aika.RankingSarjaKansio(this.Vuosi);
            }
        }

        [XmlIgnore]
        public string Tiedosto
        {
            get
            {
                return Path.Combine(
                    this.Kansio,
                    string.Format("{0}_{1}.xml", 
                        Enum.GetName(typeof(Laji), this.Laji),
                        Tyypit.Aika.RankingSarjanTiedostonNimi(this.Pituus, this.SarjanNumero)));
            }
        }

        public void Avaa(Loki loki)
        {
            string tiedosto = this.Tiedosto;

            if (!File.Exists(tiedosto))
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Rankingsarja tiedostoa {0} ei löydy.", tiedosto));
                }

                return;
            }

            if (loki != null)
            {
                loki.Kirjoita(string.Format("Avataan rankingsarja tiedostosta {0}", tiedosto));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

            RankingSarja sarja = null;

            using (TextReader reader = new StreamReader(tiedosto))
            {
                sarja = (RankingSarja)serializer.Deserialize(reader);
                reader.Close();
            }

            if (sarja != null)
            {                
                if (sarja.Vuosi != 0)
                {
                    this.Vuosi = sarja.Vuosi;
                }

                this.Pituus = sarja.Pituus;
                this.SarjanNumero = sarja.SarjanNumero;
                this.Laji = sarja.Laji;

                this.muokattu = false;

                this.Osakilpailut.Clear();

                foreach (var o in sarja.Osakilpailut.OrderByDescending(x => x.AlkamisAikaDt))
                {
#if !DEBUG
                    if (!string.IsNullOrEmpty(o.Nimi) &&
                        o.Osallistujat.Count > 0)
                    {
#endif
                        o.PaivitaTilanneTeksti();
                        this.Osakilpailut.Add(o);
#if !DEBUG
                    }
#endif
                }
            }
        }

        public void Tallenna(Loki loki, bool tallennaVaikkaEiOlisiMuokattu)
        {
            try
            {
                if (!muokattu && !tallennaVaikkaEiOlisiMuokattu)
                {
                    if (loki != null)
                    {
                        loki.Kirjoita(string.Format("Rankingsarja {0} ei sisällä muutoksia. Ei tallenneta", this.Nimi));
                    }

                    return;
                }

                string tiedosto = this.Tiedosto;

                if (this.Osakilpailut.Count == 0 &&
                    this.Osallistujat.Count == 0)
                {
                    if (File.Exists(tiedosto))
                    {
                        if (loki != null)
                        {
                            loki.Kirjoita(string.Format("Rankingsarja {0} ei sisällä osakilpailuja eikä osallistujia. Poistetaan tiedosto {0}", this.Nimi, tiedosto));
                        }

                        File.Delete(tiedosto);
                    }

                    return;
                }

                Directory.CreateDirectory(this.Kansio);

                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Tallennetaan rankingsarja {0} tiedostoon {1}", this.Nimi, tiedosto));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

                string nimiTmp = Path.GetTempFileName();

                using (TextWriter writer = new StreamWriter(nimiTmp))
                {
                    serializer.Serialize(writer, this);
                    writer.Close();
                }

                File.Copy(nimiTmp, this.Tiedosto, true);
                File.Delete(nimiTmp);
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Rankingsarjan {0} tallennus epäonnistui!", this.Nimi), e, false);
                }
            }
        }

        public void Luo(Ranking ranking, RankingAsetukset asetukset, Loki loki)
        {
            try
            {
                int kk0 = 0;
                int kk1 = 0;
                Tyypit.Aika.RankingSarjaKuukaudet(this.Pituus, this.SarjanNumero, out kk0, out kk1);

                this.Osallistujat.Clear();
                this.Osakilpailut.Clear();

                for (int kk = kk0; kk <= kk1; ++kk)
                {
                    var rankingKuukausi = ranking.AvaaRankingKuukausi(this.Vuosi, kk);
                    if (rankingKuukausi != null)
                    {
                        var osakilpailut = rankingKuukausi.Osakilpailut
                            .Where(x => x.OnRankingOsakilpailu && SisaltaaOsakilpailun(x))
                            .OrderBy(x => x.PvmDt);

                        foreach (var osakilpailu in osakilpailut)
                        {
                            if (SisaltaaOsakilpailun(osakilpailu))
                            {
                                var kilpailu = osakilpailu.LataaKilpailu(loki);
                                if (kilpailu != null)
                                {
                                    LisaaKilpailu(ranking, kilpailu, osakilpailu, asetukset, false);
                                }
                            }
                        }
                    }
                }

                PaivitaTilanne();

                Tallenna(loki, true);
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita("Rankingsarjan päivitys epäonnistui", e, false);
                }
            }
        }

        public bool TarkistaOnkoSarjaMuuttunut(Ranking ranking, Loki loki)
        {
            try
            {
                int kk0 = 0;
                int kk1 = 0;
                Tyypit.Aika.RankingSarjaKuukaudet(this.Pituus, this.SarjanNumero, out kk0, out kk1);

                List<RankingOsakilpailuTietue> oikeatOsakilpailut = new List<RankingOsakilpailuTietue>();

                for (int kk = kk0; kk <= kk1; ++kk)
                {
                    var rankingKuukausi = ranking.AvaaRankingKuukausi(this.Vuosi, kk);
                    if (rankingKuukausi != null)
                    {
                        oikeatOsakilpailut.AddRange(rankingKuukausi.Osakilpailut
                            .Where(x => x.OnRankingOsakilpailu && SisaltaaOsakilpailun(x))
                            .OrderBy(x => x.PvmDt));
                    }
                }

                if (oikeatOsakilpailut.Count != this.Osakilpailut.Count)
                {
                    if (loki != null)
                    {
                        loki.Kirjoita(string.Format("Ranking sarja {0} muuttunut. Eri määrä kilpailuja", this.Nimi));
                    }
                    return true;
                }

                foreach (var osakilpailu in this.Osakilpailut)
                {
                    if (!oikeatOsakilpailut.Any(x => string.Equals(x.Id, osakilpailu.Id)))
                    {
                        if (loki != null)
                        {
                            loki.Kirjoita(string.Format("Ranking sarja {0} muuttunut. Kilpailua {1} ei löydy", this.Nimi, osakilpailu.Id));
                        }
                        return true;
                    }

                    if (!oikeatOsakilpailut.Any(x => string.Equals(x.KilpailunTarkistusSumma, osakilpailu.KilpailunTarkistusSumma)))
                    {
                        if (loki != null)
                        {
                            loki.Kirjoita(string.Format("Ranking sarja {0} muuttunut. Kilpailun {1} tarkistussumma on muuttunut", this.Nimi, osakilpailu.Id));
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita("Rankingsarjan tarkistus epäonnistui", e, false);
                }
                return true;
            }

            return false;
        }

        public void PaivitaTilanne()
        {
            this.Osallistujat.Clear();

            List<RankingPelaajaTietue> kaikkiPelaajat = new List<RankingPelaajaTietue>();

            int osakilpailunNumero = 0;

            foreach (var o in this.Osakilpailut)
            {
                o.KilpailunNumero = osakilpailunNumero++;

                foreach (var p in o.Osallistujat)
                {
                    if (!kaikkiPelaajat.Any(x => Tyypit.Nimi.Equals(x.Nimi, p.Nimi)))
                    {
                        kaikkiPelaajat.Add(new RankingPelaajaTietue() 
                        {
                            Nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(p.Nimi),
                        });
                    }
                }
            }

            foreach (var o in this.Osakilpailut.OrderBy(x => x.AlkamisAikaDt))
            {
                foreach (var p in kaikkiPelaajat)
                {
                    var pelaaja = o.Osallistujat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, p.Nimi));
                    if (pelaaja == null)
                    {
                        p.LisaaOsakilpailunPisteet(0, "xxx");
                    }
                    else
                    {
                        p.LisaaOsakilpailunPisteet(pelaaja.RankingPisteet, pelaaja.RankingPisteString);
                    }
                }
            }

            int kumulatiivinenSijoitus = 1;
            int sijoitus = 1;
            int edellisetPisteet = 99999999;
            int edellinenSijoitus = 99999999;

            foreach (var p in kaikkiPelaajat.OrderByDescending(x => x.RankingPisteet))
            {
                p.Sijoitus = sijoitus;
                p.KumulatiivinenSijoitus = kumulatiivinenSijoitus;

                if (p.RankingPisteet != edellisetPisteet)
                {
                    kumulatiivinenSijoitus++;
                    edellisetPisteet = p.RankingPisteet;
                    p.Sijoitus = sijoitus;
                    edellinenSijoitus = sijoitus;
                }
                else
                {
                    p.Sijoitus = edellinenSijoitus;
                }

                this.Osallistujat.Add(p);

                sijoitus++;
            }

            PaivitaTilanneTeksti();
        }

        public void KirjoitaViimeisinTulos(Tyypit.Teksti teksti)
        {
            var kisa = this.Osakilpailut.OrderBy(x => x.AlkamisAikaDt).LastOrDefault();
            if (kisa != null)
            {
                var voittajat = kisa.Osallistujat.Where(x => x.Sijoitus == 1);
                var kakkoset = kisa.Osallistujat.Where(x => x.Sijoitus == 2);
                var kolmoset = kisa.Osallistujat.Where(x => x.Sijoitus == 3);

                teksti.NormaaliRivi(string.Format("{0}.{1}.{2} {3} viikkokilpailuun osallistui {4} pelaajaa.",
                    kisa.AlkamisAikaDt.Day,
                    kisa.AlkamisAikaDt.Month,
                    kisa.AlkamisAikaDt.Year,
                    this.Laji,
                    kisa.Osallistujat.Count));

                if (voittajat.Count() == 1)
                {
                    teksti.NormaaliTeksti(string.Format("Voittaja {0}, ", voittajat.First().Nimi));
                }
                else if (voittajat.Count() > 1)
                {
                    teksti.NormaaliTeksti(string.Format("Voittajat {0}, ", string.Join(", ", voittajat.Select(x => x.Nimi).ToArray())));
                }

                if (kakkoset.Count() == 1)
                {
                    teksti.NormaaliTeksti(string.Format("toinen {0}, ", kakkoset.First().Nimi));
                }
                else if (kakkoset.Count() > 1)
                {
                    teksti.NormaaliTeksti(string.Format("toisia {0}, ", string.Join(", ", kakkoset.Select(x => x.Nimi).ToArray())));
                }

                if (kolmoset.Count() == 1)
                {
                    teksti.NormaaliTeksti(string.Format("kolmas {0}.", kolmoset.First().Nimi));
                }
                else if (kolmoset.Count() > 1)
                {
                    teksti.NormaaliTeksti(string.Format("kolmansia {0}.", string.Join(", ", kolmoset.Select(x => x.Nimi).ToArray())));
                }

                teksti.RivinVaihto();
            }
        }

        public void KirjoitaTilanne(Tyypit.Teksti teksti)
        {
            teksti.NormaaliRivi(this.Nimi);

            if (this.Osakilpailut.Count == 1)
            {
                teksti.NormaaliRivi("Tilanne ensimmäisen osakilpailun jälkeen:");
            }
            else
            {
                int summa = (int)(this.Osakilpailut.Select(x => x.Osallistujat.Count).Sum());

                teksti.NormaaliRivi(string.Format("Tilanne {0} osakilpailun jälkeen: ({1}={2})",
                    this.Osakilpailut.Count,
                    string.Join("+", this.Osakilpailut.OrderBy(x => x.AlkamisAikaDt).Select(x => x.Osallistujat.Count.ToString()).ToArray()),
                    summa));
            }

            teksti.RivinVaihto();

            foreach (var p in this.Osallistujat.OrderByDescending(x => x.RankingPisteet))
            {
                teksti.NormaaliTeksti(string.Format("{0}. {1} ", p.Sijoitus, p.Nimi));
                teksti.PaksuTeksti(string.Format("{0}p ", p.RankingPisteet));
                teksti.PieniTeksti(string.Format(" ({0})", p.RankingPisteString));
                teksti.RivinVaihto();
            }
        }

        public void PaivitaTilanneTeksti()
        {
            this.tilanneLyhyt = new Tyypit.Teksti();
            this.tilannePitka = new Tyypit.Teksti();

            KirjoitaViimeisinTulos(this.tilannePitka);
            KirjoitaTilanne(this.tilannePitka);
            KirjoitaTilanne(this.tilanneLyhyt);
        }

        public void LisaaKilpailu(Ranking rankingit, Kilpailu kilpailu, RankingOsakilpailuTietue tietue, RankingAsetukset asetukset, bool paivitaTilanne)
        {
            if (this.Osakilpailut.Any(x => x.AlkamisAikaDt > kilpailu.AlkamisAikaDt))
            {
                return; // Rankingkilpailuja voidaan lisätä vain sarjan loppuun
            }

            var osakilpailu = this.Osakilpailut.FirstOrDefault(x => string.Equals(x.Id, kilpailu.Id, StringComparison.OrdinalIgnoreCase));
            if (osakilpailu == null)
            {
                osakilpailu = new RankingOsakilpailu() 
                {
                    Id = kilpailu.Id,
                    Nimi = kilpailu.Nimi,
                    AlkamisAika = kilpailu.AlkamisAika,
                    KilpailunTarkistusSumma = tietue != null ? tietue.KilpailunTarkistusSumma : string.Empty
                };

                if (this.Osakilpailut.Count == 0)
                {
                    this.Osakilpailut.Add(osakilpailu);
                }
                else 
                {
                    this.Osakilpailut.Insert(0, osakilpailu);
                }
            }

            osakilpailu.PaivitaKilpailu(rankingit, kilpailu, this, asetukset);

            if (paivitaTilanne)
            {
                PaivitaTilanne();
            }

            muokattu = true;
        }

        public bool OnSarjanEnsimmainenKilpailu(Kilpailu kilpailu)
        {
            var eka = this.Osakilpailut.OrderBy(x => x.AlkamisAikaDt).FirstOrDefault();

            if (eka != null)
            {
                if (string.Equals(eka.Id, kilpailu.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public List<RankingPelaajaTietue> HaeRankingKarjetKilpailulle(Asetukset asetukset, Ranking ranking, Kilpailu kilpailu)
        {
            List<RankingPelaajaTietue> rankingKarjet = new List<RankingPelaajaTietue>();

            var a = asetukset.RankingPisteytys(kilpailu.RankingKisaLaji);

            var r = RankingEnnenOsakilpailua(kilpailu.AlkamisAikaDt);

            if (OnSarjanEnsimmainenKilpailu(kilpailu)
                && a.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta)
            {
                var s = ranking.AvaaEdellinenSarja(kilpailu);
                if (s != null)
                {
                    r = s.RankingEnnenOsakilpailua(kilpailu.AlkamisAikaDt);
                }
            }

            if (r != null)
            {
                if (a.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista)
                {
                    var parhaatSijat = r
                        .Where(x => kilpailu.Osallistujat.Any(y => Tyypit.Nimi.Equals(y.Nimi, x.Nimi)))
                        .OrderBy(x => x.KumulatiivinenSijoitus)
                        .Select(x => x.KumulatiivinenSijoitus)
                        .Distinct();

                    foreach (var pelaaja in r)
                    {
                        if (parhaatSijat.Count() > 0 && parhaatSijat.ElementAt(0) == pelaaja.KumulatiivinenSijoitus)
                        {
                            pelaaja.KumulatiivinenSijoitus = 1;
                            pelaaja.Sijoitus = 1;
                        }
                        else if (parhaatSijat.Count() > 1 && parhaatSijat.ElementAt(1) == pelaaja.KumulatiivinenSijoitus)
                        {
                            pelaaja.KumulatiivinenSijoitus = 2;
                            pelaaja.Sijoitus = 2;
                        }
                        else if (parhaatSijat.Count() > 2 && parhaatSijat.ElementAt(2) == pelaaja.KumulatiivinenSijoitus)
                        {
                            pelaaja.KumulatiivinenSijoitus = 3;
                            pelaaja.Sijoitus = 3;
                        }
                    }

                    rankingKarjet.AddRange(r
                        .Where(x => x.KumulatiivinenSijoitus >= 1 && x.KumulatiivinenSijoitus <= 3)
                        .Where(x => kilpailu.Osallistujat.Any(y => Tyypit.Nimi.Equals(x.Nimi, y.Nimi)))
                        .OrderBy(x => x.KumulatiivinenSijoitus));
                }
                else
                {
                    rankingKarjet.AddRange(r
                        .Where(x => x.KumulatiivinenSijoitus >= 1 && x.KumulatiivinenSijoitus <= 3)
                        .OrderBy(x => x.KumulatiivinenSijoitus));
                }
            }

            return rankingKarjet;
        }

        public List<RankingPelaajaTietue> RankingEnnenOsakilpailua(DateTime aika)
        {
            List<RankingPelaajaTietue> pelaajat = new List<RankingPelaajaTietue>();

            foreach (var osakilpailu in this.Osakilpailut.Where(x => x.AlkamisAikaDt < aika))
            {
                foreach (var pelaaja in osakilpailu.Osallistujat)
                {
                    var p = pelaajat.FirstOrDefault(x => Tyypit.Nimi.Equals(x.Nimi, pelaaja.Nimi));
                    if (p != null)
                    {
                        p.RankingPisteet += pelaaja.RankingPisteet;
                    }
                    else
                    {
                        pelaajat.Add(new RankingPelaajaTietue() 
                        {
                            Nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(pelaaja.Nimi),
                            RankingPisteet = pelaaja.RankingPisteet
                        });
                    }
                }
            }

            var pt = pelaajat.OrderByDescending(x => x.RankingPisteet).ToArray();
            pelaajat.Clear();
            pelaajat.AddRange(pt);

            int sijoitus = 1;
            int kumulatiivinenSijoitus = 1;
            int edellinenSijoitus = 99999;
            int edellinenKumulatiivinenSijoitus = 99999;
            int edellisetPisteet = 99999;

            foreach (var pelaaja in pelaajat)
            {
                if (pelaaja.RankingPisteet == edellisetPisteet)
                {
                    pelaaja.Sijoitus = edellinenSijoitus;
                    pelaaja.KumulatiivinenSijoitus = edellinenKumulatiivinenSijoitus;
                }
                else
                {
                    pelaaja.Sijoitus = sijoitus;
                    pelaaja.KumulatiivinenSijoitus = kumulatiivinenSijoitus;

                    kumulatiivinenSijoitus++;

                    edellisetPisteet = pelaaja.RankingPisteet;
                    edellinenSijoitus = pelaaja.Sijoitus;
                    edellinenKumulatiivinenSijoitus = pelaaja.KumulatiivinenSijoitus;
                }

                sijoitus++;
            }

            return pelaajat;
        }
    }
}

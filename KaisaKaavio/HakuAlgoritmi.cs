using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KaisaKaavio
{ 
    public class HakuAlgoritmi : IHakuAlgoritmi
    {
        private static readonly float PisteytysSakkoKierrosVirheesta = 1000.0f;
        private static readonly float PisteytysSakkoUusintaOttelusta = 1.0f;
        private static readonly float PisteytysSakkoSeuraavaltaKierrokseltaHausta = 0.001f;

        public class Hyppy
        {
            public Pelaaja Pelaaja = null;
            public string Syy = string.Empty;
        }

        public class Pelaajat
        {
            public Pelaaja Pelaaja1 = null;
            public Pelaaja Pelaaja2 = null;
            public int Kierros = 0;
            public List<Hyppy> Hypyt = null;

            public bool SisaltaaPelaajan(Pelaaja pelaaja)
            {
                if (pelaaja == null)
                {
                    return false;
                }

                return 
                    ((Pelaaja1 != null) && (pelaaja.Id == Pelaaja1.Id)) || 
                    ((Pelaaja2 != null) && (pelaaja.Id == Pelaaja2.Id));
            }

            public bool SisaltaaPelaajat(Pelaaja pelaaja1, Pelaaja pelaaja2)
            {
                return SisaltaaPelaajan(pelaaja1) && SisaltaaPelaajan(pelaaja2);
            }
        }

        public class HakuTulos
        {
            public List<Pelaajat> PeliParit = new List<Pelaajat>();
        }

        public class HakuPelaaja
        {
            public Pelaaja Pelaaja = null;
            public int Tappioita = 0;
        }

        public class HakuPeli
        {
            public int Kierros = 0;
            public int KierrosPelaaja1 = -1;
            public int KierrosPelaaja2 = -1;
            public int PeliNumero = 0;
            public Pelaaja Pelaaja1 = null;
            public Pelaaja Pelaaja2 = null;
            public PelinTulos Tulos = PelinTulos.EiTiedossa;
            public PelinTilanne Tilanne = PelinTilanne.Tyhja;

            public List<Hyppy> Hypyt = null;
            public bool PeliOnPelattuKaaviossa = false;

            public bool SisaltaaPelaajan(int id)
            {
                return
                    (Pelaaja1 != null && Pelaaja1.Id == id) ||
                    (Pelaaja2 != null && Pelaaja2.Id == id);
            }

            public bool SisaltaaPelaajat(int id1, int id2)
            {
                return SisaltaaPelaajan(id1) && SisaltaaPelaajan(id2);
            }

            public bool SisaltaaJommankummanPelaaja(int id1, int id2)
            {
                return SisaltaaPelaajan(id1) || SisaltaaPelaajan(id2);
            }

            public bool Havisi(int id)
            {
                return
                    (Pelaaja1 != null && Pelaaja1.Id == id && (Tulos == PelinTulos.Pelaaja2Voitti || Tulos == PelinTulos.MolemmatHavisi)) ||
                    (Pelaaja2 != null && Pelaaja2.Id == id && (Tulos == PelinTulos.Pelaaja1Voitti || Tulos == PelinTulos.MolemmatHavisi));
            }
        }

        IStatusRivi status = null;

        List<HakuPeli> Pelatut = new List<HakuPeli>();
        List<HakuPeli> PelitKesken = new List<HakuPeli>();
        List<HakuTulos> Hakutulokset = new List<HakuTulos>();

        public List<Pelaajat> UudetPelit { get; private set; }
        public Exception HakuVirhe { get; private set; }
        public bool PeruutaHaku { get; set; }
        public bool UusiHakuTarvitaan { get; private set; }
        public bool AutomaattinenTestausMenossa { get; set; }
        public int Kierros { get; private set; }

        Kilpailu Kilpailu = null;
        int EkaPudariKierros = 0;
        bool HakuKesken = true;

        public HakuAlgoritmi(Kilpailu kilpailu, Loki loki, int kierros, IStatusRivi status)
        {
            this.status = status;
            this.Kilpailu = kilpailu;
            this.Kierros = kierros;
            this.HakuVirhe = null;
            this.PeruutaHaku = false;
            this.UudetPelit = new List<Pelaajat>();
            this.UusiHakuTarvitaan = false;
            this.AutomaattinenTestausMenossa = false;

            switch (this.Kilpailu.KaavioTyyppi)
            {
                case KaavioTyyppi.Pudari2Kierros:
                    this.EkaPudariKierros = 2;
                    break;

                case KaavioTyyppi.Pudari3Kierros:
                    this.EkaPudariKierros = 3;
                    break;

                case KaavioTyyppi.Pudari4Kierros:
                    this.EkaPudariKierros = 4;
                    break;

                default:
                    this.EkaPudariKierros = 999;
                    break;
            }

            foreach (var peli in kilpailu.Pelit)
            {
                HakuPeli p = new HakuPeli()
                {
                    Kierros = peli.Kierros,
                    KierrosPelaaja1 = peli.KierrosPelaaja1,
                    KierrosPelaaja2 = peli.KierrosPelaaja2,
                    Pelaaja1 = kilpailu.Osallistujat.FirstOrDefault(y => y.Id == peli.Id1),
                    Pelaaja2 = kilpailu.Osallistujat.FirstOrDefault(y => y.Id == peli.Id2),
                    Tulos = peli.Tulos,
                    Tilanne = peli.Tilanne,
                    PeliNumero = peli.PeliNumero,
                    PeliOnPelattuKaaviossa = peli.Tilanne == PelinTilanne.Pelattu
                };

#if DEBUG
                if (p.Pelaaja1 == null)
                {
                    throw new NullReferenceException(string.Format("Pelaajaa ei löytynyt id:lle {0} peliin {1}", peli.Id1, peli.Kuvaus()));
                }

                if (p.Pelaaja2 == null)
                {
                    // walkover peli - OK
                    //throw new NullReferenceException(string.Format("Pelaajaa ei löytynyt id:lle {0} peliin {1}", peli.Id2, peli.Kuvaus()));
                }
#endif

                if (peli.Tilanne == PelinTilanne.Pelattu)
                {
                    Pelatut.Add(p);
                }
                else
                {
                    PelitKesken.Add(p);
                }
            }
        }

        private void PaivitaStatusRivi(string viesti, int vaihe)
        {
            if (!this.PeruutaHaku)
            {
                if (this.status != null)
                {
                    this.status.PaivitaStatusRivi(viesti, true, vaihe, 100);
                }
            }
        }

#if DEBUG
        private int DebugSisennys = 0;
#endif

        private void DebugSisenna(int i)
        {
#if DEBUG
            DebugSisennys += i;
#endif
        }

        private void DebugViesti(string viesti, params object[] parametrit)
        { 
#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                Debug.Write("| ");

                for (int i = 0; i < DebugSisennys; ++i)
                {
                    Debug.Write("  ");
                }

                Debug.WriteLine(string.Format(viesti, parametrit));
            }
#endif
        }

        /// <summary>
        /// Hakee kaikki mahdolliset kaaviot nykytilanteesta eteenpäin,
        /// ja laskee sellaiset uudet pelit jotka tapahtuvat kaikissa skenaarioissa
        /// </summary>
        /// <returns></returns>
        public void Hae()
        {
            if (PelitKesken.Count > Asetukset.PelejaEnintaanKeskenHaettaessa)
            {
                return;
            }

            try
            {
                PaivitaStatusRivi("Haetaan pelejä...", 10);

                this.HakuKesken = true;

#if DEBUG
                DebugViesti("=======================================================================================================================");
                DebugViesti("Tilanne ennen hakua:");

                foreach (var pp in Pelatut.Where(x => x.Kierros >= this.Kierros - 1))
                {
                    if (pp.Pelaaja2 == null)
                    {
                        DebugViesti("Pelattu {0} {1} - w.o., {2}, {3}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Tilanne, pp.Tulos);
                    }
                    else
                    {
                        DebugViesti("Pelattu {0} {1} - {2}, {3}, {4}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Pelaaja2.Nimi, pp.Tilanne, pp.Tulos);
                    }
                }

                foreach (var pp in PelitKesken.Where(x => x.Kierros >= this.Kierros - 1))
                {
                    DebugViesti("Kesken {0} {1} - {2}, {3}, {4}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Pelaaja2.Nimi, pp.Tilanne, pp.Tulos);
                }
#endif
                int maxPermutations = (int)Math.Pow(2.0, (double)PelitKesken.Count);
                if (maxPermutations <= 1)
                {
#if DEBUG
                    DebugViesti("Haetaan pelejä:");
#endif
                    PaivitaStatusRivi("Haetaan pelejä... skenaario 1/1", 50);

                    List<HakuPeli> pelatutPelit = new List<HakuPeli>();
                    pelatutPelit.AddRange(Pelatut);

                    List<HakuPeli> arvotutPelit = new List<HakuPeli>();

                    HakuTulos hakutulos = new HakuTulos();

                    HaePelitKierrokselle(pelatutPelit, arvotutPelit, hakutulos.PeliParit);

                    this.Hakutulokset.Add(hakutulos);
                }
                else
                {
#if DEBUG
                    DebugViesti("Haetaan pelejä: {0} keskeneräistä peliä, {1} erilaista skenaariota", PelitKesken.Count, maxPermutations);
#endif

                    // Tämä käy läpi kaikki mahdolliset skenaariot kesken olevien pelien lopputuloksista
                    for (int permutation = 0; permutation < maxPermutations; ++permutation)
                    {
                        if (PeruutaHaku)
                        {
                            return;
                        }

                        PaivitaStatusRivi(
                            string.Format("Haetaan pelejä... skenaario {0}/{1}", permutation + 1, maxPermutations), 
                            10 + (int)(((float)permutation / (float)maxPermutations) * 90.0f));

#if DEBUG
                        string voittajat = string.Format("P[{0}] voittajat:", permutation + 1);
#endif
                        List<HakuPeli> pelatutPelit = new List<HakuPeli>();
                        pelatutPelit.AddRange(Pelatut);

                        List<HakuPeli> arvotutPelit = new List<HakuPeli>();

                        for (int i = 0; i < PelitKesken.Count; ++i)
                        {
                            HakuPeli peli = PelitKesken[i];
                            int bitmask = 1 << i;
                            bool p1voitti = (permutation & bitmask) == bitmask;

                            HakuPeli uusiPeli = new HakuPeli()
                            {
                                Kierros = peli.Kierros,
                                KierrosPelaaja1 = peli.KierrosPelaaja1,
                                KierrosPelaaja2 = peli.KierrosPelaaja2,
                                Pelaaja1 = peli.Pelaaja1,
                                Pelaaja2 = peli.Pelaaja2,
                                Tulos = p1voitti ? PelinTulos.Pelaaja1Voitti : PelinTulos.Pelaaja2Voitti,
                                Tilanne = PelinTilanne.Pelattu,
                                PeliOnPelattuKaaviossa = false
                            };

                            pelatutPelit.Add(uusiPeli);

#if DEBUG
                            voittajat += (p1voitti ? peli.Pelaaja1.Nimi : peli.Pelaaja2.Nimi) + " ";
#endif
                        }

#if DEBUG
                        DebugViesti(voittajat);
#endif
                        HakuTulos hakutulos = new HakuTulos();

                        HaePelitKierrokselle(pelatutPelit, arvotutPelit, hakutulos.PeliParit);

                        this.Hakutulokset.Add(hakutulos);
                    }
                }

                PaivitaUudetPelit();
            }
            catch (Exception ee)
            {
                this.HakuVirhe = ee;
                this.Hakutulokset = null;
            }
            finally
            {
                if (UudetPelit.Count > 0)
                {
                    PaivitaStatusRivi(string.Format("Haku valmis. Haettiin {0} uutta peliä", UudetPelit.Count), 100);
                }
                else
                {
                    PaivitaStatusRivi("Haku valmis. Ei uusia pelejä", 100);
                }

                this.HakuKesken = false;
            }
        }

        public bool HakuValmis { get { return !this.HakuKesken; } }

        private void PaivitaUudetPelit()
        {
            this.UudetPelit.Clear();

            if (Hakutulokset == null || Hakutulokset.Count == 0)
            {
                return;
            }

            else if (Hakutulokset.Count() == 1)
            {
                // Algoritmi tuotti vain yhden skenaarion, lisätään kaikki peliparit
                foreach (var pari in Hakutulokset.First().PeliParit)
                {
                    UudetPelit.Add(new Pelaajat() 
                    {
                        Pelaaja1 = pari.Pelaaja1,
                        Pelaaja2 = pari.Pelaaja2,
                        Kierros = pari.Kierros,
                        Hypyt = pari.Hypyt
                    });
                }
            }
            else
            {
#if DEBUG
                if (!this.AutomaattinenTestausMenossa)
                {
                    DebugViesti("Mahdolliset hakutulokset:");

                    foreach (var tulos in Hakutulokset)
                    {
                        DebugViesti(" -{0}", string.Join(",",
                            tulos.PeliParit.Select(x => string.Format("{0}-{1}", x.Pelaaja1.Nimi, x.Pelaaja2.Nimi)).ToArray()));
                    }
                }
#endif
                List<HakuAlgoritmi.Pelaajat> kaikkiPeliparit = new List<HakuAlgoritmi.Pelaajat>();

                foreach (var tulos in Hakutulokset)
                {
                    kaikkiPeliparit.AddRange(tulos.PeliParit);
                }

                List<HakuAlgoritmi.Pelaajat> mahdollisetPeliparit = new List<HakuAlgoritmi.Pelaajat>();
                foreach (var pari in kaikkiPeliparit)
                {
                    if (!mahdollisetPeliparit.Any(x => x.SisaltaaPelaajat(pari.Pelaaja1, pari.Pelaaja2)))
                    {
                        mahdollisetPeliparit.Add(pari);
                    }
                }

                foreach (var pari in mahdollisetPeliparit)
                {
                    // Lisätään peliparit jotka toteutuvat kaikissa mahdollisissa skenaarioissa
                    if (Hakutulokset.All(x => x.PeliParit.Any(y => y.SisaltaaPelaajat(pari.Pelaaja1, pari.Pelaaja2))))
                    {
                        UudetPelit.Add(new Pelaajat() 
                        {
                            Pelaaja1 = pari.Pelaaja1,
                            Pelaaja2 = pari.Pelaaja2,
                            Kierros = pari.Kierros,
                            Hypyt = pari.Hypyt
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Hakee pelejä annetulle kierrokselle.
        /// Tämä funktio olettaa että kaikki aiempien kierrosten pelit on jo pelattu
        /// </summary>
        private bool HaePelitKierrokselle(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, List<Pelaajat> peliparit)
        {
            if (PeruutaHaku)
            {
                return false;
            }

#if DEBUG
            foreach (var peli in pelatutPelit)
            {
                if (peli.Tilanne != PelinTilanne.Pelattu)
                {
                    throw new Exception("Bugi! Pelejä pelaamatta haettaessa uusia pelejä");
                }

                if (peli.Kierros > this.Kierros + 1)
                {
                    throw new Exception("Bugi! Pelattu peli kierros >= haku kierros");
                }
            }

            foreach (var peli in arvotutPelit)
            {
                if (peli.Kierros < this.Kierros)
                {
                    throw new Exception("Bugi! Jo arvottu peli kierros < haku kierros");
                }
            }
#endif
            var mukana = this.Kilpailu.Osallistujat.Where(x => 
                (x.Id >= 0) &&
                Mukana(pelatutPelit, arvotutPelit, x.Id, 99999));

            if (mukana.Count() < 2)
            {
#if DEBUG
                DebugViesti("Mukana alle 2 pelaajaa. Haku on päättynyt");
#endif
                return false;
            }

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                Debug.WriteLine(string.Format("Mukana : {0}", string.Join(",", mukana.Select(x => string.Format("{0}({1})", x.Nimi, x.Id)).ToArray())));
            }
#endif

            List<Pelaaja> pelaajat = new List<Pelaaja>();
            pelaajat.AddRange(mukana);

            List<HakuPeli> pelit = new List<HakuPeli>();
            pelit.AddRange(pelatutPelit);
            pelit.AddRange(arvotutPelit);

            bool haettiinJotain = false;

            while (true)
            {
                if (PeruutaHaku)
                {
                    return false;
                }

                var peli = HaeSeuraavaPeliKierrokselle(pelit, pelatutPelit, arvotutPelit, pelaajat);
                if (peli != null)
                {
                    if (pelit.Any(x => x.Tilanne != PelinTilanne.Pelattu && x.SisaltaaJommankummanPelaaja(peli.Pelaaja1.Id, peli.Pelaaja2.Id)))
                    {
#if DEBUG
                        DebugViesti("Ei haeta pelejä pelaajille joilla on peli vielä kesken. Haku keskeytetään tältä erää");
#endif
                        break; // Keskeytetään tämänkertainen haku heti jos haettiin peli pelaajalle jolla on aiempi peli kesken
                    }

                    haettiinJotain = true;
                    arvotutPelit.Add(peli);
                    pelit.Add(peli);
                    peliparit.Add(new Pelaajat()
                    {
                        Pelaaja1 = peli.Pelaaja1,
                        Pelaaja2 = peli.Pelaaja2,
                        Kierros = peli.Kierros,
                        Hypyt = peli.Hypyt,
                    });

                    if (peli.KierrosPelaaja1 < peli.Kierros ||
                        peli.KierrosPelaaja2 < peli.Kierros)
                    {
#if DEBUG
                        DebugViesti("Haettiin peli jossa toinen pelaaja on kierroksen perässä. Haku keskeytetään tältä erää");
#endif
                        this.UusiHakuTarvitaan = true;
                        break; // Keskeytetään tämänkertainen haku heti jos haettiin peli jossa toinen pelaaja on kierroksen perässä (yksinkertaistaa hakua)
                               // Tässä tilanteessa uusi haku käynnistyy automaattisesti perään
                    }

                    if (mukana.Count() < Asetukset.HuolellisenHaunPelaajamaara)
                    {
#if DEBUG
                        DebugViesti("Haettiin peli kaavion loppupuolella. Haku keskeytetään tältä erää");
#endif
                        this.UusiHakuTarvitaan = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (haettiinJotain &&
                pelaajat.Count(x => LaskePelit(pelit, x.Id) < this.Kierros) == 1)
            {
#if DEBUG
                DebugViesti("Yksi pelaaja jäi odottamaan vastustajaa seuraavalta kierrokselta. Tilataan uusi haku perään");
#endif
                this.UusiHakuTarvitaan = true;
            }

            return haettiinJotain;
        }

        public struct HakuData
        {
            public float Pisteytys;
            public string Polku;
        }

        private HakuPeli HaeSeuraavaPeliKierrokselle(
            List<HakuPeli> kaikkiPelit, 
            List<HakuPeli> pelatutPelit, 
            List<HakuPeli> arvotutPelit, 
            List<Pelaaja> pelaajat)
        {
#if DEBUG
            DebugSisenna(1);
            DebugViesti("---====---");
#endif

            var mukana = pelaajat.Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id, this.Kierros + 1));
            var mukana2 = mukana.ToArray();

            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, this.Kierros + 1));

            if (hakijat.Count() < 2)
            {
#if DEBUG
                if (hakijat.Count() == 1)
                {
                    DebugViesti("{0} huilaa", hakijat.First().Nimi);
                }
                DebugSisenna(-1);
#endif
                return null;
            }

            // Erikoistapaus kun on kolme pelaajaa jäljellä. Saatetaan joutua "hyppäämään hakijan yli"
            if (hakijat.Count() == 3)
            {
                var eka = hakijat.ElementAt(0);
                var toka = hakijat.ElementAt(1);
                var kolmas = hakijat.ElementAt(2);

                if (LaskeKeskenaisetPelit(kaikkiPelit, eka.Id, toka.Id) > 0 &&
                    LaskeKeskenaisetPelit(kaikkiPelit, eka.Id, kolmas.Id) > 0 &&
                    LaskeKeskenaisetPelit(kaikkiPelit, toka.Id, kolmas.Id) == 0)
                {
                    if (LaskePelit(kaikkiPelit, toka.Id) <= LaskePelit(kaikkiPelit, eka.Id) &&
                        LaskePelit(kaikkiPelit, kolmas.Id) <= LaskePelit(kaikkiPelit, eka.Id))
                    {
                        var peli = LisaaPeli(kaikkiPelit, pelatutPelit, arvotutPelit, toka, kolmas);

                        peli.Hypyt = new List<Hyppy>();
                        peli.Hypyt.Add(new Hyppy() { Pelaaja = eka, Syy = "On pelannut jo kaikkia muita vastaan" });
#if DEBUG
                        DebugViesti("Erikoishaku => Hakija {0} hypätty yli. {1} ja {2} pelaa", eka.Nimi, toka.Nimi, kolmas.Nimi);
                        DebugSisenna(-1);
#endif
                        return peli;
                    }
                }
            }

            var hakija = hakijat.First();

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, hakija.Id, this.Kierros))
            {
#if DEBUG
                DebugViesti("Hakija {0} ei välttämättä mukana. Pysäytetään haku", hakija.Nimi);
                DebugSisenna(-1);
#endif
                return null;
            }

            var vastustajat = hakijat.Skip(1);

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                DebugViesti("{0} hakee {1}",
                    string.Format("{0}[{1}]", hakija.Nimi, LaskePelit(pelatutPelit, arvotutPelit, hakija.Id, this.Kierros)),
                    string.Join(",", vastustajat.Select(x =>
                        string.Format("{0}[{1}]", x.Nimi, LaskePelit(pelatutPelit, arvotutPelit, x.Id, this.Kierros))).ToArray()));
            }
#endif

            Pelaaja vastustaja = null;
            List<Hyppy> hypyt = new List<Hyppy>();

            var kaikkiPelaajat = this.Kilpailu.Osallistujat.Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id, 999999));
            if ((this.Kierros >= 4) &&
                (kaikkiPelaajat.Count() < Asetukset.HuolellisenHaunPelaajamaara))
            {
                if (this.Kilpailu.EvaluointiTietokanta == null)
                {
                    this.Kilpailu.EvaluointiTietokanta = new Tyypit.EvaluointiTietokanta(this.Kierros - 1);
                }

                StringBuilder tilanneAvain = new StringBuilder();
                StringBuilder polku = new StringBuilder();

                foreach (var peli in pelatutPelit.Where(x => x.Kierros >= this.Kilpailu.EvaluointiTietokanta.AlkuKierros))
                {
                    string avain = PelinAvain(peli);

                    if (tilanneAvain.Length > 0)
                    {
                        tilanneAvain.Append(" ");
                    }
                    tilanneAvain.Append(avain);

                    if (!peli.PeliOnPelattuKaaviossa)
                    {
                        if (polku.Length > 0)
                        {
                            polku.Append(" ");
                        }
                        polku.Append(avain);
                    }
                }

                vastustaja = HaeVastustajaHuolellisesti(
                    pelatutPelit, 
                    kaikkiPelaajat, 
                    hakija, 
                    vastustajat, 
                    tilanneAvain.ToString(),
                    polku.ToString(),
                    this.Kilpailu.EvaluointiTietokanta,
                    hypyt);

#if DEBUG
                this.Kilpailu.EvaluointiTietokanta.Tulosta();
#endif
            }
            else
            {
                vastustaja = HaeVastustajaNopeasti(kaikkiPelit, mukana2, hakija, vastustajat, hypyt);
            }

            if (vastustaja == null) 
            {
                // Jos tähän on tultu, kaikilla hakujärestyksillä tulee uusintapelejä (toivottavasti ollaan finaalissa tai lähellä, muuten kaaviossa on tapahtunut hakuvirhe)
#if DEBUG
                DebugViesti("-Kaikki haut olivat huonoja. Haetaan ensimmäinen vastustaja listalta");
#endif
                vastustaja = vastustajat.First();
            }

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, vastustaja.Id, this.Kierros))
            {
#if DEBUG
                DebugViesti("-Vastustaja {0} ei välttämättä mukana. Pysäytetään haku", vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return null;
            }

            var uusiPeli = LisaaPeli(kaikkiPelit, pelatutPelit, arvotutPelit, hakija, vastustaja);

            if (hypyt.Any())
            {
                uusiPeli.Hypyt = hypyt;
            }

#if DEBUG
            DebugSisenna(-1);
            DebugViesti("-{0} haki {1}", hakija.Nimi, vastustaja.Nimi);
#endif

            return uusiPeli;
        }

        private Pelaaja HaeVastustajaNopeasti(
            List<HakuPeli> kaikkiPelit, 
            Pelaaja[] mukana, 
            Pelaaja hakija, 
            IEnumerable<Pelaaja> vastustajat,
            List<Hyppy> hypyt)
        {
#if DEBUG
            DebugSisenna(1);
#endif
            foreach (var vastustajaEhdokas in vastustajat)
            {
                if (this.PeruutaHaku)
                {
#if DEBUG
                    DebugSisenna(-1);
#endif
                    return null;
                }

                List<string> syyt = new List<string>();
                if (TarkistaHaku(kaikkiPelit, mukana, hakija, vastustajaEhdokas, false, string.Format("({0}-{1})", hakija.Id, vastustajaEhdokas.Id), syyt))
                {
#if DEBUG
                    DebugSisenna(-1);
#endif
                    return vastustajaEhdokas;
                }
                else
                {
                    hypyt.Add(new Hyppy() { Pelaaja = vastustajaEhdokas, Syy = string.Join(", ", syyt.ToArray()) });
                }
            }

#if DEBUG
            DebugSisenna(-1);
#endif
            return null;
        }

        private Pelaaja HaeVastustajaHuolellisesti(
            List<HakuPeli> pelatutPelit, 
            IEnumerable<Pelaaja> pelaajat, 
            Pelaaja hakija, 
            IEnumerable<Pelaaja> vastustajat, 
            string tilanneAvain,
            string polku,
            Tyypit.EvaluointiTietokanta evaluoidutTilanteet,
            List<Hyppy> hypyt)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            var parasPisteytys = new Tyypit.EvaluointiTietokanta.Evaluointi() { Pisteytys = float.MaxValue, Kuvaus = polku };
            Pelaaja parasHaku = null;

            if (this.PeruutaHaku)
            {
#if DEBUG
                DebugSisenna(-1);
#endif
                return null;
            }

            List<Hyppy> paikallisetHypyt = null;
            if (hypyt != null)
            {
                paikallisetHypyt = new List<Hyppy>();
            }

            foreach (var vastustaja in vastustajat)
            {
#if DEBUG
                StringBuilder lokiRivi = null;
                if (hypyt != null)
                {
                    lokiRivi = new StringBuilder();
                    lokiRivi.Append(string.Format("- ({0} - {1}) = ", hakija.Nimi, vastustaja.Nimi));
                }
#endif
                var pisteytys = PisteytaHaku(pelatutPelit, pelaajat, hakija, vastustaja, tilanneAvain, polku, evaluoidutTilanteet);
                if (pisteytys.Pisteytys < parasPisteytys.Pisteytys)
                {
                    parasPisteytys = pisteytys;
                    parasHaku = vastustaja;
                }

#if DEBUG
                if (hypyt != null)
                {
                    lokiRivi.Append(string.Format("{0}      ({1})", pisteytys.Pisteytys, pisteytys.Kuvaus));
                    DebugViesti(lokiRivi.ToString());
                }
#endif

                if (pisteytys.Pisteytys <= 0.0f) // Heti kun löydetään virheetön haku, voidaan lopettaa
                {
                    break;
                }
                else if (paikallisetHypyt != null)
                {
                    string kuvaus = pisteytys.Kuvaus;

                    if (pisteytys.Pelaaja1 != null && pisteytys.Pelaaja2 != null)
                    {
                        paikallisetHypyt.Add(new Hyppy()
                        {
                            Pelaaja = vastustaja,
                            Syy = pisteytys.Pisteytys < 1000.0f ?
                                string.Format("Uusintaottelu {1}-{2} jos pelit etenee {0}", kuvaus, pisteytys.Pelaaja1.Id, pisteytys.Pelaaja2.Id) :
                                string.Format("Toinen pelaaja kaksi kierrosta edellä pelissä {1}-{2} jos pelit etenee {0}", kuvaus, pisteytys.Pelaaja1.Id, pisteytys.Pelaaja2.Id)
                        });
                    }
                    else
                    {
                        string syy = string.Empty;

                        if (pisteytys.Pisteytys >= 1000.0f)
                        {
                            syy = string.Format("Toinen pelaaja kaksi kierrosta edellä jos pelit etenee {0}", kuvaus);
                        }
                        else if (pisteytys.Pisteytys >= 0.1f)
                        {
                            syy = string.Format("Uusintaottelu jos pelit etenee {0}", kuvaus);
                        }

                        paikallisetHypyt.Add(new Hyppy()
                        {
                            Pelaaja = vastustaja,
                            Syy = syy
                        });
                    }
                }
            }

            if (hypyt != null && parasHaku != null)
            {
#if DEBUG
                DebugViesti("-# ==> Haettiin {0}", parasHaku.Nimi);
#endif
                foreach (var vastustaja in vastustajat)
                {
                    if (vastustaja == parasHaku)
                    {
                        break;
                    }

                    var hyppy = paikallisetHypyt.FirstOrDefault(x => x.Pelaaja == vastustaja);
                    if (hyppy != null)
                    {
                        hypyt.Add(hyppy);
                    }
                }
            }

#if DEBUG
            DebugSisenna(-1);
#endif
            return parasHaku;
        }

        private string PelinAvain(HakuPeli peli)
        {
            switch (peli.Tulos)
            {
                case PelinTulos.Pelaaja1Voitti: return string.Format("({0}v-{1})", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                case PelinTulos.Pelaaja2Voitti: return string.Format("({0}-{1}v)", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                default: return string.Format("({0}-{1})", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
            }
        }

        private Tyypit.EvaluointiTietokanta.Evaluointi PisteytaHaku(
            List<HakuPeli> pelatutPelit,
            IEnumerable<Pelaaja> pelaajat,
            Pelaaja hakija,
            Pelaaja vastustaja,
            string tilanneAvain,
            string polku,
            Tyypit.EvaluointiTietokanta evaluoidutTilanteet)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            HakuPeli peli = LisaaPeli(null, pelatutPelit, null, hakija, vastustaja);

            pelatutPelit.Add(peli);

            peli.Tilanne = PelinTilanne.Pelattu;
            peli.Tulos = PelinTulos.Pelaaja1Voitti;

            string avainA = PelinAvain(peli);
            var pisteytysA = PisteytaTilanne(
                pelatutPelit,
                pelaajat,
                string.Format("{0} {1}", tilanneAvain, avainA),
                string.Format("{0} {1}", polku, avainA),
                evaluoidutTilanteet);

            // Jos A haarassa tulee kierrosvirhe, ei tarvitse evaluoida B haaraa
            if (pisteytysA.Pisteytys >= PisteytysSakkoKierrosVirheesta)
            {
#if DEBUG
                DebugViesti("PisteytaHaku {0} ---- {1} - {2} = {3} (optimointi)", polku, hakija.Nimi, vastustaja.Nimi, pisteytysA.Pisteytys);
                DebugSisenna(-1);
#endif
                pelatutPelit.RemoveAt(pelatutPelit.Count - 1);
                return pisteytysA;
            }

            peli.Tulos = PelinTulos.Pelaaja2Voitti;

            string avainB = PelinAvain(peli);
            var pisteytysB = PisteytaTilanne(
                pelatutPelit,
                pelaajat,
                string.Format("{0} {1}", tilanneAvain, avainB),
                string.Format("{0} {1}", polku, avainB),
                evaluoidutTilanteet);
            
            pelatutPelit.RemoveAt(pelatutPelit.Count - 1);

            if (pisteytysA.Pisteytys > pisteytysB.Pisteytys)
            {
#if DEBUG
                DebugViesti("PisteytaHaku {0} ---- {1} - {2} = {3}", polku, hakija.Nimi, vastustaja.Nimi, pisteytysA.Pisteytys);
                DebugSisenna(-1);
#endif
                return pisteytysA;
            }
            else 
            {
#if DEBUG
                DebugViesti("PisteytaHaku {0} ---- {1} - {2} = {3}", polku, hakija.Nimi, vastustaja.Nimi, pisteytysB.Pisteytys);
                DebugSisenna(-1);
#endif
                return pisteytysB;
            }
        }

        private Tyypit.EvaluointiTietokanta.Evaluointi PisteytaTilanne(
            List<HakuPeli> pelatutPelit,
            IEnumerable<Pelaaja> pelaajat,
            string tilanneAvain,
            string polku,
            Tyypit.EvaluointiTietokanta evaluoidutTilanteet)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            if (evaluoidutTilanteet.HaeEvaluointi(tilanneAvain, out Tyypit.EvaluointiTietokanta.Evaluointi evaluointi))
            {
#if DEBUG
                DebugViesti("PisteytaTilanne {0} = {1} (cached)", polku, evaluointi.Pisteytys);
                DebugSisenna(-1);
#endif
                return evaluointi;
            }

            Tyypit.EvaluointiTietokanta.Evaluointi pisteytys = new Tyypit.EvaluointiTietokanta.Evaluointi() 
            { 
                Pisteytys = 0.0f, 
                Kuvaus = polku 
            };

            var mukana = pelaajat.Where(x => MukanaHuolellisessaHaussa(pelatutPelit, x.Id));
            if (mukana.Count() == 2)
            {
                // Finaali(t) menossa. Ei tarvitse evaluoida syvemmälle

                HakuPeli finaali = LisaaPeli(null, pelatutPelit, null, mukana.First(), mukana.Last());
                finaali.Tilanne = PelinTilanne.Pelattu;
                finaali.Tulos = PelinTulos.Pelaaja2Voitti; // Evaluoinnin kannalta ei ole väliä onko tämä viimeinen finaali tai kumpi voitti

                pelatutPelit.Add(finaali);
                pisteytys.Pisteytys = PisteytaLopputilanne(pelatutPelit, out pisteytys.Pelaaja1, out pisteytys.Pelaaja2);
                pelatutPelit.RemoveAt(pelatutPelit.Count - 1);
            }
            else if (mukana.Count() < 2)
            {
                // Kisa päättynyt, pisteytetään haku
                pisteytys.Pisteytys = PisteytaLopputilanne(pelatutPelit, out pisteytys.Pelaaja1, out pisteytys.Pelaaja2);
            }
            else
            {
                var hakijat = mukana
                    .OrderBy(x => x.Id)
                    .OrderBy(x => LaskePelit(pelatutPelit, x.Id));

                var hakija = hakijat.First();

                var vastustajat = hakijat
                    .Skip(1);

                Pelaaja vastustaja = null;

                if (vastustajat.Count() == 1)
                {
                    vastustaja = vastustajat.First();
                }
                else
                {
                    vastustaja = HaeVastustajaHuolellisesti(pelatutPelit, mukana, hakija, vastustajat, tilanneAvain, polku, evaluoidutTilanteet, null);
                }

                if (vastustaja == null)
                {
#if DEBUG
                    DebugViesti("# Ei löytynyt vastustajaa huolellisella haulla!!!");
                    DebugSisenna(-1);
#endif
                    return new Tyypit.EvaluointiTietokanta.Evaluointi() { Pisteytys = float.MaxValue, Kuvaus = polku };
                }

                pisteytys = PisteytaHaku(pelatutPelit, mukana, hakija, vastustaja, tilanneAvain, polku, evaluoidutTilanteet);
            }

            if (pisteytys.Pisteytys != float.MaxValue)
            {
                evaluoidutTilanteet.TallennaEvaluointi(tilanneAvain, pisteytys);
            }

#if DEBUG
            DebugViesti("PisteytäTilanne {0} = {1}", polku, pisteytys.Pisteytys);
            DebugSisenna(-1);
#endif
            return pisteytys;
        }

        private static float PisteytaLopputilanne(List<HakuPeli> pelatutPelit, out Pelaaja pelaaja1, out Pelaaja pelaaja2)
        {
            float pisteet = 0.0f;

            HakuPeli finaaliPeli = pelatutPelit.Last();

            Pelaaja p1Edella = null;
            Pelaaja p2Edella = null;
            Pelaaja p1Uusinta = null;
            Pelaaja p2Uusinta = null;

            foreach (var peli in pelatutPelit)
            {
                // Valtava sakko jos toinen pelaaja on 2 kierrosta edellä
                if (Math.Abs(peli.KierrosPelaaja1 - peli.KierrosPelaaja2) > 1)
                {
                    pisteet += PisteytysSakkoKierrosVirheesta;
                    p1Edella = peli.Pelaaja1;
                    p2Edella = peli.Pelaaja2;
                }

                // Pikkuruinen sakko jos toinen pelaaja on 1 kierros edellä (jotta haku suosii saman kierroksen hakuja)
                else if (
                    (PisteytysSakkoSeuraavaltaKierrokseltaHausta > 0.0f) &&
                    (peli.KierrosPelaaja1 != peli.KierrosPelaaja2) && 
                    (peli.Kierros > 3))
                {
                    pisteet += PisteytysSakkoSeuraavaltaKierrokseltaHausta * (peli.Kierros - 3);
                }

                // Sakko uusintaotteluista
                foreach (var p in pelatutPelit.Where(x =>
                    x.SisaltaaPelaajat(peli.Pelaaja1.Id, peli.Pelaaja2.Id) &&
                    x.PeliNumero > peli.PeliNumero))
                {
                    if (p != finaaliPeli && p != peli)
                    {
                        pisteet += PisteytysSakkoUusintaOttelusta / p.Kierros; // Tämä antaa paremman pisteytyksen mitä myöhempään kaaviossa uusintaottelu tulee.
                                                        // En tiedä onko perusteltua
                        p1Uusinta = peli.Pelaaja1;
                        p2Uusinta = peli.Pelaaja2;
                    }
                }
            }

            pelaaja1 = p1Edella != null ? p1Edella : p1Uusinta;
            pelaaja2 = p2Edella != null ? p2Edella : p2Uusinta;

            return pisteet;
        }

        private bool TarkistaHaku(
            IEnumerable<HakuPeli> kaikkiPelit, 
            IEnumerable<Pelaaja> pelaajat, 
            Pelaaja hakija, 
            Pelaaja vastustaja, 
            bool rekursiossa,
            string polku,
            List<string> syyt)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            if (!TarkistaKeskenaisetPelit(kaikkiPelit, hakija.Id, vastustaja.Id, rekursiossa))
            {
                syyt.Add(string.Format("Uusintaottelu {0}-{1} jos haetaan {2}", hakija.Id, vastustaja.Id, polku));
#if DEBUG
                DebugViesti(" - !Virhe! - {0} ---- Uusintaottelu {1} - {2}", polku, hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return false;
            }

            List<Pelaaja> mukana = new List<Pelaaja>();
            mukana.AddRange(pelaajat);
            mukana.Remove(hakija);
            mukana.Remove(vastustaja);

            if (mukana.Count() < 2)
            {
#if DEBUG
                if (mukana.Count() == 1)
                {
                    DebugViesti(" - Ok - {0} ---- {1} - {2} ---- {3} huilaa", polku, hakija.Nimi, vastustaja.Nimi, mukana.First().Nimi);
                }
                else 
                {
                    DebugViesti(" - Ok - {0} ---- {1} - {2}", polku, hakija.Nimi, vastustaja.Nimi);
                }
                DebugSisenna(-1);
#endif
                return true;
            }

            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => LaskePelit(kaikkiPelit, x.Id, this.Kierros));

            var seuraavaHakija = hakijat.First();
            var seuraavatVastustajat = hakijat.Skip(1);

            foreach (var seuraavaVastustajaEhdokas in seuraavatVastustajat)
            {
                if (TarkistaHaku(
                    kaikkiPelit, 
                    hakijat, 
                    seuraavaHakija, 
                    seuraavaVastustajaEhdokas, 
                    true, 
                    polku + string.Format(" ({0}-{1})", seuraavaHakija.Id, seuraavaVastustajaEhdokas.Id),
                    syyt))
                {
#if DEBUG
                    DebugViesti(" - Ok - {0} ---- {1} - {2}", polku, seuraavaHakija.Nimi, seuraavaVastustajaEhdokas.Nimi);
                    DebugSisenna(-1);
#endif
                    return true;
                }
            }

            int hakijanKierros = LaskePelit(kaikkiPelit, hakija.Id, this.Kierros);
            int vastustajanKierros = LaskePelit(kaikkiPelit, vastustaja.Id, this.Kierros);
            int kierros = Math.Max(hakijanKierros, vastustajanKierros);
            foreach (var pelaaja in mukana)
            {
                int pelaajanKierros = LaskePelit(kaikkiPelit, pelaaja.Id, this.Kierros);
                if (pelaajanKierros <= kierros)
                {
#if DEBUG
                    DebugViesti(" - !Virhe! - {0} ---- Haku ei onnistu haun {1} - {2} jälkeen", polku, hakija.Nimi, vastustaja.Nimi);
                    DebugSisenna(-1);
#endif
                    return false;
                }
            }

#if DEBUG
            DebugViesti(" - Ok - {0} ---- Haku ei onnistu haun {1} - {2} jälkeen mutta hakija ja vastustaja ovat muita kierroksen perässä", polku, hakija.Nimi, vastustaja.Nimi);
            DebugSisenna(-1);
#endif
            return true;
        }

        private HakuPeli LisaaPeli(List<HakuPeli> kaikkiPelit, List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, Pelaaja pelaaja1, Pelaaja pelaaja2)
        {
            int kierros1 = LaskePelit(pelatutPelit, arvotutPelit, pelaaja1.Id, 99999) + 1;
            int kierros2 = LaskePelit(pelatutPelit, arvotutPelit, pelaaja2.Id, 99999) + 1;
            int kierros = Math.Max(kierros1, kierros2);

            HakuPeli peli = new HakuPeli()
            {
                Kierros = Math.Max(kierros1, kierros2),
                KierrosPelaaja1 = kierros1,
                KierrosPelaaja2 = kierros2,
                PeliNumero = 0,
                Pelaaja1 = pelaaja1,
                Pelaaja2 = pelaaja2,
                Tulos = PelinTulos.EiTiedossa,
                Tilanne = PelinTilanne.Tyhja,
                PeliOnPelattuKaaviossa = false
            };

            if (kaikkiPelit != null)
            {
                peli.PeliNumero = kaikkiPelit.Count;
            }
            else
            {
                if (pelatutPelit != null)
                {
                    peli.PeliNumero += pelatutPelit.Count;
                }

                if (arvotutPelit != null)
                {
                    peli.PeliNumero += arvotutPelit.Count;
                }
            }

            peli.PeliNumero += 1;

            return peli;
        }

        private int LaskePelit(List<HakuPeli> pelit, int pelaaja)
        {
            return pelit.Count(x => x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskeKeskenaisetPelit(IEnumerable<HakuPeli> pelit, int pelaaja1, int pelaaja2)
        {
            return pelit.Count(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2));
        }

        private bool TarkistaKeskenaisetPelit(IEnumerable<HakuPeli> pelit, int pelaaja1, int pelaaja2, bool rekursiossa)
        {
            var peli = pelit.FirstOrDefault(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2));
            if (peli != null)
            {
                if (!rekursiossa)
                {
                    return false;
                }

                if (peli.Tilanne != PelinTilanne.Pelattu)
                {
                    if (LaskeTappiot(pelit, pelaaja1, this.Kierros) >= 1 &&
                        LaskeTappiot(pelit, pelaaja2, this.Kierros) >= 1)
                    {
                        return true; // Toinen pelaaja putoaa varmasti tästä pelistä, ei siis haittaa että ovat pelanneet
                    }
                }

                return false;
            }

            return true; // Ei keskinaisiä pelejä
        }

        private bool MukanaHuolellisessaHaussa(List<HakuPeli> pelit, int pelaaja)
        {
            if (LaskeTappiotHuolellisessaHaussa(pelit, pelaaja) > 1)
            {
                return false;
            }

            return true;
        }

        private bool Mukana(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja, int kierros)
        {
            if (LaskeTappiot(pelatutPelit, arvotutPelit, pelaaja) > 1)
            {
                return false;
            }

            if (LaskePelit(pelatutPelit, arvotutPelit, pelaaja, kierros) >= kierros)
            {
                return false;
            }

            return true;
        }

        private bool VarmastiMukana(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja, int kierros)
        {
            int tappiot = LaskeTappiot(pelatutPelit, arvotutPelit, pelaaja);
            if (tappiot > 1)
            {
                return false;
            }

            int kesken = LaskeKeskeneraisetPelit(pelatutPelit, arvotutPelit, pelaaja, kierros);
            if ((tappiot + kesken) > 1)
            {
                return false;
            }

            int pudaritKesken = LaskeKeskeneraisetPudariPelit(pelatutPelit, arvotutPelit, pelaaja, kierros);
            if (pudaritKesken > 0)
            {
                return false;
            }

            return true;
        }

        private int LaskeTappiot(IEnumerable<HakuPeli> kaikkiPelit, int pelaaja, int kierros)
        {
            int tappiot = kaikkiPelit.Count(x => x.Kierros <= kierros && x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja));
            if (tappiot < 2)
            {
                if (kaikkiPelit.Any(x => x.Kierros <= kierros &&
                    x.SisaltaaPelaajan(pelaaja) &&
                    x.Tilanne == PelinTilanne.Pelattu &&
                    x.Havisi(pelaaja) &&
                    x.Kierros >= this.EkaPudariKierros))
                {
                    tappiot = 2;
                }
            }

            return tappiot;
        }

        private int LaskeTappiotHuolellisessaHaussa(List<HakuPeli> pelit, int pelaaja)
        {
            int tappiot = pelit.Count(x => x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja));
            if (tappiot < 2)
            {
                if (pelit.Any(x =>
                    x.SisaltaaPelaajan(pelaaja) &&
                    x.Tilanne == PelinTilanne.Pelattu &&
                    x.Havisi(pelaaja) &&
                    x.Kierros >= this.EkaPudariKierros))
                {
                    tappiot = 2;
                }
            }

            return tappiot;
        }

        private int LaskeTappiot(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            int tappiot = 
                pelatutPelit.Count(x => x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja)) +
                arvotutPelit.Count(x => x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja));

            if (tappiot < 2)
            {
                if (pelatutPelit.Any(x => 
                    x.SisaltaaPelaajan(pelaaja) && 
                    x.Tilanne == PelinTilanne.Pelattu && 
                    x.Havisi(pelaaja) && 
                    x.Kierros >= this.EkaPudariKierros))
                {
                    tappiot = 2;
                }
                else if (arvotutPelit.Any(x =>
                    x.SisaltaaPelaajan(pelaaja) &&
                    x.Tilanne == PelinTilanne.Pelattu &&
                    x.Havisi(pelaaja) &&
                    x.Kierros >= this.EkaPudariKierros))
                {
                    tappiot = 2;
                }
            }


            return tappiot;
        }

        private int LaskeKeskeneraisetPelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja, int kierros)
        {
            return pelatutPelit.Count(x =>
                x.Kierros <= kierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu) +
                arvotutPelit.Count(x =>
                x.Kierros <= kierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu);
        }

        private int LaskeKeskeneraisetPudariPelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja, int kierros)
        {
            return pelatutPelit.Count(x =>
                x.Kierros <= kierros &&
                x.Kierros >= this.EkaPudariKierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu) +
                arvotutPelit.Count(x =>
                x.Kierros <= kierros &&
                x.Kierros >= this.EkaPudariKierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu);
        }

        private int LaskePelit(IEnumerable<HakuPeli> pelit, int pelaaja, int kierros)
        {
            return pelit.Count(x => x.Kierros <= kierros && x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskePelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja, int kierros)
        {
            int peleja = pelatutPelit.Count(x => x.Kierros <= kierros && x.SisaltaaPelaajan(pelaaja));

            if (arvotutPelit != null)
            {
                peleja += arvotutPelit.Count(x => x.Kierros <= kierros && x.SisaltaaPelaajan(pelaaja));
            }

            return peleja;
        }
    }
}
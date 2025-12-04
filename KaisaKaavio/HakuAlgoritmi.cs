using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using static KaisaKaavio.HakuAlgoritmi;
using static KaisaKaavio.Tyypit.EvaluointiTietokanta;

namespace KaisaKaavio
{ 
    public class HakuAlgoritmi : IHakuAlgoritmi
    {
        private static readonly int PisteytysSakkoKierrosVirheesta = 1023;
        private static readonly int PisteytysSakkoUusintaOttelusta = 100;
        private static readonly int PisteytysSakkoTuplaUusintaOttelusta = 500;
        private static readonly int PisteytysSakkoSeuraavaltaKierrokseltaHausta = 20;
        private static readonly int PisteytysSakkoHakijanYliHypysta = 1;

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
            public int PelinumeroKierroksella = 0;
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
            public List<HakuPeli> Pelit = null;
            public int Id = 0;
            public string Nimi = string.Empty;
            public int Tappioita = 0;
            public int PelattujaPeleja = 0;
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
        public bool PeruutaHaku { get; set; } = false;
        public bool UusiHakuTarvitaan { get; private set; } = false;
        public bool AutomaattinenTestausMenossa { get; set; } = false;

        Kilpailu Kilpailu = null;
        int EkaPudariKierros = 0;
        bool HakuKesken = true;

        // Tällä estetään hakeminen "Kaavioiden yhdistäminen"-kierroksen yli kun pelataan usealla pelipaikalla
        public int MaxKierros { get; set; } = Int32.MaxValue;

        public HakuAlgoritmi(Kilpailu kilpailu, Loki loki, IStatusRivi status)
        {
            this.status = status;
            this.Kilpailu = kilpailu;
            this.HakuVirhe = null;
            this.UudetPelit = new List<Pelaajat>();

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
            if (!this.AutomaattinenTestausMenossa)
            {
                DebugSisennys += i;
            }
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

                foreach (var pp in Pelatut)
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

                foreach (var pp in PelitKesken)
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

            var osallistujat = this.Kilpailu.Osallistujat.Where(x => x.Id >= 0);
            var mukana = osallistujat.Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id, 99999));

            if (mukana.Count() < 2)
            {
#if DEBUG
                DebugViesti("Mukana alle 2 pelaajaa. Haku on päättynyt");
#endif
                return false;
            }

            int kierros = mukana
                .Select(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999))
                .Min();

            // Jos on vain yksi pelaaja hakemassa kierroksella (huilaaja), siirrytään hakemaan seuraavaa kierrosta
            if (mukana.Count(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999) == kierros) == 1)
            {
                kierros++;
            }

            kierros++;

            if (kierros < 3)
            {
                kierros = 3;
            }

#if DEBUG
            foreach (var peli in pelatutPelit)
            {
                if (peli.Tilanne != PelinTilanne.Pelattu)
                {
                    throw new Exception("Bugi! Pelejä pelaamatta haettaessa uusia pelejä");
                }
            }

            if (!this.AutomaattinenTestausMenossa)
            {
                Debug.WriteLine(string.Format("Haetaan pelejä kierrokselle {0}. Mukana : {1}", 
                    kierros, 
                    string.Join(",", mukana.Select(x => string.Format("{0}[{1}]({2})", x.Nimi, LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999), x.Id)).ToArray())));
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

                var peli = HaeSeuraavaPeliKierrokselle(kierros, pelit, pelatutPelit, arvotutPelit, pelaajat);
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

                    if (peli.Kierros > kierros)
                    {
#if DEBUG
                        DebugViesti("Haettiin peli seuraavalle kierrokselle. Haku keskeytetään tältä erää");
#endif

                        this.UusiHakuTarvitaan = true;
                        break;
                    }

                    if (mukana.Count() <= Asetukset.HuolellisenHaunPelaajamaara)
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
                pelaajat.Count(x => LaskePelit(pelit, x.Id) < kierros) == 1)
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

        private HakuPeli HaeSeuraavaPeliKierrokselleHuolellisesti(
            int kierros,
            List<HakuPeli> kaikkiPelit,
            List<HakuPeli> pelatutPelit,
            List<HakuPeli> arvotutPelit,
            List<Pelaaja> pelaajat,
            IOrderedEnumerable<Pelaaja> mukana)
        {
            //List<HakuPeli> uudetPelit = new List<HakuPeli>();
            //List<Hyppy> hypyt = new List<Hyppy>();
            List<HakuPelaaja> haussaMukana = new List<HakuPelaaja>();

            foreach (var p in mukana)
            {
                HakuPelaaja hakuPelaaja = new HakuPelaaja()
                {
                    Pelaaja = p,
                    Id = p.Id,
                    Nimi = p.Nimi,
                    PelattujaPeleja = LaskePelit(kaikkiPelit, p.Id),
                    Tappioita = LaskeTappiot(kaikkiPelit, p.Id, 9999),
                    Pelit = new List<HakuPeli>()
                };

                foreach (var peli in kaikkiPelit.Where(x => x.SisaltaaPelaajan(p.Id)))
                {
                    hakuPelaaja.Pelit.Add(peli);
                }

                haussaMukana.Add(hakuPelaaja);
            }

            var haku = HaeHuolellisesti(pelatutPelit, haussaMukana, 0);
            if (haku == null)
            {
                return null;
            }

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, haku.Hakija.Pelaaja.Id, kierros))
            {
                return null;
            }

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, haku.Vastustaja.Pelaaja.Id, kierros))
            {
                return null;
            }

            if (PelitKesken.Any() && haku.Pisteytys >= PisteytysSakkoSeuraavaltaKierrokseltaHausta)
            {
                // Ei haeta huonoja hakuja jos pelejä on vielä kesken
                return null;
            }

            var uusiPeli = LisaaPeli(kaikkiPelit, pelatutPelit, arvotutPelit, haku.Hakija.Pelaaja, haku.Vastustaja.Pelaaja);

            if (uusiPeli.Kierros > this.MaxKierros)
            {
#if DEBUG
                DebugViesti("Algoritmi haki pelin {0} - {1} jossa toinen pelaaja liian pitkällä (MaxKierros). Hylätään tämä haku", 
                    haku.Hakija.Nimi,
                    haku.Vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return null;
            }

            if (Math.Abs(uusiPeli.KierrosPelaaja1 - uusiPeli.KierrosPelaaja2) > 1)
            {
                if (PelitKesken.Any())
                {
#if DEBUG
                   // DebugViesti("Algoritmi haki pelin jossa toinen pelaaja on kaksi kierrosta edellä {0} - {1}. Pelejä on vielä kesken joten hylätään tämä haku ja haetaan myöhemmin uudelleen", hakija.Nimi, vastustaja.Nimi);
                    DebugSisenna(-1);
#endif
                    return null;
                }
                else
                {
#if DEBUG
                    //Debug.WriteLine(string.Format("## Hakuvirhe !!! Haettiin peli jossa toinen pelaaja on kaksi kierrosta edellä {0} - {1} !!!", hakija.Nimi, vastustaja.Nimi));
#endif
                }
            }

            //if (hypyt.Any())
            //{
            //    uusiPeli.Hypyt = hypyt;
            //}

#if DEBUG
            DebugSisenna(-1);
            //DebugViesti("-{0} haki {1}", hakija.Nimi, vastustaja.Nimi);
#endif

            return uusiPeli;
        }

        private HakuPeli HaeSeuraavaPeliKierrokselle(
            int kierros,
            List<HakuPeli> kaikkiPelit, 
            List<HakuPeli> pelatutPelit, 
            List<HakuPeli> arvotutPelit, 
            List<Pelaaja> pelaajat)
        {
#if DEBUG
            DebugSisenna(1);
            DebugViesti("---====---");
#endif

            var mukana = pelaajat
                .Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id, 99999))
                .OrderBy(x => x.Id);

            if (mukana.Count() <= Asetukset.HuolellisenHaunPelaajamaara)
            {
                return HaeSeuraavaPeliKierrokselleHuolellisesti(kierros, kaikkiPelit, pelatutPelit, arvotutPelit, pelaajat, mukana);
            }

            // Erikoistapaus kun on kolme pelaajaa jäljellä. Saatetaan joutua "hyppäämään hakijan yli"
            if (mukana.Count() == 3)
            {
                var eka = mukana.ElementAt(0);
                var toka = mukana.ElementAt(1);
                var kolmas = mukana.ElementAt(2);

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

            var hakijat = mukana
                .Where(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999) < kierros)
                .OrderBy(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999));

            if (!hakijat.Any())
            {
#if DEBUG
                DebugSisenna(-1);
#endif
                return null;

            }

            var hakija = hakijat.First();

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, hakija.Id, 99999))
            {
#if DEBUG
                DebugViesti("Hakija {0} ei välttämättä mukana kierroksella {1}. Pysäytetään haku", hakija.Nimi, kierros);
                DebugSisenna(-1);
#endif
                return null;
            }

            int hakijanKierros = LaskePelit(pelatutPelit, arvotutPelit, hakija.Id, 99999);

            var vastustajat = mukana
                .Where(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999) >= hakijanKierros)
                .OrderBy(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999))
                .Where(x => x != hakija);

            // Usean pelipaikan kilpailussa estettävä hakemasta "pelipaikkojen yhdistäminen" kierroksen yli
            if ((this.MaxKierros < Int32.MaxValue) &&
                (LaskePelit(kaikkiPelit, hakija.Id) < this.MaxKierros))
            {
                vastustajat = vastustajat.Where(x => LaskePelit(kaikkiPelit, x.Id) < this.MaxKierros);
            }

            if (!vastustajat.Any())
            {
#if DEBUG
                DebugViesti("Hakija {0} huilaa. Pysäytetään haku", hakija.Nimi);
                DebugSisenna(-1);
#endif
                return null;
            }

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                DebugViesti("{0} hakee {1}",
                    string.Format("{0}[{1}]", hakija.Nimi, LaskePelit(pelatutPelit, arvotutPelit, hakija.Id, 99999)),
                    string.Join(",", vastustajat.Select(x =>
                        string.Format("{0}[{1}]", x.Nimi, LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999))).ToArray()));
            }
#endif

            Pelaaja vastustaja = null;
            List<Hyppy> hypyt = new List<Hyppy>();

            if (vastustajat.Count() == 1)
            {
                vastustaja = vastustajat.First();
            }
            else
            { 
                vastustaja = HaeVastustajaNopeasti(
                    kierros,
                    kaikkiPelit,
                    mukana,
                    hakija,
                    hakijat,
                    vastustajat,
                    hypyt);
            }

            if (vastustaja == null) 
            {
                if (this.PelitKesken.Any())
                {
#if DEBUG
                    DebugViesti("-Kaikki haut olivat huonoja. Kaaviossa on vielä pelejä kesken. Keskeytetään haku tältä erää");
                    DebugSisenna(-1);
#endif
                    return null;
                }
                else
                {
                    // Jos tähän on tultu, kaikilla hakujärestyksillä tulee uusintapelejä (toivottavasti ollaan finaalissa tai lähellä, muuten kaaviossa on tapahtunut hakuvirhe)
#if DEBUG
                    DebugViesti("-Kaikki haut olivat huonoja. Haetaan ensimmäinen vastustaja listalta");
#endif
                    vastustaja = vastustajat.First();
                }
            }

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, vastustaja.Id, 99999))
            {
#if DEBUG
                DebugViesti("-Vastustaja {0} ei välttämättä mukana kierroksella {1}. Pysäytetään haku", vastustaja.Nimi, kierros);
                DebugSisenna(-1);
#endif
                return null;
            }

            if (mukana.Count() > 2 &&
                LaskeKeskenaisetPelit(kaikkiPelit, hakija.Id, vastustaja.Id) > 0)
            {
                if (PelitKesken.Any())
                {
                    // Jos hakija on pelannut jo kaikkia muita vastaan niin mennään tällä haulla (ei tarvii odottaa turhaan)
                    if (!mukana.Any(x => x != hakija && LaskeKeskenaisetPelit(kaikkiPelit, hakija.Id, x.Id) == 0))
                    {
#if DEBUG
                        DebugViesti("Hakija on jo pelannut kaikkien muiden kanssa. Annetaan hakea uusintaottelu {0} - {1}", hakija.Nimi, vastustaja.Nimi);
#endif
                        //return null;
                    }
                    else
                    {
#if DEBUG
                        DebugViesti("Algoritmi haki uusintaottelun {0} - {1}. Pelejä on vielä kesken joten hylätään tämä haku ja haetaan myöhemmin uudelleen", hakija.Nimi, vastustaja.Nimi);
                        DebugSisenna(-1);
#endif
                        return null;
                    }
                }
                else
                {
#if DEBUG
                    Debug.WriteLine(string.Format("## Hakuvirhe !!! Haettiin uusintaottelu {0} - {1} !!!", hakija.Nimi, vastustaja.Nimi));
#endif
                }
            }

            var uusiPeli = LisaaPeli(kaikkiPelit, pelatutPelit, arvotutPelit, hakija, vastustaja);

            if (uusiPeli.Kierros > this.MaxKierros)
            {
#if DEBUG
                DebugViesti("Algoritmi haki pelin {0} - {1} jossa toinen pelaaja liian pitkällä (MaxKierros). Hylätään tämä haku", hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return null;
            }

            if (Math.Abs(uusiPeli.KierrosPelaaja1 - uusiPeli.KierrosPelaaja2) > 1)
            {
                if (PelitKesken.Any())
                {
#if DEBUG
                    DebugViesti("Algoritmi haki pelin jossa toinen pelaaja on kaksi kierrosta edellä {0} - {1}. Pelejä on vielä kesken joten hylätään tämä haku ja haetaan myöhemmin uudelleen", hakija.Nimi, vastustaja.Nimi);
                    DebugSisenna(-1);
#endif
                    return null;
                }
                else
                {
#if DEBUG
                    Debug.WriteLine(string.Format("## Hakuvirhe !!! Haettiin peli jossa toinen pelaaja on kaksi kierrosta edellä {0} - {1} !!!", hakija.Nimi, vastustaja.Nimi));
#endif
                }
            }

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
            int kierros,
            List<HakuPeli> kaikkiPelit, 
            IEnumerable<Pelaaja> mukana, 
            Pelaaja hakija, 
            IEnumerable<Pelaaja> hakijat,
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
                List<HakuPeli> kaikkiHakupelit = new List<HakuPeli>();
                kaikkiHakupelit.AddRange(kaikkiPelit);

                if (TarkistaHaku(
                    kierros, 
                    kaikkiHakupelit, 
                    mukana,
                    hakijat,
                    vastustajat,
                    hakija, 
                    vastustajaEhdokas, 
                    false, 
                    string.Format("({0}-{1})", hakija.Id, vastustajaEhdokas.Id), 
                    syyt))
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

        public class HuolellisenHaunTulos
        {
            public HakuPelaaja Hakija { get; set; } = null;
            public HakuPelaaja Vastustaja { get; set; } = null;
            public int Pisteytys { get; set; } = 0;
            //public Tyypit.EvaluointiTietokanta.Evaluointi Pisteytys { get; set; } = null;
        }

        /*
        public class HaunPisteytys
        {
            public int Haku { get; set; } = -1;
            public Tyypit.EvaluointiTietokanta.Evaluointi Pisteytys { get; set; } = null;
        }
        */

        private HuolellisenHaunTulos HaeHuolellisesti(
            List<HakuPeli> pelatutPelit,
            IEnumerable<HakuPelaaja> mukana,
            int pisteytysSumma)
        {
#if DEBUG
            DebugSisenna(1);

            HakuPelaaja tallennettuHakija = null;
            HakuPelaaja tallennettuVastustaja = null;
#endif

            ulong hakuAvain = 0;
            Tyypit.EvaluointiTietokanta.ParasHaku tallennettuHaku = null;

            if (mukana.Count() <= Asetukset.MaxPelaajiaJottaHautTallennetaan)
            {
                hakuAvain = TilanneAvain(pelatutPelit, mukana);
                tallennettuHaku = Tyypit.EvaluointiTietokanta.AnnaParasHakuTilanteessa(hakuAvain);
                if (tallennettuHaku != null)
                {
                    var pelaajat = mukana.OrderBy(x => x.Id);
                    var hakija = pelaajat.ElementAt(tallennettuHaku.Hakija);
                    var vastustaja = pelaajat.ElementAt(tallennettuHaku.Vastustaja);

#if DEBUG
                    tallennettuHakija = hakija;
                    tallennettuVastustaja = vastustaja;

                    DebugSisenna(-1);
#endif
                    return new HuolellisenHaunTulos()
                    {
                        Hakija = hakija,
                        Vastustaja = vastustaja,
                        Pisteytys = pisteytysSumma + tallennettuHaku.Pisteytys
                    };
                }
            }

            int? parasPisteytys = null;
            HakuPelaaja parasHakija = null;
            HakuPelaaja parasVastustaja = null;

            //List<Hyppy> paikallisetHypyt = null;
            //if (hypyt != null)
            //{
            //    paikallisetHypyt = new List<Hyppy>();
            //}

            // TODO!!! Tarviiko lajitella vai ovatko jo oikein
            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => x.PelattujaPeleja)
                .ToArray();

            int minKierros = hakijat.Select(x => x.PelattujaPeleja).Min();

            int maxI = 1;

            if (mukana.Count() <= Asetukset.MaxPelaajiaJottaSaaHyppiaHakijanYli)
            {
                maxI = Math.Min(hakijat.Length - 1, Asetukset.SallittujaHakijanYliHyppyjaHuolellisessaHaussa + 1);
            }

            for (int i = 0; i < maxI; ++i)
            {
                if (parasPisteytys == null || parasPisteytys.Value > 0.0f)
                {
                    var hakija = hakijat[i];
                    if (hakija.PelattujaPeleja > minKierros + 1)
                    {
                        break;
                    }

                    foreach (var vastustaja in hakijat.Skip(i + 1).Where(x => 
                        x != hakija && 
                        x.PelattujaPeleja >= hakija.PelattujaPeleja &&
                        x.PelattujaPeleja <= hakija.PelattujaPeleja + 1))
                    {
                        if (this.PeruutaHaku)
                        {
#if DEBUG
                            DebugSisenna(-1);
#endif
                            return null;
                        }

                        if (vastustaja.PelattujaPeleja > minKierros + 1)
                        {
                            break;
                        }

                        var pisteytys = PisteytaHaku(
                            pelatutPelit,
                            mukana,
                            hakija,
                            vastustaja);

                        pisteytys += i * PisteytysSakkoHakijanYliHypysta;

                        pisteytys = Math.Min(PisteytysSakkoKierrosVirheesta, pisteytys);

                        if (parasPisteytys == null || (pisteytys < parasPisteytys.Value))
                        {
                            parasPisteytys = pisteytys;
                            parasHakija = hakija;
                            parasVastustaja = vastustaja;
                        }

                        if (pisteytys <= 0) // Täydellinen haku, ei tarvitse etsiä kauempaa
                        {
                            break;
                        }
                    }
                }
            }

#if DEBUG
            DebugSisenna(-1);
#endif

            if (parasHakija == null)
            {
                return null;
            }

            if (mukana.Count() <= Asetukset.MaxPelaajiaJottaHautTallennetaan)
            {
#if DEBUG
                if (tallennettuVastustaja == null)
#endif
                if (hakuAvain != 0)
                {
                    var pelaajat = mukana.OrderBy(x => x.Id).ToList();
                    Tyypit.EvaluointiTietokanta.TallennaHaku(hakuAvain, 
                        pelaajat.IndexOf(parasHakija),
                        pelaajat.IndexOf(parasVastustaja),
                        parasPisteytys.Value);

#if DEBUG
                    /*
                    if (pelaajat.Count <= 3)
                    {
                        Debug.WriteLine(string.Format("### {0} => {1} - {2} => {3}", 
                            hakuAvain,
                            pelaajat.IndexOf(parasHakija),
                            pelaajat.IndexOf(parasVastustaja),
                            parasPisteytys.Pisteytys));
                    }
                    */
#endif
                }
#if DEBUG
                else
                {
                    if (tallennettuVastustaja != parasVastustaja)
                    {
                        int iii = 0; //"BUGSU!!!";
                    }

                    if (tallennettuHakija != parasHakija)
                    {
                        int kkk = 0;
                    }

                    if (tallennettuHaku.Pisteytys != parasPisteytys.Value)
                    {
                        int jjj = 0;
                    }
                }
#endif
            }

            return new HuolellisenHaunTulos()
            {
                Hakija = parasHakija,
                Vastustaja = parasVastustaja,
                Pisteytys = Math.Min(PisteytysSakkoKierrosVirheesta, parasPisteytys.Value + pisteytysSumma)
            };
        }

        /*
        private string PelinAvain(HakuPeli peli, bool kierrosVirhe, bool uusintaOttelu)
        {
            if (kierrosVirhe)
            {
                switch (peli.Tulos)
                {
                    case PelinTulos.Pelaaja1Voitti: return string.Format("[[{0}v-{1}]]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    case PelinTulos.Pelaaja2Voitti: return string.Format("[[{0}-{1}v]]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    default: return string.Format("[[{0}-{1}]]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                }
            }
            else if (uusintaOttelu)
            {
                switch (peli.Tulos)
                {
                    case PelinTulos.Pelaaja1Voitti: return string.Format("[{0}v-{1}]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    case PelinTulos.Pelaaja2Voitti: return string.Format("[{0}-{1}v]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    default: return string.Format("[{0}-{1}]", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                }
            }
            else
            {
                switch (peli.Tulos)
                {
                    case PelinTulos.Pelaaja1Voitti: return string.Format("({0}v-{1})", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    case PelinTulos.Pelaaja2Voitti: return string.Format("({0}-{1}v)", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                    default: return string.Format("({0}-{1})", peli.Pelaaja1.Id, peli.Pelaaja2.Id);
                }
            }
        }
        */

#if DEBUG
        static Dictionary<int, int> s_Bittimaara = new Dictionary<int, int>();
        static Dictionary<string, ulong> s_Avaimet = new Dictionary<string, ulong>();
#endif

        private ulong TilanneAvain(List<HakuPeli> pelatutPelit, IEnumerable<HakuPelaaja> pelaajat)
        {
            var mukana = pelaajat.OrderBy(x => x.Id);

            int minPeleja = mukana.Select(x => x.PelattujaPeleja).Min();

            ulong mask = 0;

#if DEBUG
            int bitti = 0;

            if (mukana.Count() == 0)
            {
                throw new Exception("Bugi");
            }

            if (mukana.Count() > 8)
            {
                throw new Exception("Bugi. Huolellinen haku mahdollista enintään 8 pelaajalle");
            }
#endif

            //mask |= 1; // Tämä bitti auttaa erottamaan eri pelaajamäärien koodit jotka loppuvat useampaan nollaan
            //mask <<= 1;
#if DEBUG
            //bitti += 1;
#endif
            mask |= (uint)(mukana.Count() - 1);
            //mask <<= 3;
#if DEBUG
            bitti += 3;
#endif

            foreach (var pelaaja in mukana)
            {
                // Pelaajan kierros (suhteessa muihinpelaajiin)             // 2 bittiä

                int kierros = pelaaja.PelattujaPeleja - minPeleja;
#if DEBUG
                if (kierros < 0 || kierros > 3)
                {
                    throw new Exception("Bugi");
                }
#endif

                mask <<= 2;
                mask |= (uint)(kierros);
#if DEBUG
                bitti += 2;
#endif

                // Pelaaja puhtaalla vai ei:                                // 1 bitti
                if (LaskeTappiotHuolellisessaHaussa(pelatutPelit, pelaaja.Id) > 0)
                {
                    mask <<= 1;
                    mask |= 1;
#if DEBUG
                    bitti += 1;
#endif
                }
                else
                {
                    mask <<= 1;
#if DEBUG
                    bitti += 1;
#endif
                }

                // Miten pelannut muita vastaan                             // 2 bittiä per pelaaja
                foreach (var vastustaja in mukana.Where(x => x.Id > pelaaja.Id))
                {
                    int keskenaisiaPeleja = LaskeKeskenaisetPelit(pelatutPelit, pelaaja.Id, vastustaja.Id);

#if DEBUG
                    if (keskenaisiaPeleja < 0 || keskenaisiaPeleja > 3)
                    {
                        throw new Exception("Bugi");
                    }
#endif

                    mask <<= 2;
                    mask |= (uint)keskenaisiaPeleja;
#if DEBUG
                    bitti += 2;
#endif
                }
            }

#if DEBUG
            if (s_Bittimaara.ContainsKey(mukana.Count()))
            {
                if (s_Bittimaara[mukana.Count()] != bitti)
                {
                    int kkkkk = 0;
                }
            }
            else
            {
                s_Bittimaara[mukana.Count()] = bitti;
                Debug.WriteLine(string.Format("###### TilanneavaimenPituus[{0}] = {1}", mukana.Count(), bitti));
            }

            if (bitti >= 64)
            {
                int jjjjj = 0;
            }
#endif

            return mask;
        }

        private string TilanneAvain(List<HakuPeli> pelatutPelit, IEnumerable<HakuPelaaja> pelaajat, HakuPelaaja hakija)
        {
            StringBuilder sb = new StringBuilder();

            var mukana = pelaajat.OrderBy(x => x.Id);

            int minPeleja = mukana.Select(x => LaskePelit(pelatutPelit, x.Id, 99999)).Min();

            foreach (var pelaaja in mukana)
            {
                int peleja = LaskePelit(pelatutPelit, pelaaja.Id, 99999);

                // Pelaajan kierros (suhteessa muihinpelaajiin)                 // 2 bittiä
                sb.Append(peleja - minPeleja);

                // Pelaaja puhtaalla vai ei:                                    // 1 bitti
                if ((peleja >= EkaPudariKierros - 1) ||
                    LaskeTappiotHuolellisessaHaussa(pelatutPelit, pelaaja.Id) > 0)
                {
                    sb.Append("H");
                }
                else
                { 
                    sb.Append("V");
                }

                // Miten pelannut muita vastaan                                 // 2 bittiä per pelaaja
                foreach (var vastustaja in mukana.Where(x => x != pelaaja))
                {
                    sb.Append(LaskeKeskenaisetPelit(pelatutPelit, pelaaja.Id, vastustaja.Id));
                }

                // Pelaajan aiemmat kierrosvirheet
                /*
                var virheet = pelatutPelit.Where(x => x.SisaltaaPelaajan(pelaaja.Id) && Math.Abs(x.KierrosPelaaja1 - x.KierrosPelaaja2) > 1);
                if (virheet.Any())
                {
                    sb.Append("_K" + virheet.Count());
                }

                // Pelaajan aiemmat uusintaottelut
                var uusinnat = pelatutPelit.Where(x => 
                    x.SisaltaaPelaajan(pelaaja.Id) &&
                    pelatutPelit.Any(y => y.PeliNumero < x.PeliNumero && y.SisaltaaPelaajat(x.Pelaaja1.Id, x.Pelaaja2.Id)));

                if (uusinnat.Any())
                {
                    sb.Append("_U" + uusinnat.Count());
                }
                */
                sb.Append("_");
            }

            int i = 0;
            foreach (var p in mukana)
            {
                if (p == hakija)
                {
                    sb.Append("-" + i);
                    break;
                }
                else
                {
                    i++;
                }
            }

            return sb.ToString();
        }

        private int PisteytaHaku(
            List<HakuPeli> pelatutPelit,
            IEnumerable<HakuPelaaja> pelaajat,
            HakuPelaaja hakija,
            HakuPelaaja vastustaja)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            HakuPeli peli = LisaaPeli(null, pelatutPelit, null, hakija.Pelaaja, vastustaja.Pelaaja);

            // Toinen pelaaja on kaksi kierrosta edellä. Välitön ei
            if (Math.Abs(peli.KierrosPelaaja1 - peli.KierrosPelaaja2) > 1)
            {
#if DEBUG
                DebugViesti(string.Format("Toinen pelaaja on kaksi kierrosta edellä pelissä {0}-{1}. Pysäytetään haku", hakija.Nimi, vastustaja.Nimi));
                DebugSisenna(-1);
#endif

                return PisteytysSakkoKierrosVirheesta;
            }

            // Enää kaksi pelaajaa tai vähemmän jäljellä. Ei ole hakuvaihtoehtoja, joten pisteytys aina 0
            if (pelaajat.Count() < 3)
            {
#if DEBUG
                DebugViesti(string.Format("Finaali {0}-{1}", hakija.Nimi, vastustaja.Nimi));
                DebugSisenna(-1);
#endif

                return 0;
            }

            int pisteet = 0;
            bool uusinta = false;

            // Uusintaottelu ennen finaalia, sakko
            int keskenaisiaPeleja = LaskeKeskenaisetPelit(pelatutPelit, hakija.Pelaaja.Id, vastustaja.Pelaaja.Id);
            if (keskenaisiaPeleja == 2)
            {
                uusinta = true;
                // Jättisakko jos tulee kaks uusintapeliä ennen finaalia
                pisteet += PisteytysSakkoTuplaUusintaOttelusta;
                pisteet += (pelaajat.Count() - 3) * 10; // Sitä isompi sakko mitä aiemmin uusintaottelu tulee
            }
            else if (keskenaisiaPeleja == 1)
            {
                uusinta = true;
                pisteet += PisteytysSakkoUusintaOttelusta;
                pisteet += (pelaajat.Count() - 3) * 10; // Sitä isompi sakko mitä aiemmin uusintaottelu tulee
            }

            // Kaksi pelaajaa jää hännille ja ovat jo pelanneet => sakko
            {
                if (peli.KierrosPelaaja1 != peli.KierrosPelaaja2)
                {
                    int pelejaMin = Math.Min(hakija.PelattujaPeleja, vastustaja.PelattujaPeleja);

                    var perassa = pelaajat.Where(x =>
                        x != hakija &&
                        x != vastustaja &&
                        x.PelattujaPeleja == pelejaMin);

                    if (perassa.Any())
//                    if (perassa.Count() == 2 &&
//                        LaskeKeskenaisetPelit(pelatutPelit, perassa.First().Id, perassa.Last().Id) > 0)
                    {
                        pisteet += PisteytysSakkoSeuraavaltaKierrokseltaHausta;
                        pisteet += (pelaajat.Count() - 3) * 2;
                    }
                }
            }

            pelatutPelit.Add(peli);
            hakija.PelattujaPeleja++;
            vastustaja.PelattujaPeleja++;

            peli.Tilanne = PelinTilanne.Pelattu;
            peli.Tulos = PelinTulos.Pelaaja1Voitti;
            vastustaja.Tappioita++;

            List<HakuPelaaja> mukanaA = new List<HakuPelaaja>();
            mukanaA.AddRange(pelaajat);

            if (!MukanaHuolellisessaHaussa(vastustaja))
            {
                mukanaA.Remove(vastustaja);
            }

            var pisteytysA = PisteytaTilanne(pelatutPelit, mukanaA, pisteet);
            vastustaja.Tappioita--;

            // Jos A haarassa tulee kierrosvirhe, ei tarvitse evaluoida B haaraa
            if (pisteytysA >= PisteytysSakkoKierrosVirheesta)
            {
                pelatutPelit.RemoveAt(pelatutPelit.Count - 1);
                hakija.PelattujaPeleja--;
                vastustaja.PelattujaPeleja--;

                return pisteytysA;
            }

            peli.Tulos = PelinTulos.Pelaaja2Voitti;

            hakija.Tappioita++;

            List<HakuPelaaja> mukanaB = new List<HakuPelaaja>();
            mukanaB.AddRange(pelaajat);

            if (!MukanaHuolellisessaHaussa(hakija))
            {
                mukanaB.Remove(hakija);
            }

            var pisteytysB = PisteytaTilanne(pelatutPelit, mukanaB, pisteet);
            
            pelatutPelit.RemoveAt(pelatutPelit.Count - 1);
            hakija.PelattujaPeleja--;
            vastustaja.PelattujaPeleja--;
            hakija.Tappioita--;

            return Math.Max(pisteytysA, pisteytysB);
        }

        private int PisteytaTilanne(
            List<HakuPeli> pelatutPelit,
            IEnumerable<HakuPelaaja> mukana,
            int pisteytysSumma)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            if (mukana.Count() == 2)
            {
#if DEBUG
                DebugSisenna(-1);
#endif
                int pisteet = pisteytysSumma + PisteytaHaku(pelatutPelit, mukana, mukana.First(), mukana.Last());

                return Math.Min(PisteytysSakkoKierrosVirheesta, pisteet);
            }

            int? pisteytys = null;

            if (mukana.Count() < 2)
            {
#if DEBUG
                DebugSisenna(-1);
#endif

                return Math.Min(PisteytysSakkoKierrosVirheesta, pisteytysSumma);
            }

            var haku = HaeHuolellisesti(pelatutPelit, mukana, pisteytysSumma);
            if (haku != null)
            {
                pisteytys = haku.Pisteytys;
            }

            if (pisteytys == null)
            {
#if DEBUG
                DebugViesti("# Ei löytynyt vastustajaa huolellisella haulla!!!");
                DebugSisenna(-1);
#endif
                return PisteytysSakkoKierrosVirheesta;
            }

#if DEBUG
            //DebugViesti("PisteytäTilanne {0} = {1}", polku, pisteytys.Pisteytys);
            DebugSisenna(-1);
#endif
            return Math.Min(PisteytysSakkoKierrosVirheesta, pisteytysSumma + pisteytys.Value);
        }

        private bool TarkistaHaku(
            int hakuKierros,
            List<HakuPeli> kaikkiPelit, 
            IEnumerable<Pelaaja> mukana,
            IEnumerable<Pelaaja> hakijat,
            IEnumerable<Pelaaja> vastustajat,
            Pelaaja hakija, 
            Pelaaja vastustaja, 
            bool rekursiossa,
            string polku,
            List<string> syyt)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            // 1) Tarkista onko keskenäisiä pelejä:
            if (!TarkistaKeskenaisetPelit(hakuKierros, kaikkiPelit, hakija.Id, vastustaja.Id, rekursiossa))
            {
                syyt.Add(string.Format("Uusintaottelu {0}-{1} jos haetaan {2}", hakija.Id, vastustaja.Id, polku));
#if DEBUG
                DebugViesti(" - !Virhe! - {0} ---- Uusintaottelu {1} - {2}", polku, hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return false;
            }

            // 2) Tarkista ettei toinen pelaaja ole 2 kierrosta edellä // TODO käytä HakuPelaaja.PelattujaPeleja
            int hakijanKierros = LaskePelit(kaikkiPelit, hakija.Id, hakuKierros);
            int vastustajanKierros = LaskePelit(kaikkiPelit, vastustaja.Id, hakuKierros);
            if (Math.Abs(hakijanKierros - vastustajanKierros) > 1)
            {
                syyt.Add(string.Format("Toinen pelaaja 2 kierrosta edellä pelissä {0} - {1} jos haetaan {2}", hakija.Id, vastustaja.Id, polku));
#if DEBUG
                DebugViesti(" - !Virhe! - {0} ---- Toinen pelaaja on 2 kierrosta edellä {1} - {2}", polku, hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return false;
            }

            // 3) Tarkista ettei kaavion hännille jäänyt kaksi pelajaa jotka ovat jo pelanneet vastakkain
            if (hakijat.Count() == 2)
            {
                if (!TarkistaKeskenaisetPelit(hakuKierros, kaikkiPelit, hakijat.First().Id, hakijat.Last().Id, rekursiossa))
                {
                    syyt.Add(string.Format("Uusintaottelu kaavion hännillä {0}-{1} jos haetaan {2}", hakijat.First().Id, hakijat.Last().Id, polku));
#if DEBUG
                    DebugViesti(" - !Virhe! - {0} ---- Uusintaottelu kaavion hännillä {1} - {2}", polku, hakijat.First().Nimi, hakijat.Last().Nimi);
                    DebugSisenna(-1);
#endif
                    return false;
                }
            }

            kaikkiPelit.Add(new HakuPeli() 
            { 
                Pelaaja1 = hakija, 
                Pelaaja2 = vastustaja,
                Kierros = Math.Max(hakijanKierros, vastustajanKierros),
                KierrosPelaaja1 = hakijanKierros,
                KierrosPelaaja2 = vastustajanKierros,
                PeliOnPelattuKaaviossa = false,
                Tilanne = PelinTilanne.Tyhja,
                Tulos = PelinTulos.EiTiedossa,
                PeliNumero = kaikkiPelit.Count + 1
            });

            List<Pelaaja> jaljelleJaavatHakijat = new List<Pelaaja>();
            jaljelleJaavatHakijat.AddRange(hakijat);
            jaljelleJaavatHakijat.Remove(hakija);
            jaljelleJaavatHakijat.Remove(vastustaja);

            List<Pelaaja> jaljelleJaavatVastustajat = new List<Pelaaja>();
            jaljelleJaavatVastustajat.AddRange(vastustajat);
            jaljelleJaavatVastustajat.Remove(hakija);
            jaljelleJaavatVastustajat.Remove(vastustaja);

            if (jaljelleJaavatHakijat.Count == 0)
            {
#if DEBUG
                DebugViesti(" - Ok - {0} ---- {1} - {2}", polku, hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return true;
            }

            var seuraavaHakija = jaljelleJaavatHakijat.First();
            jaljelleJaavatVastustajat.Remove(seuraavaHakija);

            if (jaljelleJaavatHakijat.Count == 1 && jaljelleJaavatVastustajat.Count == 0)
            {
#if DEBUG
                DebugViesti(" - Ok - {0} ---- {1} - {2} ---- {3} huilaa", polku, hakija.Nimi, vastustaja.Nimi, jaljelleJaavatHakijat.First().Nimi);
                DebugSisenna(-1);
#endif
                return true;
            }
            
            foreach (var seuraavaVastustajaEhdokas in jaljelleJaavatVastustajat)
            {
                List<HakuPeli> pelit = new List<HakuPeli>();
                pelit.AddRange(kaikkiPelit);

                if (TarkistaHaku(
                    hakuKierros,
                    pelit,
                    mukana,
                    jaljelleJaavatHakijat,
                    jaljelleJaavatVastustajat,
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

            int kierros = Math.Max(hakijanKierros, vastustajanKierros);
            foreach (var pelaaja in mukana)
            {
                int pelaajanKierros = LaskePelit(kaikkiPelit, pelaaja.Id, hakuKierros);
                if (pelaajanKierros <= kierros)
                {
#if DEBUG
                    DebugViesti(" - !Virhe! - {0} ---- Haku ei onnistu haun {1} - {2} jälkeen", polku, hakija.Nimi, vastustaja.Nimi);
                    DebugSisenna(-1);
#endif
                    syyt.Add(string.Format("Loppukierroksen haku ei onnistu jos haetaan {0}", polku));

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
            int kierros1 = LaskePelit(pelatutPelit, arvotutPelit, pelaaja1.Id, 99999) + 1; // TODO käytä HakuPelaaja.PelattujaPeleja
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

        private int LaskeKeskenaisetPelit(IEnumerable<HakuPeli> pelit, IEnumerable<HakuPeli> arvotutPelit, int pelaaja1, int pelaaja2)
        {
            return 
                pelit.Count(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2)) +
                arvotutPelit.Count(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2));
        }

        private bool TarkistaKeskenaisetPelit(int kierros, IEnumerable<HakuPeli> pelit, int pelaaja1, int pelaaja2, bool rekursiossa)
        {
            var peli = pelit.FirstOrDefault(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2));
            if (peli != null)
            {
                if (peli.Tilanne != PelinTilanne.Pelattu)
                {
                    if (peli.KierrosPelaaja1 >= this.EkaPudariKierros &&
                        peli.KierrosPelaaja2 >= this.EkaPudariKierros)
                    {
                        return true; // Toinen pelaaja putoaa varmasti tästä pelistä, ei siis haittaa että ovat pelanneet
                    }

                    if (LaskeTappiot(pelit, pelaaja1, kierros) >= 1 &&
                        LaskeTappiot(pelit, pelaaja2, kierros) >= 1)
                    {
                        return true; // Toinen pelaaja putoaa varmasti tästä pelistä, ei siis haittaa että ovat pelanneet
                    }
                }

                return false;
            }

            return true; // Ei keskinaisiä pelejä
        }

        private bool MukanaHuolellisessaHaussa(HakuPelaaja pelaaja) //List<HakuPeli> pelit, int pelaaja)
        {
            if (pelaaja.Tappioita > 1)
            {
                return false;
            }

            if (pelaaja.PelattujaPeleja >= EkaPudariKierros)
            {
                return false;
            }

            //if (LaskeTappiotHuolellisessaHaussa(pelit, pelaaja) > 1)
            //{
            //    return false;
            //}

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
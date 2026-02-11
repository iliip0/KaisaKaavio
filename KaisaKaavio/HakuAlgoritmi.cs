using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            public bool PeliOnPelattu = false; // Välittää tiedon CUP kaavion W.O.ista jotka on valmiita
            public int CupSijoitus1 = 0;
            public int CupSijoitus2 = 0;
            public string CupTeksti1 = string.Empty;
            public string CupTeksti2 = string.Empty;

            public int Id1 => Pelaaja1 != null ? Pelaaja1.Id : -1;
            public int Id2 => Pelaaja2 != null ? Pelaaja2.Id : -1;

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
            public int KeskeneraisiaPeleja = 0;
            public int KeskeneraisiaPudotuspeleja = 0;
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

            if (kilpailu.TestiKilpailu)
            {
                this.AutomaattinenTestausMenossa = true;
            }

            switch (this.Kilpailu.KaavioTyyppi)
            {
                case KaavioTyyppi.Pudari2Kierros:
                    this.EkaPudariKierros = 2;
                    break;

                case KaavioTyyppi.Pudari3Kierros:
                case KaavioTyyppi.KaksiKierrostaJaCup:
                    this.EkaPudariKierros = 3;
                    break;

                case KaavioTyyppi.Pudari4Kierros:
                    this.EkaPudariKierros = 4;
                    break;

                case KaavioTyyppi.Pudari5Kierros:
                    this.EkaPudariKierros = 5;
                    break;

                case KaavioTyyppi.Pudari6Kierros:
                    this.EkaPudariKierros = 6;
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
                Debug.WriteLine("Haetaan pelejä kierrokselle {0}. Mukana : {1}", 
                    kierros, 
                    string.Join(",", mukana.Select(x => string.Format("{0}[{1}]({2})", x.Nimi, LaskePelit(pelatutPelit, arvotutPelit, x.Id, 99999), x.Id)).ToArray()));
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

                bool haettiinHuolellisesti = false;

                var peli = HaeSeuraavaPeliKierrokselle(kierros, pelit, pelatutPelit, arvotutPelit, pelaajat, out haettiinHuolellisesti);
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

                    if (haettiinHuolellisesti)
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

        private void HaeSeuraavaPeliKierrokselleNopeasti(
            int kierros,
            List<HakuPeli> kaikkiPelit,
            List<HakuPeli> pelatutPelit,
            List<HakuPeli> arvotutPelit,
            List<Pelaaja> pelaajat,
            IEnumerable<HakuPelaaja> mukana,
            List<HuolellisenHaunTulos> hakujenPisteytys,
            out Pelaaja haettuHakija,
            out Pelaaja haettuVastustaja)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            haettuHakija = null;
            haettuVastustaja = null;

            var hakijat = mukana
                .Where(x => x.PelattujaPeleja < kierros)
                .OrderBy(x => x.PelattujaPeleja);

            if (!hakijat.Any())
            {
#if DEBUG
                DebugViesti("Ei hakijoita. Pysäytetään haku");
                DebugSisenna(-1);
#endif
                return;
            }

            var hakija = hakijat.First();

            if (!VarmastiMukana(hakija))
            {
#if DEBUG
                DebugViesti("Hakija {0} ei välttämättä mukana. Pysäytetään haku", hakija.Nimi);
                DebugSisenna(-1);
#endif
                return;
            }

            int hakijanKierros = LaskePelit(pelatutPelit, arvotutPelit, hakija.Id, 99999);

            var vastustajat = mukana
                .Where(x => x.PelattujaPeleja >= hakijanKierros)
                .OrderBy(x => x.PelattujaPeleja)
                .Where(x => x != hakija);

            if (!vastustajat.Any())
            {
#if DEBUG
                DebugViesti("Hakija {0} huilaa. Pysäytetään haku", hakija.Nimi);
                DebugSisenna(-1);
#endif
                return;
            }

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                DebugViesti("{0} hakee {1}",
                    string.Format("{0}[{1}]", hakija.Nimi, hakija.PelattujaPeleja),
                    string.Join(",", vastustajat.Select(x =>
                        string.Format("{0}[{1}]", x.Nimi, x.PelattujaPeleja)).ToArray()));
            }
#endif

            HakuPelaaja vastustaja = null;

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
                    hakujenPisteytys);
            }

            if (vastustaja == null)
            {
#if DEBUG
                DebugViesti("Ei löytynyt vastustajaa. Pysäytetään haku");
                DebugSisenna(-1);
#endif
                return;
            }

            if (!VarmastiMukana(vastustaja))
            {
#if DEBUG
                DebugViesti("Vastustaja {0} ei välttämättä mukana. Pysäytetään haku", vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return;
            }

            haettuHakija = hakija.Pelaaja;
            haettuVastustaja = vastustaja.Pelaaja;
        }

        private void HaeSeuraavaPeliKierrokselleHuolellisesti(
            int kierros,
            List<HakuPeli> kaikkiPelit,
            List<HakuPeli> pelatutPelit,
            IEnumerable<HakuPelaaja> mukana,
            List<HuolellisenHaunTulos> hakujenPisteytys,
            out Pelaaja hakija,
            out Pelaaja vastustaja)
        {
            hakija = null;
            vastustaja = null;

            var haku = HaeHuolellisesti(pelatutPelit, mukana, hakujenPisteytys, 0);
            if (haku == null)
            {
                // Huolellinen haku ei löytänyt peliparia
                return;
            }

            if (PelitKesken.Any() && haku.Pisteytys >= PisteytysSakkoSeuraavaltaKierrokseltaHausta)
            {
                // Ei haeta huonoja hakuja jos pelejä on vielä kesken
                return;
            }

            hakija = haku.Hakija.Pelaaja;
            vastustaja = haku.Vastustaja.Pelaaja;
        }

        private Hyppy LuoHypynSelitys(Pelaaja pelaaja, bool hakija, int pisteytys)
        {
            string selitys = string.Empty;

            if (pisteytys >= PisteytysSakkoKierrosVirheesta)
            {
                selitys = string.Format("{0} {1} yli hypätty jotta vältettäisin tilanne jossa pelaaja on kaksi kierrosta muita edellä",
                    hakija ? "Hakijan" : "Pelaajan",
                    pelaaja.Id);
            }
            else if (pisteytys >= PisteytysSakkoUusintaOttelusta)
            {
                selitys = string.Format("{0} {1} yli hypätty uusintaottelun välttämiseksi",
                    hakija ? "Hakijan" : "Pelaajan",
                    pelaaja.Id);
            }
            else if (pisteytys >= PisteytysSakkoSeuraavaltaKierrokseltaHausta)
            {
                selitys = string.Format("{0} {1} yli hypätty jotta kaavion hännille ei jää kahta jo keskenään pelannutta pelaajaa",
                    hakija ? "Hakijan" : "Pelaajan",
                    pelaaja.Id);
            }
            else
            {
                // Ei pitäisi päätyä tänne
                selitys = string.Format("{0} {1} yli hypätty hakuvirheiden todennäköisyyden minimoimiseksi",
                    hakija ? "Hakijan" : "Pelaajan",
                    pelaaja.Id);
            }

#if DEBUG
            DebugViesti("  *{0}", selitys);
#endif

            return new Hyppy()
            {
                Pelaaja = pelaaja,
                Syy = selitys
            };
        }

        private HakuPeli HaeSeuraavaPeliKierrokselle(
            int kierros,
            List<HakuPeli> kaikkiPelit, 
            List<HakuPeli> pelatutPelit, 
            List<HakuPeli> arvotutPelit, 
            List<Pelaaja> pelaajat,
            out bool haettiinHuolellisesti)
        {
            haettiinHuolellisesti = false;

#if DEBUG
            DebugViesti("---==( Haetaan seuraava peli )==---");
            DebugSisenna(1);
#endif

            List<HakuPelaaja> mukana = new List<HakuPelaaja>();
            List<HuolellisenHaunTulos> hakujenPisteytys = new List<HuolellisenHaunTulos>();

            foreach (var pelaaja in pelaajat
                .Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id, 99999))
                .OrderBy(x => x.Id))
            {
                HakuPelaaja hakuPelaaja = new HakuPelaaja()
                {
                    Pelaaja = pelaaja,
                    Id = pelaaja.Id,
                    Nimi = pelaaja.Nimi,
                    Pelit = new List<HakuPeli>()
                };

                foreach (var peli in kaikkiPelit.Where(x => x.SisaltaaPelaajan(pelaaja.Id)))
                {
                    hakuPelaaja.PelattujaPeleja++;

                    if (peli.Tilanne != PelinTilanne.Pelattu)
                    {
                        hakuPelaaja.KeskeneraisiaPeleja++;

                        if (peli.Pelaaja1 == hakuPelaaja.Pelaaja && peli.KierrosPelaaja1 >= this.EkaPudariKierros)
                        {
                            hakuPelaaja.KeskeneraisiaPudotuspeleja++;
                        }
                        else if (peli.Pelaaja2 == hakuPelaaja.Pelaaja && peli.KierrosPelaaja2 >= this.EkaPudariKierros)
                        {
                            hakuPelaaja.KeskeneraisiaPudotuspeleja++;
                        }
                    }
                    else
                    {
                        if (peli.Havisi(hakuPelaaja.Id))
                        {
                            hakuPelaaja.Tappioita++;
                        }
                    }

                        hakuPelaaja.Pelit.Add(peli);
                }

                if (hakuPelaaja.PelattujaPeleja < this.MaxKierros)
                {
                    mukana.Add(hakuPelaaja);
                }
                else
                {
#if DEBUG
                    DebugViesti(string.Format("Pelaaja {0} ei osallistu hakuun. Liian pitkällä kaaviossa", hakuPelaaja.Nimi));
#endif
                }
            }

#if DEBUG
            DebugViesti(string.Format("Mukana {0}", string.Join(", ", mukana.Select(x => string.Format("[{0}]({1}) {2}", 
                x.PelattujaPeleja,
                x.Tappioita > 0 ? "V" : "H",
                x.Pelaaja.LyhytNimi)))));
#endif

            Pelaaja hakija = null;
            Pelaaja vastustaja = null;

            if (mukana.Count() < 2)
            {
#if DEBUG
                DebugViesti("Mukana alle 2 pelaajaa. Ei saada hakuja");
                DebugSisenna(-1);
#endif
                return null;
            }

            if (mukana.Count() <= Asetukset.HuolellisenHaunPelaajamaara)
            {
                if (arvotutPelit.Any())
                {
#if DEBUG
                    DebugViesti("Pelaajamäärä on niin pieni että pitäisi hakea huolellisesti ja kaavioon on arvottu uusi peli. Pysäytetään haku tällä kertaa");
                    DebugSisenna(-1);
#endif
                    this.UusiHakuTarvitaan = true;
                    return null;
                }

                HaeSeuraavaPeliKierrokselleHuolellisesti(
                    kierros, 
                    kaikkiPelit, 
                    pelatutPelit, 
                    mukana, 
                    hakujenPisteytys, 
                    out hakija, 
                    out vastustaja);

                haettiinHuolellisesti = true;
            }
            else 
            {
                HaeSeuraavaPeliKierrokselleNopeasti(
                    kierros, 
                    kaikkiPelit, 
                    pelatutPelit, 
                    arvotutPelit, 
                    pelaajat, 
                    mukana, 
                    hakujenPisteytys, 
                    out hakija, 
                    out vastustaja);
            }

            if (hakija == null || vastustaja == null)
            {
#if DEBUG
                DebugViesti("Ei löytynyt hakuja");
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
                    if (!mukana.Any(x => x.Pelaaja != hakija && LaskeKeskenaisetPelit(kaikkiPelit, hakija.Id, x.Id) == 0))
                    {
#if DEBUG
                        DebugViesti("Hakija on jo pelannut kaikkien muiden kanssa. Annetaan hakea uusintaottelu {0} - {1}", hakija.Nimi, vastustaja.Nimi);
#endif
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

            // Luodaan selitykset mahdollisille hypyille pelaajien yli kaaviossa
            if (!this.AutomaattinenTestausMenossa)
            {
                var hakijat = mukana
                    .OrderBy(x => x.Id)
                    .OrderBy(x => x.PelattujaPeleja);
                if (hakijat.Count() > 2)
                {
                    int skip = 1;

                    foreach (var hakijaEhdokas in hakijat)
                    {
                        if (!uusiPeli.SisaltaaPelaajan(hakijaEhdokas.Id))
                        {
                            if (uusiPeli.Hypyt == null)
                            {
                                uusiPeli.Hypyt = new List<Hyppy>();
                            }

                            // Hypättiin hakijan yli
                            var pisteytykset = hakujenPisteytys.Where(x => x.Hakija == hakijaEhdokas || x.Vastustaja == hakijaEhdokas);
                            if (pisteytykset.Any())
                            {
                                var parasPisteytys = pisteytykset.Select(x => x.Pisteytys).Min();
                                uusiPeli.Hypyt.Add(LuoHypynSelitys(hakijaEhdokas.Pelaaja, true, parasPisteytys));
                            }
                            else
                            {
                                uusiPeli.Hypyt.Add(LuoHypynSelitys(hakijaEhdokas.Pelaaja, true, 0));
                            }
                        }
                        else
                        {
                            foreach (var vastustajaEhdokas in hakijat.Skip(skip))
                            {
                                if (!uusiPeli.SisaltaaPelaajan(vastustajaEhdokas.Id))
                                {
                                    if (uusiPeli.Hypyt == null)
                                    {
                                        uusiPeli.Hypyt = new List<Hyppy>();
                                    }

                                    // Hypättiin vastustajan yli
                                    var pisteytykset = hakujenPisteytys.Where(x => 
                                        (x.Hakija == hakijaEhdokas || x.Vastustaja == hakijaEhdokas) &&
                                        (x.Hakija == vastustajaEhdokas || x.Vastustaja == vastustajaEhdokas));
                                    if (pisteytykset.Any())
                                    {
                                        var parasPisteytys = pisteytykset.Select(x => x.Pisteytys).Min();
                                        uusiPeli.Hypyt.Add(LuoHypynSelitys(vastustajaEhdokas.Pelaaja, false, parasPisteytys));
                                    }
                                    else
                                    {
                                        uusiPeli.Hypyt.Add(LuoHypynSelitys(vastustajaEhdokas.Pelaaja, false, 0));
                                    }
                                }
                                else 
                                {
                                    break;
                                }
                            }

                            break;
                        }
                        skip++;
                    }
                }
            }

#if DEBUG
            DebugSisenna(-1);
            DebugViesti("[( Haettiin peli: )] {0} - {1}", hakija.Nimi, vastustaja.Nimi);
#endif

            return uusiPeli;
        }

        private HakuPelaaja HaeVastustajaNopeasti(
            int kierros,
            List<HakuPeli> kaikkiPelit, 
            IEnumerable<HakuPelaaja> mukana, 
            HakuPelaaja hakija, 
            IEnumerable<HakuPelaaja> hakijat,
            IEnumerable<HakuPelaaja> vastustajat,
            List<HuolellisenHaunTulos> hakujenPisteytys)
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
                int pistetytys = 0;

                if (TarkistaHaku(
                    kierros, 
                    kaikkiHakupelit, 
                    mukana,
                    hakijat,
                    vastustajat,
                    hakija, 
                    vastustajaEhdokas,
                    hakujenPisteytys,
                    out pistetytys))
                {
#if DEBUG
                    DebugSisenna(-1);
#endif
                    return vastustajaEhdokas;
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
        }

        private HuolellisenHaunTulos HaeHuolellisesti(
            List<HakuPeli> pelatutPelit,
            IEnumerable<HakuPelaaja> mukana,
            List<HuolellisenHaunTulos> hakujenPisteytys,
            int pisteytysSumma)
        {
#if DEBUG
            DebugSisenna(1);

            HakuPelaaja tallennettuHakija = null;
            HakuPelaaja tallennettuVastustaja = null;
#endif

            int pelaajiaMukana = mukana.Count();
            ulong hakuAvain = 0;
            Tyypit.EvaluointiTietokanta.ParasHaku tallennettuHaku = null;

            if (pelaajiaMukana <= Asetukset.MaxPelaajiaJottaHautTallennetaan)
            {
                hakuAvain = TilanneAvain(pelatutPelit, mukana);
                tallennettuHaku = Tyypit.EvaluointiTietokanta.AnnaParasHakuTilanteessa(hakuAvain);
                if (tallennettuHaku != null)
                {
                    var pelaajat = mukana.OrderBy(x => x.Id);
                    var hakija = pelaajat.ElementAt(tallennettuHaku.Hakija);
                    var vastustaja = pelaajat.ElementAt(tallennettuHaku.Vastustaja);

                    var tulos = new HuolellisenHaunTulos()
                    {
                        Hakija = hakija,
                        Vastustaja = vastustaja,
                        Pisteytys = pisteytysSumma + tallennettuHaku.Pisteytys
                    };

                    if (hakujenPisteytys != null)
                    {
#if DEBUG
                        DebugViesti(" -haku {0} - {1} = <{2}>", hakija.Nimi, vastustaja.Nimi, tulos.Pisteytys);
#endif
                        hakujenPisteytys.Add(tulos);
                    }

#if DEBUG
                    tallennettuHakija = hakija;
                    tallennettuVastustaja = vastustaja;

                    DebugSisenna(-1);
#endif

                    return tulos;
                }
            }

            int? parasPisteytys = null;
            HakuPelaaja parasHakija = null;
            HakuPelaaja parasVastustaja = null;

            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => x.PelattujaPeleja)
                .ToArray();

            int minKierros = hakijat.Select(x => x.PelattujaPeleja).Min();

            int maxI = 1;

            if (pelaajiaMukana <= Asetukset.MaxPelaajiaJottaSaaHyppiaHakijanYli)
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

                        if (hakujenPisteytys != null)
                        {
                            var tulos = new HuolellisenHaunTulos()
                            {
                                Hakija = hakija,
                                Vastustaja = vastustaja,
                                Pisteytys = Math.Min(PisteytysSakkoKierrosVirheesta, pisteytysSumma + pisteytys)
                            };

                            hakujenPisteytys.Add(tulos);

#if DEBUG
                            DebugViesti(" -haku {0} - {1} = <{2}>", hakija.Nimi, vastustaja.Nimi, tulos.Pisteytys);
#endif
                        }

                        if (pisteytys <= 0) // Täydellinen haku, ei tarvitse etsiä kauempaa
                        {
                            break;
                        }
                    }
                }
            }

            if (parasHakija == null)
            {
#if DEBUG
                DebugViesti("-Ei löytynyt hakuja");
                DebugSisenna(-1);
#endif
                return null;
            }

            if ((hakuAvain != 0) && (pelaajiaMukana <= Asetukset.MaxPelaajiaJottaHautTallennetaan))
            {
                var pelaajat = mukana.OrderBy(x => x.Id).ToList();
                Tyypit.EvaluointiTietokanta.TallennaHaku(hakuAvain,
                    pelaajat.IndexOf(parasHakija),
                    pelaajat.IndexOf(parasVastustaja),
                    parasPisteytys.Value);
            }

#if DEBUG
            DebugViesti("-Haettiin {0} - {1} = <{2}>", parasHakija.Nimi, parasVastustaja.Nimi, Math.Min(PisteytysSakkoKierrosVirheesta, parasPisteytys.Value + pisteytysSumma));
            DebugSisenna(-1);
#endif

            return new HuolellisenHaunTulos()
            {
                Hakija = parasHakija,
                Vastustaja = parasVastustaja,
                Pisteytys = Math.Min(PisteytysSakkoKierrosVirheesta, parasPisteytys.Value + pisteytysSumma)
            };
        }

#if DEBUG
        static Dictionary<int, int> s_Bittimaara = new Dictionary<int, int>();
#endif

        private ulong TilanneAvain(List<HakuPeli> pelatutPelit, IEnumerable<HakuPelaaja> pelaajat)
        {
            var mukana = pelaajat.OrderBy(x => x.Id);
            int minPeleja = mukana.Select(x => x.PelattujaPeleja).Min();
            ulong mask = 0;

#if DEBUG
            if (mukana.Count() == 0)
            {
                throw new Exception("Bugi");
            }

            if (mukana.Count() > 7) // TilanneAvain ei mahdu 64 bittiin jos pelaajia on yli 7
            {
                throw new Exception("Bugi. Huolellinen haku mahdollista enintään 7 pelaajalle");
            }
#endif

            mask |= 1; // Tämä bitti auttaa erottamaan eri pelaajamäärien koodit jotka loppuvat useampaan nollaan
                        // Seitsemällä pelaajalla tämä bitti työntyy maskista ulos, mutta se ei haittaa koska bitti
                        // ei sisällä informaatiota joka vaikuttaisi hakuun

            foreach (var pelaaja in mukana)
            {
                // Pelaajan kierros (suhteessa muihinpelaajiin)
                int kierros = pelaaja.PelattujaPeleja - minPeleja;
#if DEBUG
                if (kierros < 0 || kierros > 3)
                {
                    throw new Exception("Bugi");
                }
#endif

                mask <<= 2;
                mask |= (uint)(kierros);

                // Pelaaja puhtaalla vai ei
                if ((pelaaja.Tappioita == 0) && (pelaaja.PelattujaPeleja < (EkaPudariKierros - 1)))
                {
                    mask <<= 1;
                    mask |= 1;
                }
                else
                {
                    mask <<= 1;
                }

                // Miten pelannut muita vastaan
                foreach (var vastustaja in mukana.Where(x => x.Id > pelaaja.Id))
                {
                    int keskenaisiaPeleja = LaskeKeskenaisetPelit(pelaaja.Pelit, pelaaja.Id, vastustaja.Id);

#if DEBUG
                    if (keskenaisiaPeleja < 0 || keskenaisiaPeleja > 3)
                    {
                        throw new Exception("Bugi");
                    }
#endif

                    mask <<= 2;
                    mask |= (uint)keskenaisiaPeleja;
                }
            }

            return mask;
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

            HakuPeli peli = LisaaPeli(hakija, vastustaja);

            // Toinen pelaaja on kaksi kierrosta edellä. Välitön ei
            if (Math.Abs(peli.KierrosPelaaja1 - peli.KierrosPelaaja2) > 1)
            {
#if DEBUG
                DebugViesti(string.Format("Toinen pelaaja on kaksi kierrosta edellä pelissä {0}-{1}. Pysäytetään haku", hakija.Nimi, vastustaja.Nimi));
                DebugSisenna(-1);
#endif

                return PisteytysSakkoKierrosVirheesta;
            }

            int pelaajia = pelaajat.Count();

            // Enää kaksi pelaajaa tai vähemmän jäljellä. Ei ole hakuvaihtoehtoja, joten pisteytys aina 0
            if (pelaajia < 3)
            {
#if DEBUG
                DebugViesti(string.Format("Finaali {0}-{1}", hakija.Nimi, vastustaja.Nimi));
                DebugSisenna(-1);
#endif

                return 0;
            }

            int pisteet = 0;

            // Uusintaottelu ennen finaalia, sakko
            int keskenaisiaPeleja = LaskeKeskenaisetPelit(hakija.Pelit, hakija.Pelaaja.Id, vastustaja.Pelaaja.Id);
            if (keskenaisiaPeleja == 2)
            {
                // Jättisakko jos tulee kaks uusintapeliä ennen finaalia
                pisteet += PisteytysSakkoTuplaUusintaOttelusta;
                pisteet += (pelaajia - 3) * 10; // Sitä isompi sakko mitä aiemmin uusintaottelu tulee
            }
            else if (keskenaisiaPeleja == 1)
            {
                pisteet += PisteytysSakkoUusintaOttelusta;
                pisteet += (pelaajia - 3) * 10; // Sitä isompi sakko mitä aiemmin uusintaottelu tulee
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
                    {
                        pisteet += PisteytysSakkoSeuraavaltaKierrokseltaHausta;
                        pisteet += (pelaajia - 3) * 2;
                    }
                }
            }

            pelatutPelit.Add(peli);
            hakija.PelattujaPeleja++;
            vastustaja.PelattujaPeleja++;
            hakija.Pelit.Add(peli);
            vastustaja.Pelit.Add(peli);

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
                hakija.Pelit.Remove(peli);
                vastustaja.Pelit.Remove(peli);

#if DEBUG
                DebugViesti(" -{0}V - {1} = <{2}> (optimointi)", hakija.Nimi, vastustaja.Nimi, pisteytysA);
                DebugSisenna(-1);
#endif
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
            hakija.Pelit.Remove(peli);
            vastustaja.Pelit.Remove(peli);
            hakija.Tappioita--;

#if DEBUG
            if (pisteytysA > pisteytysB)
            {
                DebugViesti(" -{0}V - {1} = <{2}>", hakija.Nimi, vastustaja.Nimi, pisteytysA);
            }
            else
            {
                DebugViesti(" -{0} - {1}V = <{2}>", hakija.Nimi, vastustaja.Nimi, pisteytysB);
            }
            DebugSisenna(-1);
#endif

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

            var haku = HaeHuolellisesti(pelatutPelit, mukana, null, pisteytysSumma);
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
            DebugSisenna(-1);
#endif
            return Math.Min(PisteytysSakkoKierrosVirheesta, pisteytysSumma + pisteytys.Value);
        }

        private bool TarkistaHaku(
            int hakuKierros,
            List<HakuPeli> kaikkiPelit, 
            IEnumerable<HakuPelaaja> mukana,
            IEnumerable<HakuPelaaja> hakijat,
            IEnumerable<HakuPelaaja> vastustajat,
            HakuPelaaja hakija,
            HakuPelaaja vastustaja,
            List<HuolellisenHaunTulos> hakujenPisteytys,
            out int pisteytys)
        {
#if DEBUG
            DebugSisenna(1);
#endif

            // 1) Tarkista onko keskenäisiä pelejä:
            if (!TarkistaKeskenaisetPelit(hakuKierros, kaikkiPelit, hakija.Id, vastustaja.Id))
            {
                if (hakujenPisteytys != null)
                {
                    hakujenPisteytys.Add(new HuolellisenHaunTulos()
                    { 
                        Hakija = hakija,
                        Vastustaja = vastustaja,
                        Pisteytys = PisteytysSakkoUusintaOttelusta
                    });
                }

                pisteytys = PisteytysSakkoUusintaOttelusta;
#if DEBUG
                DebugViesti(" - !Virhe! - Uusintaottelu {0} - {1}",hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return false;
            }

            // 2) Tarkista ettei toinen pelaaja ole 2 kierrosta edellä // TODO käytä HakuPelaaja.PelattujaPeleja
            int hakijanKierros = LaskePelit(kaikkiPelit, hakija.Id, hakuKierros);
            int vastustajanKierros = LaskePelit(kaikkiPelit, vastustaja.Id, hakuKierros);
            if (Math.Abs(hakijanKierros - vastustajanKierros) > 1)
            {
                if (hakujenPisteytys != null)
                {
                    hakujenPisteytys.Add(new HuolellisenHaunTulos()
                    {
                        Hakija = hakija,
                        Vastustaja = vastustaja,
                        Pisteytys = PisteytysSakkoKierrosVirheesta
                    });
                }

                pisteytys = PisteytysSakkoKierrosVirheesta;
#if DEBUG
                DebugViesti(" - !Virhe! - Toinen pelaaja on 2 kierrosta edellä {0} - {1}", hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                return false;
            }

            // 3) Tarkista ettei kaavion hännille jäänyt kaksi pelajaa jotka ovat jo pelanneet vastakkain
            if (hakijat.Count() == 2)
            {
                if (!TarkistaKeskenaisetPelit(hakuKierros, kaikkiPelit, hakijat.First().Id, hakijat.Last().Id))
                {
                    if (hakujenPisteytys != null)
                    {
                        hakujenPisteytys.Add(new HuolellisenHaunTulos()
                        {
                            Hakija = hakija,
                            Vastustaja = vastustaja,
                            Pisteytys = PisteytysSakkoSeuraavaltaKierrokseltaHausta
                        });
                    }

                    pisteytys = PisteytysSakkoSeuraavaltaKierrokseltaHausta;
#if DEBUG
                    DebugViesti(" - !Virhe! - Uusintaottelu kaavion hännillä {0} - {1}", hakijat.First().Nimi, hakijat.Last().Nimi);
                    DebugSisenna(-1);
#endif
                    return false;
                }
            }

            kaikkiPelit.Add(new HakuPeli() 
            { 
                Pelaaja1 = hakija.Pelaaja, 
                Pelaaja2 = vastustaja.Pelaaja,
                Kierros = Math.Max(hakijanKierros, vastustajanKierros),
                KierrosPelaaja1 = hakijanKierros,
                KierrosPelaaja2 = vastustajanKierros,
                PeliOnPelattuKaaviossa = false,
                Tilanne = PelinTilanne.Tyhja,
                Tulos = PelinTulos.EiTiedossa,
                PeliNumero = kaikkiPelit.Count + 1
            });

            List<HakuPelaaja> jaljelleJaavatHakijat = new List<HakuPelaaja>();
            jaljelleJaavatHakijat.AddRange(hakijat);
            jaljelleJaavatHakijat.Remove(hakija);
            jaljelleJaavatHakijat.Remove(vastustaja);

            List<HakuPelaaja> jaljelleJaavatVastustajat = new List<HakuPelaaja>();
            jaljelleJaavatVastustajat.AddRange(vastustajat);
            jaljelleJaavatVastustajat.Remove(hakija);
            jaljelleJaavatVastustajat.Remove(vastustaja);

            if (jaljelleJaavatHakijat.Count == 0)
            {
#if DEBUG
                DebugViesti(" - Ok - {0} - {1}", hakija.Nimi, vastustaja.Nimi);
                DebugSisenna(-1);
#endif
                pisteytys = 0;
                return true;
            }

            var seuraavaHakija = jaljelleJaavatHakijat.First();
            jaljelleJaavatVastustajat.Remove(seuraavaHakija);

            if (jaljelleJaavatHakijat.Count == 1 && jaljelleJaavatVastustajat.Count == 0)
            {
#if DEBUG
                DebugViesti(" - Ok - {0} - {1} ---- {2} huilaa", hakija.Nimi, vastustaja.Nimi, jaljelleJaavatHakijat.First().Nimi);
                DebugSisenna(-1);
#endif
                pisteytys = 0;
                return true;
            }
            
            foreach (var seuraavaVastustajaEhdokas in jaljelleJaavatVastustajat)
            {
                List<HakuPeli> pelit = new List<HakuPeli>();
                pelit.AddRange(kaikkiPelit);

                int haunPisteytys = 0;
                if (TarkistaHaku(
                    hakuKierros,
                    pelit,
                    mukana,
                    jaljelleJaavatHakijat,
                    jaljelleJaavatVastustajat,
                    seuraavaHakija,
                    seuraavaVastustajaEhdokas,
                    null,
                    out haunPisteytys))
                {
#if DEBUG
                    DebugViesti(" - Ok - {0} - {1}", seuraavaHakija.Nimi, seuraavaVastustajaEhdokas.Nimi);
                    DebugSisenna(-1);
#endif
                    pisteytys = haunPisteytys;
                    return true;
                }
                else
                {
                    if (hakujenPisteytys != null)
                    {
                        hakujenPisteytys.Add(new HuolellisenHaunTulos()
                        {
                            Hakija = seuraavaHakija,
                            Vastustaja = seuraavaVastustajaEhdokas,
                            Pisteytys = haunPisteytys
                        });
                    }
                }
            }

            int kierros = Math.Max(hakijanKierros, vastustajanKierros);
            foreach (var pelaaja in mukana)
            {
                int pelaajanKierros = LaskePelit(kaikkiPelit, pelaaja.Id, hakuKierros);
                if (pelaajanKierros <= kierros)
                {
#if DEBUG
                    DebugViesti(" - !Virhe! - Haku ei onnistu haun {0} - {1} jälkeen", hakija.Nimi, vastustaja.Nimi);
                    DebugSisenna(-1);
#endif
                    if (hakujenPisteytys != null)
                    {
                        hakujenPisteytys.Add(new HuolellisenHaunTulos()
                        {
                            Hakija = hakija,
                            Vastustaja = vastustaja,
                            Pisteytys = PisteytysSakkoUusintaOttelusta
                        });
                    }

                    pisteytys = PisteytysSakkoUusintaOttelusta;
                    return false;
                }
            }

#if DEBUG
            DebugViesti(" - Ok - Haku ei onnistu haun {0} - {1} jälkeen mutta hakija ja vastustaja ovat muita kierroksen perässä", hakija.Nimi, vastustaja.Nimi);
            DebugSisenna(-1);
#endif

            pisteytys = 0;
            return true;
        }

        private HakuPeli LisaaPeli(HakuPelaaja pelaaja1, HakuPelaaja pelaaja2)
        {
            int kierros1 = pelaaja1.PelattujaPeleja + 1;
            int kierros2 = pelaaja2.PelattujaPeleja + 1;
            int kierros = Math.Max(kierros1, kierros2);

            HakuPeli peli = new HakuPeli()
            {
                Kierros = Math.Max(kierros1, kierros2),
                KierrosPelaaja1 = kierros1,
                KierrosPelaaja2 = kierros2,
                PeliNumero = 0,
                Pelaaja1 = pelaaja1.Pelaaja,
                Pelaaja2 = pelaaja2.Pelaaja,
                Tulos = PelinTulos.EiTiedossa,
                Tilanne = PelinTilanne.Tyhja,
                PeliOnPelattuKaaviossa = false
            };

            return peli;
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

        private bool TarkistaKeskenaisetPelit(int kierros, IEnumerable<HakuPeli> pelit, int pelaaja1, int pelaaja2)
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

        private bool VarmastiMukana(HakuPelaaja pelaaja)
        {
            if ((pelaaja.KeskeneraisiaPudotuspeleja > 0) ||
                (pelaaja.KeskeneraisiaPeleja + pelaaja.Tappioita) > 1)
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
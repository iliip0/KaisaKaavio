using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    public class HakuAlgoritmi
    {
        public class Pelaajat
        {
            public Pelaaja Pelaaja1 = null;
            public Pelaaja Pelaaja2 = null;

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
        List<HakuPeli> JoArvotutPelit = new List<HakuPeli>();
        List<HakuTulos> Hakutulokset = new List<HakuTulos>();

        public List<Pelaajat> UudetPelit = new List<Pelaajat>();
        public Exception HakuVirhe = null;
        public bool PeruutaHaku = false;
        public bool UusiHakuTarvitaan = false;

        Kilpailu Kilpailu = null;
        int Kierros = 0;
        int EkaPudariKierros = 0;
        bool HakuKesken = true;

        public bool AutomaattinenTestausMenossa = false;

        public HakuAlgoritmi(Kilpailu kilpailu, Loki loki, int kierros, IStatusRivi status)
        {
            this.status = status;
            this.Kilpailu = kilpailu;
            this.Kierros = kierros;

            switch (this.Kilpailu.KaavioTyyppi)
            {
                case KaavioTyyppi.Pudari2Kierros:
                    this.EkaPudariKierros = 2;
                    break;

                case KaavioTyyppi.Pudari3Kierros:
                    this.EkaPudariKierros = 3;
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
                    Tilanne = peli.Tilanne
                };

                if (peli.Kierros < this.Kierros)
                {
                    if (peli.Tilanne == PelinTilanne.Pelattu)
                    {
                        Pelatut.Add(p);
                    }
                    else
                    {
                        PelitKesken.Add(p);
                    }
                }
                else if ((peli.Kierros == this.Kierros) && !peli.Tyhja())
                {
                    if (peli.KierrosPelaaja1 < peli.Kierros || peli.KierrosPelaaja2 < peli.Kierros)
                    {
                        if (peli.Tilanne == PelinTilanne.Pelattu)
                        {
                            Pelatut.Add(p);
                        }
                        else
                        {
                            PelitKesken.Add(p);
                        }
                    }
                    else
                    {
                        JoArvotutPelit.Add(p);
                    }
                }
                else 
                {
#if DEBUG
                    throw new Exception(string.Format("BUGI!!! HakuAlgoritmissä peli joka on pidemmällä kaaviossa kuin haettava kierros {0}", peli.Kuvaus()));
#endif
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

        private void DebugViesti(string viesti, params object[] parametrit)
        { 
#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
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
            try
            {
                PaivitaStatusRivi("Haetaan pelejä...", 10);

                this.HakuKesken = true;

#if DEBUG
                DebugViesti("=======================================================================================================================");
                DebugViesti("Tilanne ennen hakua:");

                foreach (var pp in Pelatut)
                {
                    DebugViesti("Pelattu {0} {1} - {2}, {3}, {4}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Pelaaja2.Nimi, pp.Tilanne, pp.Tulos);
                }

                foreach (var pp in PelitKesken)
                {
                    DebugViesti("Kesken {0} {1} - {2}, {3}, {4}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Pelaaja2.Nimi, pp.Tilanne, pp.Tulos);
                }

                foreach (var pp in JoArvotutPelit)
                {
                    DebugViesti("Arvottu jo {0} {1} - {2}, {3}, {4}", pp.Kierros, pp.Pelaaja1.Nimi, pp.Pelaaja2.Nimi, pp.Tilanne, pp.Tulos);
                }
#endif
                int maxPermutations = (int)Math.Pow(2.0, (double)PelitKesken.Count);
                if (maxPermutations <= 1)
                {
                    DebugViesti("Haetaan pelejä:");
                    PaivitaStatusRivi("Haetaan pelejä... skenaario 1/1", 50);

                    List<HakuPeli> pelatutPelit = new List<HakuPeli>();
                    pelatutPelit.AddRange(Pelatut);

                    List<HakuPeli> arvotutPelit = new List<HakuPeli>();
                    arvotutPelit.AddRange(JoArvotutPelit);

                    HakuTulos hakutulos = new HakuTulos();

                    HaePelitKierrokselle(pelatutPelit, arvotutPelit, hakutulos.PeliParit);

                    this.Hakutulokset.Add(hakutulos);
                }
                else
                {
                    DebugViesti("Haetaan pelejä: {0} keskeneräistä peliä, {1} erilaista skenaariota", PelitKesken.Count, maxPermutations);

                    // Tämä käy läpi kaikki mahdolliset skenaariot kesken olevien pelien lopputuloksista
                    for (int permutation = 0; permutation < maxPermutations; ++permutation)
                    {
                        if (PeruutaHaku)
                        {
                            return;
                        }

                        PaivitaStatusRivi(
                            string.Format("Haetaan pelejä... skenaario {0}/{1}", permutation + 1, maxPermutations + 1), 
                            10 + (int)(((float)permutation / (float)maxPermutations) * 90.0f));

#if DEBUG
                        string voittajat = string.Format("P[{0}] voittajat:", permutation + 1);
#endif
                        List<HakuPeli> pelatutPelit = new List<HakuPeli>();
                        pelatutPelit.AddRange(Pelatut);

                        List<HakuPeli> arvotutPelit = new List<HakuPeli>();
                        arvotutPelit.AddRange(JoArvotutPelit);

                        for (int i = 0; i < PelitKesken.Count; ++i)
                        {
                            HakuPeli peli = PelitKesken[i];
                            int bitmask = 1 << i;
                            bool p1voitti = (permutation & bitmask) == bitmask;

                            pelatutPelit.Add(new HakuPeli()
                            {
                                Kierros = peli.Kierros,
                                KierrosPelaaja1 = peli.KierrosPelaaja1,
                                KierrosPelaaja2 = peli.KierrosPelaaja2,
                                Pelaaja1 = peli.Pelaaja1,
                                Pelaaja2 = peli.Pelaaja2,
                                Tulos = p1voitti ? PelinTulos.Pelaaja1Voitti : PelinTulos.Pelaaja2Voitti,
                                Tilanne = PelinTilanne.Pelattu
                            });

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
                        Pelaaja2 = pari.Pelaaja2
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
                            Pelaaja2 = pari.Pelaaja2
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

                if (peli.Kierros > this.Kierros)
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
                Mukana(pelatutPelit, arvotutPelit, x.Id));

            if (mukana.Count() < 2)
            {
                DebugViesti("Mukana alle 2 pelaajaa. Haku on päättynyt");
                return false;
            }

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                Debug.WriteLine(string.Format("Mukana : {0}", string.Join(",", mukana.Select(x => x.Nimi).ToArray())));
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
                if (peli != null && peli.Kierros <= this.Kierros)
                {
                    haettiinJotain = true;
                    arvotutPelit.Add(peli);
                    pelit.Add(peli);
                    peliparit.Add(new Pelaajat()
                    {
                        Pelaaja1 = peli.Pelaaja1,
                        Pelaaja2 = peli.Pelaaja2
                    });

                    if (peli.KierrosPelaaja1 < peli.Kierros ||
                        peli.KierrosPelaaja2 < peli.Kierros)
                    {
                        DebugViesti("Haettiin peli jossa toinen pelaaja on kierroksen perässä. Haku keskeytetään tältä erää");
                        this.UusiHakuTarvitaan = true;
                        break; // Keskeytetään tämänkertainen haku heti jos haettiin peli jossa toinen pelaaja on kierroksen perässä (yksinkertaistaa hakua)
                               // Tässä tilanteessa uusi haku käynnistyy automaattisesti perään
                    }
                }
                else
                {
                    break;
                }
            }

            return haettiinJotain;
        }

        private HakuPeli HaeSeuraavaPeliKierrokselle(List<HakuPeli> kaikkiPelit, List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, List<Pelaaja> pelaajat)
        {
            var mukana = pelaajat.Where(x => Mukana(pelatutPelit, arvotutPelit, x.Id));
            var mukana2 = mukana.ToArray();

            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => LaskePelit(pelatutPelit, arvotutPelit, x.Id));

            if (hakijat.Count() < 2)
            {
#if DEBUG
                if (!this.AutomaattinenTestausMenossa && hakijat.Count() == 1)
                {
                    DebugViesti(" -{0} huilaa", hakijat.First().Nimi);
                }
#endif
                return null;
            }

            var hakija = hakijat.First();

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, hakija.Id))
            {
                DebugViesti(" -Hakija {0} ei välttämättä mukana. Pysäytetään haku", hakija.Nimi);
                return null;
            }

            var vastustajat = hakijat
                .Skip(1)
                .OrderBy(x => LaskeKeskenaisetPelit(kaikkiPelit, hakija.Id, x.Id));

#if DEBUG
            if (!this.AutomaattinenTestausMenossa)
            {
                Debug.WriteLine(string.Format(" -{0} hakee {1}",
                    string.Format("{0}[{1}]", hakija.Nimi, LaskePelit(pelatutPelit, arvotutPelit, hakija.Id)),
                    string.Join(",", vastustajat.Select(x =>
                        string.Format("{0}[{1}]", x.Nimi, LaskePelit(pelatutPelit, arvotutPelit, x.Id))).ToArray())));
            }
#endif

            Pelaaja vastustaja = null;

            // Tämä luuppi käy läpi hakijan mahdolliset vastustajat, ja valitsee ekan josta ei seuraa uusintapelejä kaaviossa
            foreach (var vastustajaEhdokas in vastustajat)
            {
                if (this.PeruutaHaku)
                {
                    return null;
                }

                if (TarkistaHaku(kaikkiPelit, mukana2, hakija, vastustajaEhdokas))
                {
                    vastustaja = vastustajaEhdokas;
                    break;
                }
            }

            if (vastustaja == null) 
            {
                // Jos tähän on tultu, kaikilla hakujärestyksillä tulee uusintapelejä (toivottavasti ollaan finaalissa, muuten kaaviossa on tapahtunut hakuvirhe)
                DebugViesti("Kaikki haut olivat huonoja. Haetaan ensimmäinen vastustaja listalta");
                vastustaja = vastustajat.First();
            }

            if (!VarmastiMukana(pelatutPelit, arvotutPelit, vastustaja.Id))
            {
                DebugViesti(" -Vastustaja {0} ei välttämättä mukana. Pysäytetään haku", vastustaja.Nimi);
                return null;
            }

            return LisaaPeli(kaikkiPelit, pelatutPelit, arvotutPelit, hakija, vastustaja);
        }

        private bool TarkistaHaku(IEnumerable<HakuPeli> kaikkiPelit, IEnumerable<Pelaaja> pelaajat, Pelaaja hakija, Pelaaja vastustaja)
        {
            if (LaskeKeskenaisetPelit(kaikkiPelit, hakija.Id, vastustaja.Id) > 0)
            {
                DebugViesti("---- Uusintaottelu {0} - {1}", hakija.Nimi, vastustaja.Nimi);
                return false;
            }

            List<Pelaaja> mukana = new List<Pelaaja>();
            mukana.AddRange(pelaajat);
            mukana.Remove(hakija);
            mukana.Remove(vastustaja);

            if (mukana.Count() < 2)
            {
                return true;
            }

            var hakijat = mukana
                .OrderBy(x => x.Id)
                .OrderBy(x => LaskePelit(kaikkiPelit, x.Id));

            var seuraavaHakija = hakijat.First();
            var seuraavatVastustajat = hakijat.Skip(1);

            foreach (var seuraavaVastustajaEhdokas in seuraavatVastustajat)
            {
                if (TarkistaHaku(kaikkiPelit, hakijat, seuraavaHakija, seuraavaVastustajaEhdokas))
                {
                    return true;
                }
            }

            int hakijanKierros = LaskePelit(kaikkiPelit, hakija.Id);
            int vastustajanKierros = LaskePelit(kaikkiPelit, vastustaja.Id);
            int kierros = Math.Max(hakijanKierros, vastustajanKierros);
            foreach (var pelaaja in mukana)
            {
                int pelaajanKierros = LaskePelit(kaikkiPelit, pelaaja.Id);
                if (pelaajanKierros <= kierros)
                {
                    DebugViesti("---- Haku ei onnistu haun {0} - {1} jälkeen", hakija.Nimi, vastustaja.Nimi);
                    return false;
                }
            }


            DebugViesti("---- Haku ei onnistu haun {0} - {1} jälkeen mutta hakija ja vastustaja ovat muita kierroksen perässä. Haku on OK", hakija.Nimi, vastustaja.Nimi);
            return true;
        }

        private HakuPeli LisaaPeli(List<HakuPeli> kaikkiPelit, List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, Pelaaja pelaaja1, Pelaaja pelaaja2)
        {
            int kierros1 = LaskePelit(pelatutPelit, arvotutPelit, pelaaja1.Id) + 1;
            int kierros2 = LaskePelit(pelatutPelit, arvotutPelit, pelaaja2.Id) + 1;
            int kierros = Math.Max(kierros1, kierros2);

            HakuPeli peli = new HakuPeli()
            {
                Kierros = Math.Max(kierros1, kierros2),
                KierrosPelaaja1 = kierros1,
                KierrosPelaaja2 = kierros2,
                PeliNumero = kaikkiPelit.Count + 1,
                Pelaaja1 = pelaaja1,
                Pelaaja2 = pelaaja2,
                Tulos = PelinTulos.EiTiedossa,
                Tilanne = PelinTilanne.Tyhja,
            };

            DebugViesti(" [{0}] : {1} - {2}", peli.Kierros, pelaaja1.Nimi, pelaaja2.Nimi);

            return peli;
        }

        private int PelatutPelit(List<HakuPeli> pelit, int pelaaja)
        {
            return pelit.Count(x => x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskePelit(List<HakuPeli> pelit, int pelaaja)
        {
            return pelit.Count(x => x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskePelit(List<HakuPeli> pelit, int pelaaja, int kierros)
        {
            return pelit.Count(x =>
                x.Kierros <= kierros &&
                x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskeKeskenaisetPelit(IEnumerable<HakuPeli> pelit, int pelaaja1, int pelaaja2)
        {
            return pelit.Count(x => x.SisaltaaPelaajat(pelaaja1, pelaaja2));
        }

        private bool Mukana(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            if (LaskeTappiot(pelatutPelit, arvotutPelit, pelaaja) > 1)
            {
                return false;
            }

            return LaskePelit(pelatutPelit, arvotutPelit, pelaaja) < this.Kierros;
        }

        private bool VarmastiMukana(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            int tappiot = LaskeTappiot(pelatutPelit, arvotutPelit, pelaaja);
            if (tappiot > 1)
            {
                return false;
            }

            int kesken = LaskeKeskeneraisetPelit(pelatutPelit, arvotutPelit, pelaaja);
            if ((tappiot + kesken) > 1)
            {
                return false;
            }

            int pudaritKesken = LaskeKeskeneraisetPudariPelit(pelatutPelit, arvotutPelit, pelaaja);
            if (pudaritKesken > 0)
            {
                return false;
            }

            return true;
        }

        private int LaskeTappiot(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            int tappiot = 
                pelatutPelit.Count(x => x.Kierros <= this.Kierros && x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja)) +
                arvotutPelit.Count(x => x.Kierros <= this.Kierros && x.SisaltaaPelaajan(pelaaja) && x.Tilanne == PelinTilanne.Pelattu && x.Havisi(pelaaja));

            if (tappiot < 2)
            {
                if (pelatutPelit.Any(x => x.Kierros <= this.Kierros && 
                    x.SisaltaaPelaajan(pelaaja) && 
                    x.Tilanne == PelinTilanne.Pelattu && 
                    x.Havisi(pelaaja) && 
                    x.Kierros >= this.EkaPudariKierros))
                {
                    tappiot = 2;
                }
                else if (arvotutPelit.Any(x => x.Kierros <= this.Kierros &&
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

        private int LaskeKeskeneraisetPelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            return pelatutPelit.Count(x =>
                x.Kierros <= this.Kierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu) +
                arvotutPelit.Count(x =>
                x.Kierros <= this.Kierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu);
        }

        private int LaskeKeskeneraisetPudariPelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            return pelatutPelit.Count(x =>
                x.Kierros <= this.Kierros &&
                x.Kierros >= this.EkaPudariKierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu) +
                arvotutPelit.Count(x =>
                x.Kierros <= this.Kierros &&
                x.Kierros >= this.EkaPudariKierros &&
                x.SisaltaaPelaajan(pelaaja) &&
                x.Tilanne != PelinTilanne.Pelattu);
        }

        private int LaskePelit(IEnumerable<HakuPeli> pelit, int pelaaja)
        {
            return pelit.Count(x => x.Kierros <= this.Kierros && x.SisaltaaPelaajan(pelaaja));
        }

        private int LaskePelit(List<HakuPeli> pelatutPelit, List<HakuPeli> arvotutPelit, int pelaaja)
        {
            return 
                pelatutPelit.Count(x => x.Kierros <= this.Kierros && x.SisaltaaPelaajan(pelaaja)) +
                arvotutPelit.Count(x => x.Kierros <= this.Kierros && x.SisaltaaPelaajan(pelaaja));
        }
    }
}
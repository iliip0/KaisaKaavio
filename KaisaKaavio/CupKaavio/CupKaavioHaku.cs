using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace KaisaKaavio.CupKaavio
{
    public class CupKaavioHaku : IHakuAlgoritmi
    {
        private Kilpailu kilpailu = null;
        private CupKaavio cupKaavio = null;

        public int MaxKierros { get; set; } = 3;

        public Exception HakuVirhe { get; private set; } = null;

        public bool PeruutaHaku { get; set; } = false;

        public bool HakuValmis { get; private set; } = true;

        public bool UusiHakuTarvitaan { get; private set; } = false;

        public bool AutomaattinenTestausMenossa { get; set; } = false;

        public List<HakuAlgoritmi.Pelaajat> UudetPelit { get; private set; } = new List<HakuAlgoritmi.Pelaajat>();
        private List<int> CupSijoitus { get; set; } = new List<int>();

        private readonly int PisteytysVoitosta = 100000;
        private readonly int PisteytysPisteesta = 1;

        public class Peluri
        {
            public Pelaaja Pelaaja = null;
            public int PelatutPelit = 0;
            public int KeskeneraisetPelit = 0;
            public int Tappiot = 0;
            public int Voitot = 0;
            public int Pisteet = 0;
            public int Lyontivuorot = 0;

            public int Pisteytys = 0;
            public int MaxPisteytys = 0;
            public int SijoitusAlkupeleissa = 0;
            public double SijoitusAlkupeleissaArvalla = 0.0f;

            public bool SijoitusOnLopullinen = false;

            public List<Peli> Pelit = new List<Peli>();
        }

        private List<Peluri> pelurit = new List<Peluri>();

        public CupKaavioHaku(Kilpailu kilpailu, Loki loki, IStatusRivi status)
        {
            this.kilpailu = kilpailu;
        }

        private int LaskeCupinKoko()
        {
            if (this.pelurit.Any(x => x.PelatutPelit == 0))
            {
                return 0; // Alotetaan cup koon laskeminen vasta kun 1 kierros on pelattu
            }

            int mukana = this.pelurit
                .Where(x => (x.PelatutPelit >= 2) && (x.Tappiot < 2))
                .Count();

            int mukanaMin = mukana;
            int mukanaMax = mukana;

            var peluritKesken = this.pelurit.Where(x => (x.PelatutPelit < 2));
            if (peluritKesken.Any())
            {
                foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros <= 2 && x.Tilanne != PelinTilanne.Pelattu))
                {
                    var peluri1 = peluritKesken.First(x => x.Pelaaja.Id == peli.Id1);
                    var peluri2 = peluritKesken.First(x => x.Pelaaja.Id == peli.Id2);

                    if (peluri1.Tappiot == 0 && peluri2.Tappiot == 0)
                    {
                        // Molemmat cuppiin
                        mukanaMax += 2;
                        mukanaMin += 2;
                    }
                    else if (peluri1.Tappiot == 1 && peluri2.Tappiot == 1)
                    {
                        // Vain toinen cuppiin
                        mukanaMax += 1;
                        mukanaMin += 1;
                    }
                    else
                    {
                        // Toinen tai molemmat cuppiin
                        mukanaMax += 2;
                        mukanaMin += 1;
                    }
                }
            }

            int cupinKokoMax = 2;
            while (cupinKokoMax < mukanaMax)
            {
                cupinKokoMax *= 2;
            }

            int cupinKokoMin = 2;
            while (cupinKokoMin < mukanaMin)
            {
                cupinKokoMin *= 2;
            }

            if (cupinKokoMin != cupinKokoMax)
            {
#if DEBUG
                Debug.WriteLine("Cup koko ei vielä selvillä");
#endif
                return 0; // Cupin koko ei vielä varmuudella tiedossa
            }

            return cupinKokoMin;
        }

        public void Hae()
        {
            // Alkukierrokset
            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros < 3))
            {
                LuePeli(peli);
            }

            foreach (var peluri in pelurit)
            {
                if (peluri.Pelit.Count != 2)
                {
                    throw new Exception(string.Format("Pelaajalla {0} on liian vähän alkukierrosten pelejä", peluri.Pelaaja.Nimi));
                }
            }

            int cupinKoko = LaskeCupinKoko();
            if (cupinKoko <= 0)
            {
#if DEBUG
                Debug.WriteLine("Cup koko ei vielä selvillä");
#endif
                return; // Cupin koko ei vielä varmuudella tiedossa
            }

            var mukana = this.pelurit.Where(x => (x.PelatutPelit < 2) || (x.Tappiot < 2));

            Debug.WriteLine("CUPPIIN:");

            // Arvotaan tasapisteissä olevien pelaajien sijoitus
            int sijoitus = 0;
            foreach (var peluri in mukana.OrderByDescending(x => x.Pisteytys))
            {
#if DEBUG
                Debug.WriteLine(string.Format("  {0}. {1} [{2}]", sijoitus, peluri.Pelaaja.Nimi, peluri.Pisteytys));
#endif
                peluri.SijoitusAlkupeleissa = sijoitus++;
                peluri.SijoitusAlkupeleissaArvalla = peluri.SijoitusAlkupeleissa + new Random().NextDouble() * 0.5; // Arpoo järjestyksen jos on tasapisteet
            }
            sijoitus = 0;
            foreach (var peluri in mukana.OrderBy(x => x.SijoitusAlkupeleissaArvalla))
            {
                peluri.SijoitusAlkupeleissa = sijoitus++;
                peluri.SijoitusOnLopullinen = true;
            }

            // Tarkistetaan onko sijoitus lopullinen jos alkupelejä on kesken
            if (pelurit.Any(x => x.PelatutPelit < 2))
            {
                for (int i = 0; i < pelurit.Count; ++i)
                {
                    var peluri = pelurit[i];

                    if (peluri.Voitot > 0)
                    {
                        peluri.SijoitusOnLopullinen = true;

                        if (pelurit.Count(x => x.Pisteytys == peluri.Pisteytys) > 1)
                        {
                            peluri.SijoitusOnLopullinen = false; // Tasapisteissä on muita pelaajia, sijoitus ei ole varma
                        }
                        else
                        {
                            var paremmat = pelurit.Where(x => x.Pisteytys > peluri.Pisteytys);
                            if (paremmat.Any(x => x.Pisteytys <= peluri.MaxPisteytys))
                            {
                                peluri.SijoitusOnLopullinen = false; // Pelaaja voi vielä kiriä toisen pelaajan edelle
                            }
                            else
                            {
                                var huonommat = pelurit.Where(x => x.Pisteytys < pelurit[i].Pisteytys);
                                if (huonommat.Any(x => x.MaxPisteytys >= peluri.Pisteytys))
                                {
                                    peluri.SijoitusOnLopullinen = false; // Joku voi vielä tulla takaa ohi
                                }
                            }
                        }
                    }
                    else
                    {
                        peluri.SijoitusOnLopullinen = false;
                    }
                }
            }

            this.kilpailu.CupKoko = cupinKoko;
            this.cupKaavio = new CupKaavio(cupinKoko);

            this.cupKaavio.SijoitaEkaCupKierros(pelurit);

            if (!this.kilpailu.Pelit.Any(x => x.Kierros <= 2 && x.Tilanne != PelinTilanne.Pelattu))
            {
                this.cupKaavio.PaivitaCupPelit(kilpailu);
            }

            foreach (var kierros in cupKaavio.Pelit)
            {
                foreach (var peli in kierros)
                {
                    HakuAlgoritmi.Pelaajat pelaajat = new HakuAlgoritmi.Pelaajat();

                    pelaajat.Pelaaja1 = peli.Pelaaja1;
                    pelaajat.Pelaaja2 = peli.Pelaaja2;
                    pelaajat.Kierros = peli.Kierros;
                    pelaajat.PelinumeroKierroksella = peli.PelinNumero;
                    pelaajat.PeliOnPelattu = peli.Pelattu;

                    this.UudetPelit.Add(pelaajat);
                }
            }

            /*
            List<int> sijoittamattomat = new List<int>();

            for (int k = 3; k <= cupinKoko; k++)
            {
                sijoittamattomat.Add(k);
            }

            List<Tuple<int, int>> parit = new List<Tuple<int, int>>();

            parit.Add(new Tuple<int, int>(2, 1));
            while ((parit.Count * 2) < cupinKoko)
            {
                var sijoitetaan = sijoittamattomat.Take(parit.Count * 2).Reverse().ToList();
                sijoittamattomat.RemoveRange(0, parit.Count * 2);

                List<Tuple<int, int>> uudetParit = new List<Tuple<int, int>>();
                foreach (var pari in parit)
                {
                    uudetParit.Add(new Tuple<int, int>(sijoitetaan[pari.Item1-1], pari.Item1));
                    uudetParit.Add(new Tuple<int, int>(sijoitetaan[pari.Item2-1], pari.Item2));
                }

                parit.Clear();
                parit.AddRange(uudetParit);
            }

            CupSijoitus.Clear();
            foreach (var p in parit)
            {
                CupSijoitus.Add(p.Item1);
                CupSijoitus.Add(p.Item2);
            }
            */

            /*
            var sijoitettavat = mukana.OrderBy(x => x.SijoitusAlkupeleissaArvalla).ToArray();

            int peliNumero = 1;

            for (int c = 0; c < CupSijoitus.Count;)
            {
                int sija1 = CupSijoitus[c++];
                int sija2 = CupSijoitus[c++];

                Peluri p1 = null;
                if (sija1 <= mukana.Count())
                {
                    p1 = sijoitettavat[sija1 - 1];
                }

                Peluri p2 = null;
                if (sija2 <= mukana.Count())
                {
                    p2 = sijoitettavat[sija2 - 1];
                }

                LisaaPeli(p1, p2, 3, peliNumero++);
            }

            // Lisätään "tyhjät pelit loppu cuppiin
            {
                int k = 4;
                int koko = cupinKoko / 2;

                while (koko >= 2)
                {
                    int numero = 1;

                    for (int ii = 0; ii < koko / 2; ++ii)
                    {
                        LisaaPeli(null, null, k, numero++);
                    }

                    k++;
                    koko /= 2;
                }
            }
            */

            // Arvotaan loput cup kierrokset
            /*
            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros >= 3))
            {
                LuePeli(peli);
            }

            int kierros = 4;
            while (true)
            {
                var pelit = this.kilpailu.Pelit.Where(x => x.Kierros == kierros - 1).ToList();

                if (pelit.Count() <= 1)
                {
                    return;
                }

                foreach (var eka in pelit.Where(x => x.PeliNumeroKierroksella % 2 == 1))
                {
                    Peluri p1 = null;
                    Peluri p2 = null;

                    if (eka.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (eka.Tulos == PelinTulos.Pelaaja1Voitti)
                        {
                            p1 = this.pelurit.FirstOrDefault(x => x.Pelaaja.Id == eka.Id1);
                        }
                        else
                        {
                            p1 = this.pelurit.FirstOrDefault(x => x.Pelaaja.Id == eka.Id2);
                        }
                    }

                    var toka = pelit.FirstOrDefault(x => x.PeliNumeroKierroksella == eka.PeliNumeroKierroksella + 1);
                    if (toka != null && toka.Tilanne == PelinTilanne.Pelattu)
                    {
                        if (toka.Tulos == PelinTulos.Pelaaja1Voitti)
                        {
                            p2 = this.pelurit.FirstOrDefault(x => x.Pelaaja.Id == toka.Id1);
                        }
                        else
                        {
                            p2 = this.pelurit.FirstOrDefault(x => x.Pelaaja.Id == toka.Id2);
                        }
                    }

                    if (p1 != null || p2 != null)
                    {
                        LisaaPeli(p1, p2, kierros, eka.PeliNumeroKierroksella / 2);
                    }
                }

                kierros++;
            }
            */
        }

        private void LisaaPeli(Peluri p1, Peluri p2, int kierros, int pelinumero)
        {
            /*
            var peli = this.kilpailu.Pelit.FirstOrDefault(x => x.Kierros == kierros && x.PeliNumeroKierroksella == pelinumero);
            if (peli != null)
            {
                bool lisattiinPelaaja = false;

                if (peli.Id1 <= 0 && p1 != null && !peli.SisaltaaPelaajan(p1.Pelaaja.Id))
                {
                    peli.PelaajaId1 = p1.Pelaaja.Id.ToString();
                    lisattiinPelaaja = true;
                }

                if (peli.Id2 <= 0 && p2 != null && !peli.SisaltaaPelaajan(p2.Pelaaja.Id))
                {
                    peli.PelaajaId2 = p2.Pelaaja.Id.ToString();
                    lisattiinPelaaja = true;
                }

                if (lisattiinPelaaja)
                {
                    if (peli.Id1 > 0 && peli.Id2 > 0)
                    {
                        peli.Tilanne = PelinTilanne.ValmiinaAlkamaan;
                    }
                }

                return;
            }
            */

            UudetPelit.Add(new HakuAlgoritmi.Pelaajat()
            {
                Pelaaja1 = p1 != null ? p1.Pelaaja : null,
                Pelaaja2 = p2 != null ? p2.Pelaaja : null,
                Kierros = kierros,
                PelinumeroKierroksella = pelinumero
            });
        }

        private void LuePeli(Peli peli)
        {
            var pelaaja1 = this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id1);
            var pelaaja2 = this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id2);

            Peluri peluri1 = null;
            if (pelaaja1 != null)
            {
                peluri1 = this.pelurit.FirstOrDefault(x => x.Pelaaja == pelaaja1);
                if (peluri1 == null)
                {
                    peluri1 = new Peluri() { Pelaaja = pelaaja1 };
                    this.pelurit.Add(peluri1);
                }

                peluri1.Pelit.Add(peli);
                peluri1.Pisteet += peli.Pisteet(pelaaja1.Id);
                peluri1.Lyontivuorot += peli.LyontivuorojaInt;

                if (peli.Tilanne == PelinTilanne.Pelattu)
                {
                    peluri1.PelatutPelit++;

                    peluri1.Pisteytys += peluri1.Pisteet * PisteytysPisteesta;
                    peluri1.MaxPisteytys += peluri1.Pisteet * PisteytysPisteesta;

                    if (peli.Voitti(pelaaja1.Id))
                    {
                        peluri1.Voitot++;

                        peluri1.Pisteytys += 1 * PisteytysVoitosta;
                        peluri1.MaxPisteytys += 1 * PisteytysVoitosta;
                    }
                    else if (peli.Havisi(pelaaja1.Id))
                    {
                        peluri1.Tappiot++;
                    }
                }
                else
                {
                    peluri1.KeskeneraisetPelit++;

                    peluri1.MaxPisteytys += 1 * PisteytysVoitosta + ((int)kilpailu.TavoitePistemaara) * PisteytysPisteesta;
                }
            }

            Peluri peluri2 = null;
            if (pelaaja2 != null)
            {
                peluri2 = this.pelurit.FirstOrDefault(x => x.Pelaaja == pelaaja2);
                if (peluri2 == null)
                {
                    peluri2 = new Peluri() { Pelaaja = pelaaja2 };
                    this.pelurit.Add(peluri2);
                }

                peluri2.Pelit.Add(peli);
                peluri2.Pisteet += peli.Pisteet(pelaaja2.Id);
                peluri2.Lyontivuorot += peli.LyontivuorojaInt;

                if (peli.Tilanne == PelinTilanne.Pelattu)
                {
                    peluri2.PelatutPelit++;

                    peluri2.Pisteytys += peluri2.Pisteet * PisteytysPisteesta;
                    peluri2.MaxPisteytys += peluri2.Pisteet * PisteytysPisteesta;

                    if (peli.Voitti(pelaaja2.Id))
                    {
                        peluri2.Voitot++;

                        peluri2.Pisteytys += 1 * PisteytysVoitosta;
                        peluri2.MaxPisteytys += 1 * PisteytysVoitosta;
                    }
                    else if (peli.Havisi(pelaaja2.Id))
                    {
                        peluri2.Tappiot++;
                    }
                }
                else
                {
                    peluri2.KeskeneraisetPelit++;

                    peluri2.MaxPisteytys += 1 * PisteytysVoitosta + ((int)kilpailu.TavoitePistemaara) * PisteytysPisteesta;
                }
            }
        }
    }
}

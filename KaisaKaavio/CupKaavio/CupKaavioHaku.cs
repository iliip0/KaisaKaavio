using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace KaisaKaavio.CupKaavio
{
    public class CupKaavioHaku : IHakuAlgoritmi
    {
        private Kilpailu kilpailu = null;

        public int MaxKierros { get; set; } = 3;

        public Exception HakuVirhe { get; private set; } = null;

        public bool PeruutaHaku { get; set; } = false;

        public bool HakuValmis { get; private set; } = true;

        public bool UusiHakuTarvitaan { get; private set; } = false;

        public bool AutomaattinenTestausMenossa { get; set; } = false;

        public List<HakuAlgoritmi.Pelaajat> UudetPelit { get; private set; } = new List<HakuAlgoritmi.Pelaajat>();
        private List<int> CupSijoitus { get; set; } = new List<int>();

        private class Peluri
        {
            public Pelaaja Pelaaja = null;
            public int PelatutPelit = 0;
            public int KeskeneraisetPelit = 0;
            public int Tappiot = 0;
            public int Voitot = 0;
            public int Pisteet = 0;
            public int Lyontivuorot = 0;

            public int Pisteytys = 0;
            public int SijoitusAlkupeleissa = 0;
            public double SijoitusAlkupeleissaArvalla = 0.0f;

            public List<Peli> Pelit = new List<Peli>();
        }

        private List<Peluri> pelurit = new List<Peluri>();

        public CupKaavioHaku(Kilpailu kilpailu, Loki loki, IStatusRivi status)
        {
            this.kilpailu = kilpailu;
        }

        public void Hae()
        {
            // Alkukierrokset
            foreach (var peli in this.kilpailu.Pelit.Where(x => x.Kierros < 3))
            {
                LuePeli(peli);
            }

            if (pelurit.Any(x => x.PelatutPelit < 2))
            {
                return;
                // Alkupelit kesken
            }

            // Arvo eka cup kierros

            var mukana = this.pelurit.Where(x => x.Tappiot < 2);

            foreach (var peluri in mukana)
            {
                peluri.Pisteytys = (peluri.Voitot * 100000) + peluri.Pisteet;
            }

            Debug.WriteLine("CUPPIIN:");

            int i = 1;
            foreach (var peluri in mukana.OrderByDescending(x => x.Pisteytys))
            {
#if DEBUG
                Debug.WriteLine(string.Format("  {0}. {1} [{2}]", i, peluri.Pelaaja.Nimi, peluri.Pisteytys));
#endif
                peluri.SijoitusAlkupeleissa = i++;
                peluri.SijoitusAlkupeleissaArvalla = peluri.SijoitusAlkupeleissa + new Random().NextDouble() * 0.5; // Arpoo järjestyksen jos on tasapisteet
            }

            int cupinKoko = 2;
            while (cupinKoko < mukana.Count())
            {
                cupinKoko *= 2;
            }

            this.kilpailu.CupKoko = cupinKoko;

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

            // Arvotaan loput cup kierrokset
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

                    if (peli.Voitti(pelaaja1.Id))
                    {
                        peluri1.Voitot++;
                    }
                    else if (peli.Havisi(pelaaja1.Id))
                    {
                        peluri1.Tappiot++;
                    }
                }
                else
                {
                    peluri1.KeskeneraisetPelit++;
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

                    if (peli.Voitti(pelaaja2.Id))
                    {
                        peluri2.Voitot++;
                    }
                    else if (peli.Havisi(pelaaja2.Id))
                    {
                        peluri2.Tappiot++;
                    }
                }
                else
                {
                    peluri2.KeskeneraisetPelit++;
                }
            }
        }
    }
}

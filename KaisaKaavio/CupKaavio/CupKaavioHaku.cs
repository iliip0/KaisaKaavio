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

        private readonly float PisteytysVoitosta = 100000;
        private readonly float PisteytysPisteesta = 1;

        public class Peluri
        {
            public Pelaaja Pelaaja = null;
            public int PelatutPelit = 0;
            public int KeskeneraisetPelit = 0;
            public int Tappiot = 0;
            public int Voitot = 0;
            public int Pisteet = 0;
            public int Lyontivuorot = 0;

            public float Pisteytys = 0;
            public float MinPisteytys = 0;
            public float MaxPisteytys = 0;
            public int SijoitusAlkupeleissa = 0;

            public bool SijoitusOnLopullinen = false;

            public List<Peli> Pelit = new List<Peli>();
        }

        private List<Peluri> pelurit = new List<Peluri>();
        private int pelaajiaMukanaVahintaan = 0;
        private int pelaajiaMukanaEnintaan = 0;

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

            pelaajiaMukanaVahintaan = mukanaMin;
            pelaajiaMukanaEnintaan = mukanaMax;

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
                peluri.MinPisteytys += peluri.Pelaaja.CupKaavioArpa;
                peluri.MaxPisteytys += peluri.Pelaaja.CupKaavioArpa;
                peluri.Pisteytys += peluri.Pelaaja.CupKaavioArpa;

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
                Debug.WriteLine(string.Format("  {0}. {1} [{2}] ({3}-{4})",
                    sijoitus + 1,
                    peluri.Pelaaja.Nimi,
                    (int)peluri.Pisteytys,
                    (int)peluri.MinPisteytys,
                    (int)peluri.MaxPisteytys));
#endif
                peluri.SijoitusAlkupeleissa = sijoitus++;
                peluri.SijoitusOnLopullinen = true;
            }

            // Tarkistetaan onko sijoitus lopullinen jos alkupelejä on kesken
            if (pelurit.Any(x => x.PelatutPelit < 2))
            {
                foreach (var peluri in pelurit)
                {
                    peluri.SijoitusOnLopullinen = false;
                    if (peluri.Voitot > 0)
                    {
                        var paremmat = pelurit.Where(x => (x != peluri) && (x.MinPisteytys > peluri.MaxPisteytys));
                        var huonommat = pelurit.Where(x => (x != peluri) && (x.MaxPisteytys < peluri.MinPisteytys));

                        peluri.SijoitusOnLopullinen = (paremmat.Count() + huonommat.Count()) == (pelurit.Count - 1);
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

                    if (pelaajat.Pelaaja1 == null)
                    {
                        if (peli.Pelattu)
                        {
                            pelaajat.CupTeksti1 = "w.o.";
                        }
                        else if (peli.Kierros == 3 && peli.Sijoitus1 >= pelaajiaMukanaEnintaan)
                        {
                            pelaajat.CupTeksti1 = "w.o.";
                        }
                        else if (peli.Kierros > 3)
                        {
                            pelaajat.CupTeksti1 = string.Format("Voittaja pelistä {0}", peli.EdellinenPeli1.Id);
                        }
                        else
                        {
                            pelaajat.CupTeksti1 = string.Format("Alkukierrosten {0}.", peli.Sijoitus1 + 1);
                            if (peli.Sijoitus1 >= pelaajiaMukanaVahintaan)
                            {
                                pelaajat.CupTeksti1 += " tai w.o.";
                            }
                        }
                    }

                    if (pelaajat.Pelaaja2 == null)
                    {
                        if (peli.Pelattu)
                        {
                            pelaajat.CupTeksti2 = "w.o.";
                        }
                        else if (peli.Kierros == 3 && peli.Sijoitus2 >= pelaajiaMukanaEnintaan)
                        {
                            pelaajat.CupTeksti2 = "w.o.";
                        }
                        else if (peli.Kierros > 3)
                        {
                            pelaajat.CupTeksti2 = string.Format("Voittaja pelistä {0}", peli.EdellinenPeli2.Id);
                        }
                        else
                        {
                            pelaajat.CupTeksti2 = string.Format("Alkukierrosten {0}.", peli.Sijoitus2 + 1);
                            if (peli.Sijoitus2 >= pelaajiaMukanaVahintaan)
                            {
                                pelaajat.CupTeksti2 += " tai w.o.";
                            }
                        }
                    }

                    if (peli.Kierros == 3)
                    {
                        pelaajat.CupSijoitus1 = peli.Sijoitus1;
                        pelaajat.CupSijoitus2 = peli.Sijoitus2;
                    }

                    this.UudetPelit.Add(pelaajat);
                }
            }
        }

        private void LuePeli(Peli peli)
        {
            var pelaaja1 = this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id1);
            var pelaaja2 = this.kilpailu.Osallistujat.FirstOrDefault(x => x.Id == peli.Id2);

            Random r = new Random();

            if (pelaaja1.CupKaavioArpa <= 0)
            {
                pelaaja1.CupKaavioArpa = (float)r.NextDouble();
            }

            if (pelaaja2.CupKaavioArpa <= 0)
            {
                pelaaja2.CupKaavioArpa = (float)r.NextDouble();
            }

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
                peluri1.Pisteytys += peli.Pisteet(pelaaja1.Id) * PisteytysPisteesta;
                peluri1.Lyontivuorot += peli.LyontivuorojaInt;

                if (peli.Tilanne == PelinTilanne.Pelattu)
                {
                    peluri1.PelatutPelit++;

                    peluri1.MinPisteytys += peli.Pisteet(pelaaja1.Id) * PisteytysPisteesta;
                    peluri1.MaxPisteytys += peli.Pisteet(pelaaja1.Id) * PisteytysPisteesta;

                    if (peli.Voitti(pelaaja1.Id))
                    {
                        peluri1.Voitot++;

                        peluri1.Pisteytys += 1 * PisteytysVoitosta;
                        peluri1.MinPisteytys += 1 * PisteytysVoitosta;
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
                peluri2.Pisteytys += peli.Pisteet(pelaaja2.Id) * PisteytysPisteesta;
                peluri2.Lyontivuorot += peli.LyontivuorojaInt;

                if (peli.Tilanne == PelinTilanne.Pelattu)
                {
                    peluri2.PelatutPelit++;

                    peluri2.MinPisteytys += peli.Pisteet(pelaaja2.Id) * PisteytysPisteesta;
                    peluri2.MaxPisteytys += peli.Pisteet(pelaaja2.Id) * PisteytysPisteesta;

                    if (peli.Voitti(pelaaja2.Id))
                    {
                        peluri2.Voitot++;

                        peluri2.Pisteytys += 1 * PisteytysVoitosta;
                        peluri2.MinPisteytys += 1 * PisteytysVoitosta;
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

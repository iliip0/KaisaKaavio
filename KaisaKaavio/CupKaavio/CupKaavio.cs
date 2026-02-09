using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KaisaKaavio.CupKaavio.CupKaavio;

namespace KaisaKaavio.CupKaavio
{
    internal class CupKaavio
    {
        private int koko = 0;

        public class CupPeli
        {
            public int Kierros = 0;
            public int PelinNumero = 0;
            public int Sijoitus1 = 0;
            public int Sijoitus2 = 0;
            public Pelaaja Pelaaja1 = null;
            public Pelaaja Pelaaja2 = null;
            public CupPeli SeuraavaPeli = null;
            public CupPeli EdellinenPeli1 = null;
            public CupPeli EdellinenPeli2 = null;

            public bool Pelattu = false;
        }

        public List<List<CupPeli>> Pelit { get; private set; } = new List<List<CupPeli>>();
        private List<Tuple<int, int>> CupSijoitus { get; set; } = new List<Tuple<int, int>>();

        public CupKaavio(int koko)
        {
            this.koko = koko;

            int peleja = koko / 2;
            int kierros = 3;

            while (peleja > 1)
            {
                List<CupPeli> kierroksenPelit = new List<CupPeli>();

                for (int i = 0; i < peleja; i++)
                {
                    kierroksenPelit.Add(new CupPeli()
                    {
                        Kierros = kierros,
                        PelinNumero = i
                    });
                }

                Pelit.Add(kierroksenPelit);

                peleja /= 2;
                kierros++;
            }

            // Lisätään finaali
            List<CupPeli> finaaliKierros = new List<CupPeli>();
            finaaliKierros.Add(new CupPeli()
            {
                Kierros = kierros
            });
            Pelit.Add(finaaliKierros);

            // Linkitetään cup pelit toisiinsa
            LinkitaCupPelit();

            // Lasketaan sijoitukset ekalle cup kierrokselle
            List<int> sijoittamattomat = new List<int>();

            for (int k = 3; k <= koko; k++)
            {
                sijoittamattomat.Add(k);
            }

            List<Tuple<int, int>> parit = new List<Tuple<int, int>>();

            parit.Add(new Tuple<int, int>(2, 1));
            while ((parit.Count * 2) < koko)
            {
                var sijoitetaan = sijoittamattomat.Take(parit.Count * 2).Reverse().ToList();
                sijoittamattomat.RemoveRange(0, parit.Count * 2);

                List<Tuple<int, int>> uudetParit = new List<Tuple<int, int>>();
                foreach (var pari in parit)
                {
                    uudetParit.Add(new Tuple<int, int>(sijoitetaan[pari.Item1 - 1], pari.Item1));
                    uudetParit.Add(new Tuple<int, int>(sijoitetaan[pari.Item2 - 1], pari.Item2));
                }

                parit.Clear();
                parit.AddRange(uudetParit);
            }

            CupSijoitus.Clear();
            foreach (var p in parit)
            {
                CupSijoitus.Add(new Tuple<int, int>(p.Item1 - 1, p.Item2 - 1));
            }

            if (CupSijoitus.Count != Pelit[0].Count)
            {
                throw new Exception("Bugi! Cup kaavio laskettu väärin");
            }

            for (int i = 0; i < CupSijoitus.Count; ++i)
            {
                if (CupSijoitus[i].Item1 < CupSijoitus[i].Item2)
                {
                    Pelit[0][i].Sijoitus1 = CupSijoitus[i].Item1;
                    Pelit[0][i].Sijoitus2 = CupSijoitus[i].Item2;
                }
                else
                {
                    Pelit[0][i].Sijoitus1 = CupSijoitus[i].Item2;
                    Pelit[0][i].Sijoitus2 = CupSijoitus[i].Item1;
                }
            }
        }

        private void LinkitaCupPelit()
        {
            for (int i = 0; i < Pelit.Count; ++i)
            {
                var kierroksenPelit = Pelit[i];
                for (int j = 0; j < kierroksenPelit.Count; ++j)
                {
                    if (i > 0)
                    {
                        var edellisenKierroksenPelit = Pelit[i - 1];
                        kierroksenPelit[j].EdellinenPeli1 = edellisenKierroksenPelit[j * 2];
                        kierroksenPelit[j].EdellinenPeli2 = edellisenKierroksenPelit[j * 2 + 1];
                    }

                    if (i < Pelit.Count - 1)
                    {
                        var seuraavanKierroksenPelit = Pelit[i + 1];
                        kierroksenPelit[j].SeuraavaPeli = seuraavanKierroksenPelit[j / 2];
                    }
                }
            }
        }

        public void SijoitaEkaCupKierros(List<CupKaavioHaku.Peluri> pelurit)
        {
            var kierros = Pelit.First();

            foreach (var peluri in pelurit)
            {
                if (peluri.SijoitusOnLopullinen)
                {
                    var peli = kierros.FirstOrDefault(x => 
                        x.Sijoitus1 == peluri.SijoitusAlkupeleissa ||
                        x.Sijoitus2 == peluri.SijoitusAlkupeleissa);

                    if (peli != null)
                    {
                        if (peli.Sijoitus1 == peluri.SijoitusAlkupeleissa)
                        {
                            if (peli.Pelaaja1 != null)
                            {
                                throw new Exception("Bugi! Cup kaavion paikka on jo varattu");
                            }
                            peli.Pelaaja1 = peluri.Pelaaja;
                        }
                        else
                        {
                            if (peli.Pelaaja2 != null)
                            {
                                throw new Exception("Bugi! Cup kaavion paikka on jo varattu");
                            }
                            peli.Pelaaja2 = peluri.Pelaaja;
                        }
                    }
                }
            }

            // Tarkista onko w.o. pelit valmiita
            bool cupValmis = !pelurit.Any(x => x.PelatutPelit < 2);
            if (cupValmis)
            {
                foreach (var peli in kierros)
                {
                    if (peli.Pelaaja1 != null && peli.Pelaaja2 == null)
                    {
                        peli.Pelattu = true;

                        if (peli.SeuraavaPeli != null)
                        {
                            if (peli.PelinNumero % 2 == 0)
                            {
                                peli.SeuraavaPeli.Pelaaja1 = peli.Pelaaja1;
                            }
                            else
                            {
                                peli.SeuraavaPeli.Pelaaja2 = peli.Pelaaja1;
                            }
                        }
                    }
                }
            }
        }

        public void PaivitaCupPelit(Kilpailu kilpailu)
        {
            foreach (var peli in kilpailu.Pelit.Where(x => x.Kierros > 2 && x.Tilanne == PelinTilanne.Pelattu))
            {
                int cupKierros = peli.Kierros - 3;
                if (cupKierros < 0 || cupKierros >= Pelit.Count)
                {
                    throw new Exception("Bugi! Peli cupin ulkopuolella");
                }

                var kierros = Pelit[cupKierros];
                var cupPeli = kierros.FirstOrDefault(x => x.PelinNumero == peli.PeliNumeroKierroksella);
                if (cupPeli == null)
                {
                    throw new Exception("Bugi! Cup peliä ei löytynyt");
                }

                if (cupPeli.Pelaaja1 == null || cupPeli.Pelaaja2 == null)
                {
                    continue;
                }

                if (cupPeli.Pelaaja1 != null && !peli.SisaltaaPelaajan(cupPeli.Pelaaja1.Id))
                {
                    continue; // Cup kaavio muuttuu tässä kohtaa haun jälkeen
                }

                if (cupPeli.Pelaaja2 != null && !peli.SisaltaaPelaajan(cupPeli.Pelaaja2.Id))
                {
                    continue; // Cup kaavio muuttuu tässä kohtaa haun jälkeen
                }

                if (peli.Voitti(peli.Id1))
                {
                    cupPeli.Pelattu = true;
                    if (cupPeli.SeuraavaPeli != null)
                    {
                        if (peli.PeliNumeroKierroksella % 2 == 0)
                        {
                            cupPeli.SeuraavaPeli.Pelaaja1 = cupPeli.Pelaaja1;
                        }
                        else
                        {
                            cupPeli.SeuraavaPeli.Pelaaja2 = cupPeli.Pelaaja1;
                        }
                    }
                }
                else if (peli.Voitti(peli.Id2))
                {
                    cupPeli.Pelattu = true;
                    if (cupPeli.SeuraavaPeli != null)
                    {
                        if (peli.PeliNumeroKierroksella % 2 == 0)
                        {
                            cupPeli.SeuraavaPeli.Pelaaja1 = cupPeli.Pelaaja2;
                        }
                        else
                        {
                            cupPeli.SeuraavaPeli.Pelaaja2 = cupPeli.Pelaaja2;
                        }
                    }
                }
            }
        }
    }
}

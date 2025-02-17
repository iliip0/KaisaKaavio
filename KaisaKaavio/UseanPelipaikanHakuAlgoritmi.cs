using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio
{
    /// <summary>
    /// Hakualgoritmin toteutus kun pelataan monella salilla samanaikaisesti
    /// </summary>
    public class UseanPelipaikanHakuAlgoritmi : IHakuAlgoritmi
    {
        private List<IHakuAlgoritmi> haut = new List<IHakuAlgoritmi>();
        private int maxKierros = 0;

        public UseanPelipaikanHakuAlgoritmi(List<Kilpailu> kilpailut, Loki loki, int kierros, IStatusRivi status, int maxKierros)
        {
            this.maxKierros = maxKierros;

            foreach (var kilpailu in kilpailut)
            {
                this.haut.Add(new HakuAlgoritmi(kilpailu, loki, kierros, status));
            }
        }

        public UseanPelipaikanHakuAlgoritmi(List<IHakuAlgoritmi> haut, int maxKierros)
        {
            this.maxKierros = maxKierros;

            foreach (var haku in haut)
            {
                this.haut.Add(haku);
            }
        }

        public Exception HakuVirhe
        {
            get 
            {
                foreach (var h in this.haut)
                {
                    if (h.HakuVirhe != null)
                    {
                        return h.HakuVirhe;
                    }
                }

                return null;
            }
        }

        public int Kierros
        {
            get
            {
                if (this.haut.Any())
                {
                    return this.haut.Select(x => x.Kierros).Max();
                }

                return 1;
            }
        }

        public bool PeruutaHaku
        {
            get
            {
                foreach (var h in this.haut)
                {
                    if (h.PeruutaHaku)
                    {
                        return true;
                    }
                }

                return false;
            }
            set
            {
                foreach (var h in this.haut)
                {
                    h.PeruutaHaku = value;
                }
            }
        }

        public bool HakuValmis
        {
            get 
            {
                foreach (var h in this.haut)
                {
                    if (!h.HakuValmis)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool UusiHakuTarvitaan
        {
            get 
            {
                foreach (var h in this.haut)
                {
                    if (h.UusiHakuTarvitaan)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool AutomaattinenTestausMenossa
        {
            get
            {
                foreach (var h in this.haut)
                {
                    if (h.AutomaattinenTestausMenossa)
                    {
                        return true;
                    }
                }

                return false;
            }
            set
            {
                foreach (var h in this.haut)
                {
                    h.AutomaattinenTestausMenossa = value;
                }
            }
        }

        public List<HakuAlgoritmi.Pelaajat> UudetPelit
        {
            get 
            {
                List<HakuAlgoritmi.Pelaajat> pelit = new List<HakuAlgoritmi.Pelaajat>();

                foreach (var h in this.haut)
                {
                    foreach (var p in h.UudetPelit)
                    {
                        if (p.Kierros <= this.maxKierros)
                        {
                            pelit.Add(p);
                        }
                    }
                }

                return pelit;
            }
        }

        public void Hae()
        {
            foreach (var h in this.haut)
            {
                h.Hae();
            }
        }
    }
}

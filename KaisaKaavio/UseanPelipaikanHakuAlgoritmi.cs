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

        public UseanPelipaikanHakuAlgoritmi(List<Kilpailu> kilpailut, Loki loki, int kierros, IStatusRivi status)
        {
            foreach (var kilpailu in kilpailut)
            {
                this.haut.Add(new HakuAlgoritmi(kilpailu, loki, kierros, status));
            }
        }

        public UseanPelipaikanHakuAlgoritmi(List<IHakuAlgoritmi> haut)
        {
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
                    pelit.AddRange(h.UudetPelit);
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

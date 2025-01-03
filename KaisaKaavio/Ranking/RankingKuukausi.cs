using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    /// <summary>
    /// Tietue, johon tallentuu lyhyt tietue jokaisesta yhden kuukauden aikana pelatusta ranking osakilpailusta
    /// </summary>
    public class RankingKuukausi
    {
        public int Vuosi { get; set; }

        public int Kuukausi { get; set; }

        [XmlIgnore]
        public string Kansio
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "KaisaKaaviot",
                    "Ranking",
                    this.Vuosi.ToString(),
                    Tyypit.Aika.KuukaudenNimi(this.Kuukausi));
            }
        }

        [XmlIgnore]
        public string TiedostonNimi 
        {
            get
            {
                return Path.Combine(this.Kansio, "RankingKilpailut.xml");
            }
        }

        public BindingList<RankingOsakilpailuTietue> Osakilpailut = new BindingList<RankingOsakilpailuTietue>();

        public RankingKuukausi()
        {
            this.Vuosi = DateTime.Now.Year;
            this.Kuukausi = DateTime.Now.Month;
        }

        public RankingKuukausi(DateTime aika)
        {
            this.Vuosi = aika.Year;
            this.Kuukausi = aika.Month;
        }

        public bool TallennaTarvittaessa(Loki loki)
        {
            if (Osakilpailut.Any(x => x.TallennusTarvitaan))
            {
                return Tallenna(loki);
            }

            return true;
        }

        public void TyhjennaTietueetMuistista(Kilpailu nykyinenKilpailu)
        {
            while (true)
            {
                var tietue = this.Osakilpailut.FirstOrDefault(x => x.Kilpailu != nykyinenKilpailu);
                if (tietue != null)
                {
                    this.Osakilpailut.Remove(tietue);
                }
                else
                {
                    return;
                }
            }
        }

        public bool Tallenna(Loki loki)
        {
            try
            {
                Directory.CreateDirectory(this.Kansio);

                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Tallennetaan ranking dataa tiedostoon {0}", this.TiedostonNimi));
                }

                bool osakilpailujenTallennusOnnistui = true;

                foreach (var o in this.Osakilpailut)
                {
                    if (!o.TallennaOsakilpailu(loki))
                    {
                        osakilpailujenTallennusOnnistui = false;
                    }

                    o.TallennusTarvitaan = false;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(RankingKuukausi));

                string nimiTmp = Path.GetTempFileName();

                using (TextWriter writer = new StreamWriter(nimiTmp))
                {
                    serializer.Serialize(writer, this);
                    writer.Close();
                }

                File.Copy(nimiTmp, this.TiedostonNimi, true);
                File.Delete(nimiTmp);

                return osakilpailujenTallennusOnnistui;
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Ranking datan tallentaminen tiedostoon {0} epäonnistui!", this.TiedostonNimi), e, false);
                }
            }

            return false;
        }

        public bool Avaa(Loki loki)
        {
            try
            {
                Directory.CreateDirectory(this.Kansio);

                XmlSerializer serializer = new XmlSerializer(typeof(RankingKuukausi));

                RankingKuukausi rankingKuukausi = null;

                using (TextReader reader = new StreamReader(this.TiedostonNimi))
                {
                    rankingKuukausi = (RankingKuukausi)serializer.Deserialize(reader);
                    reader.Close();
                }

                if (rankingKuukausi != null)
                {
                    this.Vuosi = rankingKuukausi.Vuosi;
                    this.Kuukausi = rankingKuukausi.Kuukausi;

                    this.Osakilpailut.Clear();
                    foreach (var o in rankingKuukausi.Osakilpailut)
                    {
                        this.Osakilpailut.Add(o);
                        o.TallennusTarvitaan = false;
                        o.RankingKuu = this;
                    }

                    if (loki != null)
                    {
                        loki.Kirjoita(string.Format("Avattiin ranking tiedosto {0}", this.TiedostonNimi));
                    }

                    return true;
                }
                else
                {
                    if (loki != null)
                    {
                        loki.Kirjoita(string.Format("Tiedostosta {0} ei löytynyt ranking dataa", this.TiedostonNimi));
                    }
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Ranking datan lataaminen tiedostosta {0} epäonnistui!", this.TiedostonNimi), e, false);
                }
            }

            return false;
        }

        public RankingOsakilpailuTietue AvaaKilpailunTietue(Kilpailu kilpailu, Loki loki)
        {
            var tietue = this.Osakilpailut.FirstOrDefault(x => string.Equals(x.Id, kilpailu.Id));

            if (tietue == null)
            {
                tietue = new RankingOsakilpailuTietue(kilpailu) 
                {
                    RankingKuu = this 
                };
                
                this.Osakilpailut.Add(tietue);

                //tietue.LataaOsakilpailu(this.Kansio, loki);
            }

            tietue.Kilpailu = kilpailu;
            tietue.Nimi = kilpailu.Nimi;
            tietue.Pvm = kilpailu.AlkamisAika;
            tietue.KilpailunTarkistusSumma = string.Empty;
            tietue.TallennusTarvitaan = true;

            return tietue;
        }
    }
}

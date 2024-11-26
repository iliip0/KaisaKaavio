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
    public class RankingSarja
    {
        public string Nimi { get; set; }
        public RankingSarjanPituus Pituus { get; set; }
        public int SarjanNumero { get; set; }

        [XmlIgnore]
        public string Tiedosto { get; private set; }

        public BindingList<RankingOsakilpailu> Osakilpailut { get; set; }

        private bool muokattu = true;

        public RankingSarja()
        {
            this.Nimi = string.Empty;
            this.Tiedosto = string.Empty;
            this.Pituus = RankingSarjanPituus.Kuukausi;
            this.SarjanNumero = 0;
            this.Osakilpailut = new BindingList<RankingOsakilpailu>();
        }

        public void Avaa(Loki loki, string tiedosto)
        {
            if (loki != null)
            {
                loki.Kirjoita(string.Format("Avataan rankingsarja tiedostosta {0}", tiedosto));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

            RankingSarja sarja = null;

            using (TextReader reader = new StreamReader(tiedosto))
            {
                sarja = (RankingSarja)serializer.Deserialize(reader);
                reader.Close();
            }

            if (sarja != null)
            {
                this.Tiedosto = tiedosto;
                this.Nimi = sarja.Nimi;
                this.Pituus = sarja.Pituus;
                this.SarjanNumero = sarja.SarjanNumero;

                this.muokattu = false;

                this.Osakilpailut.Clear();

                foreach (var o in sarja.Osakilpailut)
                {
                    if (!string.IsNullOrEmpty(o.Nimi) &&
                        o.Osallistujat.Count > 0)
                    {
                        this.Osakilpailut.Add(o);
                    }
                }
            }
        }

        public void Tallenna(Loki loki, string kansio)
        {
            if (!muokattu)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.Nimi))
            { 
                this.Nimi = string.Format("RankingSarja_{0}", this.SarjanNumero);
            }

            this.Tiedosto = Path.Combine(kansio, this.Nimi + ".xml");

            if (loki != null)
            {
                loki.Kirjoita(string.Format("Tallennetaan rankingsarja {0} tiedostoon {1}", this.Nimi, this.Tiedosto));
            }

            XmlSerializer serializer = new XmlSerializer(typeof(RankingSarja));

            string nimiTmp = Path.GetTempFileName();

            using (TextWriter writer = new StreamWriter(nimiTmp))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }

            File.Copy(nimiTmp, this.Tiedosto, true);
            File.Delete(nimiTmp);
        }

        public void LisaaKilpailu(Kilpailu kilpailu, RankingAsetukset asetukset)
        {
            muokattu = true;
        }
    }
}

using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    /// <summary>
    /// Tietue johon tallentuu kilpailun Ranking osakilpailu-parametrit lyhykäisyydessään
    /// </summary>
    public class RankingOsakilpailuTietue
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Id { get; set; }

        private string nimi = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi 
        {
            get
            {
                return this.nimi;
            }
            set
            {
                if (!string.Equals(this.nimi, value))
                {
                    this.nimi = value;
                    this.tallennusTarvitaan = true;
                }
            }
        }

        private string pvm = string.Empty;

        [XmlAttribute]
        [DefaultValue("")]
        public string Pvm 
        {
            get
            {
                return this.pvm;
            }
            set
            {
                if (!string.Equals(this.pvm, value))
                {
                    this.pvm = value;
                    this.tallennusTarvitaan = true;
                }
            }
        }

        /// <summary>
        /// Satunnainen numero joka muuttuu joka kerta kun kilpailu tallennetaan.
        /// Tämän tiedon perusteella katsotaan tarvitseeko ranking osakilpailun pisteet laskea
        /// uudelleen tämän kilpailun osalta
        /// </summary>
        [XmlAttribute]
        [DefaultValue("")]
        public string KilpailunTarkistusSumma { get; set; }

        [XmlIgnore]
        public DateTime PvmDt { get { return Tyypit.Aika.ParseDateTime(this.Pvm); } }

        private bool onRankingOsakilpailu = true;

        [XmlAttribute]
        [DefaultValue(true)]
        public bool OnRankingOsakilpailu 
        {
            get
            {
                return this.onRankingOsakilpailu;
            }
            set
            {
                if (this.onRankingOsakilpailu != value)
                {
                    this.onRankingOsakilpailu = value;
                    this.tallennusTarvitaan = true;
                }
            }
        }

        private RankingSarjanPituus sarjanPituus = RankingSarjanPituus.Kuukausi;

        [XmlAttribute]
        public RankingSarjanPituus SarjanPituus 
        {
            get
            {
                return this.sarjanPituus;
            }
            set
            {
                if (this.sarjanPituus != value)
                {
                    this.sarjanPituus = value;
                    this.tallennusTarvitaan = true;
                }
            }
        }

        private Laji laji = Laji.Kaisa;

        [XmlAttribute]
        public Laji Laji 
        {
            get
            {
                return this.laji;
            }
            set
            {
                if (this.laji != value)
                {
                    this.laji = value;
                    this.tallennusTarvitaan = true;
                }
            }
        }

        [XmlIgnore]
        public RankingOsakilpailu Osakilpailu { get; private set; }

        [XmlIgnore]
        public Kilpailu Kilpailu { get; set; }

        [XmlIgnore]
        public RankingKuukausi RankingKuu { get; set; }

        private bool tallennusTarvitaan = false;

        [XmlIgnore]
        public bool TallennusTarvitaan 
        { 
            get { return this.Kilpailu != null || this.tallennusTarvitaan; }
            set { this.tallennusTarvitaan = value; }
        }

        public RankingOsakilpailuTietue()
        {
            this.Id = string.Empty;
            this.Nimi = string.Empty;
            this.Pvm = string.Empty;
            this.Osakilpailu = null;
            this.Kilpailu = null;
            this.KilpailunTarkistusSumma = string.Empty;
            this.RankingKuu = null;
            this.TallennusTarvitaan = false;
        }

        public RankingOsakilpailuTietue(Kilpailu kilpailu)
        {
            this.Id = kilpailu.Id;
            this.Nimi = kilpailu.Nimi;
            this.Pvm = kilpailu.AlkamisAika;
            this.Laji = kilpailu.Laji;
            this.Osakilpailu = null;
            this.Kilpailu = kilpailu;
            this.KilpailunTarkistusSumma = string.Empty;
            this.RankingKuu = null;
            this.TallennusTarvitaan = true;
        }

        public bool LataaOsakilpailu(Loki loki)
        {
            return false;
        }

        private string KilpailuTiedostonNimi()
        {
            return Path.Combine(this.RankingKuu.Kansio, string.Format("{0}.xml", this.Id));
        }

        public Kilpailu LataaKilpailu(Loki loki)
        {
            string tiedosto = KilpailuTiedostonNimi();

            try
            {
                if (File.Exists(tiedosto))
                {
                    Kilpailu kilpailu = new Kilpailu();

                    kilpailu.Avaa(tiedosto, false);

                    return kilpailu;
                }

                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Kilpailutiedostoa {0} ei löydy", tiedosto), null, false);
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Kilpailutiedoston {0} avaaminen epäonnistui", tiedosto), e, false);
                } 
            }

            return null;
        }

        public void PoistaKilpailu(Loki loki)
        {
            string tiedosto = KilpailuTiedostonNimi();

            try
            {
                if (File.Exists(tiedosto))
                {
                    File.Delete(tiedosto);
                }

                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Poistettiin rankingkilpailutiedosto {0}", tiedosto), null, false);
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Kilpailutiedoston {0} poistaminen epäonnistui", tiedosto), e, false);
                }
            }
        }

        public bool TallennaOsakilpailu(Loki loki)
        {
#if !ALLOW_MULTIPLE_INSTANCES // Rankingeja ei tallenneta kun useita KaisaKaavioita voi olla auki samanaikaisesti
            try
            {
                if (this.Kilpailu != null && !string.IsNullOrEmpty(this.Kilpailu.Id))
                {
                    this.Pvm = this.Kilpailu.AlkamisAika;
                    this.Nimi = this.Kilpailu.Nimi;

                    string tiedosto = KilpailuTiedostonNimi();

                    if (this.OnRankingOsakilpailu)
                    {
                        this.Kilpailu.TallennaNimella(tiedosto, false);
                        this.KilpailunTarkistusSumma = Guid.NewGuid().GetHashCode().ToString("X");
                    }
                    else 
                    {
                        if (File.Exists(tiedosto))
                        {
                            File.Delete(tiedosto);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("RankingOsakilpailuTietueen {0} tallennus epäonnistui", Id), e, false); 
                }
                return false;
            }
#endif
            return true;
        }
    }
}

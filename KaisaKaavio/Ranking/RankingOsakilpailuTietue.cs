using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private bool tallennusTarvitaan = false;

        [XmlIgnore]
        public bool TallennusTarvitaan 
        { 
            get { return this.tallennusTarvitaan; }
            set { this.tallennusTarvitaan = value; }
        }

        public RankingOsakilpailuTietue()
        {
            this.Id = string.Empty;
            this.Osakilpailu = null;
        }

        public RankingOsakilpailuTietue(Kilpailu kilpailu)
        {
            this.Id = kilpailu.Id;
            this.Nimi = kilpailu.Nimi;
            this.Pvm = kilpailu.AlkamisAika;
            this.Osakilpailu = null;
        }

        public bool LataaOsakilpailu(string kansio, Loki loki)
        {
            return false;
        }

        public bool TallennaOsakilpailu(string kansio, Loki loki)
        {
            return true;
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Sali
    {
        [XmlAttribute]
        [DefaultValue(true)]
        public bool Kaytossa { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Nimi { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Osoite { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string PuhelinNumero { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Seura { get; set; }

        [XmlAttribute]
        [DefaultValue("")]
        public string Lyhenne { get; set; }

        [XmlAttribute]
        [DefaultValue(0)]
        public int Pelaajia { get; set; }

        [XmlIgnore]
        public int MinPelaajaId = 0;

        [XmlIgnore]
        public int MaxPelaajaId = 0;

        public BindingList<Linkki> Linkit { get; set; }

        public BindingList<Poyta> Poydat { get; set; }

        public BindingList<Toimitsija> Toimitsijat { get; set; }

        [XmlIgnore]
        public decimal PelaajiaPoytaaKohden
        {
            get
            {
                if (PoytiaKaytettavissa > 0)
                {
                    return (decimal)(((int)(((decimal)Pelaajia / PoytiaKaytettavissa) * 10)) / (decimal)10);
                }
                else
                {
                    return 0;
                }
            }
        }

        [XmlIgnore]
        public string LyhytNimi
        {
            get 
            {
                if (!string.IsNullOrEmpty(this.Lyhenne))
                {
                    return this.Lyhenne;
                }

                return this.Nimi;
            }
        }

        [XmlIgnore]
        public bool Tyhja
        {
            get 
            {
                return string.IsNullOrEmpty(this.Nimi) && string.IsNullOrEmpty(this.Lyhenne);
            }
        }

        [XmlIgnore]
        public int Poytia 
        { 
            get 
            { 
                return this.Poydat.Where(x => !string.IsNullOrEmpty(x.Numero)).Count(); 
            }
        }

        [XmlIgnore]
        public decimal PoytiaKaytettavissa 
        { 
            get 
            { 
                return (decimal)this.Poydat.Where(x => x.Kaytossa && !string.IsNullOrEmpty(x.Numero)).Count(); 
            }
        }

        public Sali()
        {
            this.Kaytossa = true;
            this.Nimi = string.Empty;
            this.Osoite = string.Empty;
            this.PuhelinNumero = string.Empty;
            this.Seura = string.Empty;
            this.Lyhenne = string.Empty;
            this.Linkit = new BindingList<Linkki>();
            this.Poydat = new BindingList<Poyta>();
            this.Toimitsijat = new BindingList<Toimitsija>();
            this.Pelaajia = 0;
        }

        public void KopioiSalista(Sali sali)
        {
            this.Kaytossa = sali.Kaytossa;
            this.Lyhenne = sali.Lyhenne;
            this.Nimi = sali.Nimi;
            this.Osoite = sali.Osoite;
            this.PuhelinNumero = sali.PuhelinNumero;
            this.Seura = sali.Seura;
            this.Pelaajia = sali.Pelaajia;

            this.Poydat.Clear();
            foreach (var p in sali.Poydat.Where(x => !string.IsNullOrEmpty(x.Numero)))
            {
                if (!this.Poydat.Any(x => string.Equals(x.Numero, p.Numero)))
                {
                    this.Poydat.Add(new Poyta()
                    {
                        Numero = p.Numero,
                        Kaytossa = p.Kaytossa,
                        StriimiLinkki = p.StriimiLinkki,
                        TulosLinkki = p.TulosLinkki
                    });
                }
            }

            VarmistaAinakinYksiPoyta();

            this.Toimitsijat.Clear();
            foreach (var t in sali.Toimitsijat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                this.Toimitsijat.Add(new Toimitsija() 
                {
                    Nimi = t.Nimi,
                    PuhelinNumero = t.PuhelinNumero,
                    Rooli = t.Rooli
                });
            }

            this.Linkit.Clear();
            foreach (var l in sali.Linkit.Where(x => !string.IsNullOrEmpty(x.Osoite)))
            {
                this.Linkit.Add(new Linkki() 
                {
                    Osoite = l.Osoite,
                    Teksti = l.Teksti
                });
            }
        }

        public void VarmistaAinakinYksiPoyta()
        {
            List<Poyta> poydat = new List<Poyta>();

            foreach (var p in this.Poydat.Where(x => !string.IsNullOrEmpty(x.Numero)))
            {
                if (!poydat.Any(x => string.Equals(x.Numero, p.Numero)))
                {
                    poydat.Add(p);
                }
            }


            this.Poydat.Clear();

            foreach (var p in poydat.OrderBy(x => x.Numero))
            {
                this.Poydat.Add(p);
            }

            if (!this.Poydat.Any(x => !string.IsNullOrEmpty(x.Numero)))
            {
                this.Poydat.Clear();
                this.Poydat.Add(new Poyta() 
                {
                    Numero = "1",
                    Kaytossa = true,
                    StriimiLinkki = string.Empty,
                    TulosLinkki = string.Empty
                });
            }
        }
    }
}

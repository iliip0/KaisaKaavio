﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public BindingList<Linkki> Linkit { get; set; }

        public BindingList<Poyta> Poydat { get; set; }

        public BindingList<Toimitsija> Toimitsijat { get; set; }

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

            this.Poydat.Add(new Poyta()
            {
                Numero = "1",
                Kaytossa = true,
                StriimiLinkki = string.Empty,
                TulosLinkki = string.Empty
            });
        }

        public void KopioiSalista(Sali sali)
        {
            this.Kaytossa = sali.Kaytossa;
            this.Lyhenne = sali.Lyhenne;
            this.Nimi = sali.Nimi;
            this.Osoite = sali.Osoite;
            this.PuhelinNumero = sali.PuhelinNumero;
            this.Seura = sali.Seura;

            this.Poydat.Clear();
            foreach (var p in sali.Poydat.Where(x => !string.IsNullOrEmpty(x.Numero)))
            {
                this.Poydat.Add(new Poyta() 
                {
                    Numero = p.Numero,
                    Kaytossa = p.Kaytossa,
                    StriimiLinkki = p.StriimiLinkki,
                    TulosLinkki = p.TulosLinkki
                });
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

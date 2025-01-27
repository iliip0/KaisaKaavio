using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio.Ranking
{
    public class RankingAsetukset
    {
        [XmlAttribute]
        public Laji Laji { get; set; }

        [XmlAttribute]
        [DefaultValue(true)]
        public bool EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta { get; set; }

        [XmlAttribute]
        [DefaultValue(true)]
        public bool KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista { get; set; }

        public BindingList<RankingPisteytysPelista> PistetytysPeleista { get; set; }
        public BindingList<RankingPisteytysSijoituksesta> PisteytysSijoituksista { get; set; }

        [XmlIgnore]
        public bool RankingKarjetRelevantteja
        {
            get
            {
                return PistetytysPeleista
                    .Where(x => x.Pisteet > 0)
                    .Any(x => 
                        x.Ehto == RankingPisteetPelista.RankingYkkosenVoitosta ||
                        x.Ehto == RankingPisteetPelista.RankingKakkosenVoitosta ||
                        x.Ehto == RankingPisteetPelista.RankingKolmosenVoitosta);
            }
        }

        [XmlIgnore]
        public bool Tyhja
        {
            get 
            {
                return 
                    !this.PistetytysPeleista.Any(x => x.Pisteet > 0) &&
                    !this.PistetytysPeleista.Any(x => x.Pisteet > 0);
            }
        }

        public RankingAsetukset()
        {
            this.PistetytysPeleista = new BindingList<RankingPisteytysPelista>();
            this.PisteytysSijoituksista = new BindingList<RankingPisteytysSijoituksesta>();
            this.Laji = KaisaKaavio.Laji.Kaisa;
            this.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta = true;
            this.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista = true;
        }

        public RankingAsetukset(Laji laji)
        {
            this.PistetytysPeleista = new BindingList<RankingPisteytysPelista>();
            this.PisteytysSijoituksista = new BindingList<RankingPisteytysSijoituksesta>();
            this.Laji = laji;
            this.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta = true;
            this.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista = true;
        }

        public void KopioiAsetuksista(RankingAsetukset asetukset)
        {
            this.Laji = asetukset.Laji;
            this.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista = asetukset.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista;
            this.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta = asetukset.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta;

            this.PistetytysPeleista.Clear();
            foreach (var p in asetukset.PistetytysPeleista)
            {
                this.PistetytysPeleista.Add(new RankingPisteytysPelista() 
                {
                    Ehto = p.Ehto,
                    Pisteet = p.Pisteet
                });
            }

            this.PisteytysSijoituksista.Clear();
            foreach (var p in asetukset.PisteytysSijoituksista)
            {
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta() 
                {
                    Ehto = p.Ehto,
                    Pisteet = p.Pisteet
                });
            }
        }

        public void AsetaOletusasetukset(Laji laji)
        {
            this.PisteytysSijoituksista.Clear();
            this.PistetytysPeleista.Clear();

            this.EnsimmaisenOsakilpailunRankingParhaatEdellisestaSarjasta = true;
            this.KorvaaPuuttuvatRankingParhaatParhaillaPaikallaOlijoista = true;

            if (laji == Laji.Kaisa)
            {
                // Oletusrankingasetukset (Puh.veli klubin rankinging pisteytys)
                this.PistetytysPeleista.Add(new RankingPisteytysPelista(RankingPisteetPelista.JokaisestaVoitosta, 1));
                this.PistetytysPeleista.Add(new RankingPisteytysPelista(RankingPisteetPelista.RankingYkkosenVoitosta, 2));
                this.PistetytysPeleista.Add(new RankingPisteytysPelista(RankingPisteetPelista.RankingKakkosenVoitosta, 1));
                this.PistetytysPeleista.Add(new RankingPisteytysPelista(RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari, 1));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.KaikilleOsallistujille, 1));
            }
            else
            { 
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Voittajalle, 15));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kakkoselle, 12));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kolmoselle, 10));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Neloselle, 8));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Vitoselle, 7));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kutoselle, 6));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Seiskalle, 5));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kasille, 4));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Ysille, 3));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.Kymmenennelle, 2));
                this.PisteytysSijoituksista.Add(new RankingPisteytysSijoituksesta(RankingPisteetSijoituksesta.KaikilleOsallistujille, 1));
            }
        }

        public int PisteitaVoitosta(int rankingSijoitus)
        {
            int pisteet = 0;

            foreach (var p in this.PistetytysPeleista)
            {
                if (p.Ehto == RankingPisteetPelista.JokaisestaVoitosta)
                {
                    pisteet += p.Pisteet;
                }

                if (p.Ehto == RankingPisteetPelista.RankingYkkosenVoitosta && rankingSijoitus == 1)
                {
                    pisteet += p.Pisteet;
                }

                if (p.Ehto == RankingPisteetPelista.RankingKakkosenVoitosta && rankingSijoitus == 2)
                {
                    pisteet += p.Pisteet;
                }

                if (p.Ehto == RankingPisteetPelista.RankingKolmosenVoitosta && rankingSijoitus == 3)
                {
                    pisteet += p.Pisteet;
                }
            }

            return pisteet;
        }
    }
}

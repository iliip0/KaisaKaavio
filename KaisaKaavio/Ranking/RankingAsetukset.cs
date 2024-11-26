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
        public BindingList<RankingPisteytysPelista> PistetytysPeleista { get; set; }
        public BindingList<RankingPisteytysSijoituksesta> PisteytysSijoituksista { get; set; }

        public RankingAsetukset()
        {
            this.PistetytysPeleista = new BindingList<RankingPisteytysPelista>();
            this.PisteytysSijoituksista = new BindingList<RankingPisteytysSijoituksesta>();
        }

        public void AsetaOletusasetukset()
        {
            this.PisteytysSijoituksista.Clear();
            this.PistetytysPeleista.Clear();

            // Oletusrankingasetukset (Puh.veli klubin rankinging pisteytys)
            this.PistetytysPeleista.Add(new RankingPisteytysPelista()
            {
                Ehto = RankingPisteetPelista.JokaisestaVoitosta,
                Pisteet = 1
            });

            this.PistetytysPeleista.Add(new RankingPisteytysPelista()
            {
                Ehto = RankingPisteetPelista.RankingYkkosenVoitosta,
                Pisteet = 2
            });

            this.PistetytysPeleista.Add(new RankingPisteytysPelista()
            {
                Ehto = RankingPisteetPelista.RankingKakkosenVoitosta,
                Pisteet = 1
            });

            this.PistetytysPeleista.Add(new RankingPisteytysPelista()
            {
                Ehto = RankingPisteetPelista.EkanKierroksenVoitostaKunTokaKierrosOnPudari,
                Pisteet = 1
            });
        }
    }
}

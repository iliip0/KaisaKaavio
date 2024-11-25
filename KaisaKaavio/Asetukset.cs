using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KaisaKaavio
{
    public class Asetukset
    {
        /// <summary>
        /// Tämä asetus määrää milloin automaattinen haku käynnistyy
        /// </summary>
        public static int PelejaEnintaanKeskenHaettaessa = 8;

        /// <summary>
        /// Tämä asetus määrää minimimäärän pelaajia joille kaavio voidaan arpoa
        /// </summary>
        public static int PelaajiaVahintaanKaaviossa = 4;

        /// <summary>
        /// Tämä asetus määrää kuinka usein automaattinen tallennus tapahtuu (sekunteja)
        /// </summary>
        public static int AutomaattisenTallennuksenTaajuus = 5 * 60;

        /// <summary>
        /// Viimeisimmän kilpailutiedoston nimi
        /// </summary>
        public string ViimeisinKilpailu { get; set; }

        /// <summary>
        /// Salin/pelipaikan tiedot. Nämä tallennetaan asetuksiin sillä eivät muutu kilpailujen välillä suuresti
        /// </summary>
        public Sali Sali { get; set; }

        /// <summary>
        /// Kisoihin osallistuneet pelaajat
        /// </summary>
        public BindingList<PelaajaTietue> Pelaajat { get; set; }
 
        private string tiedosto = null;

        public Asetukset()
        {
            this.ViimeisinKilpailu = string.Empty;
            this.Sali = new Sali();
            this.Pelaajat = new BindingList<PelaajaTietue>(); 

            this.tiedosto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaavioAsetukset.xml");
        }

        public void Tallenna()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Asetukset));

            using (TextWriter writer = new StreamWriter(this.tiedosto))
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }
        }

        public void Lataa()
        {
            if (File.Exists(tiedosto))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Asetukset));

                Asetukset asetukset = null;

                using (TextReader reader = new StreamReader(tiedosto))
                {
                    asetukset = (Asetukset)serializer.Deserialize(reader);
                    reader.Close();
                }

                if (asetukset != null)
                {
                    this.ViimeisinKilpailu = asetukset.ViimeisinKilpailu;
                    this.Sali.Nimi = asetukset.Sali.Nimi;
                    this.Sali.Osoite = asetukset.Sali.Osoite;
                    this.Sali.PuhelinNumero = asetukset.Sali.PuhelinNumero;

                    this.Sali.Linkit.Clear();
                    foreach (var linkki in asetukset.Sali.Linkit)
                    {
                        if (!string.IsNullOrEmpty(linkki.Osoite))
                        {
                            this.Sali.Linkit.Add(linkki);
                        }
                    }

                    this.Sali.Poydat.Clear();
                    foreach (var poyta in asetukset.Sali.Poydat)
                    {
                        if (!string.IsNullOrEmpty(poyta.Numero))
                        {
                            this.Sali.Poydat.Add(poyta);
                        }
                    }

                    this.Pelaajat.Clear();
                    foreach (var pelaaja in asetukset.Pelaajat)
                    {
                        if (!string.IsNullOrEmpty(pelaaja.Nimi))
                        {
                            this.Pelaajat.Add(pelaaja);
                        }
                    }
                }
            }
        }

        public void TallennaPelaajat(Kilpailu kilpailu)
        {
            foreach (var osallistuja in kilpailu.Osallistujat.Where(x => !string.IsNullOrEmpty(x.Nimi)))
            {
                // Tallenna vain pelaajat joilla on vähintään kaksi nimeä (etu & suku)
                if (osallistuja.Nimi.Split(' ').Count() > 1)
                {
                    PelaajaTietue vanhaPelaaja = this.Pelaajat.FirstOrDefault(x => string.Equals(x.Nimi, osallistuja.Nimi, StringComparison.OrdinalIgnoreCase));
                    if (vanhaPelaaja != null)
                    {
                        vanhaPelaaja.Seura = osallistuja.Seura;
                    }
                    else
                    {
                        Pelaajat.Add(new PelaajaTietue()
                        {
                            Nimi = osallistuja.Nimi,
                            Seura = osallistuja.Seura
                        });
                    }
                }
            }

            var pelaajatJarjestyksessa = this.Pelaajat.OrderBy(x => x.Nimi).ToArray();
            this.Pelaajat.Clear();
            foreach (var p in pelaajatJarjestyksessa)
            {
                this.Pelaajat.Add(p);
            }
        }
    }
}

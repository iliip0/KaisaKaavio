using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Integraatio
{
    public class BiljardiOrg
    {
        public static string LataaIlmoittautuneetSivu(string kilpailunId, Laji laji, KilpaSarja sarja, Loki loki)
        {
            string url = string.Empty;
            string lajinimi = Enum.GetName(typeof(Laji), laji).ToLower();

            switch (sarja)
            { 
                case KilpaSarja.Parikilpailu:
                case KilpaSarja.MixedDoubles:
                    url = string.Format("https://www.biljardi.org/{0}/parit/{1}/ilmoittautuneet.php", lajinimi, kilpailunId);
                    break;

                case KilpaSarja.Joukkuekilpailu:
                    url = string.Format("https://www.biljardi.org/{0}/joukkueet/{1}/ilmoittautuneet.php", lajinimi, kilpailunId);
                    break;

                default:
                    url = string.Format("https://www.biljardi.org/{0}/{1}/ilmoittautuneet.php", lajinimi, kilpailunId);
                    break;
            }

            try
            {
                return Integraatio.HtmlLukija.Lue(url, loki, true);

            }
            catch (Exception ex)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Ilmoittautuneiden haku biljardi.org sivulta {0} epäonnistui", url), ex, true);
                }
            }

            return string.Empty;
        }

        private static HtmlElement s_EdellinenH3Elementti = null;

        private static void ParsiKisalinkki(List<BiljardiOrgKilpailu> kisat, string nimi, string osoite, Loki loki, Laji laji, KilpaSarja sarja)
        {
            if (laji == Laji.Kara)
            {
                return; // Kara pelataan lohkoissa. Ohjelma ei tue tätä
            }

            string lajinimi = Enum.GetName(typeof(Laji), laji).ToLower();

            string uri = lajinimi;
            List<string> kielletyt = new List<string>();

            switch (sarja)
            {
                case KilpaSarja.Joukkuekilpailu:
                    uri = string.Format("/{0}/joukkueet/", lajinimi);
                    break;

                case KilpaSarja.MixedDoubles:
                case KilpaSarja.Parikilpailu:
                    uri = string.Format("/{0}/parit/", lajinimi);
                    break;

                default:
                    uri = string.Format("/{0}/", lajinimi);
                    kielletyt.Add("parit");
                    kielletyt.Add("joukkueet");
                    break;
            }

            if (osoite.Contains(uri) &&
                osoite.Contains("/ilmoittautuneet.php"))
            {
                try
                {
                    foreach (var k in kielletyt)
                    {
                        if (osoite.Contains(k))
                        {
                            return;
                        }
                    }

                    int i = osoite.IndexOf(uri);
                    var id = osoite.Substring(i + uri.Length);

                    int j = id.IndexOf("/");
                    id = id.Substring(0, j);

                    if (!string.IsNullOrEmpty(nimi))
                    {
                        nimi = nimi.Trim();

                        string lajiIso = Enum.GetName(typeof(Laji), laji);
                        if (nimi.StartsWith(lajiIso))
                        {
                            nimi = nimi.Substring(lajiIso.Length).Trim();
                        }

                        kisat.Add(new BiljardiOrgKilpailu()
                        {
                            Nimi = nimi,
                            Id = id
                        });
                    }
                }
                catch
                {
                }
            }
        }

        private static void LataaTulevatKisat(List<BiljardiOrgKilpailu> kisat, HtmlElement e, Loki loki, Laji laji, KilpaSarja sarja)
        {
            if (string.Equals(e.TagName, "h3", StringComparison.OrdinalIgnoreCase))
            {
                s_EdellinenH3Elementti = e;
            }
            else if (string.Equals(e.TagName, "a", StringComparison.OrdinalIgnoreCase))
            {
                if (e.InnerText.Contains("Ilmoittautumislista"))
                {
                    string nimi = string.Empty;
                    if (s_EdellinenH3Elementti != null)
                    {
                        nimi = s_EdellinenH3Elementti.InnerText;
                    }

                    string osoite = e.GetAttribute("href");

                    ParsiKisalinkki(kisat, nimi, osoite, loki, laji, sarja);
                }
            }
            else
            {
                foreach (var child in e.Children)
                {
                    HtmlElement c = (HtmlElement)child;
                    if (c != null)
                    {
                        LataaTulevatKisat(kisat, c, loki, laji, sarja);
                    }
                }
            }
        }
        
        public static List<BiljardiOrgKilpailu> LataaTulevatKisat(Loki loki, Laji laji, KilpaSarja sarja)
        {
            s_EdellinenH3Elementti = null;

            List<BiljardiOrgKilpailu> kisat = new List<BiljardiOrgKilpailu>();

            string url = string.Format("https://www.biljardi.org");

            try
            {
                string html = Integraatio.HtmlLukija.Lue(url, loki, false);
                var sivu = Integraatio.HtmlLukija.Dokumentti(html);
                LataaTulevatKisat(kisat, sivu.Body, loki, laji, sarja);
            }
            catch (Exception ex)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Ilmoittautuneiden haku biljardi.org sivulta {0} epäonnistui", url), ex, true);
                }
            }

            return kisat;
        }

        /// <summary>
        /// Hakee kilpailuun ilmoittautuneet pelaajat RG järjestyksessä biljardi.org sivulta
        /// </summary>
        public static List<Pelaaja> ParsiIlmoittautuneetSivulta(string html, Loki loki)
        {
            List<Pelaaja> pelaajat = new List<Pelaaja>();

            try
            {
                var sivu = Integraatio.HtmlLukija.Dokumentti(html);

                foreach (var c in sivu.Body.Children)
                {
                    HtmlElement e = (HtmlElement)c;
                    if (e != null)
                    {
                        HaeIlmoittautuneet(e, pelaajat, loki);
                    }
                }
            }
            catch (Exception ex)
            {
                pelaajat.Clear();

                if (loki != null)
                {
                    loki.Kirjoita("Ilmoittautuneiden haku biljardi.org sivulta epäonnistui", ex, true);
                }
            }

            return pelaajat;
        }

        private static void HaeIlmoittautuneet(HtmlElement e, List<Pelaaja> pelaajat, Loki loki)
        {
            int riveja = 0;
            string joukkue = string.Empty;

            if (string.Equals(e.TagName, "table", StringComparison.OrdinalIgnoreCase))
            {
                var rivit = e.GetElementsByTagName("TR");
                if (rivit != null && rivit.Count > 1)
                {
                    if (rivit[0].OuterText.Contains("SijaNimiSeura"))
                    {
                        bool otsikkorivi = true;
                        foreach (var rivi in rivit)
                        {
                            if (!otsikkorivi)
                            {
                                if (riveja == 0)
                                {
                                    riveja = ParsiEnsimmainenPelaajaRivi((HtmlElement)rivi, pelaajat, loki, out joukkue);
                                }
                                else
                                {
                                    ParsiJoukkuePelaajaRivi((HtmlElement)rivi, pelaajat, loki, joukkue);
                                    riveja--;
                                }
                            }
                            otsikkorivi = false;
                        }
                    }
                }
            }
            else
            {
                foreach (var c in e.Children)
                {
                    HtmlElement ce = (HtmlElement)c;
                    if (ce != null)
                    { 
                        HaeIlmoittautuneet(ce, pelaajat, loki);
                    }
                }
            }
        }
        
        private static int ParsiEnsimmainenPelaajaRivi(HtmlElement e, List<Pelaaja> pelaajat, Loki loki, out string joukkue)
        {
            joukkue = string.Empty;

            var luokka = e.GetAttribute("class");
            if (luokka != null && string.Equals(luokka, "sulkurivi"))
            {
                return 0;
            }

            var solut = e.GetElementsByTagName("TD");
            if (solut != null && solut.Count > 2)
            {
                var rowspan = solut[0].GetAttribute("rowspan");
                if (!string.IsNullOrEmpty(rowspan))
                {
                    int riveja = 0;
                    Int32.TryParse(rowspan, out riveja);
                    joukkue = solut[0].InnerText;
                    string nimi = solut[1].InnerText;
                    string seura = solut[2].InnerText;

                    LisaaParsittuPelaaja(pelaajat, joukkue, nimi, seura);

                    return Math.Max(0, riveja -1);
                }
                else
                {
                    joukkue = solut[0].InnerText.Replace(".", string.Empty).Trim();
                    string nimi = solut[1].InnerText;
                    string seura = solut[2].InnerText;

                    LisaaParsittuPelaaja(pelaajat, joukkue, nimi, seura);
                }
            }

            return 0;
        }

        private static void ParsiJoukkuePelaajaRivi(HtmlElement e, List<Pelaaja> pelaajat, Loki loki, string joukkue)
        {
            var luokka = e.GetAttribute("class");
            if (luokka != null && string.Equals(luokka, "sulkurivi"))
            {
                return;
            }

            var solut = e.GetElementsByTagName("TD");
            if (solut != null && solut.Count > 2)
            {
                string nimi = solut[0].InnerText;
                string seura = solut[1].InnerText;

                LisaaParsittuPelaaja(pelaajat, joukkue, nimi, seura);
            }
        }

        private static void LisaaParsittuPelaaja(List<Pelaaja> pelaajat, string sija, string nimi, string seura)
        {
            if (sija.Equals("-") ||
                string.IsNullOrEmpty(nimi) ||
                nimi.Length < 5)
            {
            }
            else if (!pelaajat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi)))
            {
                pelaajat.Add(new Pelaaja()
                {
                    IlmoittautumisNumero = sija,
                    Nimi = nimi,
                    Seura = seura,
                });
            }
        }
    }
}

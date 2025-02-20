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
        public static string LataaIlmoittautuneetSivu(string kilpailunId, Loki loki)
        {
            string url = string.Format("https://www.biljardi.org/kaisa/{0}/ilmoittautuneet.php", kilpailunId);

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

        public static List<BiljardiOrgKilpailu> LataaTulevatKisat(Loki loki)
        {
            List<BiljardiOrgKilpailu> kisat = new List<BiljardiOrgKilpailu>();

            string url = string.Format("https://www.biljardi.org");

            try
            {
                string html = Integraatio.HtmlLukija.Lue(url, loki, false);
                var rivit = html.Split('\n');
                if (rivit != null && rivit.Any())
                {
                    string edellinenRivi = string.Empty;
                    foreach (var rivi in rivit)
                    {
                        if (rivi.Contains("/kaisa/") &&
                            rivi.Contains("/ilmoittautuneet.php") &&
                            !rivi.Contains("/parit/") &&
                            !rivi.Contains("/joukkueet/"))
                        {
                            try
                            {
                                int i = rivi.IndexOf("/kaisa/");
                                var id = rivi.Substring(i + 7);

                                int j = id.IndexOf("/");
                                id = id.Substring(0, j);

                                if (!string.IsNullOrEmpty(edellinenRivi))
                                {
                                    int k = edellinenRivi.IndexOf("</span>");
                                    var nimi = edellinenRivi.Substring(k + 7).Trim();

                                    int l = nimi.IndexOf("</h3>");
                                    nimi = nimi.Substring(0, l);

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

                        edellinenRivi = rivi;
                    }
                }

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
                                ParsiPelaajaRivi((HtmlElement)rivi, pelaajat, loki);  
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

        private static void ParsiPelaajaRivi(HtmlElement e, List<Pelaaja> pelaajat, Loki loki)
        {
            var luokka = e.GetAttribute("class");
            if (luokka != null && string.Equals(luokka, "sulkurivi"))
            {
                return;
            }

            var solut = e.GetElementsByTagName("TD");
            if (solut != null && solut.Count > 2)
            {
                string sija = solut[0].InnerText.Replace(".", string.Empty).Trim();
                string nimi = solut[1].InnerText;
                string seura = solut[2].InnerText;

                if (sija.Equals("-") ||
                    string.IsNullOrEmpty(nimi) ||
                    nimi.Length < 5)
                {
                    return;
                }

                if (!pelaajat.Any(x => Tyypit.Nimi.Equals(x.Nimi, nimi)))
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
}

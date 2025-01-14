using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Nimen parsimiseen ja muotoiluun liittyvät apumetodit
    /// </summary>
    public class Nimi
    {
        /// <summary>
        /// Poistaa pelaajan nimestä mahdolliset ylimääräset merkinnät kuten tasurit
        /// 
        /// Esim Matti Meikäläinen (3)
        /// </summary>
        public static string PoistaTasuritJaSijoituksetNimesta(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return string.Empty;
            }

            var osat = nimi.Split(' ');

            List<string> nimiOsat = new List<string>();

            foreach (var osa in osat
                .Where(x => x.Any(y => Char.IsLetter(y)))
                .Select(x => x.Trim()))
            {
                if (osa.Length > 0 && Char.IsLetter(osa[0]))
                {
                    StringBuilder o = new StringBuilder();

                    bool edellinenKirjainOliAakkonen = false;

                    foreach (var c in osa)
                    {
                        if (Char.IsLetter(c))
                        {
                            o.Append(c);
                            edellinenKirjainOliAakkonen = true;
                        }
                        else
                        {
                            if (c == '-' && edellinenKirjainOliAakkonen)
                            {
                                o.Append(c);
                            }

                            edellinenKirjainOliAakkonen = false;
                        }
                    }

                    if (o.ToString().Length > 0)
                    {
                        nimiOsat.Add(o.ToString());
                    }
                }
            }

            if (nimiOsat.Count > 0)
            {
                return string.Join(" ", nimiOsat.ToArray());
            }
            else 
            {
                return string.Empty;
            }
        }

        public static bool Equals(string nimi1, string nimi2)
        {
            string lyhytNimi1 = PoistaTasuritJaSijoituksetNimesta(nimi1);
            string lyhytNimi2 = PoistaTasuritJaSijoituksetNimesta(nimi2);

            var nimet1 = lyhytNimi1.Split(' ');
            var nimet2 = lyhytNimi2.Split(' ');

            if (nimet1.Count() == 2 && nimet2.Count() == 2)
            {
                return
                    (string.Equals(nimet1[0], nimet2[0], StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(nimet1[1], nimet2[1], StringComparison.OrdinalIgnoreCase)) ||
                    (string.Equals(nimet1[0], nimet2[1], StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(nimet1[1], nimet2[0], StringComparison.OrdinalIgnoreCase));
            }
            else 
            {
                return string.Equals(lyhytNimi1, lyhytNimi2, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string MuotoileNimi(string muotoilematonNimi)
        {
            if (string.IsNullOrEmpty(muotoilematonNimi))
            {
                return string.Empty;
            }

            var nimi = Tyypit.Nimi.PoistaTasuritJaSijoituksetNimesta(muotoilematonNimi);

            var nimet = nimi.Split(' ');
            if (nimet == null || nimet.Count() == 1)
            {
                return KapiteeliksiEkaKirjain(nimi);
            }

            List<string> kapiteeliNimet = new List<string>();
            foreach (var n in nimet)
            {
                kapiteeliNimet.Add(KapiteeliksiEkaKirjain(n.Trim()));
            }

            return string.Join(" ", kapiteeliNimet);
        }

        private static string KapiteeliksiEkaKirjain(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return nimi;
            }

            if (Char.IsUpper(nimi[0]))
            {
                return nimi;
            }

            StringBuilder s = new StringBuilder();
            s.Append(Char.ToUpper(nimi[0]));

            if (nimi.Length > 1)
            {
                s.Append(nimi.Substring(1));
            }

            return s.ToString();
        }
    }
}

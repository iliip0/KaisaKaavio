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

        public static string LyhytNimi(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return string.Empty;
            }

            if (nimi.Contains(' '))
            {
                var alku = nimi.Split(' ').FirstOrDefault();
                if (alku != null && !alku.Contains('.') && alku.Length > 2)
                {
                    return alku;
                }
            }

            return nimi;
        }

        /// <summary>
        /// Muuttaa "Sukunimi Etunimi" muotoon "Sukunimi E."
        /// </summary>
        public static string NimiParikisassa(string nimi)
        {
            if (string.IsNullOrEmpty(nimi))
            {
                return nimi;
            }

            var parts = nimi.Split(' ');
            if (parts.Count() <= 1)
            {
                return nimi;
            }

            if (string.IsNullOrEmpty(parts[1]))
            {
                return parts[0].Trim();
            }

            return parts[0].Trim() + " " + parts[1][0] + ".";
        }

        public static string KeksiNimi(Random random)
        {
            string[] etunimet = 
            { 
                "Antti", "Anna", "Aino", "Arto", "Arhi", "Aku", "Alli", "Asku", "Asko", "Bert", "Dave", "Daalia", 
                "Eero", "Eki", "Ensiö", "Emilia", "Elli", "Essi", "Esko", "Eerik", "Ella", "Eetu", "Fred", "Gabriel", "Heikki", 
                "Hiski", "Hanna", "Hilkka", "Hilma", "Hannele", "Helvi", "Hannu", "Ilpo", "Ile", "Ilona", "Iivo", "Iiro", "Iiris", "Jaakko", "Jokke",
                "Jaana", "Johanna", "Juha", "Juho", "Jens", "Klaus", "Kalle", "Kim", "Kiira", "Kaisa", "Kaija", "Keke", "Laija", "Linda",
                "Laila",
                "Lauri", "Late", "Mauno", "Meri", "Maarit", "Mari", "Marjut", "Mauri", "Niilo", "Niko", "Nea", "Nelli", "Olavi", "Outi", "Oula",
                "Pete", "Paula", "Pauli", "Pieta", "Ripa", "Raija", "Risto", "Reija", "Raakel", "Sven", "Sakari", "Sulevi", "Seija", "Sanna",
                "Taru", "Tuija", "Tero", "Timo", "Tauri", "Ulla", "Urho", "Ville", "Vili", "Veera", "Ylermi"
            };

            string[] sukunimet = 
            {
                "Alinen", "Autti", "Alajärvi", "Autio", "Brunhilde", "Brecht", "Croft", "Erämies", "Eskelinen", "Fränti", "Giers", 
                "Hannula", "Hietamies", "Hämäläinen", "Hilla", "Hanski", "Hietanen", "Hietala", "Hakala", "Hautala", "Hummais", 
                "Ilonen", "Ilola", "Jukola", "Jäntti", "Jukola", "Jukarainen", "Jämerä", "Jantunen", "Jouppi", 
                "Janatuinen", "Joukainen", "Kivi", "Kokko", "Kekäle", "Korpi", "Korpela", "Korpinen", "Källström", 
                "Karalahti", "Kanerva", "Kuutamo", "Kolehmainen", "Karakorpi", "Kallio", "Kilpi", "Klemetti",
                "Kokkonen", "Kerminen", "Kurvinen", "Kielo", "Lemi", "Lemmetyinen", "Lahtela", "Lemmetyinen", "Liehu", "Lahti", "Lahtinen", "Lehterä",
                "Liimatainen", "Manninen", "Mannila", "Mononen", "Montonen", "Niemi", "Nieminen", "Niemelä", "Naumann", 
                "Nuotio", "Orpo", "Olli", "Ovaskainen", "Pollari", "Pajari", "Piippo", "Pusa", "Poikolainen", "Piri", "Puujärvi",
                "Ryti", "Rantunen", "Räihä", "Räsänen", "Sukari", "Sahamies", "Säynävä", "Sieviö", "Sauri", "Tuomi",
                "Tapani", "Tiitinen", "Uski", "Uotila", "Uotinen", "Vähäsarja", "Viertiö", "Vanhanen", "Vahanen",
                "Virta", "Virtanen", "Väyrynen"
            };


            int e = random.Next(0, etunimet.Count() - 1);
            int s = random.Next(0, sukunimet.Count() - 1);
            return sukunimet[s] + " " + etunimet[e];
        }
    }
}

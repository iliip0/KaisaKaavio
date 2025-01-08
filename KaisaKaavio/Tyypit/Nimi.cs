﻿using System;
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

                    foreach (var c in osa)
                    {
                        if (Char.IsLetter(c))
                        {
                            o.Append(c);
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
    }
}

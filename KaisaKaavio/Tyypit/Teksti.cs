﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Työkalu Sbil tekstien luomiseksi helposti
    /// </summary>
    public class Teksti
    {
        private StringBuilder rtf = new StringBuilder();
        private StringBuilder sbil = new StringBuilder();
        private StringBuilder plain = new StringBuilder();

        public string Rtf { get { return this.rtf.ToString() + "}"; } }
        public string Sbil { get { return this.sbil.ToString(); } }
        public string Plain { get { return this.plain.ToString(); } }

        public Teksti()
        {
            this.rtf.AppendLine(@"{\rtf1\ansi\fs20");
            this.rtf.AppendLine(@"{\colortbl;\red0\green0\blue0;\red255\green0\blue0;\red0\green0\blue255;\red16\green82\blue137;}");
        }

        public Teksti RivinVaihto()
        {
            this.rtf.Append(@" \line ");
            this.sbil.AppendLine();
            this.plain.AppendLine();

            return this;
        }

        public Teksti OsionVaihto()
        {
            this.rtf.Append(@" ------------------------------------------------------------ \line ");
            this.sbil.AppendLine(" ------------------------------------------------------------------------------------- ");
            this.plain.AppendLine(" ------------------------------------------------------------------------------------- ");

            return this;
        }

        public Teksti Linkki(string otsikko, string sisalto, string osoite)
        {
            if (!string.IsNullOrEmpty(otsikko))
            {
                PaksuTeksti(otsikko);
                NormaaliTeksti(" : ");
            }

            if (!string.IsNullOrEmpty(sisalto))
            {
                this.rtf.Append(@"\cf3");
                if (sisalto.Length > 32)
                {
                    this.rtf.Append(sisalto.Substring(0, 15) + "..." + sisalto.Substring(sisalto.Length - 15));
                }
                else
                {
                    this.rtf.Append(sisalto);
                }
                this.rtf.Append(@"\cf1");

                if (!string.IsNullOrEmpty(otsikko))
                {
                    this.sbil.Append(string.Format("[url={0}]{1}[/url]", osoite, sisalto));
                }
                else
                {
                    this.sbil.Append("[url]" + sisalto + "[/url]");
                }

                this.plain.Append(sisalto);
            }

            return this;
        }

        public Teksti Otsikko(string otsikko)
        {
            PaksuTeksti(otsikko);
            RivinVaihto();

            return this;
        }

        public Teksti PaksuTeksti(string teksti)
        {
            this.rtf.Append(@"\b " + teksti + @" \b0");
            this.sbil.Append("[b]" + teksti + "[/b]");
            this.plain.Append(teksti);

            return this;
        }

        public Teksti PunainenTeksti(string teksti)
        {
            if (!string.IsNullOrEmpty(teksti))
            {
                this.rtf.Append(@"\b \cf2 " + teksti + @" \cf1 \b0");
                this.sbil.Append("[b][color=#FF0000]" + teksti + "[/color][/b]");
                this.plain.Append(teksti);
            }

            return this;
        }

        public Teksti NormaaliTeksti(string teksti)
        {
            if (!string.IsNullOrEmpty(teksti))
            {
                this.rtf.Append(teksti);
                this.sbil.Append(teksti);
                this.plain.Append(teksti);
            }

            return this;
        }

        public Teksti NormaaliRivi(string teksti)
        {
            NormaaliTeksti(teksti);
            RivinVaihto();

            return this;
        }

        public Teksti InfoRivi(string otsikko, string sisalto)
        {
            if (!string.IsNullOrEmpty(sisalto))
            {
                PaksuTeksti(otsikko);
                NormaaliTeksti(" : ");
                NormaaliTeksti(sisalto);
                RivinVaihto();
            }

            return this;
        }

        public Teksti PieniTeksti(string teksti)
        {
            if (!string.IsNullOrEmpty(teksti))
            {
                this.rtf.Append(@"\fs18\cf4");
                this.rtf.Append(teksti);
                this.rtf.Append(@"\fs20\cf1");

                this.sbil.Append(string.Format("[size=85][color=#115099]{0}[/color][/size]", teksti));
                this.plain.Append(teksti);
            }

            return this;
        }

        public Teksti PelinTilanneTeksti(string poyta, string alkoi)
        {
            this.rtf.Append(@"\cf3");
            this.sbil.Append("[color=#0000FF]");

            if (string.IsNullOrEmpty(poyta))
            {
                NormaaliTeksti(" - Käynnissä");
            }
            else
            {
                NormaaliTeksti(string.Format(" - Pöytä {0}", poyta));
            }

            if (!string.IsNullOrEmpty(alkoi))
            {
                NormaaliTeksti(string.Format(" ({0})", alkoi));
            }

            this.rtf.Append(@". \cf1");
            this.sbil.Append(".[/color]");

            return this;
        }

        public Teksti LoppuMainos()
        {
            this.sbil.AppendLine(
                string.Format("[size=85]Kilpailu vedetty ilmaisella, avoimen lähdekoodin [url=https://github.com/iliip0/KaisaKaavio] KaisaKaavio [/url] -ohjelmalla[color=#999999] (versio {0})[/color][/size]",
                    Assembly.GetEntryAssembly().GetName().Version));

            return this;
        }
    }
}

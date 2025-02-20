﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio.Integraatio
{
    /// <summary>
    /// Työkalu html sivujen hakemiseksi ohjelman käyttöön
    /// </summary>
    public class HtmlLukija
    {
        public static string Lue(string osoite, Loki loki, bool popupJosEiOnnistu)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; 
                
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(osoite);

                    if (!string.IsNullOrEmpty(html))
                    {
                        if (loki != null)
                        {
                            loki.Kirjoita(string.Format("Luettiin html sivu {0} onnistuneesti", osoite));
                        }
                    }

                    return html;
                }
            }
            catch (Exception e)
            {
                if (loki != null)
                {
                    loki.Kirjoita(string.Format("Html sivun {0} lukeminen epäonnistui. Virhe: {1}", osoite, e.Message), e, popupJosEiOnnistu);
                }
            }

            return string.Empty;
        }

        public static HtmlDocument Dokumentti(string html)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentText = html;
            browser.Document.OpenNew(true);
            browser.Document.Write(html);
            //browser.Refresh();
            return browser.Document;
        }
    }
}

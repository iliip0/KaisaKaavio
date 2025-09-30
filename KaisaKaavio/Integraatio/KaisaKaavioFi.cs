using KaisaKaavio.Tyypit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;

namespace KaisaKaavio.Integraatio
{
    public class KaisaKaavioFi
    {
        private class KilpailunTiedot
        {
            public Kilpailu Kilpailu = null;
            public string Id = string.Empty;
            public string TiedostonNimi = string.Empty;
            public string Tiedosto = string.Empty;
            public Loki Loki = null;
        }

        /// <summary>
        /// Tallentaa kilpailun kaisakaavio.fi palveluun. Tämä funktio palaa välittömästi.
        /// Lataaminen tapahtuu taustasäikeessä ja epäonnistuessaa epäonnistuu hiljaisesti
        /// </summary>
        public static void TallennaKilpailuServerille(Kilpailu kilpailu, string id, string tiedosto, Loki loki)
        {
            try
            {
                if (File.Exists(tiedosto))
                {
                    string tiedostonNimi = Path.GetFileName(tiedosto);
                    string polku = Path.Combine(Path.GetTempPath(), "KaisaKaavioLataukset");
                    Directory.CreateDirectory(polku);

                    string tempTiedostonNimi = Path.Combine(polku, "Temp_" + tiedostonNimi);

                    File.Copy(tiedosto, tempTiedostonNimi, true);

                    KilpailunTiedot tiedot = new KilpailunTiedot() 
                    { 
                        Kilpailu = kilpailu,
                        Id = id, 
                        Tiedosto = tempTiedostonNimi, 
                        TiedostonNimi = tiedostonNimi,
                        Loki = loki 
                    };
                    
                    ThreadPool.QueueUserWorkItem(TallennaKilpailuServerilleAsync, tiedot);
                }
            }
            catch (Exception e) 
            {
                if (loki != null)
                {
#if DEBUG
                    loki.Kirjoita("Kilpailun tallennus serverille epäonnistui", e, true);
#else
                    loki.Kirjoita("Kilpailun tallennus serverille epäonnistui", e, false);
#endif
                }
            }
        }

        public static void TallennaKilpailuServerilleAsync(object param)
        {
            KilpailunTiedot tiedot = (KilpailunTiedot)param;

            try
            {
                byte[] teksti = File.ReadAllBytes(tiedot.Tiedosto);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback
                                = ((sender, cert, chain, errors) => cert.Subject.Contains("KaisaKaavio"));

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/xml");

                    string address = string.Format("{0}/api/TallennaKilpailu?Id={1}&TiedostonNimi={2}", 
                        Asetukset.KaisaKaavioServeri, 
                        HttpUtility.UrlEncode(tiedot.Id),
                        HttpUtility.UrlEncode(tiedot.TiedostonNimi));

                    client.UploadData(address, teksti);

                    tiedot.Kilpailu.SivustonPaivitysTarvitaan = false;
                }
            }
            catch (Exception e)
            {
                if (tiedot != null && tiedot.Loki != null)
                {
                    tiedot.Loki.Kirjoita("Kilpailun tallennus serverille epäonnistui taustalla", e, false);
                }
            }
            finally
            {
                try
                {
                    File.Delete(tiedot.Tiedosto);
                }
                catch
                { 
                }
            }
        }

        public delegate void DataHaettu(CsvTietue tietue);

        public class DatanHaku
        {
            public string Id;
            public System.Windows.Forms.Control Invoker;
            public DataHaettu Callback;
            public Loki Loki;
            public string Query;
        }

        public static void HaeDataa(string id, string query, System.Windows.Forms.Control invoker, Loki loki, DataHaettu callback)
        {
            DatanHaku Haku = new DatanHaku()
            {
                Id = id,
                Invoker = invoker,
                Callback = callback,
                Loki = loki,
                Query = query
            };

            ThreadPool.QueueUserWorkItem(HaeDataaAsync, Haku);
        }

        public static void HaeDataaAsync(object param)
        {
            DatanHaku tiedot = (DatanHaku)param;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback
                                = ((sender, cert, chain, errors) => cert.Subject.Contains("KaisaKaavio"));

                using (HttpClient client = new HttpClient())
                {
                    string address = string.Format("{0}/api/QueryCsv/{1}?{2}",
                        Asetukset.KaisaKaavioServeri,
                        HttpUtility.UrlEncode(tiedot.Id),
                        tiedot.Query);

                    var teksti = client.GetStringAsync(address).GetAwaiter().GetResult();
                    //var response = client.GetAsync(address).GetAwaiter().GetResult();

                    //var teksti = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    CsvTietue tietue = new CsvTietue();
                    tietue.Lue(teksti);

                    if (tiedot.Invoker != null && tiedot.Invoker.InvokeRequired)
                    {
                        tiedot.Invoker.Invoke(new Action<CsvTietue>(tiedot.Callback), tietue);
                    }
                    else
                    {
                        tiedot.Callback(tietue);
                    }
                }
            }
            catch (Exception e)
            {
                if (tiedot != null && tiedot.Loki != null)
                {
                    tiedot.Loki.Kirjoita("Datan haku serveriltä epäonnistui taustalla", e, false);
                }
            }
            finally
            {
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavioUpdater
{
    /// <summary>
    /// Komentorivityökalu joka lataa uusimman version KaisaKaavio ohjelmasta palvelimelta, ja päivittää nykyisen version uudempaan
    /// jos on tarvis
    /// </summary>
    static class Program
    {
        static void Loki(string teksti, params object[] parametrit)
        {
            try
            {
#if DEBUG
                Console.Out.WriteLine(string.Format(teksti, parametrit));
#else
                string kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KaisaKaaviot", "Loki");
                Directory.CreateDirectory(kansio);

                string tiedosto = Path.Combine(kansio, string.Format("Paivitykset_{0:D2}_{1}.txt", DateTime.Now.Month, DateTime.Now.Year));
                File.AppendAllText(tiedosto, DateTime.Now.ToString() + " : " + string.Format(teksti, parametrit) + Environment.NewLine);
#endif
            }
            catch
            { 
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            try
            {
#if DEBUG
                Console.Out.WriteLine("KaisaKaavioUpdater");
                Console.Out.WriteLine("===================================================================================");
#endif

                string kaisaKaavioExe = args[0];    // Arg[0] = KaisaKaavio exe tiedoston nimi
                int pid = Int32.Parse(args[1]);     // Arg[1] = KaisaKaavio prosessin id, joka kutsui päivitystä

                Loki("Yritetään päivittää ohjelma {0} (pid {1})...", kaisaKaavioExe, pid);

                // Odotetaan että KaisaKaavio on sulkeutunut ennen päivitystä...
                if (!OdotaOhjelmanSulkeutumista(pid))
                {
                    Loki("KaisaKaavio.exe on käynnissä. Päivitys epäonnistui");
                    return -3;
                }

                string downloadFolder = Path.Combine(Path.GetTempPath(), "KaisaKaavioDownloads");
                Directory.CreateDirectory(downloadFolder);

                string downloadPath = Path.Combine(downloadFolder, "KaisaKaavio.exe");

                string url = Properties.Settings.Default.KaisaKaavioExeDownloadUrl;

                LataaUusinKaisaKaavioExe(downloadPath, url);

                // Jostain syystä exe on deletoitu. Kopioidaan uusin tilalle
                if (!File.Exists(kaisaKaavioExe))
                {
                    Loki("KaisaKaavio.exe puuttuu. Kopioidaan uusin versio tilalle.");

                    File.Copy(downloadPath, kaisaKaavioExe);

                    Loki("Valmis!");
                    
                    return 2;
                }

                // Päivitetään exe jos uudempi versio on tarjolla
                else if (File.Exists(kaisaKaavioExe) && File.Exists(downloadPath))
                {
                    var nykyVersio = FileVersionInfo.GetVersionInfo(kaisaKaavioExe);
                    var uusinVersio = FileVersionInfo.GetVersionInfo(downloadPath);

                    Version nyky = new Version(nykyVersio.FileMajorPart, nykyVersio.FileMinorPart, nykyVersio.FileBuildPart, nykyVersio.FilePrivatePart);
                    Version uusin = new Version(uusinVersio.FileMajorPart, uusinVersio.FileMinorPart, uusinVersio.FileBuildPart, uusinVersio.FilePrivatePart);

                    if (nyky < uusin)
                    {
                        Loki("Päivitetään KaisaKaavio.exe versiosta {0} versioon {1}.", nyky, uusin);

                        File.Delete(kaisaKaavioExe);
                        File.Copy(downloadPath, kaisaKaavioExe);

                        Loki("Valmis!");

                        return 1;
                    }
                    else
                    {
                        Loki("KaisaKaavio.exe on uusimmassa versiossa {0}. Ei tarvetta päivittää.", nyky);

                        return 0;
                    }
                }
                else
                {
                    Loki("KaisaKaavion päivitysten nouto palvelimelta epäonnistui.");

                    return -4;
                }
            }
            catch (Exception e)
            {
                Loki("Virhe päivitettäessä KaisaKaavio ohjelmaa: {0}{1}{2}", e.Message, Environment.NewLine, e.StackTrace);
                return -1;
            }
            finally 
            {
#if DEBUG
                Console.Out.WriteLine("Press enter to quit...");
                Console.In.ReadLine();
#endif
            }
        }

        private static bool OdotaOhjelmanSulkeutumista(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                if (process != null)
                {
                    DateTime start = DateTime.Now;

                    while (!process.HasExited)
                    {
                        Thread.Sleep(50);

                        if ((DateTime.Now - start).Seconds > 10)
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
            }

            // Tarkistetaan että muitakaan KaisaKaavio instansseja ei ole käynnissä
            using (Mutex kaisaKaavioRunning = new Mutex(false, "Global\\KaisaKaavioRunning"))
            {
                if (!kaisaKaavioRunning.WaitOne(0, false))
                {
                    return false;
                }
            }

            return true;
        }

        private static void LataaUusinKaisaKaavioExe(string downloadPath, string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            using (var client = new HttpClient())
            {
                var result = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
                if (result != null)
                {
                    File.WriteAllBytes(downloadPath, result);
                }
            }
        }
    }
}

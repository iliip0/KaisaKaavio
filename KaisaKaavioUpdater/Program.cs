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
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            try
            {
                string kaisaKaavioExe = args[0];    // Arg[0] = KaisaKaavio exe tiedoston nimi
                int pid = Int32.Parse(args[1]);     // Arg[1] = KaisaKaavio prosessin id, joka kutsui päivitystä

                // Odotetaan että KaisaKaavio on sulkeutunut ennen päivitystä...
                if (!OdotaOhjelmanSulkeutumista(pid))
                {
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
                    File.Copy(downloadPath, kaisaKaavioExe);
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
                        File.Delete(kaisaKaavioExe);
                        File.Copy(downloadPath, kaisaKaavioExe);

                        return 1;
                    }
                }

                return 0;

                // Ei lataus käyttöliittymää (ainakaan toistaiseksi)
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
            }
            catch (Exception e)
            {
                return -1;
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

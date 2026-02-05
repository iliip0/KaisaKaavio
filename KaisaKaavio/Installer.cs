using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaisaKaavio
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();

            BeforeInstall += Installer_BeforeInstall;
            Committed += Installer_Committed;
        }

        private void Installer_BeforeInstall(object sender, InstallEventArgs e)
        {
            // Tarkistaa että KaisaKaavio ei ole auki
            using (Mutex kaisaKaavioRunning = new Mutex(false, "Global\\KaisaKaavioRunning"))
            {
                try
                {
                    if (!kaisaKaavioRunning.WaitOne(0, false))
                    {
                        throw new InstallException("KaisaKaavio ohjelma on käynnissä. Sulje ohjelma ennen uuden version asentamista");
                    }
                }
                catch
                {
                    throw new InstallException("KaisaKaavio ohjelma on käynnissä. Sulje ohjelma ennen uuden version asentamista");
                }
            }
        }

        private void Installer_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                // Käynnistää ohjelman
                var kansio = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Directory.SetCurrentDirectory(kansio);
                Process.Start(Path.Combine(kansio, "KaisaKaavio.exe"));
            }
            catch
            {
            }
        }
    }
}

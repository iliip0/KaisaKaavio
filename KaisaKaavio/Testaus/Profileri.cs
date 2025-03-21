﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaisaKaavio.Testaus
{
    /// <summary>
    /// Työkalu funktioiden suoritusajan mittaamiseen sekä 
    /// kutsupuiden tarkasteluun.
    /// 
    /// Tämä helpottaa ohjelman optimointia
    /// </summary>
    public class Profileri : IDisposable
    {
        public Profileri(string funktio)
        {
#if PROFILE
            AloitaKutsu(funktio);
#endif
        }

        public void Dispose()
        {
#if PROFILE
            LopetaKutsu();
#endif
        }

#if PROFILE
        private class Kutsu
        {
            public string Funktio { get; set; }
            public long Ticks { get; set; }
        }

        private static Dictionary<int, Stack<Kutsu>> kutsut = new Dictionary<int, Stack<Kutsu>>();

        private static Stack<Kutsu> Kutsupino(int thread)
        {
            lock (kutsut)
            {
                if (kutsut.ContainsKey(thread))
                {
                    return kutsut[thread];
                }

                Stack<Kutsu> pino = new Stack<Kutsu>();

                kutsut.Add(thread, pino);

                return pino;
            }
        }
#endif

        public static void AloitaKutsu(string funktio)
        {
#if PROFILE
            int id = Thread.CurrentThread.ManagedThreadId;

            var pino = Kutsupino(id);

            Kutsu kutsu = new Kutsu()
            {
                Funktio = funktio,
                Ticks = DateTime.Now.Ticks
            };

            pino.Push(kutsu);

            int indent = pino.Count;
            StringBuilder rivi = new StringBuilder(id.ToString() + ": ");

            for (int i = 0; i < indent; ++i)
            {
                rivi.Append("-");
            }

            rivi.Append(funktio + " (");

            Trace.WriteLine(rivi.ToString());
#endif
        }

        public static void LopetaKutsu()
        {
#if PROFILE
            int id = Thread.CurrentThread.ManagedThreadId;

            var pino = Kutsupino(id);

            long ticks = 0;
            string nimi = string.Empty;

            int indent = pino.Count;

            if (pino.Count > 0)
            {
                ticks = DateTime.Now.Ticks - pino.First().Ticks;
                nimi = pino.First().Funktio;
                pino.Pop();
            }

            StringBuilder rivi = new StringBuilder(id.ToString() + ": ");

            for (int i = 0; i < indent; ++i)
            {
                rivi.Append("-");
            }

            TimeSpan kesto = new TimeSpan(ticks);
            rivi.Append(string.Format(") {0}ms // {1}", kesto.Milliseconds, nimi));

            Trace.WriteLine(rivi.ToString());
#endif
        }

        public static void KirjaaKutsu(string funktio)
        {
#if PROFILE
            int id = Thread.CurrentThread.ManagedThreadId;

            var pino = Kutsupino(id);

            int indent = pino.Count;
            StringBuilder rivi = new StringBuilder(id.ToString() + ":");

            for (int i = 0; i <= indent; ++i)
            {
                rivi.Append("-");
            }

            rivi.Append(funktio + "(...)");
            Trace.WriteLine(rivi.ToString());
#endif
        }
    }
}

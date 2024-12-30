using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaisaKaavio.Tyypit
{
    /// <summary>
    /// Työkaluja aikaan liittyviin laskutoimituksiin
    /// </summary>
    public class Aika
    {
        public static bool AikaTunneiksiJaMinuuteiksi(string aika, out int tunnit, out int minuutit)
        {
            tunnit = 0;
            minuutit = 0;

            try
            {
                var osat = aika.Split(':');
                if (osat == null || osat.Count() < 2)
                {
                    osat = aika.Split('.');
                }

                if (osat != null && osat.Count() == 2)
                {
                    if (!Int32.TryParse(osat[0], out tunnit))
                    {
                        return false;
                    }

                    if (!Int32.TryParse(osat[1], out minuutit))
                    {
                        return false;
                    }

                    if (tunnit < 0 || tunnit > 23)
                    {
                        return false;
                    }

                    if (minuutit < 0 || minuutit > 59)
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch
            { 
            }

            return false;
        }

        public static bool AikaeroMinuutteina(string aika0, string aika1, out int aikaero)
        {
            aikaero = 0;

            try
            {
                int tunnit0 = 0;
                int minuutit0 = 0;
                int tunnit1 = 0;
                int minuutit1 = 0;

                if (AikaTunneiksiJaMinuuteiksi(aika0, out tunnit0, out minuutit0) &&
                    AikaTunneiksiJaMinuuteiksi(aika1, out tunnit1, out minuutit1))
                {
                    aikaero = tunnit1 * 60 - tunnit0 * 60;
                    aikaero += minuutit1 - minuutit0;

#if DEBUG
                    //Debug.WriteLine(string.Format("Aikaero {0} - {1} = {2} minuuttia", aika0, aika1, aikaero));
#endif
                    return true;
                }

#if DEBUG
                //Debug.WriteLine(string.Format("Aikaero {0} - {1} = tuntematon", aika0, aika1));
#endif
            }
            catch
            { 
            }

            return false;
        }
    }
}

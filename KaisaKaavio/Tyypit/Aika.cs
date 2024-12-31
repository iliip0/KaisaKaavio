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
        public static string KuukaudenNimi(int kuukausi)
        {
            switch (kuukausi)
            {
                case 1: return "Tammikuu";
                case 2: return "Helmikuu";
                case 3: return "Maaliskuu";
                case 4: return "Huhtikuu";
                case 5: return "Toukokuu";
                case 6: return "Kesakuu";
                case 7: return "Heinakuu";
                case 8: return "Elokuu";
                case 9: return "Syyskuu";
                case 10: return "Lokakuu";
                case 11: return "Marraskuu";
                case 12: return "Joulukuu";
                default: return kuukausi.ToString();
            }
        }

        public static string DateTimeToString(DateTime aika)
        {
            return string.Format("{0}.{1}.{2}", aika.Day, aika.Month, aika.Year);
        }

        public static DateTime ParseDateTime(string aika)
        {
            try
            {
                int vuosi = 0;
                int kuukausi = 0;
                int paiva = 0;

                if (!string.IsNullOrEmpty(aika))
                {
                    // Dates starting with yyyy-mm-dd
                    var parts = aika.Split('-');
                    if (parts.Count() >= 3 && parts[0].Length == 4 && parts[1].Length == 2 && parts[2].Length >= 2)
                    {
                        if (Int32.TryParse(parts[0], out vuosi) &&
                            Int32.TryParse(parts[1], out kuukausi) &&
                            Int32.TryParse(parts[2].Substring(0, 2), out paiva))
                        {
                            return new DateTime(vuosi, kuukausi, paiva); 
                        }
                    }

                    // Dates d.m.yyyy and dd.mm.yyyy
                    parts = aika.Split('.');
                    if (parts.Count() >= 3 &&
                        (parts[0].Length >= 1 && parts[0].Length <= 2) &&
                        (parts[1].Length >= 1 && parts[1].Length <= 2) &&
                        parts[2].Length >= 4)
                    {
                        if (Int32.TryParse(parts[0], out paiva) &&
                            Int32.TryParse(parts[1], out kuukausi) &&
                            Int32.TryParse(parts[2].Substring(0, 4), out vuosi))
                        {
                            return new DateTime(vuosi, kuukausi, paiva);
                        }
                    }
                }
            }
            catch
            {
                int iii = 0;
            }

            return DateTime.Parse(aika);
        }

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

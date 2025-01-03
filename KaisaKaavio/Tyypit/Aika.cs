using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static int RankingSarjanNumeroAjasta(Ranking.RankingSarjanPituus pituus, DateTime aika)
        {
            switch (pituus)
            {
                case Ranking.RankingSarjanPituus.Vuosi: return aika.Year;
                case Ranking.RankingSarjanPituus.Puolivuotta: return (aika.Month - 1) / 6;
                case Ranking.RankingSarjanPituus.Vuodenaika: return (aika.Month - 1) / 3;
                case Ranking.RankingSarjanPituus.Kuukausi: return aika.Month;
                default: throw new NotImplementedException(string.Format("Ranking sarjan pituus {0}", pituus));
            }
        }

        public static string RankingSarjanNimi(Ranking.RankingSarjanPituus pituus, int numero)
        {
            switch (pituus)
            {
                case Ranking.RankingSarjanPituus.Vuosi: return "VuosiRanking";

                case Ranking.RankingSarjanPituus.Puolivuotta:
                    switch (numero)
                    {
                        case 0: return "Kevät";
                        case 1: return "Syksy";
                    }
                    break;

                case Ranking.RankingSarjanPituus.Vuodenaika:
                    switch (numero)
                    {
                        case 0: return "Tammikuu-Maaliskuu";
                        case 1: return "Huhtikuu-Kesäkuu";
                        case 2: return "Heinäkuu-Syyskuu";
                        case 3: return "Lokakuu-Joulukuu";
                    }
                    break;

                case Ranking.RankingSarjanPituus.Kuukausi:
                    switch (numero)
                    {
                        case 1: return "Tammikuu";
                        case 2: return "Helmikuu";
                        case 3: return "Maaliskuu";
                        case 4: return "Huhtikuu";
                        case 5: return "Toukokuu";
                        case 6: return "Kesäkuu";
                        case 7: return "Heinäkuu";
                        case 8: return "Elokuu";
                        case 9: return "Syyskuu";
                        case 10: return "Lokakuu";
                        case 11: return "Marraskuu";
                        case 12: return "Joulukuu";
                    }
                    break;
            }

#if DEBUG
            throw new NotImplementedException(string.Format("Pituus {0}, Numero {1}", pituus, numero));
#else
            return string.Format("{0}_{1}", pituus, numero);
#endif
        }

        public static string RankingSarjanTiedostonNimi(Ranking.RankingSarjanPituus pituus, int numero)
        {
            switch (pituus)
            {
                case Ranking.RankingSarjanPituus.Vuosi: return "Vuosi";

                case Ranking.RankingSarjanPituus.Puolivuotta:
                    switch (numero)
                    {
                        case 0: return "Kevat";
                        case 1: return "Syksy";
                    }
                    break;

                case Ranking.RankingSarjanPituus.Vuodenaika:
                    switch (numero)
                    {
                        case 0: return "Tammikuu_Maaliskuu";
                        case 1: return "Huhtikuu_Kesakuu";
                        case 2: return "Heinakuu_Syyskuu";
                        case 3: return "Lokakuu_Joulukuu";
                    }
                    break;

                case Ranking.RankingSarjanPituus.Kuukausi:
                    switch (numero)
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
                    }
                    break;
            }

#if DEBUG
            throw new NotImplementedException(string.Format("Pituus {0}, Numero {1}", pituus, numero));
#else
            return string.Format("{0}_{1}", pituus, numero);
#endif
        }

        public static void RankingSarjaKuukaudet(Ranking.RankingSarjanPituus pituus, int numero, out int kk0, out int kk1)
        {
            kk0 = 0;
            kk1 = 0;

            switch (pituus)
            {
                case Ranking.RankingSarjanPituus.Vuosi:
                    kk0 = 1;
                    kk1 = 12;
                    break;

                case Ranking.RankingSarjanPituus.Puolivuotta:
                    if (numero == 0)
                    {
                        kk0 = 1;
                        kk1 = 6;
                    }
                    else
                    {
                        kk0 = 7;
                        kk1 = 12;
                    }
                    break;

                case Ranking.RankingSarjanPituus.Vuodenaika:
                    switch (numero)
                    {
                        case 0:
                            kk0 = 1;
                            kk1 = 3;
                            break;

                        case 1:
                            kk0 = 4;
                            kk1 = 6;
                            break;

                        case 2:
                            kk0 = 7;
                            kk1 = 9;
                            break;

                        default:
                            kk0 = 10;
                            kk1 = 12;
                            break;
                    }
                    break;

                case Ranking.RankingSarjanPituus.Kuukausi:
                    kk0 = numero;
                    kk1 = numero;
                    break;
            }
        }

        public static string RankingSarjaKansio(int vuosi)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "KaisaKaaviot", 
                "Ranking",
                vuosi.ToString());
        }
    }
}

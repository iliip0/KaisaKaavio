using System.Net;

namespace KaisaKaavio.Integraatio
{
    internal class Turvallisuus
    {
        public static void AsetaHttpsAsetukset()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback
             = ((sender, cert, chain, errors) => true);
        }
    }
}

using System.Net;
using System.Security.Cryptography.X509Certificates;

public class IgnoreCertificate
{
    // Este método se ejecutará al cargar el proyecto y permitirá certificados no válidos
    [System.Obsolete]
    public static void AllowInvalidCertificates()
    {
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true; // Acepta todos los certificados
            };
    }
}
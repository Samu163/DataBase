using System.Net;
using System.Security.Cryptography.X509Certificates;

public class IgnoreCertificate
{
    // Este m�todo se ejecutar� al cargar el proyecto y permitir� certificados no v�lidos
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
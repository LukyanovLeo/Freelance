using System.Security.Cryptography.X509Certificates;

namespace Freelance.Services.Interfaces
{
    public interface ICertificateService
    {
        public X509Certificate2 LoadCertificate(string subject);
        public string GetPublicKey(X509Certificate2 cert);
        string TryDecrypt(X509Certificate2 cert, string input);
    }
}

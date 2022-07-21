using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Freelance.WebApi.Contracts.Settings
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenLifeTime { get; set; }
        public int AuthSaltRequestLifeTime { get; set; }
        public string SecureRandomString { get; set; }
        public string CertificateSubject { get; set; }
        public int RefreshTokenLifeTime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}

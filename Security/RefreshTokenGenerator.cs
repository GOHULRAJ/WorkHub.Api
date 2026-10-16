using System.Security.Cryptography;
using System.Text;

namespace WorkHub.Api.Security
{
    public class RefreshTokenGenerator
    {
        public static string Generate()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public static string Hash(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = SHA256.HashData(tokenBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}

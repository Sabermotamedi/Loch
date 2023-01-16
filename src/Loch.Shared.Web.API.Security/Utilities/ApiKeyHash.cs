using System.Security.Cryptography;
using System.Text;

namespace Loch.Shared.Web.API.Security.Utilities
{
    public class ApiKeyHash : IApiKeyHash
    {
        public string ComputeHash(string stringToHash, string salt = null)
        {
            var stringWithSalt = new List<byte>(Encoding.UTF8.GetBytes(stringToHash));
            if (!string.IsNullOrEmpty(salt))
                stringWithSalt.AddRange(Encoding.UTF8.GetBytes(salt.ToLower()));

            var hashAlgorithm = new SHA256Managed();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(stringWithSalt.ToArray()));
        }
    }
}

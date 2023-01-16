using System;
using System.Security.Cryptography;
using System.Text;

namespace Loch.Shared.Application.Helpers;
public class HashingHelper
{
    public static string HashUsingPbkdf2(string password, string salt)
    {
        using var bytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);
        var derivedRandomKey = bytes.GetBytes(32);
        var hash = Convert.ToBase64String(derivedRandomKey);
        return hash;
    }

    public static string CreateSalt(int size)
    {
        //Generate a cryptographic random number.
        var buff = new byte[size];
        var rng = new RNGCryptoServiceProvider();

        rng.GetBytes(buff);

        // Return a Base64 string representation of the random number.
        return Convert.ToBase64String(buff);
    }

    public string HashKeyGenerate(string serviceName, params object[] args)
    {

        var sb = new StringBuilder();

        sb.Append($"{serviceName}");

        foreach (var arg in args)
        {
            sb.Append("_" + arg );
        }

        return sb.ToString();

    }
}

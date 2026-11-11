using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public static class PasswordHasher
    {
        private const string Salt = "APAE_PORTAL_ELO_SALT_2026_SECURE";

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            using var sha256 = SHA256.Create();
            var combined = password + Salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash)) return false;
            var computed = HashPassword(password);
            return computed == hash;
        }
    }
}

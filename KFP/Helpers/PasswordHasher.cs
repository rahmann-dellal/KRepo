using System;
using System.Security.Cryptography;

namespace KFP.Helpers
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool verifyPassword(string password, string hash)
        {
            if (HashPassword(password) == hash) { 
                return true;
            } else
            {
                return false;
            }
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace SelectelSharpCore.Common
{
    public static class Helpers
    {
        public static long DateToUnixTimestamp(DateTime date)
        {
            var ts = date - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long) ts.TotalSeconds;
        }

        public static string CalculateSha1(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            using (var sha1 = SHA1.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha1.ComputeHash(bytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("X2"));

                return sb.ToString().ToLower();
            }
        }
    }
}
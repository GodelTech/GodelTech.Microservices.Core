using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GodelTech.Microservices.Core.Services
{
    public class Sha512HashCalculator : ISha512HashCalculator
    {
        public string ComputeHash(Stream content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            using (var sha = new SHA512Managed())
            {
                return ToHex(sha.ComputeHash(content));
            }
        }

        private static string ToHex(byte[] data, bool lowercase = true)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var sb = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
    }
}

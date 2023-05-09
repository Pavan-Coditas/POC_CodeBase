
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Api.Helper
{
    public class HashingHelper
    {
        public string OneWayHash(string password)
        {
            using (SHA256 sha256hash = SHA256.Create())
            {
                var hash = GetHash(sha256hash, password);
                Console.WriteLine(hash);
                return hash;
            }
        }
        public bool VerifyHash(string password, string userPassword)
        {
            SHA256 sha256hash = SHA256.Create();
            var hash = password;
            var hashOfInput = GetHash(sha256hash, userPassword);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }

        private string GetHash(HashAlgorithm hashAlgorithm, string password)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}

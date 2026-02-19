using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PaymentAPI.Controllers
{
    [ApiController]
    [Route("api/v1/cert")]
    public class CertificateController(IConfiguration configuration) : ControllerBase
    {

        private string GetCertSubject()
        {
            return configuration.GetSection("Appsettings").GetValue<string>("cert")!;
        }

        private StoreLocation GetStoreLocation()
        {
            var isAzure = configuration.GetSection("Appsettings").GetValue<bool>("azureEnv")!;
            return !isAzure ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;
        }

        [HttpGet("encrypt")]
        public IActionResult EncryptString([FromQuery] string val)
        {
            var encrtptedStr = Encrypt(val);
            return Ok(encrtptedStr);
        }

        [HttpGet("decrypt")]
        public IActionResult DecryptString([FromQuery] string val)
        {
            var res = Decrypt(val);
            return Ok(res);
        }

        private X509Certificate2 GetCertificateByCn(string cn, StoreLocation storeLocation)
        {
            using var store = new X509Store(storeLocation);
            store.Open(OpenFlags.ReadOnly);

            // Find by Subject Name (CN)
            var cert = store.Certificates
                .Find(X509FindType.FindBySubjectName, cn, validOnly: false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();

            if (cert == null)
                throw new Exception($"Certificate with CN '{cn}' not found.");

            return cert;
        }

        private string Encrypt(string plainText)
        {
            var cert = GetCertificateByCn(
                GetCertSubject(),
                GetStoreLocation()
            );

            using RSA rsa = cert.GetRSAPublicKey();

            if (rsa == null)
                throw new Exception("Certificate does not contain RSA public key.");

            byte[] data = Encoding.UTF8.GetBytes(plainText);

            byte[] encryptedBytes = rsa.Encrypt(
                data,
                RSAEncryptionPadding.OaepSHA256
            );

            return Convert.ToBase64String(encryptedBytes);
        }

        private string Decrypt(string encryptedBase64)
        {
            var cert = GetCertificateByCn(
               GetCertSubject(),
               GetStoreLocation()
           );

            using RSA rsa = cert.GetRSAPrivateKey();

            if (rsa == null)
                throw new Exception("Certificate does not contain private key.");

            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

            byte[] decryptedBytes = rsa.Decrypt(
                encryptedBytes,
                RSAEncryptionPadding.OaepSHA256   // Must match encryption
            );

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

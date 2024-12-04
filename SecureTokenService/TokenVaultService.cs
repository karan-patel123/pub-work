using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SecureTokenService
{
    public class TokenVaultService : ServiceBase
    {
        private const string CacheFilePath = @"E:\dot_net\SecureTokenService\CacheFile.txt";
        private const string VaultUrl = "http://myvault.com/gettoken";
        private static readonly HttpClient httpClient = new HttpClient();

        public TokenVaultService()
        {
            this.ServiceName = "TokenVaultService";
        }

        protected override void OnStart(string[] args)
        {
            // Start the service asynchronously
            Task.Run(() => GetSecretAsync());
        }

        protected override void OnStop()
        {
            // Clean up resources if needed
        }

        private async Task GetSecretAsync()
        {
            string secret = await RetrieveFromRemoteVaultAsync() ?? RetrieveFromLocalCache();
            // Do something with the secret
        }

        private async Task<string> RetrieveFromRemoteVaultAsync()
        {
            try
            {
                var response = await httpClient.GetAsync(VaultUrl);
                response.EnsureSuccessStatusCode();
                string secret = await response.Content.ReadAsStringAsync();
                CacheSecret(secret);
                return secret;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private string RetrieveFromLocalCache()
        {
            if (File.Exists(CacheFilePath))
            {
                byte[] encryptedData = File.ReadAllBytes(CacheFilePath);
                return DecryptSecret(encryptedData);
            }
            return null;
        }

        private void CacheSecret(string secret)
        {
            byte[] encryptedData = EncryptSecret(secret);
            File.WriteAllBytes(CacheFilePath, encryptedData);
        }

        private byte[] EncryptSecret(string secret)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            return ProtectedData.Protect(secretBytes, null, DataProtectionScope.LocalMachine);
        }

        private string DecryptSecret(byte[] encryptedData)
        {
            byte[] secretBytes = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(secretBytes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                httpClient.Dispose();
            }
            base.Dispose(disposing);
        }

        public static void Main()
        {
            ServiceBase.Run(new TokenVaultService());
        }
    }
}

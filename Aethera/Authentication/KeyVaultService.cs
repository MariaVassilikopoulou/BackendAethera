using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Aethera.Authentication
{
    public class KeyVaultService
    {
        private readonly SecretClient _secretClient;

        public KeyVaultService(IConfiguration config)
        {
            var keyVaultName = config["KeyVaultName"];   
            var vaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            var credential = new DefaultAzureCredential();

            _secretClient = new SecretClient(vaultUri, credential);
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            var secret = await _secretClient.GetSecretAsync(secretName);
            return secret.Value.Value;
        }

    }



}

using HoneyComb.Vault.Exceptions;
using HoneyComb.Vault.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace HoneyComb.Vault
{
    public static class Extensions
    {
        private const string SectionName = "vault";
        public static IWebHostBuilder UseVault(this IWebHostBuilder builder, string keyValuePath = null,
            string sectionName = SectionName)
            => builder.ConfigureServices(services =>
            {
                if (string.IsNullOrWhiteSpace(sectionName))
                    sectionName = SectionName;

                IConfiguration config;
                using var sp = services.BuildServiceProvider();
                config = sp.GetRequiredService<IConfiguration>();

                var options = config.GetSettings<VaultOptions>(sectionName);
                services.AddSingleton(options);
            })
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                var options = cfg.Build().GetSettings<VaultOptions>(sectionName);
                if (!options.Enabled)
                    return;

                cfg.AddVaultAsync(options, keyValuePath).GetAwaiter().GetResult();
            });


        private static async Task AddVaultAsync(this IConfigurationBuilder builder, VaultOptions options,
            string keyValuePath)
        {
            var kvPath = string.IsNullOrWhiteSpace(keyValuePath) ? options.Kv?.Path : keyValuePath;
            var (client, _) = GetClientAndSettings(options);
            if (!string.IsNullOrWhiteSpace(kvPath) && options.Kv.Enabled)
            {
                Console.WriteLine($"Loading settings from Vault: '{options.Url}', KV path: '{kvPath}'.");
                var keyValueSecrets = new KeyValueSecret(client, options);
                var secret = await keyValueSecrets.GetAsync(kvPath);
                var parser = new JsonParser();
                var data = parser.Parse(JObject.FromObject(secret));
                var source = new MemoryConfigurationSource { InitialData = data };
                builder.Add(source);
            }

        }

        private static (IVaultClient client, VaultClientSettings settings) GetClientAndSettings(VaultOptions options)
        {
            var settings = new VaultClientSettings(options.Url, GetAuthMethod(options));
            var client = new VaultClient(settings);

            return (client, settings);
        }

        private static IAuthMethodInfo GetAuthMethod(VaultOptions options)
            => options.AuthType?.ToLowerInvariant() switch
            {
                "token" => new TokenAuthMethodInfo(options.Token),
                "userpass" => new UserPassAuthMethodInfo(options.Username, options.Password),
                _ => throw new VaultAuthTypeNotSupportedException(
                    $"Vault auth type: '{options.AuthType}' is not supported.", options.AuthType)
            };
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Polly;
using Prodigy.Vault.Exceptions;
using Prodigy.Vault.Internals;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace Prodigy.Vault
{
    public static class Extensions
    {
        private const string SectionName = "vault";
        public static IWebHostBuilder UseVault(this IWebHostBuilder builder, string keyValuePath = null,
            string sectionName = SectionName, bool retryOnError = true, Action<IWebHostBuilder, IServiceProvider> configure = null)
            => builder.ConfigureServices(services =>
            {
                if (string.IsNullOrWhiteSpace(sectionName))
                    sectionName = SectionName;

                IConfiguration config;
                using var sp = services.BuildServiceProvider();
                config = sp.GetRequiredService<IConfiguration>();

                var options = config.GetSettings<VaultOptions>(sectionName);
                services.AddSingleton(options);
                configure?.Invoke(builder, sp);
            })
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                var env = ctx.HostingEnvironment.EnvironmentName;
                var options = cfg.Build().GetSettings<VaultOptions>(sectionName);
                if (!options.Enabled)
                    return;


                cfg.AddVaultAsync(options, keyValuePath, retryOnError).GetAwaiter().GetResult();
                cfg.AddJsonFile("appsettings.json", false);
                cfg.AddJsonFile($"appsettings.{env}.json", true);

                //cfg.AddVaultAsync(options, keyValuePath, retryOnError).GetAwaiter().GetResult();
                //cfg.AddJsonFile("appsettings.json", false);
                //cfg.AddJsonFile($"appsettings.{env}.json", true);
            });


        private static async Task AddVaultAsync(this IConfigurationBuilder builder, VaultOptions options,
            string keyValuePath, bool retryOnError)
        {
            var logger = Prodigy.Logging.Extensions.BuildLoggerConfiguration().CreateLogger();
            var kvPath = string.IsNullOrWhiteSpace(keyValuePath) ? options.Kv?.Path : keyValuePath;
            if (!string.IsNullOrWhiteSpace(kvPath) && options.Kv.Enabled)
            {
                logger.Information($"Loading settings from Vault: '{options.Url}', KV path: '{kvPath}'.");
                
                IDictionary<string, object> secret = new Dictionary<string, object>();

                if (retryOnError)
                {
                    var policyResult = await Policy
                        .Handle<Exception>()
                        .WaitAndRetryForeverAsync((r, e, ctx) => TimeSpan.FromSeconds(3 * r), OnRetryException)
                        .ExecuteAndCaptureAsync(async () => 
                        {
                            var (client, _) = GetClientAndSettings(options);
                            var keyValueSecrets = new KeyValueSecret(client, options);
                            return await keyValueSecrets.GetAsync(kvPath);
                        });

                    secret = policyResult.Result;
                    Task OnRetryException(Exception ex, int r, TimeSpan ts, Context ctx)
                    {
                        logger.Error(ex, $"Retry [ {r} / {double.PositiveInfinity} ]. Error while loading settings from Vault: '{options.Url}', KV path: '{kvPath}'. {ex.Message}");
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    var (client, _) = GetClientAndSettings(options);
                    var keyValueSecrets = new KeyValueSecret(client, options);
                    secret = await keyValueSecrets.GetAsync(kvPath);
                }
                    

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

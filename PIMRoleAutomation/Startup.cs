using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using PIMRoleAutomation.Common;
using Microsoft.Graph;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using static System.Formats.Asn1.AsnWriter;

namespace PIMRoleAutomation
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var localRoot = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
            var azureRoot = $"{Environment.GetEnvironmentVariable("HOME")}/site/wwwroot";
            var actualRoot = localRoot ?? azureRoot;

            var configBuilder = new ConfigurationBuilder()
                                .SetBasePath(actualRoot)
                                .AddEnvironmentVariables()
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            var config = configBuilder.Build();
            //var keyVaultUri = config.GetValue<string>(KeyVaultUri);
            //if (!string.IsNullOrWhiteSpace(keyVaultUri))
            //{
            //    var msiCredential = new ManagedIdentityCredential();
            //    configBuilder.AddAzureKeyVault(new Uri(keyVaultUri), new ChainedTokenCredential(msiCredential, new DefaultAzureCredential()));
            //    config = configBuilder.Build();
            //}

            // Replace the existing config with the new one
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            builder.Services.AddSingleton(ConfigureGraphClient);
            builder.Services.AddSingleton(ConfigureGraphService);
        }

        public static GraphServiceClient ConfigureGraphClient(IServiceProvider provider)
        {
            var msiCredential = new ManagedIdentityCredential();
            var credential = new ChainedTokenCredential(msiCredential, new DefaultAzureCredential());
            var scope = new[] { "https://graph.microsoft.com/.default" };
            //// Create a new instance of GraphServiceClient with the authentication provider
            return new GraphServiceClient(credential, scope);
        }

        public static GraphService ConfigureGraphService(IServiceProvider provider)
        {
            var graphClient = provider.GetService<GraphServiceClient>();
            return new GraphService(graphClient);
        }
    }
}

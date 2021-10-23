using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Prodigy;
using Prodigy.WebApi;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        => await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services => services
                        .AddProdigy()
                        .AddWebApi())
                    .Configure(app => app
                        .UseEndpoints(endpoints => endpoints
                            .Get("", ctx => ctx.Response.WriteAsync("WebApi app using Prodigy :)"))
                            .Get<Test>("test/{customText?}",
                                async (query, ctx) => await ctx.Response.WriteAsync(JsonConvert.SerializeObject(query)))
                            .Post<EmptyCommand>("/test/empty")

                        ));
                });
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using HoneyComb;
using HoneyComb.WebApi;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
                        .AddHoneyComb()
                        .AddWebApi())
                    .Configure(app => app
                        .UseEndpoints(endpoints => endpoints
                            .Get("", ctx => ctx.Response.WriteAsync("WebApi app using HoneyComb :)"))
                            .Get<Test>("test/{customText?}",
                                async (query, ctx) => await ctx.Response.WriteAsync(JsonConvert.SerializeObject(query)))
                            .Post<EmptyCommand>("/test/empty")

                        ));
                });
    }
}

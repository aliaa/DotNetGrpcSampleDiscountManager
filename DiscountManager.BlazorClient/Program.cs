using DiscountManager.ProtoDefinitions;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DiscountManager.BlazorClient;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddSingleton(services =>
        {
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress("https://localhost:7286",
                new GrpcChannelOptions
                {
                    HttpClient = httpClient
                });
            return new Discount.DiscountClient(channel);
        });

        await builder.Build().RunAsync();
    }
}

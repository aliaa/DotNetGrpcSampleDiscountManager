using DiscountManager.ProtoDefinitions;
using Grpc.Net.Client;
using System.Diagnostics;

namespace DiscountManager.ConsoleClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Connecting to GRPC.");
        using var channel = GrpcChannel.ForAddress("https://localhost:7286");
        var client = new Discount.DiscountClient(channel);

        while (true)
        {
            Console.Write("Press G to generate, E to get get a code, U to use, S to stress test, or Q to quit: ");
            var methodInput = Console.ReadLine().ToUpper();
            if (methodInput == "Q")
            {
                break;
            }
            switch (methodInput)
            {
                case "G":
                    await Generate(client);
                    break;

                case "E":
                    await GetCode(client);
                    break;

                case "U":
                    await UseCode(client);
                    break;

                case "S":
                    await StressTest(client);
                    break;

                default:
                    Console.WriteLine("Wrong input.");
                    break;
            }
        }
        Console.WriteLine("Finished.");
        Console.ReadKey();
    }

    private static async Task Generate(Discount.DiscountClient client)
    {
        Console.Write("How many codes to generate: ");
        var count = uint.Parse(Console.ReadLine());
        Console.Write("Enter the length of codes: ");
        var length = uint.Parse(Console.ReadLine());
        var response = await client.GenerateAsync(new GenerateRequest { Count = count, Length = length });
        Console.WriteLine($"Response: {response?.Result}");
    }

    private static async Task GetCode(Discount.DiscountClient client)
    {
        var response = await client.GetCodeAsync(new Empty());
        if (response?.Result == true)
        {
            Console.WriteLine($"Code: {response.Code}");
        }
        else
        {
            Console.WriteLine("Failed to retrieve code");
        }
    }

    private static async Task UseCode(Discount.DiscountClient client)
    {
        Console.Write("Enter code to use: ");
        var code = Console.ReadLine();
        var response = await client.UseCodeAsync(new UseCodeRequest { Code = code });
        Console.WriteLine($"Response: {response?.Result}");
    }

    private static async Task StressTest(Discount.DiscountClient client)
    {
        Console.WriteLine("How many calls: ");
        var callCount = uint.Parse(Console.ReadLine());

        var generateRequest = new GenerateRequest { Count = 1, Length = 8 };
        var getCodeRequest = new Empty();

        var startTime = Stopwatch.GetTimestamp();
        for (int i = 0; i < callCount; i++)
        {
            await client.GenerateAsync(generateRequest);
            var getCodeResp = await client.GetCodeAsync(getCodeRequest);
            await client.UseCodeAsync(new UseCodeRequest { Code = getCodeResp.Code });
        }
        var duration = Stopwatch.GetElapsedTime(startTime);
        Console.WriteLine($"It took {duration} to run {callCount} times.");
    }

}

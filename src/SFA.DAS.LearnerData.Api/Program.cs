using SFA.DAS.LearnerData.Api;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseNServiceBusContainer()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureKestrel(c => c.AddServerHeader = false)
                    .UseStartup<Startup>();
            });
}
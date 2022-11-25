using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DynamoDBSeed.Model;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2;

var host =
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        // Add configuration
        SeedConfig config = new();
        context.Configuration.Bind("SeedConfiguration", config);
        services.AddSingleton(config);

        // Add AWS configuration
        services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();        
        services.AddHostedService<DataSeedService>();
    })

    .UseConsoleLifetime()
    .Build();











// Application code should start here.

await host.RunAsync();
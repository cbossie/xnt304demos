using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2;
using Amazon.DAX;
using DynamoDbDax.Model;
using Amazon.Extensions.NETCore.Setup;
using System.Numerics;
using DynamoDbDax;

//Initialize the Random Number Generator
Random rnd = new Random(DateTime.Now.Microsecond * 1000 + DateTime.Now.Nanosecond);

var host =
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configuration for our demo
        DaxConfig config = new DaxConfig();
        context.Configuration.Bind("DaxTesting", config);
        services.AddSingleton(config);

        services.AddLogging();
        // Add AWS configuration
        services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());
        
        //Add plain AWS DynamoDB CLient
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton(services =>
        {
            DaxConfig cfg = services.GetRequiredService<DaxConfig>();
            AWSOptions opt = services.GetRequiredService<AWSOptions>();
            ClusterDaxClient dax = new ClusterDaxClient(new DaxClientConfig(cfg.DaxEndpoint)
            {
                AwsCredentials = opt.Credentials
            });
            return dax;
        });
    })
    .UseConsoleLifetime()
    .Build();

// Get parameters (order ID and iterations)
var config = host.Services.GetRequiredService<DaxConfig>();
int orderId = rnd.Next(config.StartingOrderId, config.EndingOrderId);
int iterations = config.Iterations;

// Run Benchmark for Regular DynamoDB
Console.WriteLine($"Starting DynamoDB Test for Order: {orderId} --- {iterations} iterations.");
var dynamoDBClient = host.Services.GetRequiredService<IAmazonDynamoDB>();
var result = await DynamoDbBenchmark.BenchmarkRequests(dynamoDBClient, TestType.DynamoDB, config.TableName, orderId, iterations);


Console.WriteLine(result);

















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
string orderId = rnd.Next(config.StartingOrderId, config.EndingOrderId).ToString();
int iterations = config.Iterations;

// Get Clients
var daxClient = host.Services.GetRequiredService<ClusterDaxClient>();
var dynamoDBClient = host.Services.GetRequiredService<IAmazonDynamoDB>();

Console.WriteLine("--------------------------------");
Console.WriteLine("-Get single item test-----------");
Console.WriteLine("--------------------------------");
// Single Get Tests
Console.WriteLine($"DynamoDB Test for Order: {orderId} --- {iterations} iterations.");
var dynamoDBresult = await DynamoDbBenchmark.BenchmarkRequestsGet(dynamoDBClient, TestType.DynamoDBGet, config.TableName, orderId, iterations);
Console.WriteLine();
Console.WriteLine(dynamoDBresult);

Console.WriteLine($" DAX Test for Order: {orderId} --- {iterations} iterations.");
var daxGetresult = await DynamoDbBenchmark.BenchmarkRequestsGet(daxClient, TestType.DaxGet, config.TableName, orderId, iterations);
Console.WriteLine();
Console.WriteLine(daxGetresult);

Console.WriteLine("--------------------------------");
Console.WriteLine("-Table scan test-----------");
Console.WriteLine("--------------------------------");
// Table Scan Tests
Console.WriteLine($"Starting DynamoDB Test for Scan: {config.Character} --- {config.ScanIterations} iterations.");
var dynamoDbScanResult = await DynamoDbBenchmark.BenchmarkRequestsScan(dynamoDBClient, TestType.DynamoDbScan, config.TableName, config.Character, config.ScanIterations);
Console.WriteLine();
Console.WriteLine(dynamoDbScanResult);

Console.WriteLine($"Starting DAX Test for Scan: {config.Character} --- {config.ScanIterations} iterations.");
var daxScanresult = await DynamoDbBenchmark.BenchmarkRequestsScan(daxClient, TestType.DaxScan, config.TableName, config.Character, config.ScanIterations);
Console.WriteLine();
Console.WriteLine(daxScanresult);


















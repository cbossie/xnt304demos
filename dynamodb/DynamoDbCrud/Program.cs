using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2;
using DynamoDbCrud;
using DynamoDbCrud.LowLevel;
using DynamoDbCrud.ObjectPersistence;

var host =
    Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();
        // Add AWS configuration
        services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();
    })
    .UseConsoleLifetime()
    .Build();

// Process for running Crud Operations
IAmazonDynamoDB client = host.Services.GetRequiredService<IAmazonDynamoDB>();
IConfiguration cfg = host.Services.GetRequiredService<IConfiguration>();

IDynamoDBCrud? crudObject;
bool continueWork = true;

while (continueWork)
{
    // Pick the model
    do
    {
        Console.WriteLine("Do you want to use the (l)ow level or (o)bject persistence model?");
        var modelType = Console.ReadLine();

        crudObject = modelType switch
        {
            "l" or "L" => new LowLevelCrud { Client = client, TableName = cfg["TableName"] },
            "o" or "O" => new ObjectPersistenceCrud { Client = client },
            _ => null
        };
    } while (crudObject is null);


    // Decide on the operation
    bool? isRead;
    do
    {
        Console.WriteLine("Do you want to (g)et or (p)ut data?");
        var operation = Console.ReadLine();
        
        
        isRead = operation?[0] switch
        {
            'g' or 'G' => true,
            'p' or 'P' => false,
            _ => null
        };
    } while (!isRead.HasValue);


    string name = null;
    do
    {
        Console.WriteLine("Please Enter the Character Name:");
        name = Console.ReadLine() ?? string.Empty;
    } while (string.IsNullOrEmpty(name));

    //Read
    if (isRead.Value)
    {
        var data = await crudObject.GetAsync(name);
        Console.WriteLine("Your info is:");
        Console.Write(data);
        Console.WriteLine();
    }
    //Put
    else
    {
        string? origin = null;
        Console.WriteLine($"Origin of {name}?");
        origin = Console.ReadLine();

        string? role = null;
        Console.WriteLine($"Role of {name}?");
        role = Console.ReadLine();

        string ageString = null;
        Console.WriteLine($"Age of {name}?");
        ageString = Console.ReadLine();
        int? age = null;
        if(int.TryParse(ageString, out int ageP))
        {
            age = ageP;
        }


        List<string> items = null;
        Console.WriteLine($"List of {name}'s items (comma separated)?");
        items = (Console.ReadLine() ?? string.Empty).Split(',').ToList();

        //Perform the put operation
        await crudObject.PutAsync(name, origin, role, age, items);
        Console.WriteLine("Item written to DynamoDB!");
    }

    Console.WriteLine("Do you want to go again?");
    continueWork = (Console.ReadLine()).PadRight(1)?.Substring(0,1)?.ToUpper() == "Y";
}

Console.WriteLine("Done!");

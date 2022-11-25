using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DynamoDBSeed.Model;

public class DataSeedService : IHostedService
{
    // Configuration for populating the DynamoDB Data
    SeedConfig _config;

    IHostApplicationLifetime _lifetime;
    IDynamoDBContext _ddbContext;
    ILogger _logger;

    private static Random _rnd = new Random(DateTime.Now.Microsecond);
    
    public DataSeedService(IHostApplicationLifetime lifetime, SeedConfig config, IAmazonDynamoDB dynamoDbCli, ILogger<DataSeedService> logger)
    {
        _lifetime = lifetime;
        _config = config;
        _ddbContext = new DynamoDBContext(dynamoDbCli);
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //Initialize Collections
        var customers = await ReadCustomerIdFile();
        var states = await ReadStateFile();
        var products = await ReadProductFile();

        List<Order> ordersToProcess = new List<Order>();

        // Simple looping
        for(int ctr = _config.StartingOrderId; ctr <= _config.EndingOrderId; ctr++)
        {
            List<Product> prods = new(ChooseRandomItems(products, _config.MaxItems));
            Order orderToPersist = new Order
            {
                Id = ctr,
                CustomerId = ChooseOneRandomItem(customers),
                Year = _config.CurrentYear,
                State = ChooseOneRandomItem(states),
                Total = prods.Sum(p => p.Price),
                NumItems = prods.Count
            };
            orderToPersist.Products.AddRange(prods);
            ordersToProcess.Add(orderToPersist);

            if(ctr % 1000 == 0)
            {
                _logger.LogInformation($"Created {ctr} items to persist");
            }

        }

        try
        {
            // Batch Write
            var orderBatch = _ddbContext.CreateBatchWrite<Order>();
            orderBatch.AddPutItems(ordersToProcess);         
            await orderBatch.ExecuteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        _logger.LogInformation($"Wrote {ordersToProcess.Count} items.");

        _lifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Done");
    }

    #region Data Retrieval

    private IEnumerable<T> ChooseRandomItems<T>(List<T> list, int num)
    {
        HashSet<T> chosen = new();
        int ctr = 0;
        int numToRetrieve = _rnd.Next(1, num);


        while(ctr < numToRetrieve)
        {
            T item = list.ElementAt(_rnd.Next(list.Count - 1));
            if (chosen.Add(item))
            {
                ctr++;
                yield return item;
            }
        }
    }

    private T ChooseOneRandomItem<T>(List<T> list)
    {
        return ChooseRandomItems(list, 1).First();
    }


    private async Task<List<Product>> ReadProductFile() 
    {
        var data = await ReadFile(_config.ProductFile);
        return data.Select(a => 
        {
            var segments = a.Split(',');
            return new Product 
            {
                Id = int.Parse(segments[0]),
                Name = segments[1],
                Price = decimal.Parse(segments[2])            
            };
        }).ToList();
    }

    private async Task<List<string>> ReadStateFile()
    {
        return await ReadFile(_config.StateFile);
    }

    private async Task<List<string>> ReadCustomerIdFile()
    {
        return await ReadFile(_config.CustomerFile);
    }

    /// <summary>
    /// This assumes that the file is relative to the working directory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private async Task<List<string>> ReadFile(string fileName)
    {
        var fileData = await File.ReadAllLinesAsync(fileName);
        return fileData.ToList();
    }

    #endregion













}




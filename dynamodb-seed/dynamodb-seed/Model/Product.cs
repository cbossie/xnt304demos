using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;

namespace DynamoDBSeed.Model;

public class Product
{
    [DynamoDBProperty("Id")]
    public int Id { get; set; }
    [DynamoDBProperty("Name")]
    public string Name { get; set; }
    [DynamoDBProperty("Price")]
    public decimal Price { get; set; }
}




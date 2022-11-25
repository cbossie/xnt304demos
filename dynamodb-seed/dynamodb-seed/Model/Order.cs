using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDBSeed.Model;

[DynamoDBTable("Orders")]
public class Order
{
    [DynamoDBHashKey]
    public required int Id { get; init; }

    [DynamoDBProperty("CustomerId")]
    [DynamoDBGlobalSecondaryIndexHashKey("CustomerIdYear")]
    public required string CustomerId { get; init; }

    [DynamoDBProperty("Year")]
    [DynamoDBGlobalSecondaryIndexRangeKey("CustomerIdYear")]
    public required int Year { get; init; }

    [DynamoDBProperty("State")]
    public required string State { get; init; }

    [DynamoDBProperty("Products")]
    public List<Product> Products { get; init; } = new List<Product>();

    [DynamoDBProperty("Total")]
    public required decimal Total { get; init; }

    public required int NumItems { get; init; }

}

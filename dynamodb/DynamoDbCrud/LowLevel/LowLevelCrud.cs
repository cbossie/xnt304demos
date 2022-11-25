using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamoDbCrud.LowLevel;

public class LowLevelCrud : CrudBase
{

    public string TableName { get; init; }

    public override async Task<string> GetAsync(string name)
    {
        // Create a request for retrieving the specified item
        GetItemRequest request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue> {
                { "Name", new AttributeValue { S = name } }
            }
        };

        // Make the call to DynamoDB
        var data = await Client.GetItemAsync(request);

        // Get the returned item and process it
        if (!data.IsItemSet)
        {
            return $"{name} not found";
        }

        var entry = data.Item;

        //Get the attributes from the result
        string? nameAtt = entry["Name"]?.S;
        string? roleAtt = entry.ContainsKey("Role") ? entry["Role"]?.S : null;
        string? originAtt = entry.ContainsKey("Origin") ? entry["Origin"]?.S : null;
        int? ageAtt = entry.ContainsKey("Age") ? int.Parse(entry["Age"]?.N) : null;

        List<string>? itemsAtt = entry.ContainsKey("Items") ? entry["Items"].SS : new List<string>();

        return $"""
            Name = {nameAtt}
            Role = {roleAtt}
            Origin = {originAtt}
            Age = {ageAtt}
            Items = {string.Join(',', itemsAtt ?? new List<string>())}
            """;
    }

    public override async Task PutAsync(string name, string? origin, string? role, int? age, List<string>? items)
    {

        // Create the new Item for inserting - Hash Key is never null
        var item = new Dictionary<string, AttributeValue> {
            { "Name", new AttributeValue { S = name } },
            { "Role", new AttributeValue { S = role } },
            { "Origin", new AttributeValue { S = origin } }
        };

        // If the age is not null, add it to the request
        if (age.HasValue)
        {
            item.Add("Age", new AttributeValue { N = age.ToString() });
        }

        // If the items are not null, add it to the request
        if (items != null)
        {
            item.Add("Items", new AttributeValue { SS = items.Distinct().ToList() });
        }

        // Create the reqwuest
        PutItemRequest request = new PutItemRequest
        {
            TableName = TableName,
            Item = item
        };

        // Make the call to the low level API
        await Client.PutItemAsync(request);
    }
}

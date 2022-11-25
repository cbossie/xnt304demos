using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MemoryDbAspnet.Model;

namespace MemoryDbAspnet.Service;

public class OrderService : IOrderService
{
    MemoryDbDemoConfig _demoConfig;
    IAmazonDynamoDB _dynamoCli;

    public OrderService(MemoryDbDemoConfig demoConfig, IAmazonDynamoDB dynamoCli)
    {
        _demoConfig = demoConfig;
        _dynamoCli = dynamoCli;
    }
    public async Task<OrderServiceTotalResult> GetOrdersForCustomerId(string character)
    {
        decimal total = 0;
        int orders = 0;

        Dictionary<string, AttributeValue> lastEvaluatedKey = null;
        do
        {
            // Create the scan request
            ScanRequest request = new()
            {
                TableName = _demoConfig.TableName,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":val", new AttributeValue { S = character } }
                },
                FilterExpression = "begins_with(CustomerId,:val)",
            };

            if (lastEvaluatedKey != null)
            {
                request.ExclusiveStartKey = lastEvaluatedKey;
            }

            var response = await _dynamoCli.ScanAsync(request);


            total += response
                .Items
                .Select(a =>
                {
                    var amt = decimal.Parse(a["Total"].N);
                    return amt;
                })
                .Sum();

            orders += response.Count;

            lastEvaluatedKey = response.LastEvaluatedKey;
        } while (lastEvaluatedKey?.Count != 0);

        return new OrderServiceTotalResult() { NumOrders = orders, TotalCost = total };
    }
}

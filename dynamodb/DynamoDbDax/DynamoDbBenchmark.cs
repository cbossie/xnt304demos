using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbDax;

public static class DynamoDbBenchmark
{
	static Stopwatch _stopWatch = new Stopwatch();

	public static async Task<DynamoDbBenchmarkResult> BenchmarkRequestsGet(IAmazonDynamoDB client, TestType testType, string table, string orderid, int numberOfRequests)
	{
		var benchmark = new DynamoDbBenchmarkResult(testType, orderid);


        var request = new GetItemRequest
		{			
			TableName = table,
			Key = new Dictionary<string, AttributeValue>
			{
				{ "Id", new AttributeValue { N = orderid } }
			}
		};

		double ticksStart = _stopWatch.ElapsedTicks;
		double ticksEnd;
		int numToSkip = 10;   // To let query stabilize

		// Loop through and retrieve the same object over and over again
        for (int i = 0; i <= numberOfRequests; i++) 
		{
			
			_stopWatch.Start();
			var data = await client.GetItemAsync(request);
			_stopWatch.Stop();
			ticksEnd = _stopWatch.ElapsedTicks;
		
			double elapsed = (ticksEnd - ticksStart) / Stopwatch.Frequency * 1000d;
			if(i > numToSkip)
			{
                benchmark.AddRequest(elapsed);
            }
            ticksStart = ticksEnd;
		}
		return benchmark;
	}

	public static async Task<DynamoDbBenchmarkResult> BenchmarkRequestsScan(IAmazonDynamoDB client, TestType testType, string table, string character, int numberOfRequests)
	{
        var benchmark = new DynamoDbBenchmarkResult(testType, character);

        decimal total = 0;
        int orders = 0;

        double ticksStart = _stopWatch.ElapsedTicks;
        double ticksEnd;

        for (int i = 0; i <= numberOfRequests; i++)
        {
            Dictionary<string, AttributeValue> lastEvaluatedKey = null;
            _stopWatch.Start();
            do
            {
                // Create the scan request
                ScanRequest request = new()
                {
                    TableName = table,
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

                var response = await client.ScanAsync(request);


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
            _stopWatch.Stop();
            ticksEnd = _stopWatch.ElapsedTicks;

            double elapsed = (ticksEnd - ticksStart) / Stopwatch.Frequency * 1000d;
            benchmark.AddRequest(elapsed);
            Console.Write(".");
            ticksStart = ticksEnd;
        }

        return benchmark;
    }


}



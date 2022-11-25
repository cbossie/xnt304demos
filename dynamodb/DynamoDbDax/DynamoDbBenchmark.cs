using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
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

	public static async Task<DynamoDbBenchmarkResult> BenchmarkRequests(IAmazonDynamoDB client, TestType testType, string table, int orderId, int numberOfRequests)
	{
		var benchmark = new DynamoDbBenchmarkResult(testType, orderId);


        var request = new GetItemRequest
		{
			TableName = table,
			Key = new Dictionary<string, AttributeValue>
			{
				{ "Id", new AttributeValue { N = $"{orderId}" } }
			}
		};
		// Loop through and retrieve the same object over and over again
        for (int i = 0; i <= numberOfRequests; i++) 
		{
			_stopWatch.Restart();
			var data = await client.GetItemAsync(request);
			long time = _stopWatch.ElapsedMilliseconds;
			benchmark.AddRequest(time);
			Console.WriteLine($"Request Time: {time}ms");
			
		}
		return benchmark;
	}










}



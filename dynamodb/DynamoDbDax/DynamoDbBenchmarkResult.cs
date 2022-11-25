using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbDax;
public class DynamoDbBenchmarkResult
{
    public TestType BenchmarkType { get; init; }
    public int OrderId { get; init; }
    public int NumberOfRequests { get; set; }
    public double FirstRequestLength { get; set; }
    private double TotalTime { get; set; }
    public double AverageRequestLength => TotalTime / NumberOfRequests;

    public DynamoDbBenchmarkResult(TestType type, int orderId)
    {
        BenchmarkType = type;
        OrderId = orderId;
    }


    public void AddRequest(double time)
    {
        if (NumberOfRequests == 0)
        {
            FirstRequestLength = time;
        }
        TotalTime += time;
        NumberOfRequests++;
    }

    public override string ToString()
    {
        return $"""
            Test Type:      {BenchmarkType}
            Order Id:       {OrderId}
            Iterations:     {NumberOfRequests}
            First Length:   {FirstRequestLength}
            Average Length: {AverageRequestLength}
            """;
    }


}


public enum TestType {  Dax, DynamoDB }

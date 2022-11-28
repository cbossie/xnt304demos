using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbDax;
public class DynamoDbBenchmarkResult
{
    public TestType BenchmarkType { get; init; }
    public string Id { get; init; }
    public int NumberOfRequests { get; set; } = -1;
    public double FirstRequestLength { get; set; }
    private double TotalTime { get; set; }

    public double MinTime { get; set; } = double.MaxValue;
    public double MaxTime { get; set; } = double.MinValue;
    
    public double AverageRequestLength => TotalTime / NumberOfRequests;

    public DynamoDbBenchmarkResult(TestType type, string id)
    {
        BenchmarkType = type;
        Id = id;
    }


    public void AddRequest(double time)
    {
        if (NumberOfRequests == -1)
        {
            FirstRequestLength = time;
        }
        else
        {
            if(time < MinTime)
            {
                MinTime = time;
            }
            if(time > MaxTime)
            {
                MaxTime = time;
            }

            TotalTime += time;
        }
        NumberOfRequests++;

    }

    public override string ToString()
    {
        return $"""
            --------------------------------------------
            Test Type:      {BenchmarkType}
            --------------------------------------------
            Id:               {Id}
            Iterations:       {NumberOfRequests}
            First Duration:   {FirstRequestLength}
            Average Duration: {AverageRequestLength}
            Min Time:         {MinTime}
            Max Time:         {MaxTime}
            --------------------------------------------
            """;
    }


}


public enum TestType {  DaxGet, DynamoDBGet, DaxScan, DynamoDbScan }

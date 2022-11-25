using System.Diagnostics.CodeAnalysis;

namespace DynamoDBSeed.Model;

public class SeedConfig
{
    public int StartingOrderId { get; set; }
    public int EndingOrderId { get; set; }
    public int MaxItems { get; set; }
    public int CurrentYear { get; set; }
    [NotNull]
    public string CustomerFile { get; set; }
    [NotNull]
    public string ProductFile { get; set; }
    [NotNull]
    public string StateFile { get; set; }
}

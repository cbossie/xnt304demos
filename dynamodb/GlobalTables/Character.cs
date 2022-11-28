using Amazon.DynamoDBv2.DataModel;

namespace GlobalTables.Model;

[DynamoDBTable("Characters")]
public class Character
{
    [DynamoDBProperty("Name")]
    [DynamoDBHashKey]
    public string Name { get; set; }

    [DynamoDBProperty("Role")]
    public string? Role { get; set; }

    [DynamoDBProperty("Origin")]
    public string? Origin { get; set; }

    [DynamoDBProperty("Age")]
    public int? Age { get; set; }

    [DynamoDBProperty("Items")]
    public List<string>? Items { get; set; } = new List<string>();

    public string SerialNumber { get; set; }

    public override string ToString()
    {
        return $"""
            Name = {Name}
            Role = {Role}
            Origin = {Origin}
            Items = {string.Join(',', Items ?? new List<string>())}
            S/N   = {SerialNumber}
            """;
    }

}
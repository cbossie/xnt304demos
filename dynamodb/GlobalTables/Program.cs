using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GlobalTables.Model;


int msSteps = 1;
int elapsed = 0;
string oldSerial = Guid.NewGuid().ToString();
string newSerial = Guid.NewGuid().ToString();

// Create Client from each
IAmazonDynamoDB useast1Client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
DynamoDBContext useast1Context = new DynamoDBContext(useast1Client);
IAmazonDynamoDB euwest1Client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
DynamoDBContext euwest1Context = new DynamoDBContext(euwest1Client);

// Create an item
Character chr = new Character
{
    Name = "Global Avenger",
    Role = "Superhero",
    Origin = "Krypton",
    Items = new List<string> { "Wand", "Battleax" },
    SerialNumber = oldSerial
};

Console.WriteLine("Saving the following Character:");
Console.WriteLine(chr);

await euwest1Context.SaveAsync(chr);
chr.SerialNumber = newSerial;

Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
Console.WriteLine($"Will update the serial number, then query the global table every 10 milliseconds until the serial number matches the new value {newSerial}");

Console.WriteLine("Beginning Process");
Console.WriteLine($"Old Serial Number = {oldSerial}");
Character charEurope;
await euwest1Context.SaveAsync(chr);
do
{    
    await Task.Delay(msSteps);
    charEurope = await useast1Context.LoadAsync<Character>(chr.Name);

    elapsed += msSteps;

} while (charEurope.SerialNumber != newSerial);

Console.WriteLine($"After {elapsed} ms, the new serial number matches.");



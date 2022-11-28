using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GlobalTables.Model;


int msSteps = 10;
int elapsed = 0;
string oldSerial = Guid.NewGuid().ToString();
string newSerial = Guid.NewGuid().ToString();
string newSerial2 = Guid.NewGuid().ToString();

string id = $"Batman - Clone number {DateTime.Now.Millisecond}";

// Create Client from each
IAmazonDynamoDB useast1Client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.USEast1);
DynamoDBContext useast1Context = new DynamoDBContext(useast1Client);
IAmazonDynamoDB euwest1Client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest1);
DynamoDBContext euwest1Context = new DynamoDBContext(euwest1Client);

// Create an item
Character chr = new Character
{
    Name = id,
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
Console.WriteLine();
Console.WriteLine("Beginning CREATE Process");
Console.WriteLine($"Old Serial Number = {oldSerial}");
Character charEurope;
await euwest1Context.SaveAsync(chr);
do
{    
    await Task.Delay(msSteps);
    charEurope = await useast1Context.LoadAsync<Character>(chr.Name);

    elapsed += msSteps;

} while (charEurope == null || charEurope.SerialNumber != newSerial);

Console.WriteLine($"After {elapsed} ms, the item was created in the other region.");

//UPDATE PROCESS

Console.WriteLine("----------------------------------------------------");
Console.WriteLine();
Console.WriteLine("Beginning UPDATE Process");
Console.WriteLine($"Old Serial Number = {chr.SerialNumber}");
chr.SerialNumber = newSerial2;
elapsed = 0;
await euwest1Context.SaveAsync(chr);
do
{
    await Task.Delay(msSteps);
    charEurope = await useast1Context.LoadAsync<Character>(chr.Name);

    elapsed += msSteps;

} while (charEurope == null || charEurope.SerialNumber != newSerial2);
Console.WriteLine($"After {elapsed} ms, the item was updated in the other region");
Console.WriteLine("----------------------------------------------------");
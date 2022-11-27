using Amazon.QLDB.Driver;
using Amazon.QLDB.Driver.Generic;
using Amazon.QLDB.Driver.Serialization;
using Amazon.QLDBSession.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

var chain = new CredentialProfileStoreChain();

const string NAME_PREFIX = "XNT";
string startingVenue = "Venetian";
string newVenue = "Aria";


// Hard Code Everything
Console.WriteLine("Create the async QLDB driver");
IAsyncQldbDriver driver = AsyncQldbDriver.Builder()
    .WithLedger("reinvent-qldb")
    .WithSerializer(new ObjectSerializer())
    .Build();

// Session Name to use In this Run
string randomName = $"{NAME_PREFIX}-{DateTime.Now.Millisecond}";

/**
    Creating the table if it does not exist
**/
try
{
    Console.WriteLine("Confirming our table is all set up for re:Invent");
    await driver.Execute(async txn =>
    {
        await txn.Execute("CREATE TABLE SessionTable");
        await txn.Execute("CREATE INDEX ON SessionTable(sessionName)");
    });
}
catch (BadRequestException ex)
{
    // Normally I would do something with this, but for demo purposes, we are going to show nothing
}

/**
    Insert a document into our table

    INSERT

**/
Console.WriteLine("Inserting a re:Invent Session");
Session mySession = new Session {
    SessionName = randomName,
    Venue  = startingVenue,
    Duration = 15
};

await driver.Execute(async txn =>
{
    IQuery<Session> myQuery = txn.Query<Session>("INSERT INTO SessionTable ?", mySession);
    await txn.Execute(myQuery);
});


/**
    Retrieve the data we just got from the table

    QUERY

**/
Console.WriteLine();
Console.WriteLine("Querying the table...");

// The result from driver.Execute() is buffered into memory because once the
// transaction is committed, streaming the result is no longer possible.
IAsyncResult<Session> selectResult = await driver.Execute(async txn =>
{
    IQuery<Session> myQuery = txn.Query<Session>("SELECT * FROM SessionTable WHERE SessionName = ?", randomName);
    return await txn.Execute(myQuery);
});

Console.WriteLine("------------");
Console.WriteLine("Original Session:");
await foreach (Session Session in selectResult)
{
    Console.WriteLine(Session);
}
Console.WriteLine("------------");
Console.WriteLine();

/**
    Update our document

    UPDATE

**/
Console.WriteLine();
Console.WriteLine($"Updating the session document for {randomName}");
Console.WriteLine();

await driver.Execute(async txn =>
{
    IQuery<Session> myQuery = txn.Query<Session>("UPDATE SessionTable SET Duration = ?, Venue = ? WHERE SessionName = ?", DateTime.Now.Second, newVenue, randomName);
    await txn.Execute(myQuery);
});


// See updated result
Console.WriteLine();
Console.WriteLine("Querying the table for the updated session info...");
IAsyncResult<Session> updateResult = await driver.Execute(async txn =>
{
    IQuery<Session> myQuery = txn.Query<Session>("SELECT * FROM SessionTable WHERE SessionName = ?", randomName);
    return await txn.Execute(myQuery);
});
Console.WriteLine("------------");
Console.WriteLine("Updated Session:");
await foreach (Session Session in updateResult)
{
    Console.WriteLine(Session);
}
Console.WriteLine("------------");

/**
    Query a list of all entries that start with our prefix
**/
Console.WriteLine();
Console.WriteLine();
Console.WriteLine($"Finding all sessions in the DB where the session ID starts with {NAME_PREFIX}");
Console.WriteLine("------------");
IAsyncResult<Session> selectAllResult = await driver.Execute(async txn =>
{
    IQuery<Session> myQuery = txn.Query<Session>($"SELECT * FROM SessionTable WHERE SessionName LIKE '{NAME_PREFIX}%'");
    return await txn.Execute(myQuery);
});

await foreach (Session Session in selectAllResult)
{
    Console.WriteLine(Session);
}

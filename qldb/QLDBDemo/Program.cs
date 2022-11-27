using Amazon.QLDB.Driver;
using Amazon.QLDB.Driver.Generic;
using Amazon.QLDB.Driver.Serialization;
using Amazon.QLDBSession.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

var chain = new CredentialProfileStoreChain();

if(!chain.TryGetAWSCredentials("reinvent", out AWSCredentials creds))
{
    throw new Exception("Cannot get credentials");
}

// Hard Code Everything
Console.WriteLine("Create the async QLDB driver");
IAsyncQldbDriver driver = AsyncQldbDriver.Builder()
    .WithAWSCredentials(creds)
    .WithLedger("reinvent-qldb")
    .WithSerializer(new ObjectSerializer())
    .Build();

// Name to use in this run
string randomName = $"John-{Guid.NewGuid()}";

/**
    Creating the table if it does not exist
**/
try
{
    Console.WriteLine("Creating the table and index");

    // Creates the table and the index in the same transaction.
    Console.WriteLine("Create Table");
    await driver.Execute(async txn =>
    {
        await txn.Execute("CREATE TABLE Person");
        await txn.Execute("CREATE INDEX ON Person(firstName)");
    });
}
catch (BadRequestException ex)
{
    Console.WriteLine("Bad QLDB Request (table is already there!!)");
    Console.WriteLine(ex.Message);
}

/**
    Insert a document into our table
**/
Console.WriteLine("Inserting a document");
Person myPerson = new Person {
    FirstName = randomName,
    LastName = "Doe",
    Age = 1
};

await driver.Execute(async txn =>
{
    IQuery<Person> myQuery = txn.Query<Person>("INSERT INTO Person ?", myPerson);
    await txn.Execute(myQuery);
});


/**
    Retrieve the data we just got from the table
**/
Console.WriteLine("Querying the table");

// The result from driver.Execute() is buffered into memory because once the
// transaction is committed, streaming the result is no longer possible.
IAsyncResult<Person> selectResult = await driver.Execute(async txn =>
{
    IQuery<Person> myQuery = txn.Query<Person>("SELECT * FROM Person WHERE FirstName = ?", randomName);
    return await txn.Execute(myQuery);
});

await foreach (Person person in selectResult)
{
    Console.WriteLine(person);
}


/**
    Update our document
**/
Console.WriteLine("Updating the document");
await driver.Execute(async txn =>
{
    IQuery<Person> myQuery = txn.Query<Person>("UPDATE Person SET Age = ? WHERE FirstName = ?", DateTime.Now.Millisecond, randomName);
    await txn.Execute(myQuery);
});


// See updated result
Console.WriteLine("Querying the table for the updated document");

IAsyncResult<Person> updateResult = await driver.Execute(async txn =>
{
    IQuery<Person> myQuery = txn.Query<Person>("SELECT * FROM Person WHERE FirstName = ?", randomName);
    return await txn.Execute(myQuery);
});

await foreach (Person person in updateResult)
{
    Console.WriteLine(person);
}


// Get List of all Johns
Console.WriteLine("Querying People whose names start with John");

IAsyncResult<Person> selectAllResult = await driver.Execute(async txn =>
{
    IQuery<Person> myQuery = txn.Query<Person>("SELECT * FROM Person WHERE FirstName LIKE 'John%'");
    return await txn.Execute(myQuery);
});

await foreach (Person person in selectAllResult)
{
    Console.WriteLine(person);
}

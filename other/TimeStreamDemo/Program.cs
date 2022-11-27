using Amazon.TimestreamWrite;
using Amazon;
using Amazon.TimestreamQuery;
using System.Threading.Tasks;
using System;
using CommandLine;





Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
    {
        MainAsync(o.KmsKey, o.CsvFile).GetAwaiter().GetResult();
    });


static async Task MainAsync(string kmsKeyId, string csvFilePath)
{

    // Create the TimeStream  Client Configuration and RW clients
    var writeClientConfig = new AmazonTimestreamWriteConfig
    {
        Timeout = TimeSpan.FromSeconds(20),
        MaxErrorRetry = 10,         
    };
    var writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
    var queryClient = new AmazonTimestreamQueryClient();



    // Create Example Classes for CRUD operations
    var crudAndSimpleIngestionExample = new CrudAndSimpleIngestionExample(writeClient);
    var queryExample = new QueryExample(queryClient);


    // CRUD simple ingestion
    await crudAndSimpleIngestionExample.CreateTable();
    await crudAndSimpleIngestionExample.DescribeTable();
    await crudAndSimpleIngestionExample.ListTables();
    await crudAndSimpleIngestionExample.UpdateTable();

    // Simple records ingestion
    await crudAndSimpleIngestionExample.WriteRecords();
    await crudAndSimpleIngestionExample.WriteRecordsWithCommonAttributes();

    // upsert records
    await crudAndSimpleIngestionExample.WriteRecordsWithUpsert();

    await queryExample.RunAllQueries();

    //// Try cancelling query
    //await queryExample.CancelQuery();

    // Run query with multiple pages
    await queryExample.RunQueryWithMultiplePages(20000);
}


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
    // Recommended Timestream write client SDK configuration:
    // - Set SDK retry count to 10
    // - Set RequestTimeout to 20 seconds
    var writeClientConfig = new AmazonTimestreamWriteConfig
    {
        Timeout = TimeSpan.FromSeconds(20),
        MaxErrorRetry = 10,         
    };

    var writeClient = new AmazonTimestreamWriteClient(writeClientConfig);
    var crudAndSimpleIngestionExample = new CrudAndSimpleIngestionExample(writeClient);
    var csvIngestionExample = new CsvIngestionExample(writeClient);

    var queryClient = new AmazonTimestreamQueryClient();
    var queryExample = new QueryExample(queryClient);

    //await crudAndSimpleIngestionExample.CreateDatabase();
    //await crudAndSimpleIngestionExample.DescribeDatabase();
    //await crudAndSimpleIngestionExample.ListDatabases();
    //if (kmsKeyId != null)
    //{
    //    await crudAndSimpleIngestionExample.UpdateDatabase(kmsKeyId);
    //}

    await crudAndSimpleIngestionExample.CreateTable();
    await crudAndSimpleIngestionExample.DescribeTable();
    await crudAndSimpleIngestionExample.ListTables();
    await crudAndSimpleIngestionExample.UpdateTable();

    // Simple records ingestion
    await crudAndSimpleIngestionExample.WriteRecords();
    await crudAndSimpleIngestionExample.WriteRecordsWithCommonAttributes();

    // upsert records
    await crudAndSimpleIngestionExample.WriteRecordsWithUpsert();

    if (csvFilePath != null)
    {
        // Bulk record ingestion for bootstrapping a table with fresh data
        await csvIngestionExample.BulkWriteRecords(csvFilePath);
    }

    await queryExample.RunAllQueries();

    // Try cancelling query
    await queryExample.CancelQuery();

    // Run query with multiple pages
    await queryExample.RunQueryWithMultiplePages(20000);
}


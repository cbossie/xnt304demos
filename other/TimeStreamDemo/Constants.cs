using CommandLine;

static class Constants
{
    public const string DATABASE_NAME = "reinvent";
    public const string TABLE_NAME = "reinventTable";
    public const long HT_TTL_HOURS = 24;
    public const long CT_TTL_DAYS = 7;
}


      public class Options
        {
            [Option('k', "kms-key", Required = false, HelpText = "Kms Key Id for UpdateDatabase.")]
            public string KmsKey { get; set; }

            [Option('f', "csv-file", Required = false, HelpText = "Csv file path for sample data.")]
            public string CsvFile { get; set; }
        }
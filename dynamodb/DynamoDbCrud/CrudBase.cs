using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbCrud;

public abstract class CrudBase : IDynamoDBCrud
{
    public IAmazonDynamoDB Client { get; init; }

    public abstract Task PutAsync(string name, string origin, string role, int? age, List<string> items);
    public abstract Task<string> GetAsync(string name);
}

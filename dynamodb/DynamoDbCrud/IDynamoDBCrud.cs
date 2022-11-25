using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbCrud;

public interface IDynamoDBCrud
{
    IAmazonDynamoDB Client { get; }

    Task PutAsync(string name, string? origin, string? role, int? age, List<string>? items);

    Task<string> GetAsync(string name);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbDax.Model
{
    public class DaxConfig
    {
        public string TableName { get; set; }
        public string DaxEndpoint { get; set; }
        public int StartingOrderId { get; set; }
        public int EndingOrderId { get; set; }
        public int Iterations { get; set; }
    }
}

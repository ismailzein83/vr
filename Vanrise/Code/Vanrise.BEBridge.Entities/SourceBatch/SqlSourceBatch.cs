using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class SqlSourceBatch : SourceBEBatch
    {
        public override string BatchName
        {
            get { return TableName; }
        }
        public string TableName { get; set; }

        public DataTable Data { get; set; }
    }
}

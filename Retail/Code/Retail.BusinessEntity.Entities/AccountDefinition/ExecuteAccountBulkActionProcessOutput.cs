using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ExecuteAccountBulkActionProcessOutput
    {
        public long? ProcessInstanceId { get; set; }
        public bool Succeed { get; set; }
        public string OutputMessage { get; set; }
    }
}

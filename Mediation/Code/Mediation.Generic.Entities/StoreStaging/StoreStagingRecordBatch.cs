using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class StoreStagingRecordBatch
    {
        public StoreStagingRecordBatch()
        {
            this.StoreStagingRecords = new List<StoreStagingRecord>();
        }
        public List<StoreStagingRecord> StoreStagingRecords { get; set; }
    }
}

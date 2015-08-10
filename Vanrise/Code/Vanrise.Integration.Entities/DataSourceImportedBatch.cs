using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceImportedBatch
    {
        public long ID { get; set; }
        public string BatchName { get; set; }
        public decimal BatchSize { get; set; }
        public int RecordsCount { get; set; }
        public MappingResult MappingResult { get; set; }
        public string MapperMessage { get; set; }
        public string QueueItemIds { get; set; }
        public DateTime LogEntryTime { get; set; }
        public ItemExecutionFlowStatus ExecutionStatus { get; set; }
    }
}

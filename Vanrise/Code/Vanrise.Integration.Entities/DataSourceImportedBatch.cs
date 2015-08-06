using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceImportedBatch
    {
        public long ID { get; set; }
        public string BatchName { get; set; }
        public decimal BatchSize { get; set; }
        public int RecordsCount { get; set; }
        public MappingResultType MappingResult { get; set; }
        public string MapperMessage { get; set; }
        public DateTime LogEntryTime { get; set; }
    }
}

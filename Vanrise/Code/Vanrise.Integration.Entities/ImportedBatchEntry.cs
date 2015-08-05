using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class ImportedBatchEntry
    {
        public long ID { get; set; }

        public string BatchDescription { get; set; }
        
        public decimal? BatchSize { get; set; }

        public int RecordsCount { get; set; }
        
        public MappingResult Result { get; set; }

        public string MapperMessage { get; set; }

        public string QueueItemsIds { get; set; }

    }
}

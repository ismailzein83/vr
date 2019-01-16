using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class RecordBatch
    {
        public List<dynamic> Records { get; set; }
    }

    public class DeleteRecordsBatch
    {
        public DateTimeRange DateTimeRange { get; set; }

        public HashSet<long> IdsToDelete { get; set; }
    }
}
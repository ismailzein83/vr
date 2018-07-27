using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class RecordBatch
    {
        public List<dynamic> Records { get; set; }
    }

    public class DeleteRecordsBatch
    {
        public List<object> InputIdsToDelete { get; set; }

        public DateTimeRange DateTimeRange { get; set; }
    }
}

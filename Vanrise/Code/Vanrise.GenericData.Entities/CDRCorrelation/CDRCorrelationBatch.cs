using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class CDRCorrelationBatch
    {
        public List<dynamic> OutputRecordsToInsert { get; set; }

        public List<object> InputIdsToDelete { get; set; }

        public DateTimeRange DateTimeRange { get; set; }

        public CDRCorrelationBatch()
        {
            OutputRecordsToInsert = new List<dynamic>();
            InputIdsToDelete = new List<object>();
        }
    }
}
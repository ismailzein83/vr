using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TestCallAnalysis.Entities
{
    public class CDRCorrelationBatch
    {
        public List<dynamic> OutputRecordsToInsert { get; set; }

        public DateTimeRange DateTimeRange { get; set; }

        public CDRCorrelationBatch()
        {
            OutputRecordsToInsert = new List<dynamic>();
            DateTimeRange = new DateTimeRange();
        }
    }
}

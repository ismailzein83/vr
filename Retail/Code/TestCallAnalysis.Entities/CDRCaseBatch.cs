using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TestCallAnalysis.Entities
{
    public class CDRCaseBatch
    {
        public List<TCAnalCorrelatedCDR> OutputRecordsToInsert { get; set; }

        public DateTimeRange DateTimeRange { get; set; }

        public CDRCaseBatch()
        {
            OutputRecordsToInsert = new List<TCAnalCorrelatedCDR>();
            DateTimeRange = new DateTimeRange();
        }
    }
}

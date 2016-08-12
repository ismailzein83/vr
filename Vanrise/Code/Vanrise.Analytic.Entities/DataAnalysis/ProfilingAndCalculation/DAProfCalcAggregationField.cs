using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAggregationField
    {
        public RecordFilterGroup RecordFilter { get; set; }

        public TimeRangeFilter TimeRangeFilter { get; set; }

        public DARecordAggregate RecordAggregate { get; set; }
    }
}

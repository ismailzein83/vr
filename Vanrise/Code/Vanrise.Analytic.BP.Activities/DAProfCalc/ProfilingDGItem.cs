using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public class ProfilingDGItem : IDataGroupingItem
    {
        public string GroupingKey { get; set; }

        public List<DARecordAggregateState> AggregateStates { get; set; }

        public Dictionary<string, dynamic> GroupingValues { get; set; }
    }
}

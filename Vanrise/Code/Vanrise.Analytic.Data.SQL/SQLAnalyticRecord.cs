using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Data.SQL
{
    public class SQLRecord
    {
        public DateTime? Time { get; set; }

        public Dictionary<string, SQLRecordGroupingValue> GroupingValuesByDimensionName { get; set; }

        public Dictionary<string, SQLRecordAggValue> AggValuesByAggName { get; set; }
    }

    public class SQLRecordGroupingValue
    {
        //public string DimensionName { get; set; }

        public dynamic Value { get; set; }
    }

    public class SQLRecordAggValue
    {
        //public string AggName { get; set; }

        public dynamic Value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticRecord
    {
        public DateTime? Time { get; set; }

        public DimensionValue[] DimensionValues { get; set; }

        public MeasureValues MeasureValues { get; set; }
    }

    public class DBAnalyticRecord
    {
        public DateTime? Time { get; set; }

        public Dictionary<string, DBAnalyticRecordGroupingValue> GroupingValuesByDimensionName { get; set; }

        public Dictionary<string, DBAnalyticRecordAggValue> AggValuesByAggName { get; set; }
    }

    public class DBAnalyticRecordGroupingValue
    {
        //public string DimensionName { get; set; }

        public dynamic Value { get; set; }
    }

    public class DBAnalyticRecordAggValue : ICloneable
    {
        //public string AggName { get; set; }

        public dynamic Value { get; set; }

        public object Clone()
        {
            return new DBAnalyticRecordAggValue
            {
                Value = this.Value
            };
        }
    }

    public interface IAnalyticTableQueryContext
    {
        AnalyticTable GetTable();

        AnalyticDimension GetDimensionConfig(string dimensionName);

        AnalyticAggregate GetAggregateConfig(string aggregateName);

        AnalyticMeasure GetMeasureConfig(string measureName);

        AnalyticJoin GetJoinContig(string joinName);

        IEnumerable<string> GetDimensionNames(RecordFilterGroup filterGroup);
    }
}

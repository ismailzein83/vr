﻿using System;
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

        public List<AnalyticRecordSubTable> SubTables { get; set; }
    }

    public class AnalyticRecordSubTable
    {
        public List<MeasureValues> MeasureValues { get; set; }
    }

    public class AnalyticFullResult
    {
        public List<AnalyticRecord> AllRecords { get; set; }

        public List<AnalyticResultSubTable> SubTables { get; set; }
    }

    public class AnalyticResultSubTable
    {
        public List<DimensionValue[]> DimensionValues { get; set; }
    }

    public class DBAnalyticRecord
    {
        public DateTime? Time { get; set; }

        public Dictionary<string, DBAnalyticRecordGroupingValue> GroupingValuesByDimensionName { get; set; }

        public Dictionary<string, DBAnalyticRecordAggValue> AggValuesByAggName { get; set; }
    }

    public class DBAnalyticRecordGroupingValue : ICloneable
    {
        //public string DimensionName { get; set; }

        public dynamic Value { get; set; }

        public List<dynamic> AllValues { get; set; }

        public object Clone()
        {
            return new DBAnalyticRecordGroupingValue
            {
                Value = this.Value
            };
        }
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
        AnalyticQuery Query { get; }
        AnalyticTable GetTable();

        Dictionary<string, AnalyticDimension> Dimensions { get; }

        Dictionary<string, AnalyticAggregate> Aggregates { get; }

        Dictionary<string, AnalyticMeasure> Measures { get; }

        Dictionary<string, AnalyticJoin> Joins { get; }

        Dictionary<string, AnalyticMeasureExternalSource> MeasureExternalSources { get; }

        AnalyticDimension GetDimensionConfig(string dimensionName);

        AnalyticAggregate GetAggregateConfig(string aggregateName);

        AnalyticMeasure GetMeasureConfig(string measureName);

        AnalyticJoin GetJoinContig(string joinName);

        List<string> GetDimensionNamesFromQueryFilters();

        AnalyticMeasureExternalSourceResult GetMeasureExternalSourceResult(string sourceName);

        DateTime FromTime { get; }

        DateTime ToTime { get; }

        TimeGroupingUnit? TimeGroupingUnit { get; }

        int? TopRecords { get; }

        T GetOrCreateCachedObjectBasedOnItemConfig<T>(object cacheName, Func<T> createObject);
    }
}

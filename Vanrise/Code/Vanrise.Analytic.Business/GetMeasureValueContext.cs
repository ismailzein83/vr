using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class GetMeasureValueContext : IGetMeasureValueContext
    {
        DBAnalyticRecord _sqlRecord;
        HashSet<string> _allDimensions;

        public GetMeasureValueContext(DBAnalyticRecord sqlRecord, HashSet<string> allDimensions)
        {
            if (sqlRecord == null)
                throw new ArgumentNullException("sqlRecord");
            if (allDimensions == null)
                throw new ArgumentNullException("allDimensions");
            _sqlRecord = sqlRecord;
            _allDimensions = allDimensions;
        }
        public dynamic GetAggregateValue(string aggregateName)
        {
            DBAnalyticRecordAggValue aggValue;
            if (!_sqlRecord.AggValuesByAggName.TryGetValue(aggregateName, out aggValue))
                throw new NullReferenceException(String.Format("aggValue. AggName '{0}'", aggregateName));
            return aggValue.Value;
        }

        public bool IsGroupingDimensionIncluded(string dimensionName)
        {
            return _allDimensions.Contains(dimensionName);
        }

        public List<dynamic> GetAllDimensionValues(string dimensionName)
        {
            DBAnalyticRecordGroupingValue groupingValue;
            if(!_sqlRecord.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimensionName '{0}'", dimensionName));
            var allValues = groupingValue.AllValues;
            if (allValues == null)
                throw new NullReferenceException("allValues");
            return allValues;
        }

        public List<dynamic> GetDistinctDimensionValues(string dimensionName)
        {
            return GetAllDimensionValues(dimensionName).Distinct().ToList();
        }
    }
}

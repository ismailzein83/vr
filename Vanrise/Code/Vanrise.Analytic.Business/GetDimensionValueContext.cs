using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class GetDimensionValueContext : IGetDimensionValueContext
    {
        DBAnalyticRecord _record;
        public GetDimensionValueContext(DBAnalyticRecord record)
        {
            if (record == null)
                throw new NullReferenceException("record");
            _record = record;
        }
        public dynamic GetDimensionValue(string dimensionName)
        {
            DBAnalyticRecordGroupingValue groupingValue;
            if (!_record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
            return groupingValue.Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.SQL
{
    public class GetDimensionValueContext : IGetDimensionValueContext
    {
        SQLRecord _record;
        public GetDimensionValueContext(SQLRecord record)
        {
            if (record == null)
                throw new NullReferenceException("record");
            _record = record;
        }
        public dynamic GetDimensionValue(string dimensionName)
        {
            SQLRecordGroupingValue groupingValue;
            if (!_record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
            return groupingValue.Value;
        }
    }
}

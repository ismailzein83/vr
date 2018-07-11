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
        IAnalyticTableQueryContext _analyticTableQueryContext;
        public GetDimensionValueContext(IAnalyticTableQueryContext analyticTableQueryContext, DBAnalyticRecord record)
        {
            if (analyticTableQueryContext == null)
                throw new ArgumentNullException("analyticTableQueryContext");
            if (record == null)
                throw new NullReferenceException("record");
            _record = record;
            _analyticTableQueryContext = analyticTableQueryContext;
        }
        public dynamic GetDimensionValue(string dimensionName)
        {
            DBAnalyticRecordGroupingValue groupingValue;
            if (!_record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
            return groupingValue.Value;
        }
        
        public DateTime GetQueryFromTime()
        {
            return _analyticTableQueryContext.FromTime;
        }

        public DateTime GetQueryToTime()
        {
            return _analyticTableQueryContext.ToTime;
        }
    }
}

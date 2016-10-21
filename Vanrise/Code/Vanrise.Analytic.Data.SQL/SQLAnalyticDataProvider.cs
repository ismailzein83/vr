using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.SQL
{
    public class SQLAnalyticDataProvider : AnalyticDataProvider
    {
        public override Guid ConfigId
        {
            get { return new Guid("3CBA3F20-6535-4EBF-9704-DF65AC605671"); }
        }

        public override Entities.IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext)
        {
            return new AnalyticDataManager() { AnalyticTableQueryContext = queryContext };
        }
    }
}

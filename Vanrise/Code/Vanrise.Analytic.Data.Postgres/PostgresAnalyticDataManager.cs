using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data.Postgres
{
    public class PostgresAnalyticDataManager : IAnalyticDataManager
    {
        IAnalyticTableQueryContext _queryContext;

        public PostgresAnalyticDataManager(IAnalyticTableQueryContext queryContext)
        {
            if (queryContext == null)
                throw new ArgumentNullException("queryContext");
            _queryContext = queryContext;
        }
        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, out HashSet<string> includedSQLDimensions)
        {
            throw new NotImplementedException();
        }
    }
}

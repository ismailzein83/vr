using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBatchQuery : BaseRDBNoDataQuery
    {
        List<BaseRDBNoDataQuery> _queries = new List<BaseRDBNoDataQuery>();
        public override RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var subQuery in _queries)
            {
                var subQueryContext = new RDBNoDataQueryGetResolvedQueryContext(context, true);
                RDBResolvedNoDataQuery resolvedSubQuery = subQuery.GetResolvedQuery(subQueryContext);
                queryBuilder.AppendLine(resolvedSubQuery.QueryText);
                queryBuilder.AppendLine();
            }

            return new RDBResolvedNoDataQuery
                {
                    QueryText = queryBuilder.ToString()
                };
        }

        public RDBBatchQuery AddQuery(BaseRDBNoDataQuery query)
        {
            this._queries.Add(query);
            return this;
        }
    }
}

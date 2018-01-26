using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBatchQuery : RDBNoDataQuery
    {
        List<RDBNoDataQuery> _queries = new List<RDBNoDataQuery>();
        public override RDBResolvedNoDataQuery GetResolvedQuery()
        {
            RDBResolvedNoDataQuery resolvedQuery = new RDBResolvedNoDataQuery();
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var subQuery in _queries)
            {
                RDBResolvedNoDataQuery resolvedSubQuery = subQuery.GetResolvedQuery();
                queryBuilder.AppendLine(resolvedSubQuery.QueryText);
                queryBuilder.AppendLine();
                if (resolvedSubQuery.ParameterValues != null)
                {
                    foreach (var prmEntry in resolvedSubQuery.ParameterValues)
                    {
                        if (resolvedQuery.ParameterValues.ContainsKey(prmEntry.Key))
                            throw new Exception(String.Format("Parameter '{0}' already added by another subquery", prmEntry.Key));
                        resolvedQuery.ParameterValues.Add(prmEntry.Key, prmEntry.Value);
                    }
                }
            }
            return resolvedQuery;
        }
    }
}

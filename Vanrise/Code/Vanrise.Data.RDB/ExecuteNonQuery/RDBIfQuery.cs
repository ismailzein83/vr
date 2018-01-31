using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBIfQuery : BaseRDBNoDataQuery
    {
        public BaseRDBCondition Condition { get; set; }

        public BaseRDBNoDataQuery TrueQuery { get; set; }

        public BaseRDBNoDataQuery FalseQuery { get; set; }
        
        public override RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(" IF ");
            var conditionContext = new RDBConditionToDBQueryContext(context, true);
            queryBuilder.Append(this.Condition.ToDBQuery(conditionContext));
            var trueQueryContext = new RDBNoDataQueryGetResolvedQueryContext(context, true);
            queryBuilder.Append(this.TrueQuery.GetResolvedQuery(trueQueryContext).QueryText);
            if(FalseQuery != null)
            {
                queryBuilder.Append(" ELSE ");
                var falseQueryContext = new RDBNoDataQueryGetResolvedQueryContext(context, true);
                queryBuilder.Append(this.FalseQuery.GetResolvedQuery(falseQueryContext).QueryText);
            }
            return new RDBResolvedNoDataQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }
    }
}

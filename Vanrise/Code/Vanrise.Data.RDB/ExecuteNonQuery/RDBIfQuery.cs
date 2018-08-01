using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBIfQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBIfQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        internal BaseRDBCondition Condition { get; set; }

        internal string _trueQueryText;

        RDBQueryContext _trueQueryContext;
        RDBQueryContext _falseQueryContext;

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(" IF ");
            var conditionContext = new RDBConditionToDBQueryContext(context, _queryBuilderContext);
            queryBuilder.AppendLine(this.Condition.ToDBQuery(conditionContext));
            queryBuilder.AppendLine(" BEGIN ");
            if (this._trueQueryText != null)
            {
                queryBuilder.AppendLine(this._trueQueryText);
            }
            else
            {
                _trueQueryContext.ThrowIfNull("_trueQueryContext");
                queryBuilder.AppendLine(_trueQueryContext.GetResolvedQuery().QueryText);
            }
            queryBuilder.AppendLine(" END ");
            if (_falseQueryContext != null)
            {
                queryBuilder.AppendLine(" ELSE ");
                queryBuilder.AppendLine(" BEGIN ");
                var falseQueryContext = new RDBQueryGetResolvedQueryContext(context);
                queryBuilder.AppendLine(_falseQueryContext.GetResolvedQuery().QueryText);
                queryBuilder.AppendLine(" END ");
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public RDBConditionContext IfCondition()
        {
            return new RDBConditionContext(_queryBuilderContext, (condition) => this.Condition = condition, null);
        }


        public RDBQueryContext ThenQuery()
        {
            _trueQueryContext = new RDBQueryContext(_queryBuilderContext.CreateChildContext());
            return _trueQueryContext;
        }


        public RDBQueryContext ElseQuery()
        {
            _falseQueryContext = new RDBQueryContext(_queryBuilderContext.CreateChildContext());
            return _falseQueryContext;
        }
    }
}

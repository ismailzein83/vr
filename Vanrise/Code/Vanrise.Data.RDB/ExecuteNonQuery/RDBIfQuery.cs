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

        internal RDBIfQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        internal RDBConditionGroup ConditionGroup { get; set; }

        internal string _trueQueryText;

        RDBQueryContext _trueQueryContext;
        RDBQueryContext _falseQueryContext;

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(" IF ");
            var conditionContext = new RDBConditionToDBQueryContext(context, _queryBuilderContext);
            queryBuilder.AppendLine(this.ConditionGroup.ToDBQuery(conditionContext));
            queryBuilder.AppendLine(" BEGIN ");
            if (this._trueQueryText != null)
            {
                queryBuilder.AppendLine(this._trueQueryText);
            }
            else
            {
                _trueQueryContext.ThrowIfNull("_trueQueryContext");
                queryBuilder.AppendLine(_trueQueryContext.GetResolvedQuery(new RDBQueryGetResolvedQueryContext(context)).QueryText);
            }
            queryBuilder.AppendLine(" END ");
            if (_falseQueryContext != null)
            {
                queryBuilder.AppendLine(" ELSE ");
                queryBuilder.AppendLine(" BEGIN ");
                var falseQueryContext = new RDBQueryGetResolvedQueryContext(context);
                queryBuilder.AppendLine(_falseQueryContext.GetResolvedQuery(new RDBQueryGetResolvedQueryContext(context)).QueryText);
                queryBuilder.AppendLine(" END ");
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        RDBConditionContext _conditionContext;
        public RDBConditionContext IfCondition(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            if (_conditionContext == null)
            {
                ConditionGroup = new RDBConditionGroup(groupOperator);
                _conditionContext = new RDBConditionContext(_queryBuilderContext, ConditionGroup, null);
            }
            else
            {
                if (ConditionGroup.Operator != groupOperator)
                    throw new Exception("IfCondition method is called multipe times with different values of groupOperator");
            }
            return _conditionContext;
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

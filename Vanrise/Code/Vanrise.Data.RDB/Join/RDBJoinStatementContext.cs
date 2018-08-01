using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBJoinStatementContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        RDBJoin _join;
        string _tableAlias;

        RDBConditionGroup _conditionGroup;

        internal RDBJoinStatementContext(RDBQueryBuilderContext queryBuilderContext, RDBJoin join, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _join = join;
            _tableAlias = tableAlias;
        }

        RDBConditionContext _conditionContext;

        public RDBConditionContext On(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            if (_conditionContext == null)
            {
                _conditionGroup = new RDBConditionGroup(groupOperator);
                _join.Condition = _conditionGroup;
                _conditionContext = new RDBConditionContext(_queryBuilderContext, _conditionGroup, this._tableAlias);
            }
            else
            {
                if (_conditionGroup.Operator != groupOperator)
                    throw new Exception("On method is called multipe times with different values of groupOperator");
            }
            return _conditionContext;
        }

        public void JoinType(RDBJoinType joinType)
        {
            _join.JoinType = joinType;
        }
    }
}

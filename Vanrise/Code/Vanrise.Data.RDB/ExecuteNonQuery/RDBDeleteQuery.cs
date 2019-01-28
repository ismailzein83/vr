using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBDeleteQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;

        internal RDBDeleteQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;

        RDBConditionGroup _conditionGroup;

        List<RDBJoin> _joins;

        bool _withNoLock;

        public void FromTable(IRDBTableQuerySource table, bool withNoLock = false)
        {
            this._table = table; 
            _queryBuilderContext.SetMainQueryTable(table);
            this._withNoLock = withNoLock;
        }

        public void FromTable(string tableName, bool withNoLock = false)
        {
            FromTable(new RDBTableDefinitionQuerySource(tableName), withNoLock);
        }

        RDBJoinContext _joinContext;

        public RDBJoinContext Join(string tableAlias)
        {
            if (_joinContext == null)
            {
                this._tableAlias = tableAlias;
                _queryBuilderContext.AddTableAlias(this._table, tableAlias);
                this._joins = new List<RDBJoin>();
                _joinContext = new RDBJoinContext(_queryBuilderContext, this._joins, _tableAlias);
            }
            return _joinContext;
        }

        RDBConditionContext _conditionContext;
        public RDBConditionContext Where(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            if (_conditionContext == null)
            {
                _conditionGroup = new RDBConditionGroup(groupOperator);
                _conditionContext = new RDBConditionContext(_queryBuilderContext, _conditionGroup, this._tableAlias);
            }
            else
            {
                if (_conditionGroup.Operator != groupOperator)
                    throw new Exception("Where method is called multipe times with different values of groupOperator");
            }
            return _conditionContext;
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolveDeleteQueryContext = new RDBDataProviderResolveDeleteQueryContext(this._table, this._tableAlias, this._conditionGroup, 
                this._joins, this._withNoLock, context, _queryBuilderContext);
            return context.DataProvider.ResolveDeleteQuery(resolveDeleteQueryContext);
        }
    }
}

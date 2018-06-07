using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBJoinContext<T> : IRDBJoinContextReady<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBJoin> _joins;

        public RDBJoinContext(T parent, RDBQueryBuilderContext queryBuilderContext, List<RDBJoin> joins)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
            _joins = joins;
        }

        public IRDBJoinContextReady<T> Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, BaseRDBCondition condition)
        {
            _queryBuilderContext.AddTableAlias(table, tableAlias);
            _joins.Add(new RDBJoin
            {
                Table = table,
                TableAlias = tableAlias,
                JoinType = joinType,
                Condition = condition
            });
            return this;
        }

        public IRDBJoinContextReady<T> Join(RDBJoinType joinType, string tableName, string tableAlias, BaseRDBCondition condition)
        {
            return Join(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias, condition);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            return Join(joinType, table, tableAlias,
                new RDBCompareCondition
                {
                    Expression1 = new RDBColumnExpression
                    {
                        TableAlias = tableAlias,
                        ColumnName = columnName
                    },
                    Operator = RDBCompareConditionOperator.Eq,
                    Expression2 = new RDBColumnExpression
                    {
                        TableAlias = otherTableAlias,
                        ColumnName = otherTableColumnName
                    }
                });
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(RDBJoinType.Inner, table, tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(new RDBTableDefinitionQuerySource(tableName), tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public RDBConditionContext<IRDBJoinContextReady<T>> Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias)
        {
            return new RDBConditionContext<IRDBJoinContextReady<T>>(
                this,
                _queryBuilderContext,
                (condition) =>
                {
                    _queryBuilderContext.AddTableAlias(table, tableAlias);
                    _joins.Add(new RDBJoin
                    {
                        Table = table,
                        TableAlias = tableAlias,
                        JoinType = joinType,
                        Condition = condition
                    });
                },
                tableAlias);
        }

        public IRDBSelectQuery<RDBConditionContext<IRDBJoinContextReady<T>>> JoinSelect(RDBJoinType joinType, string inlineQueryAlias)
        {
            var selectQuery = new RDBSelectQuery<RDBConditionContext<IRDBJoinContextReady<T>>>(_queryBuilderContext.CreateChildContext());
            var conditionContext = Join(joinType, selectQuery, inlineQueryAlias);
            selectQuery.Parent = conditionContext;
            return selectQuery;
        }

        public RDBConditionContext<IRDBJoinContextReady<T>> Join(RDBJoinType joinType, string tableName, string tableAlias)
        {
            return Join(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias);
        }

        T IRDBJoinContextReady<T>.EndJoin()
        {
            return _parent;
        }
    }

    public interface IRDBJoinContextReady<T>
    {
        IRDBJoinContextReady<T> Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, BaseRDBCondition condition);

        IRDBJoinContextReady<T> Join(RDBJoinType joinType, string tableName, string tableAlias, BaseRDBCondition condition);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName);
        
        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName);
        
        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName);
        
        RDBConditionContext<IRDBJoinContextReady<T>> Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias);
        
        RDBConditionContext<IRDBJoinContextReady<T>> Join(RDBJoinType joinType, string tableName, string tableAlias);

        IRDBSelectQuery<RDBConditionContext<IRDBJoinContextReady<T>>> JoinSelect(RDBJoinType joinType, string inlineQueryAlias);

        T EndJoin();
    }
}

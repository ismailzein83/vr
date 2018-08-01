using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBJoinContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBJoin> _joins;

        public RDBJoinContext(RDBQueryBuilderContext queryBuilderContext, List<RDBJoin> joins)
        {
            _queryBuilderContext = queryBuilderContext;
            _joins = joins;
        }

        public RDBJoinContext Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, BaseRDBCondition condition)
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

        public void Join(RDBJoinType joinType, string tableName, string tableAlias, BaseRDBCondition condition)
        {
             Join(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias, condition);
        }

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
             Join(joinType, table, tableAlias,
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

        public void JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
             JoinOnEqualOtherTableColumn(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public void JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
             JoinOnEqualOtherTableColumn(RDBJoinType.Inner, table, tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public void JoinOnEqualOtherTableColumn(string tableName, string tableAlias, string columnName, string otherTableAlias, string otherTableColumnName)
        {
             JoinOnEqualOtherTableColumn(new RDBTableDefinitionQuerySource(tableName), tableAlias, columnName, otherTableAlias, otherTableColumnName);
        }

        public RDBConditionContext Join(RDBJoinType joinType, IRDBTableQuerySource table, string tableAlias)
        {
            return new RDBConditionContext(
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

        public RDBSelectQuery JoinSelect(RDBJoinType joinType, string inlineQueryAlias)
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
            var conditionContext = Join(joinType, selectQuery, inlineQueryAlias);
            return selectQuery;
        }

        public RDBConditionContext Join(RDBJoinType joinType, string tableName, string tableAlias)
        {
            return Join(joinType, new RDBTableDefinitionQuerySource(tableName), tableAlias);
        }
    }
}

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
        List<RDBJoin> _joins;

        public RDBJoinContext(T parent, List<RDBJoin> joins)
        {
            _parent = parent;
            _joins = joins;
        }

        public IRDBJoinContextReady<T> Join(IRDBTableQuerySource table, RDBJoinType joinType, BaseRDBCondition condition)
        {
            _joins.Add(new RDBJoin
            {
                Table = table,
                JoinType = joinType,
                Condition = condition
            });
            return this;
        }

        public IRDBJoinContextReady<T> Join(string tableName, RDBJoinType joinType, BaseRDBCondition condition)
        {
            return Join(new RDBTableDefinitionQuerySource(tableName), joinType, condition);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName)
        {
            return Join(table, joinType,
                new RDBCompareCondition
                {
                    Expression1 = new RDBColumnExpression
                    {
                        Table = table,
                        ColumnName = columnName
                    },
                    Operator = RDBCompareConditionOperator.Eq,
                    Expression2 = new RDBColumnExpression
                    {
                        Table = otherTable,
                        ColumnName = otherTableColumnName
                    }
                });
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(joinType, new RDBTableDefinitionQuerySource(tableName), columnName, otherTable, otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string columnName, string otherTableName, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(joinType, new RDBTableDefinitionQuerySource(tableName), columnName, new RDBTableDefinitionQuerySource(otherTableName), otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(RDBJoinType.Inner, table, columnName, otherTable, otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(new RDBTableDefinitionQuerySource(tableName), columnName, otherTable, otherTableColumnName);
        }

        public IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string columnName, string otherTableName, string otherTableColumnName)
        {
            return JoinOnEqualOtherTableColumn(new RDBTableDefinitionQuerySource(tableName), columnName, new RDBTableDefinitionQuerySource(otherTableName), otherTableColumnName);
        }

        T IRDBJoinContextReady<T>.EndJoin()
        {
            return _parent;
        }
    }

    public interface IRDBJoinContextReady<T>
    {
        IRDBJoinContextReady<T> Join(IRDBTableQuerySource table, RDBJoinType joinType, BaseRDBCondition condition);

        IRDBJoinContextReady<T> Join(string tableName, RDBJoinType joinType, BaseRDBCondition condition);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, IRDBTableQuerySource table, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(RDBJoinType joinType, string tableName, string columnName, string otherTableName, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(IRDBTableQuerySource table, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string columnName, IRDBTableQuerySource otherTable, string otherTableColumnName);

        IRDBJoinContextReady<T> JoinOnEqualOtherTableColumn(string tableName, string columnName, string otherTableName, string otherTableColumnName);

        T EndJoin();
    }
}

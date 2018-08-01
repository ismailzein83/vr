using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByHavingContext
    {
        IRDBTableQuerySource _table;
        string _tableAlias;
        RDBConditionGroup _conditionGroup;

        internal RDBGroupByHavingContext(RDBConditionGroup conditionGroup, IRDBTableQuerySource table, string tableAlias)
        {
            _conditionGroup = conditionGroup;
            _table = table;
            _tableAlias = tableAlias;
        }
        
        public RDBGroupByHavingContext ChildConditionGroup(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            var childConditionGroup = new RDBConditionGroup(groupOperator);
            this._conditionGroup.Conditions.Add(childConditionGroup);
            return new RDBGroupByHavingContext(childConditionGroup, _table, _tableAlias);
        }

        public void CompareCount(RDBCompareConditionOperator oper, BaseRDBExpression compareToValue)
        {
            _conditionGroup.Conditions.Add(new RDBCompareCondition
            {
                Expression1 = new RDBCountExpression(),
                Operator = oper,
                Expression2 = compareToValue
            });
        }

        public void CompareCount(RDBCompareConditionOperator oper, long value)
        {
             CompareCount(oper, new RDBFixedLongExpression { Value = value });
        }
        
        public void CompareAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression valueToAggregate, RDBCompareConditionOperator oper, BaseRDBExpression compareToValue)
        {
            BaseRDBExpression aggregateExpression = RDBSelectAggregateContext.CreateNonCountAggregate(aggregateType, valueToAggregate);
            _conditionGroup.Conditions.Add(new RDBCompareCondition
            {
                Expression1 = aggregateExpression,
                Operator = oper,
                Expression2 = compareToValue
            });
        }

        public void CompareAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression valueToAggregate, RDBCompareConditionOperator oper, Decimal value)
        {
             CompareAggregate(aggregateType, valueToAggregate, oper, new RDBFixedDecimalExpression { Value = value});
        }

        public void CompareAggregate(RDBNonCountAggregateType aggregateType, string tableAlias, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
             CompareAggregate(aggregateType, new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, oper, value);
        }

        public void CompareAggregate(RDBNonCountAggregateType aggregateType, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
             CompareAggregate(aggregateType, _tableAlias, columnName, oper, value);
        }
    }
}

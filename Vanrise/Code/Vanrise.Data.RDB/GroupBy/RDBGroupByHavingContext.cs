using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByHavingContext
    { 
        Action<BaseRDBCondition> _setCondition;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBGroupByHavingContext(Action<BaseRDBCondition> setCondition, IRDBTableQuerySource table, string tableAlias)
        {
            _setCondition = setCondition;
            _table = table;
            _tableAlias = tableAlias;
        }


        public RDBGroupByHavingContext()
        {
        }

        internal Action<BaseRDBCondition> SetConditionAction
        {
            set
            {
                _setCondition = value;
            }
        }

        internal IRDBTableQuerySource Table
        {
            set
            {
                _table = value;
            }
        }

        public void CompareCount(RDBCompareConditionOperator oper, BaseRDBExpression compareToValue)
        {
            _setCondition(new RDBCompareCondition
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
            _setCondition(new RDBCompareCondition
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

        public RDBGroupByHavingAndConditionContext And()
        {
            return new RDBGroupByHavingAndConditionContext(_setCondition, _table);
        }

        public RDBGroupByHavingOrConditionContext Or()
        {
            return new RDBGroupByHavingOrConditionContext(_setCondition, _table);
        }
    }
}

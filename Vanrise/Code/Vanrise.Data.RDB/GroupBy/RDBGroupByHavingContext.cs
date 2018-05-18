using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBGroupByHavingContext<T>
    { 
        T _parent;
        Action<BaseRDBCondition> _setCondition;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBGroupByHavingContext(T parent, Action<BaseRDBCondition> setCondition, IRDBTableQuerySource table, string tableAlias)
        {
            _parent = parent;
            _setCondition = setCondition;
            _table = table;
            _tableAlias = tableAlias;
        }


        public RDBGroupByHavingContext()
        {
        }

        internal T Parent
        {
            set
            {
                _parent = value;
            }
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

        public T CompareCount(RDBCompareConditionOperator oper, BaseRDBExpression compareToValue)
        {
            _setCondition(new RDBCompareCondition
            {
                Expression1 = new RDBCountExpression(),
                Operator = oper,
                Expression2 = compareToValue
            });
            return _parent;
        }

        public T CompareCount(RDBCompareConditionOperator oper, long value)
        {
            return CompareCount(oper, new RDBFixedLongExpression { Value = value });
        }


        public T CompareAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression valueToAggregate, RDBCompareConditionOperator oper, BaseRDBExpression compareToValue)
        {
            BaseRDBExpression aggregateExpression = RDBSelectAggregateContext<T>.CreateNonCountAggregate(aggregateType, valueToAggregate);
            _setCondition(new RDBCompareCondition
            {
                Expression1 = aggregateExpression,
                Operator = oper,
                Expression2 = compareToValue
            });
            return _parent;
        }

        public T CompareAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression valueToAggregate, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareAggregate(aggregateType, valueToAggregate, oper, new RDBFixedDecimalExpression { Value = value});
        }

        public T CompareAggregate(RDBNonCountAggregateType aggregateType, string tableAlias, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareAggregate(aggregateType, new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName }, oper, value);
        }

        public T CompareAggregate(RDBNonCountAggregateType aggregateType, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareAggregate(aggregateType, _tableAlias, columnName, oper, value);
        }

        public RDBGroupByHavingAndConditionContext<T> And()
        {
            return new RDBGroupByHavingAndConditionContext<T>(_parent, _setCondition, _table);
        }

        public RDBGroupByHavingOrConditionContext<T> Or()
        {
            return new RDBGroupByHavingOrConditionContext<T>(_parent, _setCondition, _table);
        }
    }
}

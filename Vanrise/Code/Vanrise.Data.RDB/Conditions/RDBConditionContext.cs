using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBConditionContext<T>
    {
        T _parent;
        Action<BaseRDBCondition> _setCondition;

        public RDBConditionContext(T parent, Action<BaseRDBCondition> setCondition)
        {
            _parent = parent;
            _setCondition = setCondition;
        }

        public RDBConditionContext()
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

        public T Condition(BaseRDBCondition condition)
        {
            _setCondition(condition);
            return _parent;
        }

        public T CompareCondition(BaseRDBExpression expression1, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            return this.Condition(new RDBCompareCondition
            {
                Expression1 = expression1,
                Operator = oper,
                Expression2 = expression2
            });
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                expression2);
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, int value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedIntExpression { Value = value });
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, long value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedLongExpression { Value = value });
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDecimalExpression { Value = value });
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, float value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedFloatExpression { Value = value });
        }

        public T CompareCondition(IRDBTableQuerySource table, string columnName, RDBCompareConditionOperator oper, DateTime value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDateTimeExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, BaseRDBExpression valueExpression)
        {
            return CompareCondition(
                    new RDBColumnExpression
                    {
                        Table = table,
                        ColumnName = columnName
                    },
                    RDBCompareConditionOperator.Eq,
                   valueExpression);
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBFixedTextExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, int value)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBFixedIntExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, long value)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBFixedLongExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, decimal value)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBFixedDecimalExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, DateTime value)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBFixedDateTimeExpression { Value = value });
        }

        public T EqualsCondition(IRDBTableQuerySource table, string columnName, IRDBTableQuerySource table2, string column2Name)
        {
            return EqualsCondition(
                    table,
                    columnName,
                   new RDBColumnExpression
                   {
                       Table = table2,
                       ColumnName = column2Name
                   });
        }

        public T ContainsCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.Contains,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotContainsCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.NotContains,
                new RDBFixedTextExpression { Value = value });
        }

        public T StartsWithCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.StartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotStartsWithCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.NotStartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T EndsWithCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.EndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotEndsWithCondition(IRDBTableQuerySource table, string columnName, string value)
        {
            return CompareCondition(
                table,
                columnName,
                RDBCompareConditionOperator.NotEndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T ListCondition(BaseRDBExpression expression, RDBListConditionOperator oper, List<BaseRDBExpression> values)
        {
            return Condition(new RDBListCondition
            {
                Expression = expression,
                Operator = oper,
                Values = values
            });
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<BaseRDBExpression> values)
        {
            return ListCondition(
                new RDBColumnExpression
                {
                    Table = table,
                    ColumnName = columnName
                },
                oper,
                values);
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<string> values)
        {
            return ListCondition(
                table,
                columnName,
                oper,
                values.Select<string, BaseRDBExpression>(itm => new RDBFixedTextExpression { Value = itm }).ToList());
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<int> values)
        {
            return ListCondition(
                table,
                columnName,
                oper,
                values.Select<int, BaseRDBExpression>(itm => new RDBFixedIntExpression { Value = itm }).ToList());
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<long> values)
        {
            return ListCondition(
                table,
                columnName,
                oper,
                values.Select<long, BaseRDBExpression>(itm => new RDBFixedLongExpression { Value = itm }).ToList());
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<decimal> values)
        {
            return ListCondition(
                table,
                columnName,
                oper,
                values.Select<decimal, BaseRDBExpression>(itm => new RDBFixedDecimalExpression { Value = itm }).ToList());
        }

        public T ListCondition(IRDBTableQuerySource table, string columnName, RDBListConditionOperator oper, List<DateTime> values)
        {
            return ListCondition(
                table,
                columnName,
                oper,
                values.Select<DateTime, BaseRDBExpression>(itm => new RDBFixedDateTimeExpression { Value = itm }).ToList());
        }

        public RDBAndConditionContext<T> And()
        {
            return new RDBAndConditionContext<T>(_parent, _setCondition);
        }

        public RDBOrConditionContext<T> Or()
        {
            return new RDBOrConditionContext<T>(_parent, _setCondition);
        }

        public T NullCondition(BaseRDBExpression expression)
        {
            return Condition(new RDBNullCondition { Expression = expression });
        }

        public T NullCondition(IRDBTableQuerySource table, string columnName)
        {
            return NullCondition(new RDBColumnExpression { Table = table, ColumnName = columnName });
        }

        public T NotNullCondition(BaseRDBExpression expression)
        {
            return Condition(new RDBNotNullCondition { Expression = expression });
        }

        public T NotNullCondition(IRDBTableQuerySource table, string columnName)
        {
            return NotNullCondition(new RDBColumnExpression { Table = table, ColumnName = columnName });
        }

        public T ConditionIf(Func<bool> shouldAddCondition, Action<RDBConditionContext<T>> conditionContextAction)
        {
            if (shouldAddCondition())
            {
                conditionContextAction(this);
            }
            return _parent;
        }

        public T ConditionIfNotDefaultValue<Q>(Q value, Action<RDBConditionContext<T>> conditionContextAction)
        {
            return ConditionIf(() => value != null && !value.Equals(default(Q)), conditionContextAction);
        }
    }
}

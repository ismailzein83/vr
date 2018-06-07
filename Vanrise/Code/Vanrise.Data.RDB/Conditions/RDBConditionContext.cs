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
        RDBQueryBuilderContext _queryBuilderContext;
        Action<BaseRDBCondition> _setCondition;
        string _tableAlias;

        public RDBConditionContext(T parent, RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
            _setCondition = setCondition;
            _tableAlias = tableAlias;
        }

        public RDBConditionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _tableAlias = tableAlias;
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

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                expression2);
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            return CompareCondition(_tableAlias, columnName, oper, expression2);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, int value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedIntExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, int value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, long value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedLongExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, long value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDecimalExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, float value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedFloatExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, float value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, DateTime value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDateTimeExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, DateTime value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, bool value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedBooleanExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, bool value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, Guid value)
        {
            return CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedGuidExpression { Value = value });
        }

        public T CompareCondition(string columnName, RDBCompareConditionOperator oper, Guid value)
        {
            return CompareCondition(_tableAlias, columnName, oper, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, BaseRDBExpression valueExpression)
        {
            return CompareCondition(
                    new RDBColumnExpression
                    {
                        TableAlias = tableAlias,
                        ColumnName = columnName
                    },
                    RDBCompareConditionOperator.Eq,
                   valueExpression);
        }

        public T EqualsCondition(string columnName, BaseRDBExpression valueExpression)
        {
            return EqualsCondition(_tableAlias, columnName, valueExpression);
        }

        public T EqualsCondition(string tableAlias, string columnName, string value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedTextExpression { Value = value });
        }

        public T EqualsCondition(string columnName, string value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, int value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedIntExpression { Value = value });
        }

        public T EqualsCondition(string columnName, int value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, long value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedLongExpression { Value = value });
        }

        public T EqualsCondition(string columnName, long value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, decimal value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedDecimalExpression { Value = value });
        }

        public T EqualsCondition(string columnName, decimal value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, DateTime value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedDateTimeExpression { Value = value });
        }

        public T EqualsCondition(string columnName, DateTime value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, bool value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedBooleanExpression { Value = value });
        }

        public T EqualsCondition(string columnName, bool value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string tableAlias, string columnName, Guid value)
        {
            return EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedGuidExpression { Value = value });
        }

        public T EqualsCondition(string columnName, Guid value)
        {
            return EqualsCondition(_tableAlias, columnName, value);
        }

        public T EqualsCondition(string table1Alias, string column1Name, string table2Alias, string column2Name)
        {
            return EqualsCondition(
                    table1Alias,
                    column1Name,
                   new RDBColumnExpression
                   {
                       TableAlias = table2Alias,
                       ColumnName = column2Name
                   });
        }

        public T ContainsCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.Contains,
                new RDBFixedTextExpression { Value = value });
        }

        public T ContainsCondition(string columnName, string value)
        {
            return ContainsCondition(_tableAlias, columnName, value);
        }

        public T NotContainsCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotContains,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotContainsCondition(string columnName, string value)
        {
            return NotContainsCondition(_tableAlias, columnName, value);
        }

        public T StartsWithCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.StartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T StartsWithCondition(string columnName, string value)
        {
            return StartsWithCondition(_tableAlias, columnName, value);
        }

        public T NotStartsWithCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotStartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotStartsWithCondition(string columnName, string value)
        {
            return NotStartsWithCondition(_tableAlias, columnName, value);
        }

        public T EndsWithCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.EndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T EndsWithCondition(string columnName, string value)
        {
            return EndsWithCondition(_tableAlias, columnName, value);
        }

        public T NotEndsWithCondition(string tableAlias, string columnName, string value)
        {
            return CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotEndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public T NotEndsWithCondition(string columnName, string value)
        {
            return NotEndsWithCondition(_tableAlias, columnName, value);
        }
        
        public IRDBSelectQuery<T> ExistsCondition()
        {
            var selectQuery = new RDBSelectQuery<T>(_parent, _queryBuilderContext.CreateChildContext());
            this.Condition(new RDBExistsCondition<T> { SelectQuery = selectQuery});
            return selectQuery;
        }

        public IRDBSelectQuery<T> NotExistsCondition()
        {
            var selectQuery = new RDBSelectQuery<T>(_parent, _queryBuilderContext.CreateChildContext());
            this.Condition(new RDBNotExistsCondition<T> { SelectQuery = selectQuery });
            return selectQuery;
        }

        public T ListCondition(BaseRDBExpression expression, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
            return Condition(new RDBListCondition
            {
                Expression = expression,
                Operator = oper,
                Values = values
            });
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
            return ListCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                values);
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<string> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<string, BaseRDBExpression>(itm => new RDBFixedTextExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<string> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<int> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<int, BaseRDBExpression>(itm => new RDBFixedIntExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<int> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<long> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<long, BaseRDBExpression>(itm => new RDBFixedLongExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<long> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<decimal> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<decimal, BaseRDBExpression>(itm => new RDBFixedDecimalExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<decimal> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<DateTime> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<DateTime, BaseRDBExpression>(itm => new RDBFixedDateTimeExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<DateTime> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public T ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<Guid> values)
        {
            return ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<Guid, BaseRDBExpression>(itm => new RDBFixedGuidExpression { Value = itm }).ToList());
        }

        public T ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<Guid> values)
        {
            return ListCondition(_tableAlias, columnName, oper, values);
        }

        public RDBAndConditionContext<T> And()
        {
            return new RDBAndConditionContext<T>(_parent, _queryBuilderContext, _setCondition, _tableAlias);
        }

        public RDBOrConditionContext<T> Or()
        {
            return new RDBOrConditionContext<T>(_parent, _queryBuilderContext, _setCondition, _tableAlias);
        }

        public RDBNullOrConditionContext<T> ConditionIfColumnNotNull(string tableAlias, string columnName)
        {
            return new RDBNullOrConditionContext<T>(_parent, _queryBuilderContext, _setCondition, tableAlias, columnName);
        }

        public RDBNullOrConditionContext<T> ConditionIfColumnNotNull(string columnName)
        {
            return ConditionIfColumnNotNull(_tableAlias, columnName);
        }

        public T NullCondition(BaseRDBExpression expression)
        {
            return Condition(new RDBNullCondition { Expression = expression });
        }

        public T NullCondition(string tableAlias, string columnName)
        {
            return NullCondition(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName });
        }

        public T NullCondition(string columnName)
        {
            return NullCondition(_tableAlias, columnName);
        }

        public T NotNullCondition(BaseRDBExpression expression)
        {
            return Condition(new RDBNotNullCondition { Expression = expression });
        }

        public T NotNullCondition(string tableAlias, string columnName)
        {
            return NotNullCondition(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName });
        }

        public T NotNullCondition(string columnName)
        {
            return NotNullCondition(_tableAlias, columnName);
        }

        public T ConditionIfNotDefaultValue<Q>(Q value, Action<RDBConditionContext<T>> trueAction)
        {
            return ConditionIf(() => value != null && !value.Equals(default(Q)), trueAction);
        }

        public T ConditionIf(Func<bool> shouldAddCondition, Action<RDBConditionContext<T>> trueAction, Action<RDBConditionContext<T>> falseAction)
        {
            if (shouldAddCondition())
                trueAction(this);
            else if (falseAction != null)
                falseAction(this);
            else
                _setCondition(new RDBAlwaysTrueCondition());
            return _parent;
        }

        public T ConditionIf(Func<bool> shouldAddCondition, Action<RDBConditionContext<T>> trueAction)
        {
            return ConditionIf(shouldAddCondition, trueAction, null);
        }
    }
}

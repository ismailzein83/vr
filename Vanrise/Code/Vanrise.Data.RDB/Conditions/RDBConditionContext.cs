using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBConditionContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        Action<BaseRDBCondition> _setCondition;
        string _tableAlias;

        public RDBConditionContext(RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBCondition> setCondition, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _setCondition = setCondition;
            _tableAlias = tableAlias;
        }

        public RDBConditionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _tableAlias = tableAlias;
        }

        internal Action<BaseRDBCondition> SetConditionAction
        {
            set
            {
                _setCondition = value;
            }
        }

        public void Condition(BaseRDBCondition condition)
        {
            _setCondition(condition);
        }

        public void TrueCondition()
        {
            CompareCondition(new RDBFixedIntExpression { Value = 1 }, RDBCompareConditionOperator.Eq, new RDBFixedIntExpression { Value = 1 });
        }

        public void FalseCondition()
        {
            CompareCondition(new RDBFixedIntExpression { Value = 1 }, RDBCompareConditionOperator.Eq, new RDBFixedIntExpression { Value = 0 });
        }

        public void CompareCondition(BaseRDBExpression expression1, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            this.Condition(new RDBCompareCondition
            {
                Expression1 = expression1,
                Operator = oper,
                Expression2 = expression2
            });
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                expression2);
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
        {
            CompareCondition(_tableAlias, columnName, oper, expression2);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, int value)
        {
            CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedIntExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, int value)
        {
            CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, long value)
        {
            CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedLongExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, long value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
            CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDecimalExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, Decimal value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, float value)
        {
             CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedFloatExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, float value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, DateTime value)
        {
             CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedDateTimeExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, DateTime value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, bool value)
        {
             CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedBooleanExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, bool value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, Guid value)
        {
             CompareCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                new RDBFixedGuidExpression { Value = value });
        }

        public void CompareCondition(string columnName, RDBCompareConditionOperator oper, Guid value)
        {
             CompareCondition(_tableAlias, columnName, oper, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, BaseRDBExpression valueExpression)
        {
             CompareCondition(
                    new RDBColumnExpression
                    {
                        TableAlias = tableAlias,
                        ColumnName = columnName
                    },
                    RDBCompareConditionOperator.Eq,
                   valueExpression);
        }

        public void EqualsCondition(string columnName, BaseRDBExpression valueExpression)
        {
             EqualsCondition(_tableAlias, columnName, valueExpression);
        }

        public void EqualsCondition(string tableAlias, string columnName, string value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedTextExpression { Value = value });
        }

        public void EqualsCondition(string columnName, string value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, int value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedIntExpression { Value = value });
        }

        public void EqualsCondition(string columnName, int value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, long value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedLongExpression { Value = value });
        }

        public void EqualsCondition(string columnName, long value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, decimal value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedDecimalExpression { Value = value });
        }

        public void EqualsCondition(string columnName, decimal value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, DateTime value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedDateTimeExpression { Value = value });
        }

        public void EqualsCondition(string columnName, DateTime value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, bool value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedBooleanExpression { Value = value });
        }

        public void EqualsCondition(string columnName, bool value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string tableAlias, string columnName, Guid value)
        {
             EqualsCondition(
                    tableAlias,
                    columnName,
                   new RDBFixedGuidExpression { Value = value });
        }

        public void EqualsCondition(string columnName, Guid value)
        {
             EqualsCondition(_tableAlias, columnName, value);
        }

        public void EqualsCondition(string table1Alias, string column1Name, string table2Alias, string column2Name)
        {
             EqualsCondition(
                    table1Alias,
                    column1Name,
                   new RDBColumnExpression
                   {
                       TableAlias = table2Alias,
                       ColumnName = column2Name
                   });
        }

        public void ContainsCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.Contains,
                new RDBFixedTextExpression { Value = value });
        }

        public void ContainsCondition(string columnName, string value)
        {
             ContainsCondition(_tableAlias, columnName, value);
        }

        public void NotContainsCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotContains,
                new RDBFixedTextExpression { Value = value });
        }

        public void NotContainsCondition(string columnName, string value)
        {
             NotContainsCondition(_tableAlias, columnName, value);
        }

        public void StartsWithCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.StartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public void StartsWithCondition(string columnName, string value)
        {
             StartsWithCondition(_tableAlias, columnName, value);
        }

        public void NotStartsWithCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotStartWith,
                new RDBFixedTextExpression { Value = value });
        }

        public void NotStartsWithCondition(string columnName, string value)
        {
             NotStartsWithCondition(_tableAlias, columnName, value);
        }

        public void EndsWithCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.EndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public void EndsWithCondition(string columnName, string value)
        {
             EndsWithCondition(_tableAlias, columnName, value);
        }

        public void NotEndsWithCondition(string tableAlias, string columnName, string value)
        {
             CompareCondition(
                tableAlias,
                columnName,
                RDBCompareConditionOperator.NotEndWith,
                new RDBFixedTextExpression { Value = value });
        }

        public void NotEndsWithCondition(string columnName, string value)
        {
             NotEndsWithCondition(_tableAlias, columnName, value);
        }
        
        public RDBSelectQuery ExistsCondition()
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
            this.Condition(new RDBExistsCondition { SelectQuery = selectQuery});
            return selectQuery;
        }

        public RDBSelectQuery NotExistsCondition()
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext());
            this.Condition(new RDBNotExistsCondition { SelectQuery = selectQuery });
            return selectQuery;
        }

        public void ListCondition(BaseRDBExpression expression, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
             Condition(new RDBListCondition
            {
                Expression = expression,
                Operator = oper,
                Values = values
            });
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
             ListCondition(
                new RDBColumnExpression
                {
                    TableAlias = tableAlias,
                    ColumnName = columnName
                },
                oper,
                values);
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<BaseRDBExpression> values)
        {
             ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<string> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<string, BaseRDBExpression>(itm => new RDBFixedTextExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<string> values)
        {
             ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<int> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<int, BaseRDBExpression>(itm => new RDBFixedIntExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<int> values)
        {
            ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<long> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<long, BaseRDBExpression>(itm => new RDBFixedLongExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<long> values)
        {
            ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<decimal> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<decimal, BaseRDBExpression>(itm => new RDBFixedDecimalExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<decimal> values)
        {
            ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<DateTime> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<DateTime, BaseRDBExpression>(itm => new RDBFixedDateTimeExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<DateTime> values)
        {
            ListCondition(_tableAlias, columnName, oper, values);
        }

        public void ListCondition(string tableAlias, string columnName, RDBListConditionOperator oper, IEnumerable<Guid> values)
        {
            ListCondition(
                tableAlias,
                columnName,
                oper,
                values.Select<Guid, BaseRDBExpression>(itm => new RDBFixedGuidExpression { Value = itm }).ToList());
        }

        public void ListCondition(string columnName, RDBListConditionOperator oper, IEnumerable<Guid> values)
        {
             ListCondition(_tableAlias, columnName, oper, values);
        }
        
        public RDBGroupConditionContext ConditionGroup(RDBGroupConditionType groupConditionType)
        {
            return new RDBGroupConditionContext(groupConditionType, _queryBuilderContext, _setCondition, _tableAlias);
        }

        public RDBAndConditionContext And()
        {
            return new RDBAndConditionContext(_queryBuilderContext, _setCondition, _tableAlias);
        }

        public RDBOrConditionContext Or()
        {
            return new RDBOrConditionContext(_queryBuilderContext, _setCondition, _tableAlias);
        }

        public RDBNullOrConditionContext ConditionIfColumnNotNull(string tableAlias, string columnName)
        {
            return new RDBNullOrConditionContext(_queryBuilderContext, _setCondition, tableAlias, columnName);
        }

        public RDBNullOrConditionContext ConditionIfColumnNotNull(string columnName)
        {
            return ConditionIfColumnNotNull(_tableAlias, columnName);
        }

        public void NullCondition(BaseRDBExpression expression)
        {
             Condition(new RDBNullCondition { Expression = expression });
        }

        public void NullCondition(string tableAlias, string columnName)
        {
             NullCondition(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName });
        }

        public void NullCondition(string columnName)
        {
             NullCondition(_tableAlias, columnName);
        }

        public void NotNullCondition(BaseRDBExpression expression)
        {
             Condition(new RDBNotNullCondition { Expression = expression });
        }

        public void NotNullCondition(string tableAlias, string columnName)
        {
             NotNullCondition(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName });
        }

        public void NotNullCondition(string columnName)
        {
             NotNullCondition(_tableAlias, columnName);
        }
    }
}

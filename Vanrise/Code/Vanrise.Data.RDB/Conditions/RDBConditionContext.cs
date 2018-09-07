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
        string _tableAlias;
        RDBConditionGroup _conditionGroup;

        internal RDBConditionContext(RDBQueryBuilderContext queryBuilderContext, RDBConditionGroup conditionGroup, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _conditionGroup = conditionGroup;
            _tableAlias = tableAlias;
        }

        public void Condition(BaseRDBCondition condition)
        {
            _conditionGroup.Conditions.Add(condition);
        }

        public RDBConditionContext ChildConditionGroup(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            var childConditionGroup = new RDBConditionGroup(groupOperator);
            this._conditionGroup.Conditions.Add(childConditionGroup);
            return new RDBConditionContext(_queryBuilderContext, childConditionGroup, _tableAlias);
        }

        public void TrueCondition()
        {
            CompareCondition(new RDBFixedIntExpression { Value = 1 }, RDBCompareConditionOperator.Eq, new RDBFixedIntExpression { Value = 1 });
        }

        public void FalseCondition()
        {
            CompareCondition(new RDBFixedIntExpression { Value = 1 }, RDBCompareConditionOperator.Eq, new RDBFixedIntExpression { Value = 0 });
        }

        public RDBExpressionContext CreateExpressionContext(Action<BaseRDBExpression> setExpression)
        {
            return new RDBExpressionContext(_queryBuilderContext, setExpression, _tableAlias);
        }

        public RDBExpressionContext GreaterThanCondition(string tableAlias, string columnName)
        {
            return CompareCondition(tableAlias, columnName, RDBCompareConditionOperator.G);
        }

        public RDBExpressionContext GreaterThanCondition(string columnName)
        {
            return GreaterThanCondition(_tableAlias, columnName);
        }

        public RDBExpressionContext GreaterOrEqualCondition(string tableAlias, string columnName)
        {
            return CompareCondition(tableAlias, columnName, RDBCompareConditionOperator.GEq);
        }

        public RDBExpressionContext GreaterOrEqualCondition(string columnName)
        {
            return GreaterOrEqualCondition(_tableAlias, columnName);
        }

        public RDBExpressionContext LessThanCondition(string tableAlias, string columnName)
        {
            return CompareCondition(tableAlias, columnName, RDBCompareConditionOperator.L);
        }

        public RDBExpressionContext LessThanCondition(string columnName)
        {
            return LessThanCondition(_tableAlias, columnName);
        }

        public RDBExpressionContext LessOrEqualCondition(string tableAlias, string columnName)
        {
            return CompareCondition(tableAlias, columnName, RDBCompareConditionOperator.LEq);
        }

        public RDBExpressionContext LessOrEqualCondition(string columnName)
        {
            return LessOrEqualCondition(_tableAlias, columnName);
        }

        public RDBExpressionContext NotEqualsCondition(string tableAlias, string columnName)
        {
            return CompareCondition(tableAlias, columnName, RDBCompareConditionOperator.NEq);
        }

        public RDBExpressionContext NotEqualsCondition(string columnName)
        {
            return NotEqualsCondition(_tableAlias, columnName);
        }

        public RDBExpressionContext CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => CompareCondition(tableAlias, columnName, oper, expression), _tableAlias);
        }

        public RDBExpressionContext CompareCondition(string columnName, RDBCompareConditionOperator oper)
        {
            return CompareCondition(_tableAlias, columnName, oper);
        }

        private void CompareCondition(string tableAlias, string columnName, RDBCompareConditionOperator oper, BaseRDBExpression expression2)
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

        public RDBCompareConditionContext CompareCondition(RDBCompareConditionOperator oper)
        {
            return new RDBCompareConditionContext(_queryBuilderContext, _tableAlias, (exp1, exp2) => CompareCondition(exp1, oper, exp2));
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

        public RDBExpressionContext EqualsCondition(string tableAlias, string columnName)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => EqualsCondition(tableAlias, columnName, expression), _tableAlias);
        }

        public RDBExpressionContext EqualsCondition(string columnName)
        {
            return EqualsCondition(_tableAlias, columnName);
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
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext(), false);
            this.Condition(new RDBExistsCondition { SelectQuery = selectQuery});
            return selectQuery;
        }

        public RDBSelectQuery NotExistsCondition()
        {
            var selectQuery = new RDBSelectQuery(_queryBuilderContext.CreateChildContext(), false);
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

        public RDBConditionContext ConditionIfColumnNotNull(string tableAlias, string columnName, RDBConditionGroupOperator childGroupOperator = RDBConditionGroupOperator.AND)
        {
            var childOfChildConditionGroup = new RDBConditionGroup(childGroupOperator);
            var childConditionGroup = new RDBConditionGroup(RDBConditionGroupOperator.OR);
            childConditionGroup.Conditions.Add(new RDBNullCondition { Expression = new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName } });
            childConditionGroup.Conditions.Add(childOfChildConditionGroup);
            Condition(childConditionGroup);
            return new RDBConditionContext(_queryBuilderContext, childOfChildConditionGroup, _tableAlias);
        }

        public RDBConditionContext ConditionIfColumnNotNull(string columnName, RDBConditionGroupOperator childGroupOperator = RDBConditionGroupOperator.AND)
        {
            return ConditionIfColumnNotNull(_tableAlias, columnName, childGroupOperator);
        }

        public RDBExpressionContext NullCondition()
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => NullCondition(expression), _tableAlias);
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

        public RDBExpressionContext NotNullCondition()
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => NotNullCondition(expression), _tableAlias);
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
    
    public class RDBTwoExpressionsContext
    {
        RDBExpressionContext _expression1Context;
        RDBExpressionContext _expression2Context;

        BaseRDBExpression _expression1;
        BaseRDBExpression _expression2;

        public RDBTwoExpressionsContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression, BaseRDBExpression> setExpressions)
        {
            _expression1Context = new RDBExpressionContext(queryBuilderContext, (expression) =>
            {
                _expression1 = expression;
                if (ShouldNotify())
                    setExpressions(_expression1, _expression2);
            },
            tableAlias);
            _expression2Context = new RDBExpressionContext(queryBuilderContext, (expression) =>
            {
                _expression2 = expression;
                if (ShouldNotify())
                    setExpressions(_expression1, _expression2);
            }, tableAlias);
        }

        bool _isFirstExpressionSet;
        private bool ShouldNotify()
        {
            if (_isFirstExpressionSet)
            {
                return true;
            }
            else
            {
                _isFirstExpressionSet = true;
                return false;
            }
        }

        protected RDBExpressionContext Exp1()
        {
            return _expression1Context;
        }

        protected RDBExpressionContext Exp2()
        {
            return _expression2Context;
        }
    }

    public class RDBCompareConditionContext : RDBTwoExpressionsContext
    {
        public RDBCompareConditionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression, BaseRDBExpression> setExpressions)
            : base(queryBuilderContext, tableAlias,  setExpressions)
        {
        }

        public RDBExpressionContext Expression1()
        {
            return base.Exp1();
        }

        public RDBExpressionContext Expression2()
        {
            return base.Exp2();
        }
    }
}

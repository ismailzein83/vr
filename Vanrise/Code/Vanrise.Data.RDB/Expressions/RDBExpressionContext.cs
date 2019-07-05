﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBExpressionContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;
        Action<BaseRDBExpression> _setExpression;

        internal RDBExpressionContext(RDBQueryBuilderContext queryBuilderContext, Action<BaseRDBExpression> setExpression, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _setExpression = setExpression;
            _tableAlias = tableAlias;
        }

        public void Value(string value)
        {
            Expression(new RDBFixedTextExpression { Value = value });
        }

        public void Value(int value)
        {
            Expression(new RDBFixedIntExpression { Value = value });
        }

        public void Value(long value)
        {
            Expression(new RDBFixedLongExpression { Value = value });
        }

        public void Value(decimal value)
        {
            Expression(new RDBFixedDecimalExpression { Value = value });
        }

        public void Value(float value)
        {
            Expression(new RDBFixedFloatExpression { Value = value });
        }

        public void Value(DateTime value)
        {
            Expression(new RDBFixedDateTimeExpression { Value = value });
        }

        public void Value(Guid value)
        {
            Expression(new RDBFixedGuidExpression { Value = value });
        }

        public void Value(bool value)
        {
            Expression(new RDBFixedBooleanExpression { Value = value });
        }

        public void Value(byte[] value)
        {
            Expression(new RDBFixedBytesExpression { Value = value });
        }

        public void Column(string columnName)
        {
            Column(_tableAlias, columnName);
        }

        public void Column(string tableAlias, string columnName)
        {
            Expression(new RDBColumnExpression(_queryBuilderContext, tableAlias, columnName));
        }

        public RDBTextConcatenationExpressionContext TextConcatenation()
        {
            return new RDBTextConcatenationExpressionContext(_queryBuilderContext, _tableAlias, (exp1, exp2) => Expression(new RDBTextConcatenationExpression { Expression1 = exp1, Expression2 = exp2 }));
        }

        public RDBExpressionContext TextLeftPart(int numberOfCharacters)
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Expression(new RDBTextLeftPartExpression { Expression = exp, NbOfCharacters = numberOfCharacters }), _tableAlias);
        }

        public RDBExpressionContext TextLength()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Expression(new RDBTextLengthExpression { Expression = exp }), _tableAlias);
        }

        public void DateNow()
        {
            Expression(new RDBNowDateTimeExpression());
        }

        public RDBExpressionContext DateTimePart(RDBDateTimePart part)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => Expression(new RDBDateTimePartExpression { DateTimeExpression = expression, Part = part }), _tableAlias);
        }

        public RDBArithmeticExpressionContext ArithmeticExpression(RDBArithmeticExpressionOperator arithmeticExpressionOperator)
        {
            return new RDBArithmeticExpressionContext(_queryBuilderContext, _tableAlias, (exp1, exp2) => Expression(new RDBArithmeticExpression { Expression1 = exp1, Operator = arithmeticExpressionOperator, Expression2 = exp2 }));
        }

        public void Null()
        {
            Expression(new RDBNullExpression());
        }

        public void EmptyString()
        {
            Expression(new RDBEmptyStringExpression());
        }

        public void ObjectValue(Object value)
        {
            if (value == null)
            {
                Expression(new RDBNullExpression());
                return;
            }
            if (value is string)
            {
                Expression(new RDBFixedTextExpression { Value = value as string });
                return;
            }
            if (value is int)
            {
                Expression(new RDBFixedIntExpression { Value = (int)value });
                return;
            }
            if (value is long)
            {
                Expression(new RDBFixedLongExpression { Value = (long)value });
                return;
            }
            if (value is decimal)
            {
                Expression(new RDBFixedDecimalExpression { Value = (decimal)value });
                return;
            }
            if (value is float)
            {
                Expression(new RDBFixedFloatExpression { Value = (float)value });
                return;
            }
            if (value is double)
            {
                Expression(new RDBFixedFloatExpression { Value = (float)(double)value });
                return;
            }
            if (value is Guid)
            {
                Expression(new RDBFixedGuidExpression { Value = (Guid)value });
                return;
            }
            if (value is DateTime)
            {
                Expression(new RDBFixedDateTimeExpression { Value = (DateTime)value });
                return;
            }
            if (value is bool)
            {
                Expression(new RDBFixedBooleanExpression { Value = (bool)value });
                return;
            }
            if (value is byte[])
            {
                Expression(new RDBFixedBytesExpression { Value = (byte[])value });
                return;
            }
            throw new NotSupportedException(string.Format("value type '{0}'", value.GetType()));
        }

        //public void Parameter(string parameterName)
        //{
        //    Expression(new RDBParameterExpression { ParameterName = parameterName });
        //}

        bool _isExpressionSet;
        public void Expression(BaseRDBExpression expression)
        {
            if (_isExpressionSet)
                throw new Exception("Expression already set");
            _setExpression(expression);
            _isExpressionSet = true;
        }

        public RDBCaseExpressionContext CaseExpression()
        {
            return new RDBCaseExpressionContext(_queryBuilderContext, _tableAlias, (exp) => Expression(exp));
        }

        public RDBDateTimeAddExpressionContext DateTimeAdd(RDBDateTimeAddInterval addInterval)
        {
            return new RDBDateTimeAddExpressionContext(_queryBuilderContext, _tableAlias, addInterval, (exp) => Expression(exp));
        }
        public RDBDateTimeDiffExpressionContext DateTimeDiff(RDBDateTimeDiffInterval diffInterval)
        {
            return new RDBDateTimeDiffExpressionContext(_queryBuilderContext, _tableAlias, diffInterval, (exp) => Expression(exp));
        }

        public RDBExpressionContext ConvertDecimal()
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => Expression(new RDBConvertDecimalExpression { Expression = expression }), _tableAlias);
        }

        public RDBExpressionContext ConvertNumberToString(int? numberPrecision = null)
        {
            return new RDBExpressionContext(_queryBuilderContext, (expression) => Expression(new RDBConvertNumberToStringExpression { Expression = expression, NumberPrecision= numberPrecision }), _tableAlias);
        }

        #region Aggregates

        public void Count(string alias)
        {
            Expression(new RDBCountExpression());
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression)
        {
            BaseRDBExpression aggregateExpression = CreateNonCountAggregate(aggregateType, expression);
            Expression(aggregateExpression);
        }

        public static BaseRDBExpression CreateNonCountAggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression)
        {
            BaseRDBExpression aggregateExpression;
            switch (aggregateType)
            {
                case RDBNonCountAggregateType.SUM: aggregateExpression = new RDBSumExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.AVG: aggregateExpression = new RDBAvgExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.MAX: aggregateExpression = new RDBMaxExpression { Expression = expression }; break;
                case RDBNonCountAggregateType.MIN: aggregateExpression = new RDBMinExpression { Expression = expression }; break;
                default: throw new NotSupportedException(String.Format("aggregateType '{0}'", aggregateType.ToString()));
            }
            return aggregateExpression;
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string tableAlias, string columnName)
        {
            Aggregate(aggregateType, new RDBColumnExpression(_queryBuilderContext, tableAlias, columnName));
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string columnName)
        {
            Aggregate(aggregateType, _tableAlias, columnName);
        }

        public RDBExpressionContext ExpressionAggregate(RDBNonCountAggregateType aggregateType)
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Aggregate(aggregateType, exp), _tableAlias);
        }

        #endregion
    }

    public class RDBArithmeticExpressionContext : RDBTwoExpressionsContext
    {
        public RDBArithmeticExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression, BaseRDBExpression> setExpressions)
            : base(queryBuilderContext, tableAlias, setExpressions)
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

    public class RDBTextConcatenationExpressionContext : RDBTwoExpressionsContext
    {
        public RDBTextConcatenationExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression, BaseRDBExpression> setExpressions)
            : base(queryBuilderContext, tableAlias, setExpressions)
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

    public class RDBCaseExpressionContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;
        RDBCaseExpression _caseExpression;

        internal RDBCaseExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression> setExpression)
        {
            _queryBuilderContext = queryBuilderContext;
            _tableAlias = tableAlias;
            _caseExpression = new RDBCaseExpression { Whens = new List<RDBCaseWhenExpression>() };
            setExpression(_caseExpression);
        }

        public RDBCaseExpressionWhenContext AddCase()
        {
            return new RDBCaseExpressionWhenContext(_queryBuilderContext, _tableAlias, _caseExpression.Whens);
        }

        public RDBExpressionContext Else()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _caseExpression.DefaultValueExpression = exp, _tableAlias);
        }
    }

    public class RDBCaseExpressionWhenContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;
        RDBCaseWhenExpression _when;
        internal RDBCaseExpressionWhenContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, List<RDBCaseWhenExpression> whens)
        {
            _queryBuilderContext = queryBuilderContext;
            
            _tableAlias = tableAlias;
            _when = new RDBCaseWhenExpression();
            whens.Add(_when);
        }


        public RDBConditionContext When(RDBConditionGroupOperator groupOperator = RDBConditionGroupOperator.AND)
        {
            _when.ConditionGroup = new RDBConditionGroup(groupOperator);
            return new RDBConditionContext(_queryBuilderContext, _when.ConditionGroup, _tableAlias);
        }

        public RDBExpressionContext Then()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _when.ValueExpression = exp, _tableAlias);
        }
    }

    public class RDBDateTimeAddExpressionContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;
        RDBDateTimeAddExpression _dateTimeAddExpression;

        internal RDBDateTimeAddExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, RDBDateTimeAddInterval addInterval, Action<BaseRDBExpression> setExpression)
        {
            _queryBuilderContext = queryBuilderContext;
            _tableAlias = tableAlias;
            _dateTimeAddExpression = new RDBDateTimeAddExpression { Interval = addInterval };
            setExpression(_dateTimeAddExpression);
        }

        public RDBExpressionContext DateTime()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _dateTimeAddExpression.DateTimeExpression = exp, _tableAlias);
        }

        public RDBExpressionContext ValueToAdd()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _dateTimeAddExpression.ValueToAddExpression = exp, _tableAlias);
        }
    }

    public class RDBDateTimeDiffExpressionContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        string _tableAlias;
        RDBDateTimeDiffExpression _dateTimeDiffExpression;

        internal RDBDateTimeDiffExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, RDBDateTimeDiffInterval diffInterval, Action<BaseRDBExpression> setExpression)
        {
            _queryBuilderContext = queryBuilderContext;
            _tableAlias = tableAlias;
            _dateTimeDiffExpression = new RDBDateTimeDiffExpression { Interval = diffInterval };
            setExpression(_dateTimeDiffExpression);
        }

        public RDBExpressionContext DateTimeExpression1()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _dateTimeDiffExpression.DateTimeExpression1 = exp, _tableAlias);
        }

        public RDBExpressionContext DateTimeExpression2()
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => _dateTimeDiffExpression.DateTimeExpression2 = exp, _tableAlias);
        }
    }
}

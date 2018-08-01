using System;
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
            Column(tableAlias, columnName, false);
        }

        public void Column(string tableAlias, string columnName, bool dontAppendTableAlias)
        {
            Expression(new RDBColumnExpression { TableAlias = tableAlias, ColumnName = columnName, DontAppendTableAlias = dontAppendTableAlias });
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

        public void ObjectValue(Object value)
        {
            if (value == null)
                Expression(new RDBNullExpression());
            if (value is string)
                Expression(new RDBFixedTextExpression { Value = value as string });
            if (value is int)
                Expression(new RDBFixedIntExpression { Value = (int)value });
            if (value is long)
                Expression(new RDBFixedLongExpression { Value = (long)value });
            if (value is decimal)
                Expression(new RDBFixedDecimalExpression { Value = (decimal)value });
            if (value is float)
                Expression(new RDBFixedFloatExpression { Value = (float)value });
            if (value is Guid)
                Expression(new RDBFixedGuidExpression { Value = (Guid)value });
            if (value is DateTime)
                Expression(new RDBFixedDateTimeExpression { Value = (DateTime)value });
            if (value is bool)
                Expression(new RDBFixedBooleanExpression { Value = (bool)value });
            if (value is byte[])
                Expression(new RDBFixedBytesExpression { Value = (byte[])value });
            throw new NotSupportedException(string.Format("value type '{0}'", value.GetType()));
        }

        public void Parameter(string parameterName)
        {
            Expression(new RDBParameterExpression { ParameterName = parameterName });
        }

        bool _isExpressionSet;
        public void Expression(BaseRDBExpression expression)
        {
            if (_isExpressionSet)
                throw new Exception("Expression already set");
            _setExpression(expression);
            _isExpressionSet = true;
        }
    }

    public class RDBArithmeticExpressionContext : RDBTwoExpressionsContext
    {
        public RDBArithmeticExpressionContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, Action<BaseRDBExpression, BaseRDBExpression> setExpressions)
            : base(queryBuilderContext, tableAlias, setExpressions)
        {
        }
    }
}

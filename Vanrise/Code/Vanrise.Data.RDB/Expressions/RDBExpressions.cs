using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBColumnExpression : BaseRDBExpression
    {
        public IRDBTableQuerySource Table { get; set; }

        public string ColumnName { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat(context.GetTableAlias(this.Table), ".", this.ColumnName);
        }
    }

    public class RDBFixedTextExpression : BaseRDBExpression
    {
        public string Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateParameterName();
            context.AddParameterValue(parameterName, this.Value);
            return parameterName;
        }
    }

    public class RDBFixedIntExpression : BaseRDBExpression
    {
        public int Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    public class RDBFixedLongExpression : BaseRDBExpression
    {
        public long Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    public class RDBFixedDecimalExpression : BaseRDBExpression
    {
        public Decimal Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    public class RDBFixedFloatExpression : BaseRDBExpression
    {
        public float Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    public class RDBFixedDateTimeExpression : BaseRDBExpression
    {
        public DateTime Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateParameterName();
            context.AddParameterValue(parameterName, this.Value);
            return parameterName;
        }
    }

    public class RDBNowDateTimeExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            switch (context.DataProviderType)
            {
                case RDBDataProviderType.MSSQL: return "GETDATE()";
                default: throw new NotSupportedException(String.Format("context.DataProviderType '{0}'", context.DataProviderType.ToString()));
            }
        }
    }

    public class RDBCaseExpression : BaseRDBExpression
    {
        public List<RDBCaseWhenExpression> Whens { get; set; }

        public BaseRDBExpression DefaultValueExpression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            StringBuilder builder = new StringBuilder();
            int whenIndex = 0;
            foreach (var when in this.Whens)
            {
                if (whenIndex == 0)
                    builder.Append(" CASE ");
                builder.Append(" WHEN ");
                builder.Append(when.Condition.ToDBQuery(context.ConditionContext));
                builder.Append(" THEN ");
                builder.Append(when.ValueExpression.ToDBQuery(context));
            }
            if (this.DefaultValueExpression != null)
            {
                builder.Append(" ELSE ");
                builder.Append(this.DefaultValueExpression.ToDBQuery(context));
            }
            builder.Append(" END ");
            return builder.ToString();
        }
    }

    public class RDBCaseWhenExpression
    {
        public BaseRDBCondition Condition { get; set; }

        public BaseRDBExpression ValueExpression { get; set; }

    }

    public enum RDBArithmeticExpressionOperator { Add = 0, Substract = 1, Multiply = 2, Divide = 3 }

    public class RDBArithmeticExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression1 { get; set; }

        public BaseRDBExpression Expression2 { get; set; }

        public RDBArithmeticExpressionOperator Operator { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Expression1.ToDBQuery(context));
            switch (this.Operator)
            {
                case RDBArithmeticExpressionOperator.Add: builder.Append(" + "); break;
                case RDBArithmeticExpressionOperator.Substract: builder.Append(" - "); break;
                case RDBArithmeticExpressionOperator.Multiply: builder.Append(" * "); break;
                case RDBArithmeticExpressionOperator.Divide: builder.Append(" / "); break;
            }
            builder.Append(this.Expression2.ToDBQuery(context));
            return builder.ToString();
        }
    }
}

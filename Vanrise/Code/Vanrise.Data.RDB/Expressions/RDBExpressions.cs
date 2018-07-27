using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBColumnExpression : BaseRDBExpression
    {
        public string TableAlias { get; set; }

        public string ColumnName { get; set; }

        public bool DontAppendTableAlias { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            IRDBTableQuerySource table = this.TableAlias != null ? context.QueryBuilderContext.GetTableFromAlias(this.TableAlias) : context.QueryBuilderContext.GetMainQueryTable();
            table.ThrowIfNull("table");
            var getColumnDBNameContext = new RDBTableQuerySourceGetDBColumnNameContext(this.ColumnName, context);
            string dbColumnName = table.GetDBColumnName(getColumnDBNameContext);
            if (this.TableAlias != null && !DontAppendTableAlias)
                return String.Concat(this.TableAlias, ".", dbColumnName);
            else
                return dbColumnName;
        }
    }

    public class RDBFixedTextExpression : BaseRDBExpression
    {
        public string Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateUniqueDBParameterName();
            context.AddParameter(new RDBParameter
            {
                Name = parameterName,
                DBParameterName = parameterName,
                Type = RDBDataType.NVarchar,
                Direction = RDBParameterDirection.In,
                Value = this.Value
            });
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
            string parameterName = context.GenerateUniqueDBParameterName();
            context.AddParameter(new RDBParameter
            {
                Name = parameterName,
                DBParameterName = parameterName,
                Type = RDBDataType.DateTime,
                Direction = RDBParameterDirection.In,
                Value = this.Value
            });
            return parameterName;
        }
    }

    public class RDBFixedGuidExpression : BaseRDBExpression
    {
        public Guid Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateUniqueDBParameterName();
            context.AddParameter(new RDBParameter
            {
                Name = parameterName,
                DBParameterName = parameterName,
                Type = RDBDataType.UniqueIdentifier,
                Direction = RDBParameterDirection.In,
                Value = this.Value
            });
            return parameterName;
        }
    }

    public class RDBFixedBooleanExpression : BaseRDBExpression
    {
        public bool Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateUniqueDBParameterName();
            context.AddParameter(new RDBParameter
                {
                    Name = parameterName,
                    DBParameterName = parameterName,
                    Type = RDBDataType.Boolean,
                    Direction = RDBParameterDirection.In,
                    Value = this.Value
                });
            return parameterName;
        }
    }

    public class RDBFixedBytesExpression : BaseRDBExpression
    {
        public byte[] Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateUniqueDBParameterName();
            context.AddParameter(new RDBParameter
            {
                Name = parameterName,
                DBParameterName = parameterName,
                Type = RDBDataType.VarBinary,
                Direction = RDBParameterDirection.In,
                Value = this.Value
            });
            return parameterName;
        }
    }

    public class RDBNowDateTimeExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return context.DataProvider.NowDateTimeFunction;
        }
    }

    public enum RDBDateTimePart { TimeOnly = 1, DateOnly = 2 }
    public class RDBDateTimePartExpression : BaseRDBExpression
    {
        public BaseRDBExpression DateTimeExpression { get; set; }

        public RDBDateTimePart Part { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string datetimeExpression = this.DateTimeExpression.ToDBQuery(context);
            switch (this.Part)
            {
                case RDBDateTimePart.DateOnly: return string.Format("Cast({0} as date)", datetimeExpression);
                case RDBDateTimePart.TimeOnly: return string.Format("Cast({0} as time)", datetimeExpression);
                default: throw new NotSupportedException(string.Format("Part '{0}'", this.Part.ToString()));
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
            var conditionContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);
            foreach (var when in this.Whens)
            {
                if (whenIndex == 0)
                    builder.Append(" CASE ");
                builder.Append(" WHEN ");
                builder.Append(when.Condition.ToDBQuery(conditionContext));
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

    public class RDBNullExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return " NULL ";
        }
    }

    public class RDBCountExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return "COUNT(*)";
        }
    }

    public class RDBSumExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("SUM(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public class RDBAvgExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("AVG(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public class RDBMaxExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("MAX(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public class RDBMinExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("MIN(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public class RDBParameterExpression : BaseRDBExpression
    {
        public string ParameterName { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return context.GetParameterWithValidate(this.ParameterName).DBParameterName;
        }
    }
}

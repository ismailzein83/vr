﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
namespace Vanrise.Data.RDB
{
    internal class RDBColumnExpression : BaseRDBExpression
    {
        public RDBColumnExpression(RDBQueryBuilderContext queryBuilderContext, string tableAlias, string columnName)
        {
            this.TableAlias = tableAlias;
            this.ColumnName = columnName;
            if(tableAlias != null)
            {
                var tableQuerySource = queryBuilderContext.GetTableFromAlias(tableAlias);
                var getExpressionColumnContext = new RDBTableQuerySourceTryGetExpressionColumnContext(queryBuilderContext, tableAlias, columnName);
                if (tableQuerySource.TryGetExpressionColumn(getExpressionColumnContext))
                {
                    var expressionColumn = getExpressionColumnContext.ExpressionColumn;
                    var rdbExpressionContext = new RDBExpressionContext(queryBuilderContext, (exp) => _expression = exp, tableAlias);
                    expressionColumn.SetExpression(new RDBTableExpressionColumnSetExpressionContext(tableAlias, rdbExpressionContext));
                    _expression.ThrowIfNull("expression", this.ColumnName);
                }
            }
        }
        
        BaseRDBExpression _expression;

        public string TableAlias { get; set; }

        public string ColumnName { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            if(_expression != null)
            {
                return _expression.ToDBQuery(context);
            }
            else
            {
                IRDBTableQuerySource table = this.TableAlias != null ? context.QueryBuilderContext.GetTableFromAlias(this.TableAlias) : context.QueryBuilderContext.GetMainQueryTable();
                table.ThrowIfNull("table");
                var getColumnDBNameContext = new RDBTableQuerySourceGetDBColumnNameContext(this.ColumnName, context);
                string dbColumnName = table.GetDBColumnName(getColumnDBNameContext);
                if (this.TableAlias != null)
                    return String.Concat(context.DataProvider.GetDBAlias(this.TableAlias), ".", dbColumnName);
                else
                    return dbColumnName;
            }
        }
    }

    internal class RDBFixedTextExpression : BaseRDBExpression
    {
        public string Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            if (context.QueryBuilderContext.DontGenerateParameters)
            {
                return string.Concat("'", (this.Value != null ? this.Value.Replace("'", "''") : string.Empty), "'");
            }
            else
            {
                string parameterName = context.GenerateUniqueDBParameterName(RDBParameterDirection.In);
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
    }

    internal class RDBFixedIntExpression : BaseRDBExpression
    {
        public int Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    internal class RDBFixedLongExpression : BaseRDBExpression
    {
        public long Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    internal class RDBFixedDecimalExpression : BaseRDBExpression
    {
        public Decimal Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    internal class RDBFixedFloatExpression : BaseRDBExpression
    {
        public float Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return Value.ToString();
        }
    }

    internal class RDBFixedDateTimeExpression : BaseRDBExpression
    {
        public DateTime Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            if (context.QueryBuilderContext.DontGenerateParameters)
            {
                return string.Concat("'", BaseDataManager.GetDateTimeForBCP(this.Value), "'");
            }
            else
            {
                string parameterName = context.GenerateUniqueDBParameterName(RDBParameterDirection.In);
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
    }

    internal class RDBFixedTimeExpression : BaseRDBExpression
    {
        public Vanrise.Entities.Time Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return BaseDataManager.GetTimeForBCP(this.Value) as string;
        }
    }

    internal class RDBFixedGuidExpression : BaseRDBExpression
    {
        public Guid Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            if (context.QueryBuilderContext.DontGenerateParameters)
            {
                return string.Concat("'", this.Value, "'");
            }
            else
            {
                string parameterName = context.GenerateUniqueDBParameterName(RDBParameterDirection.In);
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
    }

    internal class RDBFixedBooleanExpression : BaseRDBExpression
    {
        public bool Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            if (context.QueryBuilderContext.DontGenerateParameters)
            {
                return this.Value ? "1" : "0";
            }
            else
            {
                string parameterName = context.GenerateUniqueDBParameterName(RDBParameterDirection.In);
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
    }

    internal class RDBFixedBytesExpression : BaseRDBExpression
    {
        public byte[] Value { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string parameterName = context.GenerateUniqueDBParameterName(RDBParameterDirection.In);
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

    internal class RDBNowDateTimeExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return context.DataProvider.NowDateTimeFunction;
        }
    }

    internal class RDBTextConcatenationExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression1 { get; set; }

        public BaseRDBExpression Expression2 { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return string.Concat(this.Expression1.ToDBQuery(context), " + ", this.Expression2.ToDBQuery(context));
        }
    }

    internal class RDBTextLeftPartExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public int NbOfCharacters { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return string.Concat("LEFT(", this.Expression.ToDBQuery(context), ", ", this.NbOfCharacters, ")");
        }
    }

    internal class RDBTextLengthExpression : BaseRDBExpression 
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return string.Concat("Len(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public enum RDBDateTimePart { TimeOnly = 1, DateOnly = 2, DateAndHour = 3, Hour = 4, Month = 5 }
    internal class RDBDateTimePartExpression : BaseRDBExpression
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
                case RDBDateTimePart.DateAndHour: return string.Format("Convert(Datetime, CONVERT(varchar(13), {0}, 121) + ':00:00')", datetimeExpression);
                case RDBDateTimePart.Hour: return $"Cast(RIGHT('00' + Convert(varchar, DatePart(Hour, {datetimeExpression})), 2) + ':00:00' as time)";
                case RDBDateTimePart.Month:return $"DATEADD(month, DATEDIFF(month, 0, {datetimeExpression}), 0)";
                default: throw new NotSupportedException(string.Format("Part '{0}'", this.Part.ToString()));
            }
        }
    }

    internal class RDBCaseExpression : BaseRDBExpression
    {
        public List<RDBCaseWhenExpression> Whens { get; set; }

        public BaseRDBExpression DefaultValueExpression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            StringBuilder builder = new StringBuilder("(");            
            bool isFirstItem = true;
            var conditionContext = new RDBConditionToDBQueryContext(context, context.QueryBuilderContext);
            foreach (var when in this.Whens)
            {
                if (isFirstItem)
                    builder.Append(" CASE ");
                builder.Append(" WHEN ");
                builder.Append(when.ConditionGroup.ToDBQuery(conditionContext));
                builder.Append(" THEN ");
                builder.Append(when.ValueExpression.ToDBQuery(context));
                isFirstItem = false;
            }
            if (this.DefaultValueExpression != null)
            {
                builder.Append(" ELSE ");
                builder.Append(this.DefaultValueExpression.ToDBQuery(context));
            }
            builder.Append(" END )");
            return builder.ToString();
        }
    }

    internal class RDBCaseWhenExpression
    {
        public RDBConditionGroup ConditionGroup { get; set; }

        public BaseRDBExpression ValueExpression { get; set; }

    }

    public enum RDBArithmeticExpressionOperator { Add = 0, Substract = 1, Multiply = 2, Divide = 3 }

    internal class RDBArithmeticExpression : BaseRDBExpression
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

    internal class RDBNullExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return " NULL ";
        }
    }

    internal class RDBEmptyStringExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return string.Concat("'", context.DataProvider.EmptyStringValue, "'");
        }
    }


    internal class RDBCountExpression : BaseRDBExpression
    {
        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return "COUNT(*)";
        }
    }

    internal class RDBSumExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("SUM(", this.Expression.ToDBQuery(context), ")");
        }
    }

    internal class RDBAvgExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("AVG(", this.Expression.ToDBQuery(context), ")");
        }
    }

    internal class RDBMaxExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("MAX(", this.Expression.ToDBQuery(context), ")");
        }
    }

    internal class RDBMinExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return String.Concat("MIN(", this.Expression.ToDBQuery(context), ")");
        }
    }

    public enum RDBDateTimeAddInterval { Seconds = 0}
    internal class RDBDateTimeAddExpression : BaseRDBExpression
    {
        public RDBDateTimeAddInterval Interval { get; set; }

        public BaseRDBExpression DateTimeExpression { get; set; }

        public BaseRDBExpression ValueToAddExpression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string sqlInteval;
            switch (this.Interval)
            {
                case RDBDateTimeAddInterval.Seconds: sqlInteval = "second";break;
                default: throw new NotSupportedException($"this.Interval '{this.Interval.ToString()}'");
            }

            return $"DATEADD({sqlInteval}, {this.ValueToAddExpression.ToDBQuery(context)}, {this.DateTimeExpression.ToDBQuery(context)})";
        }
    }

    public enum RDBDateTimeDiffInterval { Seconds = 0 }
    internal class RDBDateTimeDiffExpression : BaseRDBExpression
    {
        public RDBDateTimeDiffInterval Interval { get; set; }

        public BaseRDBExpression DateTimeExpression1 { get; set; }

        public BaseRDBExpression DateTimeExpression2 { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            string sqlInteval;
            switch (this.Interval)
            {
                case RDBDateTimeDiffInterval.Seconds: sqlInteval = "second"; break;
                default: throw new NotSupportedException($"this.Interval '{this.Interval.ToString()}'");
            }

            return $"DATEDiff({sqlInteval}, {this.DateTimeExpression1.ToDBQuery(context)}, {this.DateTimeExpression2.ToDBQuery(context)})";
        }
    }

    internal class RDBConvertDecimalExpression : BaseRDBExpression
    {
        public BaseRDBExpression Expression { get; set; }

        public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
        {
            return string.Concat("CONVERT(DECIMAL(38, 8), ", this.Expression.ToDBQuery(context), ")");
        }
    }
    //internal class RDBParameterExpression : BaseRDBExpression
    //{
    //    public string ParameterName { get; set; }

    //    public override string ToDBQuery(IRDBExpressionToDBQueryContext context)
    //    {
    //        return context.GetParameterWithValidate(this.ParameterName).DBParameterName;
    //    }
    //}
}

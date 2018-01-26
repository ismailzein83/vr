using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBCompareConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5, Contains = 6, NotContains = 7, StartWith = 8, NotStartWith = 9, EndWith = 10, NotEndWith = 11 }

    public class RDBCompareCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression1 { get; set; }

        public BaseRDBExpression Expression2 { get; set; }

        public RDBCompareConditionOperator Operator { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            string expression1String = this.Expression1.ToDBQuery(context.ExpressionContext);
            string expression2String = this.Expression2.ToDBQuery(context.ExpressionContext);
            string expr2Prefix;
            string expr2Suffix = null;
            switch (this.Operator)
            {
                case RDBCompareConditionOperator.Eq: expr2Prefix = " = "; break;
                case RDBCompareConditionOperator.NEq: expr2Prefix = " <> "; break;
                case RDBCompareConditionOperator.G: expr2Prefix = " > "; break;
                case RDBCompareConditionOperator.GEq: expr2Prefix = " >= "; break;
                case RDBCompareConditionOperator.L: expr2Prefix = " < "; break;
                case RDBCompareConditionOperator.LEq: expr2Prefix = " <= "; break;
                case RDBCompareConditionOperator.StartWith: expr2Prefix = " like '%' + "; break;
                case RDBCompareConditionOperator.EndWith:
                    expr2Prefix = " like ";
                    expr2Suffix = " + '%'";
                    break;
                case RDBCompareConditionOperator.Contains:
                    expr2Prefix = " like '%' + ";
                    expr2Suffix = " + '%'";
                    break;
                case RDBCompareConditionOperator.NotStartWith: expr2Prefix = " Not like '%' + "; break;
                case RDBCompareConditionOperator.NotEndWith:
                    expr2Prefix = " Not like ";
                    expr2Suffix = " + '%'";
                    break;
                case RDBCompareConditionOperator.NotContains:
                    expr2Prefix = " Not like '%' + ";
                    expr2Suffix = " + '%'";
                    break;
                default: throw new NotSupportedException(String.Format("Operator '{0}'", this.Operator.ToString()));
            }
            if (expr2Suffix != null)
                return String.Concat(expression1String, expr2Prefix, expression2String, expr2Suffix);
            else
                return String.Concat(expression1String, expr2Prefix, expression2String);
        }
    }

    public enum RDBListConditionOperator { IN = 0, NotIN = 1 }

    public class RDBListCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }

        public RDBListConditionOperator Operator { get; set; }

        public List<BaseRDBExpression> Values { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            return String.Concat(this.Expression.ToDBQuery(context.ExpressionContext), this.Operator == RDBListConditionOperator.IN ? " IN (" : " NOT IN (", String.Join(",", this.Values.Select(itm => itm.ToDBQuery(context.ExpressionContext))), ")");
        }
    }

    public abstract class BaseRDBGroupCondition : BaseRDBCondition
    {
        public List<BaseRDBCondition> Conditions { get; set; }
    }

    public class RDBAndCondition : BaseRDBGroupCondition
    {
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            List<string> validConditionsAsStrings = this.Conditions.Select(itm => itm.ToDBQuery(context)).Where(itm => !String.IsNullOrEmpty(itm)).ToList();
            if (validConditionsAsStrings.Count > 0)
                return string.Concat(" (", string.Join(" AND ", validConditionsAsStrings), ") ");
            else
                return null;
        }
    }

    public class RDBOrCondition : BaseRDBGroupCondition
    {
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            List<string> validConditionsAsStrings = this.Conditions.Select(itm => itm.ToDBQuery(context)).Where(itm => !String.IsNullOrEmpty(itm)).ToList();
            if (validConditionsAsStrings.Count > 0)
                return string.Concat(" (", string.Join(" OR ", validConditionsAsStrings), ") ");
            else
                return null;
        }
    }

    public class RDBNullCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            return string.Concat(this.Expression.ToDBQuery(context.ExpressionContext), " IS NULL ");
        }
    }

    public class RDBNotNullCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            return string.Concat(this.Expression.ToDBQuery(context.ExpressionContext), " IS NOT NULL ");
        }
    }
}

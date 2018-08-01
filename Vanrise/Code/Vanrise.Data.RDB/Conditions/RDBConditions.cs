﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public enum RDBCompareConditionOperator { Eq = 0, NEq = 1, G = 2, GEq = 3, L = 4, LEq = 5, Contains = 6, NotContains = 7, StartWith = 8, NotStartWith = 9, EndWith = 10, NotEndWith = 11 }

    internal class RDBCompareCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression1 { get; set; }

        public BaseRDBExpression Expression2 { get; set; }

        public RDBCompareConditionOperator Operator { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var expressionContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            string expression1String = this.Expression1.ToDBQuery(expressionContext);
            string expression2String = this.Expression2.ToDBQuery(expressionContext);
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

    internal class RDBListCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }

        public RDBListConditionOperator Operator { get; set; }

        public IEnumerable<BaseRDBExpression> Values { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var expressionContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            return String.Concat(this.Expression.ToDBQuery(expressionContext), this.Operator == RDBListConditionOperator.IN ? " IN (" : " NOT IN (", String.Join(",", this.Values.Select(itm => itm.ToDBQuery(expressionContext))), ")");
        }
    }

    public enum RDBConditionGroupOperator {  AND = 0, OR = 1}
    internal class RDBConditionGroup : BaseRDBCondition
    {
        public RDBConditionGroup(RDBConditionGroupOperator groupOperator)
        {
            this.Operator = groupOperator;
            this.Conditions = new List<BaseRDBCondition>();
        }

        public RDBConditionGroupOperator Operator { get; private set; }
        
        public List<BaseRDBCondition> Conditions { get; private set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            if (this.Conditions == null || this.Conditions.Count == 0)
                return null;
            List<string> validConditionsAsStrings = this.Conditions.Select(itm => itm.ToDBQuery(context)).Where(itm => !String.IsNullOrEmpty(itm)).ToList();
            if (validConditionsAsStrings.Count > 0)
            {
                if (validConditionsAsStrings.Count == 1)
                    return validConditionsAsStrings[0];
                else
                return string.Concat(" (", string.Join(string.Concat(" ", this.Operator.ToString(), " "), validConditionsAsStrings), ") ");
            }
            else
            {
                return null;
            }
        }
    }

    internal class RDBNullCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var expressionContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            return string.Concat(this.Expression.ToDBQuery(expressionContext), " IS NULL ");
        }
    }

    internal class RDBNotNullCondition : BaseRDBCondition
    {
        public BaseRDBExpression Expression { get; set; }
        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var expressionContext = new RDBExpressionToDBQueryContext(context, context.QueryBuilderContext);
            return string.Concat(this.Expression.ToDBQuery(expressionContext), " IS NOT NULL ");
        }
    }

    internal class RDBExistsCondition : BaseRDBCondition
    {
        public RDBSelectQuery SelectQuery { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var selectQueryAsDB = this.SelectQuery.ToDBQuery(new RDBTableQuerySourceToDBQueryContext(context));
            return string.Concat(" EXISTS ", selectQueryAsDB);
        }
    }

    internal class RDBNotExistsCondition : BaseRDBCondition
    {
        public RDBSelectQuery SelectQuery { get; set; }

        public override string ToDBQuery(IRDBConditionToDBQueryContext context)
        {
            var selectQueryAsDB = this.SelectQuery.ToDBQuery(new RDBTableQuerySourceToDBQueryContext(context));
            return string.Concat(" NOT EXISTS (", selectQueryAsDB, ")");
        }
    }
}

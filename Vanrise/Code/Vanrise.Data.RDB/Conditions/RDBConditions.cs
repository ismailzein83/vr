using System;
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
            string compareOperator;
            switch (this.Operator)
            {
                case RDBCompareConditionOperator.Eq: compareOperator = " = "; break;
                case RDBCompareConditionOperator.NEq: compareOperator = " <> "; break;
                case RDBCompareConditionOperator.G: compareOperator = " > "; break;
                case RDBCompareConditionOperator.GEq: compareOperator = " >= "; break;
                case RDBCompareConditionOperator.L: compareOperator = " < "; break;
                case RDBCompareConditionOperator.LEq: compareOperator = " <= "; break;
                case RDBCompareConditionOperator.StartWith: 
                    compareOperator = " like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings(expression2String, "'%'");
                    break;
                case RDBCompareConditionOperator.EndWith:
                    compareOperator = " like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings("'%'", expression2String);
                    break;
                case RDBCompareConditionOperator.Contains:
                    compareOperator = " like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings("'%'", expression2String, "'%'");
                    break;
                case RDBCompareConditionOperator.NotStartWith: 
                    compareOperator = " not like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings(expression2String, "'%'");
                    break;
                case RDBCompareConditionOperator.NotEndWith:
                    compareOperator = " not like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings("'%'", expression2String);
                    break;
                case RDBCompareConditionOperator.NotContains:
                    compareOperator = " not like ";
                    expression2String = context.DataProvider.GetQueryConcatenatedStrings("'%'", expression2String, "'%'");
                    break;
                default: throw new NotSupportedException(String.Format("Operator '{0}'", this.Operator.ToString()));
            }
            return String.Concat(expression1String, compareOperator, expression2String);
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

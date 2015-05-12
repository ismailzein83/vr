using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CountAggregate:IAggregate
    {

        public CountAggregate(string conditionExpression, ExpressionEvaluator.TypeRegistry tRegistry)
        {
            ExpressionEvaluator.CompiledExpression expression = new ExpressionEvaluator.CompiledExpression
            {
                StringToParse = conditionExpression,
                TypeRegistry = tRegistry,
                ExpressionType = ExpressionEvaluator.CompiledExpressionType.StatementList
            };
            expression.Compile();
            expression.Eval();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void EvaluateCDR(NormalCDR normalCDR)
        {
            throw new NotImplementedException();
        }

        public decimal GetResult()
        {
            throw new NotImplementedException();
        }
    }

    
}
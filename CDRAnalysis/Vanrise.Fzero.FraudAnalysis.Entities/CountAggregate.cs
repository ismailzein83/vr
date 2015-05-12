using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CountAggregate:IAggregate
    {
        string _conditionExpression;

        int Count;

        public CountAggregate(string conditionExpression)
        {
            _conditionExpression = conditionExpression;
        }

        public void Reset()
        {
            Count = 0;
        }

        public void EvaluateCDR(NormalCDR normalCDR)
        {
            ExpressionEvaluator.TypeRegistry tRegistry =  new ExpressionEvaluator.TypeRegistry();
            tRegistry.RegisterSymbol("normalCDR", normalCDR);
            tRegistry.RegisterSymbol("incomingVoiceCall", (int)Enums.CallType.incomingVoiceCall);

            ExpressionEvaluator.CompiledExpression expression = new ExpressionEvaluator.CompiledExpression
            {
                StringToParse = _conditionExpression,
                TypeRegistry = tRegistry,
                ExpressionType = ExpressionEvaluator.CompiledExpressionType.StatementList                 
            };
            expression.Compile();

            if (bool.Parse(expression.Eval().ToString()))
                Count++;
        }

        public decimal GetResult()
        {
            return decimal.Parse(Count.ToString());
        }
    }

    
}
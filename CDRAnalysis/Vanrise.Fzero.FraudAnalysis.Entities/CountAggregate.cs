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
        Func<NormalCDR, bool> _condition;
        int Count;


        public CountAggregate(string conditionExpression)
        {
            _conditionExpression = conditionExpression;
        }


        public CountAggregate(Func<NormalCDR,bool> condition)
        {
            _condition = condition;
        }

        public void Reset()
        {
            Count = 0;
        }

        public void EvaluateCDR(NormalCDR cdr)
        {
            //Condition output = new Condition();
            //ExpressionEvaluator.TypeRegistry tRegistry = new ExpressionEvaluator.TypeRegistry();
            //tRegistry.RegisterSymbol("normalCDR", normalCDR);
            //tRegistry.RegisterSymbol("output", output);
            //tRegistry.RegisterSymbol("incomingVoiceCall", (int)Enums.CallType.incomingVoiceCall);

            //ExpressionEvaluator.CompiledExpression expression = new ExpressionEvaluator.CompiledExpression
            //{
            //    StringToParse = _conditionExpression,
            //    TypeRegistry = tRegistry,
            //    ExpressionType = ExpressionEvaluator.CompiledExpressionType.StatementList
            //};
            //expression.Compile();

            if (_condition == null || _condition(cdr))
                Count++;
        }

        public decimal GetResult()
        {
            return decimal.Parse(Count.ToString());
        }
    }

    
}
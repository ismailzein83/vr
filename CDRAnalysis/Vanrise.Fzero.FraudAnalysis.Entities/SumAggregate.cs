using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SumAggregate:IAggregate
    {

        Func<NormalCDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<NormalCDR, Decimal> _cdrExpressionToSum;
        decimal Sum;


        //public SumAggregate(string conditionExpression)
        //{
        //    _conditionExpression = conditionExpression;
        //}


        public SumAggregate(string propertyName, Func<NormalCDR, bool> condition)
        {
            _propertyGetMethod = typeof(NormalCDR).GetProperty(propertyName).GetGetMethod();
            _condition = condition;
        }

        public SumAggregate(Func<NormalCDR, Decimal> cdrExpressionToSum, Func<NormalCDR, bool> condition)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
            _condition = condition;
        }

        public void Reset()
        {
            Sum = 0;
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
            {
                if(_cdrExpressionToSum != null)
                    Sum += _cdrExpressionToSum(cdr);
                else
                    Sum += (Decimal)_propertyGetMethod.Invoke(cdr, null);
            }
        }

        public decimal GetResult()
        {
            return decimal.Parse(Sum.ToString());
        }























    }

    
}
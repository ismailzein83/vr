using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AverageAggregate : IAggregate
    {

        Func<NormalCDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<NormalCDR, Decimal> _cdrExpressionToSum;
        decimal Sum;
        int Count;


        //public SumAggregate(string conditionExpression)
        //{
        //    _conditionExpression = conditionExpression;
        //}


        public AverageAggregate(string propertyName, Func<NormalCDR, bool> condition)
        {
            _propertyGetMethod = typeof(NormalCDR).GetProperty(propertyName).GetGetMethod();
            _condition = condition;
        }

        public AverageAggregate(Func<NormalCDR, Decimal> cdrExpressionToSum, Func<NormalCDR, bool> condition)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
            _condition = condition;
        }

        public void Reset()
        {
            Sum = 0;
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
            {
                if (_cdrExpressionToSum != null)
                { 
                    Sum += _cdrExpressionToSum(cdr);
                    Count++;
                }
                    
                else
                {
                    Sum += (Decimal)_propertyGetMethod.Invoke(cdr, null);
                    Count++;
                }
                    
            }
        }

        public decimal GetResult()
        {
            if (Sum == 0 || Count == 0)
                return 0;
            else
                return decimal.Parse((Sum/Count).ToString());
        }























    }

    
}
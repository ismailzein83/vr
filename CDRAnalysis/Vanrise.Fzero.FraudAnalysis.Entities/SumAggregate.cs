using System;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SumAggregate:IAggregate
    {

        Func<CDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<CDR, Decimal> _cdrExpressionToSum;
        decimal Sum;

        public SumAggregate(string propertyName, Func<CDR, bool> condition)
        {
            _propertyGetMethod = typeof(CDR).GetProperty(propertyName).GetGetMethod();
            _condition = condition;
        }

        public SumAggregate(Func<CDR, Decimal> cdrExpressionToSum, Func<CDR, bool> condition)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
            _condition = condition;
        }

        public void Reset()
        {
            Sum = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
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
using System;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SumAggregate:IAggregate
    {

        Func<CDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<CDR, Decimal> _cdrExpressionToSum;
        decimal _sum;

       

        public SumAggregate(Func<CDR, Decimal> cdrExpressionToSum, Func<CDR, bool> condition)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
            _condition = condition;
        }

        public void Reset()
        {
            _sum = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (_condition == null || _condition(cdr))
            {
                if(_cdrExpressionToSum != null)
                    _sum += _cdrExpressionToSum(cdr);
                else
                    _sum += (Decimal)_propertyGetMethod.Invoke(cdr, null);
            }
        }

        public decimal GetResult()
        {
            return decimal.Parse(_sum.ToString());
        }























    }

    
}
using System;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AverageAggregate : IAggregate
    {

        Func<CDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<CDR, Decimal> _cdrExpressionToSum;
        decimal _sum;
        int _count;


     


        public void Reset()
        {
            this._sum = 0;
            this._count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                if (this._cdrExpressionToSum != null)
                {
                    this._sum += this._cdrExpressionToSum(cdr);
                    this._count++;
                }
                    
                else
                {
                    this._sum += (Decimal)this._propertyGetMethod.Invoke(cdr, null);
                    this._count++;
                }
                    
            }
        }

        public decimal GetResult()
        {
            if (this._sum == 0 || this._count == 0)
                return 0;
            else
                return _sum / _count;
        }























    }

    
}
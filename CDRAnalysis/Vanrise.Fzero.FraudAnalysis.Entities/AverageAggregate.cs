using System;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AverageAggregate : IAggregate
    {

        Func<CDR, bool> condition;
        MethodInfo propertyGetMethod;
        Func<CDR, Decimal> cdrExpressionToSum;
        decimal sum;
        int count;


        public AverageAggregate(string propertyName, Func<CDR, bool> condition)
        {
            this.propertyGetMethod = typeof(CDR).GetProperty(propertyName).GetGetMethod();
            this.condition = condition;
        }

        public AverageAggregate(Func<CDR, Decimal> cdrExpressionToSum, Func<CDR, bool> condition)
        {
            this.cdrExpressionToSum = cdrExpressionToSum;
            this.condition = condition;
        }

        public void Reset()
        {
            this.sum = 0;
            this.count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this.condition == null || this.condition(cdr))
            {
                if (this.cdrExpressionToSum != null)
                {
                    this.sum += this.cdrExpressionToSum(cdr);
                    this.count++;
                }
                    
                else
                {
                    this.sum += (Decimal)this.propertyGetMethod.Invoke(cdr, null);
                    this.count++;
                }
                    
            }
        }

        public decimal GetResult()
        {
            if (this.sum == 0 || this.count == 0)
                return 0;
            else
                return decimal.Parse((this.sum / this.count).ToString());
        }























    }

    
}
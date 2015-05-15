using System;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AverageAggregate : IAggregate
    {

        Func<NormalCDR, bool> condition;
        MethodInfo propertyGetMethod;
        Func<NormalCDR, Decimal> cdrExpressionToSum;
        decimal sum;
        int count;


        public AverageAggregate(string propertyName, Func<NormalCDR, bool> condition)
        {
            this.propertyGetMethod = typeof(NormalCDR).GetProperty(propertyName).GetGetMethod();
            this.condition = condition;
        }

        public AverageAggregate(Func<NormalCDR, Decimal> cdrExpressionToSum, Func<NormalCDR, bool> condition)
        {
            this.cdrExpressionToSum = cdrExpressionToSum;
            this.condition = condition;
        }

        public void Reset()
        {
            this.sum = 0;
            this.count = 0;
        }

        public void EvaluateCDR(NormalCDR cdr)
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
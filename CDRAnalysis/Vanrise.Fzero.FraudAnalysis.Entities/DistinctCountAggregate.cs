using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DistinctCountAggregate : IAggregate
    {

        Func<NormalCDR, bool> condition;
        MethodInfo propertyGetMethod;
        Func<NormalCDR, Object> cdrExpressionToCountDistinct;
        HashSet<Object> distinctItems = new HashSet<Object>();

        public DistinctCountAggregate(string propertyName, Func<NormalCDR, bool> condition)
        {
            this.propertyGetMethod = typeof(NormalCDR).GetProperty(propertyName).GetGetMethod();
            this.condition = condition;
        }

        public DistinctCountAggregate(Func<NormalCDR, Object> cdrExpressionToCountDistinct, Func<NormalCDR, bool> condition)
        {
            this.cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this.condition = condition;
        }

        public void Reset()
        {
            distinctItems.Clear();
        }

        public void EvaluateCDR(NormalCDR cdr)
        {
            if (this.condition == null || this.condition(cdr))
            {
                if (this.cdrExpressionToCountDistinct != null)
                {
                    distinctItems.Add(this.cdrExpressionToCountDistinct(cdr));
                }

                else
                {
                    distinctItems.Add((String)this.propertyGetMethod.Invoke(cdr, null));
                }
            }
        }

        public decimal GetResult()
        {
            return decimal.Parse(distinctItems.Count().ToString());
        }


    }
    
}
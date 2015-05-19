using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DistinctCountAggregate : IAggregate
    {

        Func<CDR, bool> condition;
        MethodInfo propertyGetMethod;
        Func<CDR, Object> cdrExpressionToCountDistinct;
        HashSet<Object> distinctItems = new HashSet<Object>();

        public DistinctCountAggregate(string propertyName, Func<CDR, bool> condition)
        {
            this.propertyGetMethod = typeof(CDR).GetProperty(propertyName).GetGetMethod();
            this.condition = condition;
        }

        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, bool> condition)
        {
            this.cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this.condition = condition;
        }

        public void Reset()
        {
            distinctItems.Clear();
        }

        public void EvaluateCDR(CDR cdr)
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
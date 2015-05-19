using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
   
    public class CountAggregate:IAggregate
    {
        string conditionExpression;
        Func<CDR, bool> condition;
        int count;


        public CountAggregate(string conditionExpression)
        {
            this.conditionExpression = conditionExpression;
        }


        public CountAggregate(Func<CDR,bool> condition)
        {
            this.condition = condition;
        }

        public void Reset()
        {
            this.count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this.condition == null || this.condition(cdr))
                this.count++;
        }

        public decimal GetResult()
        {
            return decimal.Parse(this.count.ToString());
        }
    }

    
}
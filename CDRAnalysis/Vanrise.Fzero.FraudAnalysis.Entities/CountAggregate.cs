using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
   
    public class CountAggregate:IAggregate
    {
        string conditionExpression;
        Func<NormalCDR, bool> condition;
        int count;


        public CountAggregate(string conditionExpression)
        {
            this.conditionExpression = conditionExpression;
        }


        public CountAggregate(Func<NormalCDR,bool> condition)
        {
            this.condition = condition;
        }

        public void Reset()
        {
            this.count = 0;
        }

        public void EvaluateCDR(NormalCDR cdr)
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
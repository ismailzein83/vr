using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
   
    public class CountAggregate:IAggregate
    {
        Func<CDR, bool> _condition;
        int _count;

                
        public CountAggregate(Func<CDR,bool> condition)
        {
            this._condition = condition;
        }

        public void Reset()
        {
            this._count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
                this._count++;
        }

        public decimal GetResult()
        {
            return _count;
        }
    }

    
}
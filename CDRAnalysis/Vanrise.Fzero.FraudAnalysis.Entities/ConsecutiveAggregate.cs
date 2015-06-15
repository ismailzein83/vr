using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class ConsecutiveAggregate : IAggregate
    {

        Func<CDR, bool> _condition;
              
        DateTime PreviousDateTime = new DateTime();
        int _Count;
        int _MinimumGapBetweenConsecutiveCallsInSeconds;
       

        public ConsecutiveAggregate( Func<CDR, bool> condition, Strategy strategy)
        {
            this._condition = condition;
            this._MinimumGapBetweenConsecutiveCallsInSeconds = strategy.GapBetweenConsecutiveCalls;
        }

        public void Reset()
        {
            PreviousDateTime = new DateTime();
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                if ((PreviousDateTime != new DateTime()) &&  cdr.ConnectDateTime.Value.Subtract(PreviousDateTime).TotalSeconds <= _MinimumGapBetweenConsecutiveCallsInSeconds)
                {
                    _Count++;
                }
                PreviousDateTime = cdr.ConnectDateTime.Value;
            }
        }

        public decimal GetResult()
        {
            return _Count;
        }


    }
    
}
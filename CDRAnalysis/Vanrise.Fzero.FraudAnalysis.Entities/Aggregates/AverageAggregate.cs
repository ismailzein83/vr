using System;
using System.Collections.Generic;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AverageAggregate : IAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, Decimal> _cdrExpressionToSum;
        decimal _sum;
        int _count;

        public AverageAggregate(Func<CDR, bool> condition, Func<CDR, Decimal> cdrExpressionToSum)
        {
            this._condition = condition;
            _cdrExpressionToSum = cdrExpressionToSum;
        }

       
        public void Reset()
        {
            this._sum = 0;
            this._count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                _sum += _cdrExpressionToSum(cdr);
                _count += 1;
            }
          
        }

        public decimal GetResult(Strategy strategy)
        {
            if (this._sum == 0 || this._count == 0)
                return 0;
            else
                return _sum / _count;
        }



        private class AverageAggregateStrategyInfo
        {
            public decimal Sum { get; set; }
            public decimal Count { get; set; }
        }
        



    }


}
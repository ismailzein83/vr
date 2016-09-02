using System;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class AverageAggregate : BaseAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, Decimal> _cdrExpressionToSum;

        public AverageAggregate(Func<CDR, bool> condition, Func<CDR, Decimal> cdrExpressionToSum)
        {
            this._condition = condition;
            this._cdrExpressionToSum = cdrExpressionToSum;
        }


        public override AggregateState CreateState()
        {
            return new AverageAggregateState();
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                AverageAggregateState averageAggregateState = state as AverageAggregateState;
                averageAggregateState.Sum += _cdrExpressionToSum(cdr);
                averageAggregateState.Count += 1;
            }
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters strategy)
        {
            AverageAggregateState averageAggregateState = state as AverageAggregateState;
            if (averageAggregateState.Sum == 0 || averageAggregateState.Count == 0)
                return 0;
            else
                return averageAggregateState.Sum / averageAggregateState.Count;
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            AverageAggregateState existingAverageState = existingState as AverageAggregateState;
            AverageAggregateState newAverageState = newState as AverageAggregateState;
            existingAverageState.Sum += newAverageState.Sum;
            existingAverageState.Count += newAverageState.Count;
        }
    }

    public class AverageAggregateState : AggregateState
    {
        public decimal Sum { get; set; }
        public int Count { get; set; }
    }
}
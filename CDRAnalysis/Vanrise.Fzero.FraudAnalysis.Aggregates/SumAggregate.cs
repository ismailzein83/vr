using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class SumAggregate:BaseAggregate
    {

        Func<CDR, Decimal> _cdrExpressionToSum;
        Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSumWithStrategy;
        List<INumberProfileParameters> _parametersSets;
       

        public SumAggregate(Func<CDR, Decimal> cdrExpressionToSum)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
        }

        public SumAggregate(Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSum, List<INumberProfileParameters> parametersSets)
        {
            this.cdrExpressionToSumWithStrategy = cdrExpressionToSum;
            _parametersSets = parametersSets;
        }

        public override AggregateState CreateState()
        {
            var state = new SumAggregateState();
            if (_parametersSets != null)
            {
                state.ItemsStates = new List<SumAggregateItemState>();
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    state.ItemsStates.Add(new SumAggregateItemState());
                }
            }            
            return state;
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            SumAggregateState sumAggregateState = state as SumAggregateState;
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    var itemState = sumAggregateState.ItemsStates[i];
                    itemState.Sum = itemState.Sum + cdrExpressionToSumWithStrategy(cdr, _parametersSets[i]);
                }
            }
            else
                sumAggregateState.Sum = sumAggregateState.Sum + _cdrExpressionToSum(cdr);
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters parametersSet)
        {
            SumAggregateState sumAggregateState = state as SumAggregateState;
            if (_parametersSets != null)
            {
                int parameterSetIndex = this._parametersSets.IndexOf(parametersSet);
                return sumAggregateState.ItemsStates[parameterSetIndex].Sum;
            }
            else
                return sumAggregateState.Sum;
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            SumAggregateState existingSumAggregateState = existingState as SumAggregateState;
            SumAggregateState newSumAggregateState = newState as SumAggregateState;
            existingSumAggregateState.Sum += newSumAggregateState.Sum;
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    existingSumAggregateState.ItemsStates[i].Sum += newSumAggregateState.ItemsStates[i].Sum;
                }
            }
        }
    }

    public class SumAggregateState : AggregateState
    {
        public decimal Sum { get; set; }

        public List<SumAggregateItemState> ItemsStates { get; set; }
    }

    public class SumAggregateItemState
    {
        public decimal Sum { get; set; }
    }
}
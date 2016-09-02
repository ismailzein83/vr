using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
   
    public class CountAggregate:BaseAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, INumberProfileParameters, bool> _conditionWithStrategy;
        List<INumberProfileParameters> _parametersSets;
        public CountAggregate(Func<CDR,bool> condition)
        {
            this._condition = condition;
        }

        public CountAggregate(Func<CDR, INumberProfileParameters, bool> condition, List<INumberProfileParameters> parametersSets)
        {
            this._conditionWithStrategy = condition;
            this._parametersSets = parametersSets;
        }

        public override AggregateState CreateState()
        {
            var state = new CountAggregateState();
            if (_parametersSets != null)
            {
                state.ItemsStates = new List<CountAggregateItemState>();
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    state.ItemsStates.Add(new CountAggregateItemState());
                }
            }
            return state;
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            CountAggregateState countAggregateState = state as CountAggregateState;
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    var parameterSet = _parametersSets[i];
                    if (_conditionWithStrategy == null || _conditionWithStrategy(cdr, parameterSet))
                    {
                        var itemState = countAggregateState.ItemsStates[i];
                        itemState.Count = itemState.Count + 1;
                    }
                }
            }
            else
            {
                if (this._condition == null || this._condition(cdr))
                    countAggregateState.Count = countAggregateState.Count + 1;
            }
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters parametersSet)
        {
            CountAggregateState countAggregateState = state as CountAggregateState;
            if (_parametersSets != null)
            {
                int parameterSetIndex = this._parametersSets.IndexOf(parametersSet);
                return countAggregateState.ItemsStates[parameterSetIndex].Count;
            }
            else
                return countAggregateState.Count;
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            CountAggregateState existingCountAggregateState = existingState as CountAggregateState;
            CountAggregateState newCountAggregateState = newState as CountAggregateState;
            existingCountAggregateState.Count += newCountAggregateState.Count;
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    existingCountAggregateState.ItemsStates[i].Count += newCountAggregateState.ItemsStates[i].Count;
                }
            }
        }
    }

    public class CountAggregateState : AggregateState
    {
        public int Count { get; set; }

        public List<CountAggregateItemState> ItemsStates { get; set; }
    }

    public class CountAggregateItemState
    {
        public int Count { get; set; }
    }
    
}
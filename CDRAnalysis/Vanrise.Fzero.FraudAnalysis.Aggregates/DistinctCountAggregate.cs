using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class DistinctCountAggregate : BaseAggregate
    {

        Func<CDR, bool> _condition;
        Func<CDR, INumberProfileParameters, bool> _conditionWithParametersSet;        
        Func<CDR, Object> _cdrExpressionToCountDistinct;
        List<INumberProfileParameters> _parametersSets;
        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, bool> condition)
        {
            this._cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this._condition = condition;
        }

        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, INumberProfileParameters, bool> condition, List<INumberProfileParameters> parametersSets)
        {
            this._cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this._conditionWithParametersSet = condition;
            this._parametersSets = parametersSets;
        }

        public override AggregateState CreateState()
        {
            var state = new DistinctCountAggregateState();
            if (_parametersSets != null)
            {
                state.ItemsStates = new List<DistinctCountAggregateItemState>();
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    state.ItemsStates.Add(new DistinctCountAggregateItemState { DistinctItems = new HashSet<object>() });
                }
            }
            else
                state.DistinctItems = new HashSet<object>();
            return state;
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            DistinctCountAggregateState distinctCountAggregateState = state as DistinctCountAggregateState;
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    var parameterSet = _parametersSets[i];
                    if (_conditionWithParametersSet == null || _conditionWithParametersSet(cdr, parameterSet))
                        distinctCountAggregateState.ItemsStates[i].DistinctItems.Add(this._cdrExpressionToCountDistinct(cdr));
                }
            }
            else
            {
                if (this._condition == null || this._condition(cdr))
                    distinctCountAggregateState.DistinctItems.Add(this._cdrExpressionToCountDistinct(cdr));
            }
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters parametersSet)
        {
            DistinctCountAggregateState distinctCountAggregateState = state as DistinctCountAggregateState;
            if (_parametersSets != null)
            {
                int parameterSetIndex = this._parametersSets.IndexOf(parametersSet);
                return distinctCountAggregateState.ItemsStates[parameterSetIndex].DistinctItems.Count();
            }
            else
                return distinctCountAggregateState.DistinctItems.Count();
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            DistinctCountAggregateState existingDistinctCountAggregateState = existingState as DistinctCountAggregateState;
            DistinctCountAggregateState newDistinctCountAggregateState = newState as DistinctCountAggregateState;
            if (existingDistinctCountAggregateState.DistinctItems != null)
            {
                foreach(var itm in newDistinctCountAggregateState.DistinctItems)
                {
                    existingDistinctCountAggregateState.DistinctItems.Add(itm);
                }
            }
            if (_parametersSets != null)
            {
                for (int i = 0; i < _parametersSets.Count; i++)
                {
                    var existingItems = existingDistinctCountAggregateState.ItemsStates[i].DistinctItems;
                    foreach (var item in newDistinctCountAggregateState.ItemsStates[i].DistinctItems)
                    {
                        existingItems.Add(item);
                    }
                }
            }
        }
    }


    public class DistinctCountAggregateState : AggregateState
    {
        public HashSet<Object> DistinctItems { get; set; }

        public List<DistinctCountAggregateItemState> ItemsStates { get; set; }
    }

    public class DistinctCountAggregateItemState
    {
        public DistinctCountAggregateItemState()
        {
            this.DistinctItems = new HashSet<object>();
        }
        public HashSet<Object> DistinctItems { get; set; }
    }

}
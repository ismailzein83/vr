using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class GroupCountAggregate : BaseAggregate
    {

        Func<CDR, INumberProfileParameters, bool> _condition;
        List<INumberProfileParameters> _parametersSets;


        public GroupCountAggregate(Func<CDR, INumberProfileParameters, bool> condition, List<INumberProfileParameters> parametersSets)
        {
            this._condition = condition;
            _parametersSets = parametersSets;
        }

        public override AggregateState CreateState()
        {
            var state = new GroupCountAggregateState();
            state.ItemsStates = new List<GroupCountAggregateItemState>();
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                state.ItemsStates.Add(new GroupCountAggregateItemState { HoursvsCalls = new Dictionary<int,int>() });
            }
            return state;
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            GroupCountAggregateState groupCountAggregateState = state as GroupCountAggregateState;
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                if (this._condition == null || this._condition(cdr, _parametersSets[i]))
                {
                    GroupCountAggregateItemState itemState = groupCountAggregateState.ItemsStates[i];
                    int value;
                    if (itemState.HoursvsCalls.TryGetValue(cdr.ConnectDateTime.Hour, out value))
                        itemState.HoursvsCalls[cdr.ConnectDateTime.Hour] = value + 1;
                    else
                        itemState.HoursvsCalls.Add(cdr.ConnectDateTime.Hour, 1);
                }
            }
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters parametersSet)
        {
            GroupCountAggregateState groupCountAggregateState = state as GroupCountAggregateState;
            decimal count = 0;
            int parameterSetIndex = this._parametersSets.IndexOf(parametersSet);
            foreach (var value in groupCountAggregateState.ItemsStates[parameterSetIndex].HoursvsCalls.Values)
            {
                if (value >= parametersSet.MinimumCountofCallsinActiveHour)
                    count++;
            }
            return count;
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            GroupCountAggregateState existingGroupCountAggregateState = existingState as GroupCountAggregateState;
            GroupCountAggregateState newGroupCountAggregateState = newState as GroupCountAggregateState;
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                GroupCountAggregateItemState existingItemState = existingGroupCountAggregateState.ItemsStates[i];
                GroupCountAggregateItemState newItemState = newGroupCountAggregateState.ItemsStates[i];
                foreach(var newHourCount in newItemState.HoursvsCalls)
                {
                    int value;
                    if (existingItemState.HoursvsCalls.TryGetValue(newHourCount.Key, out value))
                        existingItemState.HoursvsCalls[newHourCount.Key] = value + 1;
                    else
                        existingItemState.HoursvsCalls.Add(newHourCount.Key, 1);
                }
            }
        }
    }

    public class GroupCountAggregateState : AggregateState
    {
        public List<GroupCountAggregateItemState> ItemsStates { get; set; }
    }

    public class GroupCountAggregateItemState
    {
        public Dictionary<int, int> HoursvsCalls { get; set; }
    }
    
}
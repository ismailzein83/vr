using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class ConsecutiveAggregate : BaseAggregate
    {

        Func<CDR, INumberProfileParameters, bool> _condition;

        Func<INumberProfileParameters, int> _getGapDistance;

        List<INumberProfileParameters> _parametersSets;

        public ConsecutiveAggregate(Func<CDR, INumberProfileParameters, bool> condition, List<INumberProfileParameters> parametersSets, Func<INumberProfileParameters, int> getGapDistance)
        {
            this._getGapDistance = getGapDistance;
            this._condition = condition;
            this._parametersSets = parametersSets; 
        }

        public override AggregateState CreateState()
        {
            var state = new ConsecutiveAggregateState();
            state.ItemsStates = new List<ConsecutiveAggregateItemState>();
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                state.ItemsStates.Add(new ConsecutiveAggregateItemState { CDRTimes = new List<DateTime>() });
            }
            return state;
        }

        public override void Evaluate(AggregateState state, CDR cdr)
        {
            ConsecutiveAggregateState consecutiveAggregateState = state as ConsecutiveAggregateState;
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                if (this._condition == null || this._condition(cdr, _parametersSets[i]))
                {
                    consecutiveAggregateState.ItemsStates[i].CDRTimes.Add(cdr.ConnectDateTime);
                }
            }
        }

        public override decimal GetResult(AggregateState state, INumberProfileParameters parametersSet)
        {
            ConsecutiveAggregateState consecutiveAggregateState = state as ConsecutiveAggregateState;
            Decimal count = 0;
            DateTime previousCDRTime = default(DateTime);
            int parameterSetIndex = this._parametersSets.IndexOf(parametersSet);
            foreach (var cdrTime in consecutiveAggregateState.ItemsStates[parameterSetIndex].CDRTimes.OrderBy(itm => itm))
            {
                if (previousCDRTime != default(DateTime) && cdrTime.Subtract(previousCDRTime).TotalSeconds <= this._getGapDistance(parametersSet))
                    count++;
                previousCDRTime = cdrTime;
            }
            return count > 0 ? (count + 1) : count;
        }

        public override void UpdateExistingFromNew(AggregateState existingState, AggregateState newState)
        {
            ConsecutiveAggregateState existingConsecutiveAggregateState = existingState as ConsecutiveAggregateState;
            ConsecutiveAggregateState newConsecutiveAggregateState = newState as ConsecutiveAggregateState;
            for (int i = 0; i < _parametersSets.Count; i++)
            {
                existingConsecutiveAggregateState.ItemsStates[i].CDRTimes.AddRange(newConsecutiveAggregateState.ItemsStates[i].CDRTimes);
            }
        }
    }

    public class ConsecutiveAggregateState : AggregateState
    {
        public List<ConsecutiveAggregateItemState> ItemsStates { get; set; }
    }
    
    public class ConsecutiveAggregateItemState
    {
        public List<DateTime> CDRTimes { get; set; }
    }
}
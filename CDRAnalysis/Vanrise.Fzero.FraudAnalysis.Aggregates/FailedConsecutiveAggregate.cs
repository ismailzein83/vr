using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class FailedConsecutiveAggregate : IAggregate
    {

        Func<CDR, INumberProfileParameters, bool> _condition;

        Dictionary<INumberProfileParameters, ConsecutiveAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _parameters;

        public FailedConsecutiveAggregate(Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> parameters)
        {
            this._condition = condition;
            _strategiesInfo = new Dictionary<INumberProfileParameters, ConsecutiveAggregateStrategyInfo>();
            _parameters = parameters;
            foreach (var strategy in _parameters)
                _strategiesInfo.Add(strategy, new ConsecutiveAggregateStrategyInfo());
        }

        public void Reset()
        {
            foreach (var strategyCountEntry in _strategiesInfo)
            {
                strategyCountEntry.Value.Count = 0;
                strategyCountEntry.Value.PreviousDateTime = default(DateTime);
            }
        }

        public void EvaluateCDR(CDR cdr)
        {
            foreach (var strategyCountEntry in _strategiesInfo)
            {
                if (this._condition == null || this._condition(cdr, strategyCountEntry.Key))
                {
                    if ((strategyCountEntry.Value.PreviousDateTime != DateTime.MinValue) && cdr.ConnectDateTime.Value.Subtract(strategyCountEntry.Value.PreviousDateTime).TotalSeconds <= strategyCountEntry.Key.GapBetweenFailedConsecutiveCalls)
                    {
                        strategyCountEntry.Value.Count++;
                    }
                    strategyCountEntry.Value.PreviousDateTime = cdr.ConnectDateTime.Value;
                }
            }
        }



        public decimal GetResult(INumberProfileParameters strategy)
        {
            return (_strategiesInfo[strategy].Count > 0 ? ++_strategiesInfo[strategy].Count : _strategiesInfo[strategy].Count);
        }

        private class ConsecutiveAggregateStrategyInfo
        {
            public int Count { get; set; }

            public DateTime PreviousDateTime { get; set; }
        }
    }
    
}
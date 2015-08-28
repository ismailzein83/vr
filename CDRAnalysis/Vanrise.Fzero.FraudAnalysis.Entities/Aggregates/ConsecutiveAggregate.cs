using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class ConsecutiveAggregate : IAggregate
    {

        Func<CDR, Strategy, bool> _condition;
              
        Dictionary<Strategy, ConsecutiveAggregateStrategyInfo> _strategiesInfo;
        List<Strategy> _strategies;

        public ConsecutiveAggregate(Func<CDR, Strategy, bool> condition, List<Strategy> strategies)
        {
            this._condition = condition;
            _strategiesInfo = new Dictionary<Strategy, ConsecutiveAggregateStrategyInfo>();
            _strategies = strategies;
            foreach (var strategy in _strategies)
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
                    if ((strategyCountEntry.Value.PreviousDateTime != DateTime.MinValue) && cdr.ConnectDateTime.Value.Subtract(strategyCountEntry.Value.PreviousDateTime).TotalSeconds <= strategyCountEntry.Key.GapBetweenConsecutiveCalls)
                    {
                        strategyCountEntry.Value.Count++;
                    }
                    strategyCountEntry.Value.PreviousDateTime = cdr.ConnectDateTime.Value;
                }
            }
        }



        public decimal GetResult(Strategy strategy)
        {
            return _strategiesInfo[strategy].Count;
        }

        private class ConsecutiveAggregateStrategyInfo
        {
            public int Count { get; set; }

            public DateTime PreviousDateTime { get; set; }
        }
    }
    
}
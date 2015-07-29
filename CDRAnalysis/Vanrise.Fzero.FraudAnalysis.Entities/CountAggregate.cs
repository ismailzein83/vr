using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
   
    public class CountAggregate:IAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, Strategy, bool> _conditionWithStrategy;
        int _count;
        Dictionary<Strategy, CountAggregateStrategyInfo> _strategiesInfo;
        List<Strategy> _strategies;
        public CountAggregate(Func<CDR,bool> condition)
        {
            this._condition = condition;
        }

        public CountAggregate(Func<CDR, Strategy, bool> condition, List<Strategy> strategies)
        {
            this._conditionWithStrategy = condition;
            _strategiesInfo = new Dictionary<Strategy, CountAggregateStrategyInfo>();
            _strategies = strategies;
            foreach (var strategy in _strategies)
                _strategiesInfo.Add(strategy, new CountAggregateStrategyInfo());
        }

        public void Reset()
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategyCountEntry in _strategiesInfo)
                {
                    strategyCountEntry.Value.Count = 0;
                }
            }
            else
                this._count = 0;
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategyCountEntry in _strategiesInfo)
                {
                    if (_conditionWithStrategy != null && _conditionWithStrategy(cdr, strategyCountEntry.Key))
                        strategyCountEntry.Value.Count++;
                }
            }
            else
            {
                if (this._condition == null || this._condition(cdr))
                    this._count++;
            }
        }

        public decimal GetResult(Strategy strategy)
        {
            if (_strategiesInfo != null)
                return _strategiesInfo[strategy].Count;
            else
                return _count;
        }

        private class CountAggregateStrategyInfo
        {
            public int Count { get; set; }
        }
    }

    
}
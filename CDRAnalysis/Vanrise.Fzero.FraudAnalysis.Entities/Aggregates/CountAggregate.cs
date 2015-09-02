using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
   
    public class CountAggregate:IAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, INumberProfileParameters, bool> _conditionWithStrategy;
        int _count;
        Dictionary<INumberProfileParameters, CountAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _strategies;
        public CountAggregate(Func<CDR,bool> condition)
        {
            this._condition = condition;
        }

        public CountAggregate(Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> strategies)
        {
            this._conditionWithStrategy = condition;
            _strategiesInfo = new Dictionary<INumberProfileParameters, CountAggregateStrategyInfo>();
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

        public decimal GetResult(INumberProfileParameters strategy)
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
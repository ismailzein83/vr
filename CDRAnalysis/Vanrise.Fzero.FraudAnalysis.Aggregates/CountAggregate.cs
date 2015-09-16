using System;
using System.Collections.Generic;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
   
    public class CountAggregate:IAggregate
    {
        Func<CDR, bool> _condition;
        Func<CDR, INumberProfileParameters, bool> _conditionWithStrategy;
        int _count;
        Dictionary<INumberProfileParameters, CountAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _parameters;
        public CountAggregate(Func<CDR,bool> condition)
        {
            this._condition = condition;
        }

        public CountAggregate(Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> parameters)
        {
            this._conditionWithStrategy = condition;
            _strategiesInfo = new Dictionary<INumberProfileParameters, CountAggregateStrategyInfo>();
            _parameters = parameters;
            foreach (var strategy in _parameters)
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

        public decimal GetResult(INumberProfileParameters parameters)
        {
            if (_strategiesInfo != null)
                return _strategiesInfo[parameters].Count;
            else
                return _count;
        }

        private class CountAggregateStrategyInfo
        {
            public int Count { get; set; }
        }
    }

    
}
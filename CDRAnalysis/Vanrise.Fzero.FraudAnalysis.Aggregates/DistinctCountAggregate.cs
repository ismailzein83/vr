using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class DistinctCountAggregate : IAggregate
    {

        Func<CDR, bool> _condition;
        Func<CDR, INumberProfileParameters, bool> _conditionWithStrategy;
        //MethodInfo _propertyGetMethod;
        Func<CDR, Object> _cdrExpressionToCountDistinct;
        HashSet<Object> _distinctItems = new HashSet<Object>();

        Dictionary<INumberProfileParameters, DistinctCountAggregateStrategyInfo> _strategiesInfo = new Dictionary<INumberProfileParameters, DistinctCountAggregateStrategyInfo>();
       

        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, bool> condition)
        {
            this._cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this._condition = condition;
        }

        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> parameters)
        {
            this._cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this._conditionWithStrategy = condition;
            foreach (var strategy in parameters)
                _strategiesInfo.Add(strategy, new DistinctCountAggregateStrategyInfo());
        }

        public void Reset()
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategyCountEntry in _strategiesInfo)
                {
                    strategyCountEntry.Value.DistinctItems.Clear();
                }
            }
            else
                _distinctItems.Clear();
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategyCountEntry in _strategiesInfo)
                {
                    if (_conditionWithStrategy != null && _conditionWithStrategy(cdr, strategyCountEntry.Key))
                        strategyCountEntry.Value.DistinctItems.Add(this._cdrExpressionToCountDistinct(cdr));
                }
            }
            else
            {
                if (this._condition == null || this._condition(cdr))
                    _distinctItems.Add(this._cdrExpressionToCountDistinct(cdr));
            }
        }

        public decimal GetResult(INumberProfileParameters strategy)
        {
            if (_strategiesInfo != null)
                return _strategiesInfo[strategy].DistinctItems.Count();
            else
                return _distinctItems.Count(); 
        }

        private class DistinctCountAggregateStrategyInfo
        {
            public DistinctCountAggregateStrategyInfo()
            {
                this.DistinctItems = new HashSet<object>();
            }
            public HashSet<Object> DistinctItems { get; set; }
        }
    }
    
}
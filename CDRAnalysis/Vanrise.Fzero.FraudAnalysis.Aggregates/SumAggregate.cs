﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class SumAggregate:IAggregate
    {

        Func<CDR, Decimal> _cdrExpressionToSum;
        Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSumWithStrategy;
        decimal _sum;
        Dictionary<INumberProfileParameters, SumAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _parameters;
       

        public SumAggregate(Func<CDR, Decimal> cdrExpressionToSum)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
        }

        public SumAggregate(Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSum, IEnumerable<INumberProfileParameters> parameters)
        {
            this.cdrExpressionToSumWithStrategy = cdrExpressionToSum;
            _strategiesInfo = new Dictionary<INumberProfileParameters, SumAggregateStrategyInfo>();
            _parameters = parameters;
            foreach (var strategy in _parameters)
                _strategiesInfo.Add(strategy, new SumAggregateStrategyInfo ());
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategySumEntry in _strategiesInfo)
                {
                    strategySumEntry.Value.Sum = strategySumEntry.Value.Sum + cdrExpressionToSumWithStrategy(cdr, strategySumEntry.Key);
                }
            }
            else
                _sum += _cdrExpressionToSum(cdr);
        }

        public decimal GetResult(INumberProfileParameters strategy)
        {
            if (_strategiesInfo != null)
                return _strategiesInfo[strategy].Sum;
            else
                return _sum;
        }

        private class SumAggregateStrategyInfo
        {
            public decimal Sum { get; set; }
        }
    }

    
}
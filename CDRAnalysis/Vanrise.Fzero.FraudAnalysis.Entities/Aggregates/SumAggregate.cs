﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SumAggregate:IAggregate
    {

        Func<CDR, Decimal> _cdrExpressionToSum;
        Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSumWithStrategy;
        decimal _sum;
        Dictionary<INumberProfileParameters, SumAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _strategies;
       

        public SumAggregate(Func<CDR, Decimal> cdrExpressionToSum)
        {
            _cdrExpressionToSum = cdrExpressionToSum;
        }

        public SumAggregate(Func<CDR, INumberProfileParameters, Decimal> cdrExpressionToSum, IEnumerable<INumberProfileParameters> strategies)
        {
            this.cdrExpressionToSumWithStrategy = cdrExpressionToSum;
            _strategiesInfo = new Dictionary<INumberProfileParameters, SumAggregateStrategyInfo>();
            _strategies = strategies;
            foreach (var strategy in _strategies)
                _strategiesInfo.Add(strategy, new SumAggregateStrategyInfo ());
        }

        public void Reset()
        {
            if (_strategiesInfo != null)
            {
                foreach (var strategyCountEntry in _strategiesInfo)
                {
                    strategyCountEntry.Value.Sum = 0;
                }
            }
            else
                _sum = 0;
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class GroupCountAggregate : IAggregate
    {

        Func<CDR, INumberProfileParameters, bool> _condition;

        Dictionary<INumberProfileParameters, GroupCountAggregateStrategyInfo> _strategiesInfo;
        IEnumerable<INumberProfileParameters> _parameters;


        public GroupCountAggregate(Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> parameters)
        {
            this._condition = condition;
            _strategiesInfo = new Dictionary<INumberProfileParameters, GroupCountAggregateStrategyInfo>();
            _parameters = parameters;
            foreach (var strategy in _parameters)
                _strategiesInfo.Add(strategy, new GroupCountAggregateStrategyInfo());
        }

        public void EvaluateCDR(CDR cdr)
        {
            foreach (var strategyCountEntry in _strategiesInfo)
            {
                if (this._condition == null || this._condition(cdr, strategyCountEntry.Key))
                {
                    int value;

                    if (!strategyCountEntry.Value.HoursvsCalls.TryGetValue(cdr.ConnectDateTime.Hour, out value))
                    {
                        strategyCountEntry.Value.HoursvsCalls.Add(cdr.ConnectDateTime.Hour, 1);
                    }
                    else
                    {
                        strategyCountEntry.Value.HoursvsCalls[cdr.ConnectDateTime.Hour] = value + 1;
                    }
                }
            }
        }

        public decimal GetResult(INumberProfileParameters strategy)
        {
            decimal count = 0;
            foreach(var value in _strategiesInfo[strategy].HoursvsCalls.Values)
            {
                if (value >= strategy.MinimumCountofCallsinActiveHour)
                    count++;
            }
            return count;
        }

        private class GroupCountAggregateStrategyInfo
        {
            public GroupCountAggregateStrategyInfo()
            {
                this.HoursvsCalls = new Dictionary<int, int>();
            }

            public Dictionary<int, int> HoursvsCalls { get; set; }
        }

    }
    
}
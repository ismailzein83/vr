using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class ConsecutiveAggregate : IAggregate
    {

        Func<CDR, INumberProfileParameters, bool> _condition;

        Dictionary<INumberProfileParameters, ConsecutiveAggregateStrategyInfo> _parameterSetsExecutionInfo;

        Func<INumberProfileParameters, int> _getGapDistance;

        public ConsecutiveAggregate(Func<CDR, INumberProfileParameters, bool> condition, IEnumerable<INumberProfileParameters> parameterSets, Func<INumberProfileParameters, int> getGapDistance)
        {
            this._getGapDistance = getGapDistance;
            this._condition = condition;
            _parameterSetsExecutionInfo = new Dictionary<INumberProfileParameters, ConsecutiveAggregateStrategyInfo>();

            foreach (var strategy in parameterSets)
                _parameterSetsExecutionInfo.Add(strategy, new ConsecutiveAggregateStrategyInfo { CDRTimes = new List<DateTime>() });
        }

        public void EvaluateCDR(CDR cdr)
        {
            foreach (var strategyCountEntry in _parameterSetsExecutionInfo)
            {
                if (this._condition == null || this._condition(cdr, strategyCountEntry.Key))
                {                    
                    strategyCountEntry.Value.CDRTimes.Add(cdr.ConnectDateTime);
                }
            }
        }

        public decimal GetResult(INumberProfileParameters parameterSet)
        {
            Decimal count = 0;
            DateTime previousCDRTime = default(DateTime);
            foreach (var cdrTime in _parameterSetsExecutionInfo[parameterSet].CDRTimes.OrderBy(itm => itm))
            {
                if (previousCDRTime != default(DateTime) && cdrTime.Subtract(previousCDRTime).TotalSeconds <= this._getGapDistance(parameterSet))
                    count++;
                previousCDRTime = cdrTime;
            }
            return count > 0 ? (count + 1) : count;
        }

        private class ConsecutiveAggregateStrategyInfo
        {
            public List<DateTime> CDRTimes { get; set; }
        }
    }
    
}
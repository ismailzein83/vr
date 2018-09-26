using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcGetMeasureValueContext : IDAProfCalcGetCalculationValueContext
    {
        Dictionary<string, dynamic> _groupingValues;
        Dictionary<string, dynamic> _aggregateValues;
        public DAProfCalcGetMeasureValueContext(Dictionary<string, dynamic> groupingValues, Dictionary<string, dynamic> aggregateValues)
        {
            _groupingValues = groupingValues;
            _aggregateValues = aggregateValues;

        }
        public dynamic GetAggregateValue(string aggregateName)
        {
            if (_aggregateValues == null)
                throw new NullReferenceException("_aggregateValues");

            dynamic aggregateValue;
            if (!_aggregateValues.TryGetValue(aggregateName, out aggregateValue))
                throw new Exception(string.Format("_aggregateValues doesn't contain {0}", aggregateName));

            return aggregateValue;
        }

        public dynamic GetGroupingValue(string groupingName)
        {
            if (_groupingValues == null)
                return null;

            dynamic groupingValue;
            if (!_groupingValues.TryGetValue(groupingName, out groupingValue))
                return null;

            return groupingValue;
        }

        public bool IsGroupingValueIncluded(string groupingName)
        {
            if (_groupingValues == null)
                return false;

            dynamic groupingValue;
            if (!_groupingValues.TryGetValue(groupingName, out groupingValue))
                return false;

            return true;
        }
    }
}
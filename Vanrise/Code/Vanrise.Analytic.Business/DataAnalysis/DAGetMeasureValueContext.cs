using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcGetMeasureValueContext : IDAProfCalcGetCalculationValueContext
    {
        Dictionary<string, dynamic> _groupingValues;
        Dictionary<string, dynamic> _aggregateValues;
        Dictionary<string, dynamic> _parameterValues;
        Dictionary<string, dynamic> _globalParameterValues;

        public DAProfCalcGetMeasureValueContext(Dictionary<string, dynamic> groupingValues, Dictionary<string, dynamic> aggregateValues, Dictionary<string, dynamic> parameterValues, Guid outputItemDefinitionId)
        {
            _groupingValues = groupingValues;
            _aggregateValues = aggregateValues;
            _parameterValues = parameterValues;

            var parameterSettings = new ConfigManager().GetDataAnalysisItemParametersSettings(outputItemDefinitionId);
            if(parameterSettings != null && parameterSettings.ParameterValues != null)
                _globalParameterValues = parameterSettings.ParameterValues;
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

        public dynamic GetParameterValue(string parameterName)
        {
            if (_parameterValues != null && _parameterValues.TryGetValue(parameterName, out dynamic parameterValue))
            {
                if (_parameterValues.TryGetValue($"Override{parameterName}", out dynamic overriddenValue))
                {
                    if (overriddenValue)
                        return parameterValue;
                    else
                        return GetGlobalParameterValue(parameterName);
                }
                else
                {
                    return parameterValue;
                }
            }

            return GetGlobalParameterValue(parameterName);
        }

        private dynamic GetGlobalParameterValue(string parameterName)
        {
            if (_globalParameterValues != null && _globalParameterValues.TryGetValue(parameterName, out dynamic globalParameterValue))
                return globalParameterValue;

            return null;
        }
    }
}
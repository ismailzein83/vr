using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteRuleQualityConfigurationEvaluator
    {
        decimal GetQualityValue(IRouteRuleGetQualityValueContext context);
    }

    public interface IRouteRuleGetQualityValueContext
    {
        dynamic GetMeasureValue(string measureName);
    }

    public class RouteRuleGetQualityValueContext : IRouteRuleGetQualityValueContext
    {
        MeasureValues _measureValues;

        public RouteRuleGetQualityValueContext(AnalyticRecord analyticRecord)
        {
            _measureValues = analyticRecord.MeasureValues;
        }

        public dynamic GetMeasureValue(string measureName)
        {
            MeasureValue measureValue;
            if (!_measureValues.TryGetValue(measureName, out measureValue))
                return null;

            return measureValue.Value;
        }
    }
}
using System;

namespace Vanrise.Analytic.Entities
{
    public interface IDAProfCalcCalculationEvaluator
    {
        dynamic GetCalculationValue(IDAProfCalcGetCalculationValueContext context);
    }

    public interface IDAProfCalcGetCalculationValueContext
    {
        dynamic GetAggregateValue(string aggregateName);

        bool IsGroupingValueIncluded(string groupingName);

        dynamic GetGroupingValue(string groupingName);
    }
}
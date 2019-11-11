using System;

namespace Vanrise.Analytic.Entities
{
    public interface IDAProfCalcAggregationEvaluator
    {
        bool IsExpressionMatched(IDAProfCalcIsExpressionMatchedContext context);
    }

    public interface IDAProfCalcIsExpressionMatchedContext
    {
        bool IsGroupingValueIncluded(string groupingName);

        dynamic GetGroupingValue(string groupingName);

        dynamic GetParameterValue(string parameterName);

        dynamic GetDataRecordValue(string fieldName);
    }
}
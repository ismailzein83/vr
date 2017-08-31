using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IMeasureEvaluator
    {
        //string GetMeasureExpression(IGetMeasureExpressionContext context);

        dynamic GetMeasureValue(IGetMeasureValueContext context);
    }

    public interface IGetMeasureExpressionContext
    {
        string GetMeasureExpression(string measureConfigName);

        bool IsGroupingDimensionIncluded(string dimensionName);
    }

    public interface IGetMeasureValueContext
    {
        dynamic GetAggregateValue(string aggregateName);

        bool IsGroupingDimensionIncluded(string dimensionName);

        bool IsFilterIncluded(string filterName);

        List<dynamic> GetAllDimensionValues(string dimensionName);

        List<dynamic> GetDistinctDimensionValues(string dimensionName);

        dynamic GetExternalSourceMatchRecordMeasureValue(string sourceName, string measureName);

        DateTime GetQueryFromTime();

        DateTime GetQueryToTime();
    }

}

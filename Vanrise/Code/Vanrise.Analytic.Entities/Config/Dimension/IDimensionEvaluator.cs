using System;

namespace Vanrise.Analytic.Entities
{
    public interface IDimensionEvaluator
    {
        dynamic GetDimensionValue(IGetDimensionValueContext context);
    }

    public interface IGetDimensionValueContext
    {
        bool FromUIReport { get; }

        dynamic GetDimensionValue(string dimensionName);
        dynamic GetDimensionDescription(string dimensionName);
        dynamic GetQueryParameter(string parameterName);
        DateTime GetQueryFromTime();
        DateTime GetQueryToTime();
    }
}
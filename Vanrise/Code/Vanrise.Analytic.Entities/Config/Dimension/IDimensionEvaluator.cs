using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IDimensionEvaluator
    {
        dynamic GetDimensionValue(IGetDimensionValueContext context);
    }

    public interface IGetDimensionValueContext
    {
        dynamic GetDimensionValue(string dimensionName);
        dynamic GetDimensionDescription(string dimensionName);
        DateTime GetQueryFromTime();
        DateTime GetQueryToTime();
    }
}

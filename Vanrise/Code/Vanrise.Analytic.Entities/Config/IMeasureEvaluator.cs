﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IMeasureEvaluator
    {
        string GetMeasureExpression(IGetMeasureExpressionContext context);
    }

    public interface IGetMeasureExpressionContext
    {
        string GetMeasureExpression(string measureConfigName);

        //bool IsGroupingDimensionIncluded(string dimensionName);
    }

}

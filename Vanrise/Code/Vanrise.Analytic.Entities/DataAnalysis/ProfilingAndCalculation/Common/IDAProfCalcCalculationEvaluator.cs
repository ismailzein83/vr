﻿using System;

namespace Vanrise.Analytic.Entities
{
    public interface IDAProfCalcCalculationEvaluator
    {
        dynamic GetCalculationValue(IDAProfCalcGetCalculationValueContext context);
    }

    public interface IDAProfCalcGetCalculationValueContext
    {
        dynamic GetAggregateValue(string aggregateName);

        dynamic GetGroupingValue(string groupingName);
    }
}
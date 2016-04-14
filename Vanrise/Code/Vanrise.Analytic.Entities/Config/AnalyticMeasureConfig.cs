﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticSummaryFunction { Count = 0, Sum = 1, Avg = 2, Max = 3, Min = 4 }

    public class AnalyticMeasureConfig
    {
        public string GetSQLColumnsExpression { get; set; }

        public string GetMeasureValueExpression { get; set; }

        public List<string> DependentMeasureConfigNames { get; set; }

        public List<string> JoinConfigNames { get; set; }

        public AnalyticSummaryFunction SummaryFunction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum ConfigurationType
    {
        Dimension = 0,
        Measure = 1,
        Filter = 2,
        Index = 3
    }
    public enum AnalyticSummary
    {
        Sum,
        Max,
        Avg
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticAggregateType {  Count = 1, Sum = 2, Max = 3, Min = 4}

    public class AnalyticAggregateConfig
    {
        public string SQLColumn { get; set; }

        public AnalyticAggregateType AggregateType { get; set; }

        public List<string> JoinConfigNames { get; set; }
    }
}

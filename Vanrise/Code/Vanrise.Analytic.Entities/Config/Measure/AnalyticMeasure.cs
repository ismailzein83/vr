using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasure
    {
        public Guid AnalyticMeasureConfigId { get; set; }

        public AnalyticMeasureConfig Config { get; set; }

        public IMeasureEvaluator Evaluator { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimension
    {
        public Guid AnalyticDimensionConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public AnalyticDimensionConfig Config { get; set; }

        public IDimensionEvaluator Evaluator { get; set; }
    }
}

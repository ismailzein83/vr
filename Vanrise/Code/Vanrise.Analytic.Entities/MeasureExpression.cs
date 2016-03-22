using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class MeasureExpression
    {
        public string SQLExpression { get; set; }
        public string CustomExpression { get; set; }
        public string ExpressionSummary { get; set; }
    }
}

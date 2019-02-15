using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticMeasureKPIRange
    {
        public decimal From { get; set; }
        public decimal To { get; set; }
        public string Name { get; set; }
        public Guid StyleDefinitionId { get; set; }
    }
}

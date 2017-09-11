using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimensionConfigInfoFilter
    {
        public List<Guid> TableIds { get; set; }
        public bool HideIsRequiredFromParent { get; set; }
    }
}

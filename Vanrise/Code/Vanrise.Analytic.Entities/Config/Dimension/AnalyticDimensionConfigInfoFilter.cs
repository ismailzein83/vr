using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimensionConfigInfoFilter
    {
        public List<Guid> TableIds { get; set; }

        public bool HideIsRequiredFromParent { get; set; }

        public bool IncludeTechnicalDimension { get; set; }
    }
}
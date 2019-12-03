using System;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcOutputFieldFilter
    {
        public DAProfCalcOutputFieldType? DAProfCalcOutputFieldType { get; set; }

        public bool IncludeTechnicalField { get; set; }
    }
}
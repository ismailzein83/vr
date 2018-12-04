using System;

namespace Retail.RA.Entities
{
    public class PeriodDefinition
    {
        public int PeriodDefinitionId { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}

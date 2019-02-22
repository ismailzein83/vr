using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class TechnicalNumberPlan
    {
        public int Id { get; set; }
        public string ZoneName { get; set; }
        public TechnicalNumberPlanSettings TechnicalNumberPlanSettings { get; set; }

    }
}

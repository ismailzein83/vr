using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class TechnicalNumberPlan
    {
        public List<ZoneCode> Codes { get; set; }
    }

    public class ZoneCode
    {
        public string Code { get; set; }
    }
}

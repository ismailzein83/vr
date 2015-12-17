using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP;

namespace TOne.WhS.CodePreparation.Entities
{
    public class SaveChangesInput
    {
        public int SellingNumberPlanId { get; set; }
        public Changes NewChanges { get; set; }
    }

    public class NewZoneInput
    {
        public int SellingNumberPlanId { get; set; }
        public NewZone NewZone { get; set; }
    }

    public class NewCodeInput
    {
        public int SellingNumberPlanId { get; set; }
        public int? ZoneId { get; set; }
        public ZoneItemStatus Status { get; set; }
        public NewCode NewCode { get; set; }
    }

    public class GetCodeItemInput {
        public int SellingNumberPlanId { get; set; }
        public int? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public ZoneItemStatus Status { get; set; }
    }
}

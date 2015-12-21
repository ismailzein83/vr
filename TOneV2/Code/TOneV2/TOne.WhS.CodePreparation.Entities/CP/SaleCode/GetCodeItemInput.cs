using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class GetCodeItemInput
    {
        public int SellingNumberPlanId { get; set; }
        public int? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public ZoneItemStatus Status { get; set; }
    }
}

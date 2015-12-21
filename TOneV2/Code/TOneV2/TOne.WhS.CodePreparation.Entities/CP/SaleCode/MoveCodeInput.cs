using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class MoveCodeInput
    {
        public int SellingNumberPlanId { get; set; }
        public string ZoneName { get; set; }
        public string OtherZoneName { get; set; }
        public ZoneItemStatus Status { get; set; }
        public List<string> Codes { get; set; }
    }
}

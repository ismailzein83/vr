using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class GetCodeItemInput
    {
        public int SellingNumberPlanId { get; set; }
        public int? ZoneId { get; set; }
        public string ZoneName { get; set; }

        public string RenamedZone { get; set; }
        public int CountryId { get; set; }
        public ZoneItemDraftStatus Status { get; set; }
    }
}

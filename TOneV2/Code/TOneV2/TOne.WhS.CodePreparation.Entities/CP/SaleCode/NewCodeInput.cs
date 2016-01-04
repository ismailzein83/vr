using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class NewCodeInput
    {
        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }
        public int? ZoneId { get; set; }
        public ZoneItemStatus Status { get; set; }
        public List<NewCode> NewCodes { get; set; }
        public DeletedCode DeletedCode { get; set; }
    }
}

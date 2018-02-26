using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ANumberSaleCode : ICode, Vanrise.Entities.IDateEffectiveSettings
    {
        public long ANumberSaleCodeId { get; set; }

        public int ANumberGroupId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}

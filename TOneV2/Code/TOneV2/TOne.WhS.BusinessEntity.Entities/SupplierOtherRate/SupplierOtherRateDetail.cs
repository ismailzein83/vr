using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierOtherRateDetail
    {
        public SupplierOtherRate Entity { get; set; }

        public string RateTypeDescription { get; set; }

        public string DisplayedCurrency { get; set; }

        public decimal DisplayedRate { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ProductPackageItem
    {
        public int PackageId { get; set; }

        public int Priority { get; set; }

        public AccountChargeEvaluator ChargeEvaluator { get; set; }
    }
}

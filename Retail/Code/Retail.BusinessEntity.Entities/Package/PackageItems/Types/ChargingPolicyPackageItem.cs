using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ChargingPolicyPackageItem : PackageItemSettings
    {
        public ChargingPolicySettings ChargingPolicySettings { get; set; }
    }
}

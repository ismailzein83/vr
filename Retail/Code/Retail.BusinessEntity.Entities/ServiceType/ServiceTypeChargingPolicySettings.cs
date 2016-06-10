using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ServiceTypeChargingPolicySettings
    {
        public Dictionary<int, ServiceTypeChargingPolicyPart> PartsByTypeId { get; set; }
    }

    public class ServiceTypeChargingPolicyPart
    {        
        public int PartConfigId { get; set; }

        public ChargingPolicyPartSettings PartSettings { get; set; }
    }
}

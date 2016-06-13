using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseChargingPolicy
    {
        public int ChargingPolicyId { get; set; }

        public string Name { get; set; }

        public ChargingPolicySettings Settings { get; set; }
    }

    public class ChargingPolicy : BaseChargingPolicy
    {
        public int ServiceTypeId { get; set; }
    }

    public class ChargingPolicyToEdit : BaseChargingPolicy
    {

    }
}

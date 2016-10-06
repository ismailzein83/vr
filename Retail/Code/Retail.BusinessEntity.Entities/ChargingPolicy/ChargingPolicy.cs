using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseChargingPolicy
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_ChargingPolicy";

        public int ChargingPolicyId { get; set; }

        public string Name { get; set; }

        public ChargingPolicySettings Settings { get; set; }
    }

    public class ChargingPolicy : BaseChargingPolicy
    {
        public Guid ServiceTypeId { get; set; }
    }

    public class ChargingPolicyToEdit : BaseChargingPolicy
    {

    }
}

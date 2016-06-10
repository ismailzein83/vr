using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeSettings
    {
        public string Description { get; set; }

        public string ChargingPolicyEditor { get; set; }

        public ServiceTypeChargingPolicySettings ChargingPolicySettings { get; set; }

        public string AccountServiceEditor { get; set; }

        public string ServiceVolumeEditor { get; set; }
    }
}

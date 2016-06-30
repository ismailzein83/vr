using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicySettings
    {
        public List<ChargingPolicyPart> Parts { get; set; }

        /// <summary>
        /// has value if it only has chargeable status in the service type's supported statuses
        /// </summary>
        public int? StatusChargingSetId { get; set; }
    }
}

using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Data.Entities
{
    public class DataChargingPolicyDefinitionSettings : ChargingPolicyDefinitionSettings
    {
        public override string ChargingPolicyEditor
        {
            get
            {
                return "retail-data-chargingpolicy-settings";
            }
            set
            {

            }
        }
    }
}

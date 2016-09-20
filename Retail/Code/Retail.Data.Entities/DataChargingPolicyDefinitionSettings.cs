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

        public override Guid ConfigId { get { return new Guid("4e993efd-6afe-4c3a-aca2-83cd5c8ffc35"); } }

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

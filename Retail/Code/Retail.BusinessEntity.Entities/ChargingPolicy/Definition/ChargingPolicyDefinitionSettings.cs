using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyDefinitionSettings
    {
        public int ConfigId { get; set; }

        public Dictionary<int, ChargingPolicyDefinitionPart> PartsByTypeId { get; set; }
    }

    public class ChargingPolicyDefinitionPart
    {        
        public int PartConfigId { get; set; }

        public ChargingPolicyPartDefinitionSettings PartDefinitionSettings { get; set; }
    }
}

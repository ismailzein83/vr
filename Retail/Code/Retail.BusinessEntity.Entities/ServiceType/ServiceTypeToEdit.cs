using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeToEdit
    {
        public int ServiceTypeId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ChargingPolicyDefinitionSettings ChargingPolicyDefinitionSettings { get; set; }
    }
}

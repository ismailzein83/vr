using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class ContractServiceType
    {
        public Guid ContractServiceTypeId { get; set; }

        public string Name { get; set; }

        public Guid ContractTypeId { get; set; }
    }
}

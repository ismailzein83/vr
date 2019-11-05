using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddContractResourceInput
    {
        public long ContractId { get; set; }

        public string Name { get; set; }

        public Guid ResourceTypeId { get; set; }

        public DateTime BET { get; set; }

        public DateTime? EET { get; set; }
    }
}

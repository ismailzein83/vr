using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Contract
    {
        public int ContractId { get; set; }

        public string Name { get; set; }

        public ContractSettings Settings { get; set; }
    }
}

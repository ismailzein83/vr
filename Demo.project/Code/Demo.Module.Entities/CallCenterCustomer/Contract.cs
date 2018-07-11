using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Contract
    {
        public long ContractId { get; set; }
        public string MobileNumber { get; set; }
        public string MSISDN { get; set; }
        public string RatePlan { get; set; }
    }
}

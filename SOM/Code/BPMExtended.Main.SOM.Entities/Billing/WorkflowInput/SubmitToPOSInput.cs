using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class SubmitToPOSInput
    {
        public string CustomerCode { get; set; }
        public string ContractId { get; set; }
        public bool DepositFlag { get; set; }
        public List<SaleService> Services { get; set; }
    }
}

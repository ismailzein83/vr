using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class FinalizeActivateTelephonyContractInput
    {
        public string ContractId { get; set; }
        public string RequestId { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}

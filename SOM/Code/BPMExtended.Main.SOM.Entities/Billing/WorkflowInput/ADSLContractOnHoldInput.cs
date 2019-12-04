using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ADSLContractOnHoldInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public ContractInfoDetails ContractInfo { get; set; }
        public CustomerType CustomerType { get; set; }
        public string RatePlanId { get; set; }
        public string UserName { get; set; }
        public string CSO { get; set; }
        public string TelephonyContractId { get; set; }
        public string SubType { get; set; }
        public string StateProvince { get; set; }
        public string ServiceResource { get; set; }
        public string LinePathId { get; set; }
        public List<ContractServiceInfo> Services { get; set; }
        public List<DepositDocument> DepositServices { get; set; }
    }
}

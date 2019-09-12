using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class TelephonyContractOnHoldInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string RatePlanId { get; set; }
        public string CSO { get; set; }
        public string SubType { get; set; }
        public string CountryId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Town { get; set; }
        public string Region { get; set; }
        public string StateProvince { get; set; }
        public string ServiceResource { get; set; }
        public string Notes { get; set; }
        public string PhoneNumber { get; set; }

        public string LinePathId { get; set; }
        public List<ContractService> ContractServices { get; set; }
        public List<DepositDocument> DepositServices { get; set; }

    }
}

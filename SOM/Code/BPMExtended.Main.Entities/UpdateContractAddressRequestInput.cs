using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace BPMExtended.Main.Entities
{
    public class UpdateContractAddressRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string  City { get; set; }
        public string Street { get; set; }
        public DirectoryInquiry Action { get; set; }
        public PaymentData PaymentData { get; set; }
        public string ServiceId { get; set; }
        public string AddressSequence { get; set; }

    }
}

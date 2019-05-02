using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ServiceAdditionRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }

        public string LinePathId { get; set; }
        public List<ContractAdditionalServicesInput> ContractAdditionalServices { get; set; }
        public PaymentData PaymentData { get; set; }

    }
}

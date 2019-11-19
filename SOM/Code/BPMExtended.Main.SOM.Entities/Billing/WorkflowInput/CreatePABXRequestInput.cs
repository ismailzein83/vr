using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class CreatePABXRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public SubmitPabxInput SubmitPabxInput { get; set; }

    }

    public class SubmitPabxInput
    {
        public PabxContractInput PilotContract { get; set; }
        public List<PabxContractInput> Contracts { get; set; }
        public PabxService PabxService { get; set; }
    }

    public class PabxContractInput
    {
        public string ContractId { get; set; }
        public string LinePathId { get; set; }
        public string PhoneNumber { get; set; }
        public ServiceParameterValue PabxParameterValue { get; set; }
    }


    public class PabxService
    {
        public string Id { get; set; }
        public string PackageId { get; set; }
        public string ParameterId { get; set; }
        public string ParameterNumber { get; set; }
    }

    public class ServiceParameterValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
    }

}

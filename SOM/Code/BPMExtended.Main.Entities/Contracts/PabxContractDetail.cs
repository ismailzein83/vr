using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PabxContractDetail
    {
        public string Id { get; set; }
        public string ContractId { get; set; }

        public string PhoneNumber { get; set; }

        public Address Address { get; set; }

        public string DeviceId { get; set; }
        public ServiceParameter Parameter { get; set; }
    }

    public class ServiceParameter
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ParameterNumber { get; set; }
        public ParameterType Type { get; set; }
        public List<ServiceParameterValue> Values { get; set; }
    }

    public class ServiceParameterValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
    }
}

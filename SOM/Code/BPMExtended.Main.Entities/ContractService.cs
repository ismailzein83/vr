using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ContractService
    {
        public string spcode { get; set; }
        public string sncode { get; set; }
    }
    public class ContractServiceInfo
    {
        public string Id { get; set; }
        public string PackageId { get; set; }
        public List<ContractServiceParameter> Parameters { get; set; }

    }
    public class ContractServiceParameter
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ParameterNumber { get; set; }
        public ParameterType Type { get; set; }
        public List<ContractServiceParameterValue> Values { get; set; }

    }
    public class ContractServiceParameterValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
    }

}

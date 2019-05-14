using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ServiceParameter
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ParameterNumber { get; set; }
        public List<ServiceParameterValue> Values { get; set; }
    }

    public class ServiceParameterValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
    }

}

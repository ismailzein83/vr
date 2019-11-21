using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ServiceParameter
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string ParameterNumber { get; set; }
        public ParameterType Type { get; set; }
        public List<ServiceParameterValue> Values { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class RatePlanServiceParameter
    {
        public string ParameterName { get; set; }
        public ParameterType ParameterType { get; set; }
    }
    public enum ParameterType
    {
        TB = 0,
        CB = 1,
        LB = 2
    }
}

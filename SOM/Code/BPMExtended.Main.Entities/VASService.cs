using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class VASService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PackageId { get; set; }
        public bool IsNetwork { get; set; }
        public bool NeedProvisioning { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
    //public class Parameter {
    //    public string ParameterName { get; set; }
    //    public string ParameterValue { get; set; }
    //}
    public class Parameter
    {
        public string Id { get; set; }
        public string ParameterNumber { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public ParameterType Type { get; set; }
        public string ParameterDisplayValue { get; set; }
        public string SequenceNumber { get; set; }
        //public string Id { get; set; }
        //public string Description { get; set; }
        //public string ParameterNumber { get; set; }
        //public ParameterType Type { get; set; }
        //public List<ParameterValue> Values { get; set; }
    }

    public class ParameterValue
    {
        public string Value { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
    }


}

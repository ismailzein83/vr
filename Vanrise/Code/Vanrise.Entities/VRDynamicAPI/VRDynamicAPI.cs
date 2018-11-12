using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPI
    {
        public int VRDynamicAPIId { get; set; }
        public string Name { get; set; }
        public int ModuleId{ get; set; }
        public VRDynamicAPISettings Settings { get; set; }
    }
    public class VRDynamicAPISettings
    {
        public List<VRDynamicAPIMethod> Methods { get; set; }
    }
    public class VRDynamicAPIMethod
    {
        public Guid VRDynamicAPIMethodId { get; set; }
        public string Name { get; set; }
        public VRDynamicAPIMethodSettings Settings { get; set; }
    }
    public abstract class VRDynamicAPIMethodSettings
    {
        public abstract Guid ConfigId { get; }
    }

    public enum VRDynamicAPIMethodType { Get = 1, Post = 2 }
    public class VRCustomCodeDynamicAPIMethod : VRDynamicAPIMethodSettings
    {
        public override Guid ConfigId { get { return new Guid("98B21112-0364-4646-AA26-80263481DCDD"); } }
        public string MethodBody { get; set; }
        public VRDynamicAPIMethodType MethodType { get; set; }
        public List<VRDynamicAPIMethodParameter> InParameters { get; set; }
        public string ReturnType { get; set; }
    }
    public class VRDynamicAPIMethodParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
    }
}

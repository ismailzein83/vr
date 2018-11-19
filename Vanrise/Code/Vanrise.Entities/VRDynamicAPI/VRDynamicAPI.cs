using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPI
    {
        public long VRDynamicAPIId { get; set; }
        public string Name { get; set; }
        public int ModuleId{ get; set; }
        public VRDynamicAPISettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
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
        public abstract void  Evaluate(IVRDynamicAPIMethodSettingsContext vrDynamicAPIMethodSettingsContext); 
    }
    public enum VRDynamicAPIMethodType { Get = 1, Post = 2 }
    public class VRDynamicAPIMethodParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
    }
    public interface IVRDynamicAPIMethodSettingsContext
    {
        string MethodBody { set; }
        string ReturnType { set; }
        List<VRDynamicAPIMethodParameter> InParameters { set; }
        VRDynamicAPIMethodType MethodType { set; }

    }
    public class VRDynamicAPIMethodSettingsContext : IVRDynamicAPIMethodSettingsContext
    {
        public string MethodBody { get; set; }
        public string ReturnType { get; set; }
        public List<VRDynamicAPIMethodParameter> InParameters { get; set; }
        public VRDynamicAPIMethodType MethodType { get; set; }

    }
}

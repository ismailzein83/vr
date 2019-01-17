using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPI
    {
        public Guid VRDynamicAPIId { get; set; }
        public string Name { get; set; }
        public Guid ModuleId { get; set; }
        public VRDynamicAPISettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
    }
    public class VRDynamicAPISettings
    {
        public VRDynamicAPISecurity Security { get; set; }
        public List<VRDynamicAPIMethod> Methods { get; set; }
    }
    public class VRDynamicAPISecurity
    {
        public Object RequiredPermissions { get; set; }
    }
    public class VRDynamicAPIMethod
    {
        public Guid VRDynamicAPIMethodId { get; set; }
        public string Name { get; set; }
        public VRDynamicAPIMethodSettings Settings { get; set; }
        public VRDynamicAPIMethodSecurity Security { get; set; }
    }
    public abstract class VRDynamicAPIMethodSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Evaluate(IVRDynamicAPIMethodSettingsContext vrDynamicAPIMethodSettingsContext);
    }
    public class VRDynamicAPIMethodSecurity
    {
        public Object RequiredPermissions { get; set; }
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
    public class VRDynamicAPICompilationResult
    {
        public bool isSucceeded { get; set; }
        public List<string> Errors { get; set; }

    }


   
}

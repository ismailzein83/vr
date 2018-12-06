using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIQuery
    {
        public string Name { get; set; }
        public int? VRDynamicAPIModuleId { get; set; }
    }



    public class VRDynamicCode
    {
        public Guid VRDynamicCodeId { get; set; }
        public string Title { get; set; }
        public VRDynamicCodeSettings Settings { get; set; }
    }
    public abstract class VRDynamicCodeSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string Generate(IVRDynamicCodeSettingsContext context);
     }
    public interface IVRDynamicCodeSettingsContext
    {

    }
    public class VRDynamicCodeSettingsContext: IVRDynamicCodeSettingsContext
    {

    }
    public class CustomCodeDynamicCodeSettings : VRDynamicCodeSettings
    {
        public override Guid ConfigId { get { return new Guid("BDD289DF-573C-44A1-9A95-D0DE2ED9DD71"); } }
        public string CustomCode { get; set; }
        public override string Generate(IVRDynamicCodeSettingsContext context)
        {
            return CustomCode;
        }
    }
    public class HttpProxyDynamicCodeSettings : VRDynamicCodeSettings
    {
        public override Guid ConfigId { get { return new Guid("5F14D26D-7B43-41BE-9A3A-6BA0A7EB8316"); } }
        public Guid ConnectionId { get; set; }
        public VRHttpMethod MethodType { get; set; }
        public VRHttpMessageFormat MessageFormat { get; set; }
        public string ActionPath { get; set; }
        public string ReturnType { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<HttpProxyParameter> Parameters { get; set; }
        public Dictionary<string, string> URLParameters { get; set; }
        public override string Generate(IVRDynamicCodeSettingsContext context)
        {
            return "";
        }
    }
    public class HttpProxyParameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public bool  IncludeInHeader { get; set; }
        public bool IncludeInBody { get; set; }
        public bool IncludeInURL { get; set; }
    }
}

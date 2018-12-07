using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRDynamicCode
{
    public class HttpProxyDynamicCodeSettings : VRDynamicCodeSettings
    {
        public override Guid ConfigId { get { return new Guid("5F14D26D-7B43-41BE-9A3A-6BA0A7EB8316"); } }
        public Guid ConnectionId { get; set; }
        public string ClassName { get; set; }
        public List<HttpProxyMethod> Methods { get; set; }
        public override string Generate(IVRDynamicCodeSettingsContext context)
        {
            return "";
        }
    }
    public class HttpProxyMethod
    {
        public string MethodName { get; set; }
        public VRHttpMethod MethodType { get; set; }
        public VRHttpMessageFormat MessageFormat { get; set; }
        public string ActionPath { get; set; }
        public string ReturnType { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public List<HttpProxyParameter> Parameters { get; set; }
        public Dictionary<string, string> URLParameters { get; set; }
        public string Generate(IHttpProxyMethodContext context)
        {
            return "";
        }
    }

    public interface IHttpProxyMethodContext
    {
        Guid ConnectionId { get; }
    }
    public class HttpProxyMethodContext : IHttpProxyMethodContext
    {
        public Guid ConnectionId { get; set; }
    }
    public class HttpProxyParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType{ get; set; }
        public bool IncludeInHeader { get; set; }
        public bool IncludeInBody { get; set; }
        public bool IncludeInURL { get; set; }
    }
}

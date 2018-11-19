using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRDynamicAPI
{
    public class VRCustomCodeDynamicAPIMethod : VRDynamicAPIMethodSettings
    {
        public override Guid ConfigId { get { return new Guid("4EB2746B-5368-4D05-B6D3-EFD075BE2DCF"); } }
        public override void Evaluate(IVRDynamicAPIMethodSettingsContext context)
        {
            context.MethodBody = MethodBody;
            context.ReturnType = (ReturnType == null || ReturnType == "") ? "void": ReturnType;
            context.MethodType = MethodType;
            context.InParameters = InParameters;
        }
        public string MethodBody { get; set; }
        public VRDynamicAPIMethodType MethodType { get; set; }
        public List<VRDynamicAPIMethodParameter> InParameters { get; set; }
        public string ReturnType { get; set; }
    }

}

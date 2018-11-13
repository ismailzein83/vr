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
        public override Guid ConfigId { get { return new Guid("98B21112-0364-4646-AA26-80263481DCDD"); } }
        public string MethodBody { get; set; }
        public VRDynamicAPIMethodType MethodType { get; set; }
        public List<VRDynamicAPIMethodParameter> InParameters { get; set; }
        public string ReturnType { get; set; }
    }
 
}

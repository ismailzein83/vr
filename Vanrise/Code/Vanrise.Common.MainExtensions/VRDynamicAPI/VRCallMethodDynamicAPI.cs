using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRDynamicAPI
{
    public class VRCallMethodDynamicAPI : VRDynamicAPIMethodSettings
    {
        public override Guid ConfigId { get { return new Guid("AAC62543-CF85-4F0D-BB42-121C7B699816"); } }
        public Guid NamespaceId { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public override void Evaluate(IVRDynamicAPIMethodSettingsContext vrDynamicAPIMethodSettingsContext)
        {
            throw new NotImplementedException();
        }
    }
}

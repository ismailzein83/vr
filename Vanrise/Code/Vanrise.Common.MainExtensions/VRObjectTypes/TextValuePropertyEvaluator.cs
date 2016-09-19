using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.VRObjectTypes
{
    public class TextValuePropertyEvaluator : VRObjectPropertyEvaluator
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("BD6BFC0B-92FF-4ECE-8A18-C3AD4B108FA0"); } }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            return context.Object;
        }
    }
}

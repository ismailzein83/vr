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
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            return context.Object;
        }
    }
}

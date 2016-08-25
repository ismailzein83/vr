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
        public string TextField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            return Vanrise.Common.Utilities.GetPropValueReader(this.TextField).GetPropertyValue(context.Object);
        }
    }
}

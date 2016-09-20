using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRObjectPropertyEvaluator
    {
        public abstract Guid ConfigId { get; }

        public abstract dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context);
    }

    public interface IVRObjectPropertyEvaluatorContext
    {
        dynamic Object { get; }

        VRObjectType ObjectType { get; }
    }

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRObjectType
    {
        public abstract Guid ConfigId { get; }

        public virtual dynamic GetDefaultValue()
        {
            return null;
        }
        public abstract object CreateObject(IVRObjectTypeCreateObjectContext context);
    }

    public interface IVRObjectTypeCreateObjectContext
    {
        dynamic ObjectId { get; }
    }
}

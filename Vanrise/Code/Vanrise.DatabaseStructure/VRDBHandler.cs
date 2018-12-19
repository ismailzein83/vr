using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DatabaseStructure
{
    public abstract class VRDBHandler
    {
        public virtual void ApplyChangesToDBStructure(IVRDBHandlerApplyChangesContext context)
        {
        }
    }

    public interface IVRDBHandlerApplyChangesContext
    {
        VRDBType DBType { get; }

        VRDBStructure DBStructure { get; }
    }
}

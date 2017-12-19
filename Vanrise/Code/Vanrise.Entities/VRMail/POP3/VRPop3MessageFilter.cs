using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRPop3MessageFilter
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsApplicable();
    }
}

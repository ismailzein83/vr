using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRActionValidity
    {
        public abstract bool IsValid(IVRActionValidityContext context);
    }

    public interface IVRActionValidityContext
    {
        VRAction Action { get; }
    }
}

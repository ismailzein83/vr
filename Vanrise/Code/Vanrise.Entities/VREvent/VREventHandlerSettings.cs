using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VREventHandlerSettings
    {
        public VREventHandlerExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VREventHandlerExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetEventTypeUniqueName();

        public abstract void Execute(IVREventHandlerContext context);
    }

    public interface IVREventHandlerContext
    {
        VREventPayload EventPayload { get; }
    }
}

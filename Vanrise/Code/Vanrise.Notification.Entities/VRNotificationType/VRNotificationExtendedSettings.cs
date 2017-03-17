using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.Notification.Entities
{
    public abstract class VRNotificationTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual string SearchRuntimeEditor { get; set; }
        public virtual string BodyRuntimeEditor { get; set; }

        public virtual VRNotificationDetail GetVRNotificationDetailMapper(IGetVRNotificationDetailMapperContext context)
        {
            VRNotificationDetail vrNotificationDetail = new VRNotificationDetail()
            {
                Entity = context.VRNotification
            };
            return vrNotificationDetail;
        }
    }

    public interface IGetVRNotificationDetailMapperContext
    {
        VRNotification VRNotification { get; }
    }

    public class GetVRNotificationDetailMapperContext : IGetVRNotificationDetailMapperContext
    {
        public VRNotification VRNotification { get; set; }
    }
}

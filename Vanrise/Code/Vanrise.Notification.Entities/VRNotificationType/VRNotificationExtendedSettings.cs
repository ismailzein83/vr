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

        public virtual VRNotificationDetail MapToNotificationDetail(IMapToNotificationDetailContext context)
        {
            VRNotificationDetail vrNotificationDetail = new VRNotificationDetail()
            {
                Entity = context.VRNotification
            };
            return vrNotificationDetail;
        }
    }

    public interface IMapToNotificationDetailContext
    {
        VRNotification VRNotification { get; }
    }

    public class MapToNotificationDetailContext : IMapToNotificationDetailContext
    {
        public VRNotification VRNotification { get; set; }
    }
}

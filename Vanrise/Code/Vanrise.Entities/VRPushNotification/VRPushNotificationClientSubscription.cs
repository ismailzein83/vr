using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRPushNotificationClientSubscription
    {
        public string ClientSubscriptionId { get; set; }

        public abstract Guid HandlerId { get; }
    }
}

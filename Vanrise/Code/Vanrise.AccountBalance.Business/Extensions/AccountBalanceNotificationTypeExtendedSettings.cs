using System;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business
{
    public abstract class AccountBalanceNotificationTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string NotificationQueryEditor { get; set; }

        public abstract bool IsVRNotificationMatched(IAccountBalanceNotificationTypeIsMatchedContext context);
    }

    public interface IAccountBalanceNotificationTypeIsMatchedContext
    {
        VRNotification VRNotification { get; }

        AccountBalanceNotificationExtendedQuery AccountBalanceNotificationExtendedQuery { get; }
    }

    public class AccountBalanceNotificationTypeIsMatchedContext : IAccountBalanceNotificationTypeIsMatchedContext
    {
        public VRNotification VRNotification { get; set;  }

        public AccountBalanceNotificationExtendedQuery AccountBalanceNotificationExtendedQuery { get; set; }
    }
}

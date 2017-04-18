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
        Guid AccountTypeId { get; }

        VRBalanceAlertEventPayload EventPayload { get; }

        AccountBalanceNotificationExtendedQuery AccountBalanceNotificationExtendedQuery { get; }
    }

    public class AccountBalanceNotificationTypeIsMatchedContext : IAccountBalanceNotificationTypeIsMatchedContext
    {
        public Guid AccountTypeId { get; set; }

        public VRBalanceAlertEventPayload EventPayload { get; set; }

        public AccountBalanceNotificationExtendedQuery AccountBalanceNotificationExtendedQuery { get; set; }
    }
}

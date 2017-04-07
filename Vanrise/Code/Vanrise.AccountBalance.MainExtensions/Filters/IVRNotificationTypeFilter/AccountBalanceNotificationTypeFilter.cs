using Vanrise.AccountBalance.MainExtensions.AccountBalanceNotification;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions.Filters
{
    public class AccountBalanceNotificationTypeFilter : IVRNotificationTypeFilter
    {
        public bool IsMatched(IVRNotificationTypeFilterContext context)
        {
            if (context.VRNotificationType.Settings == null)
                return false;

            VRNotificationTypeSettings vrNotificationTypeSettings = context.VRNotificationType.Settings as VRNotificationTypeSettings;
            if (vrNotificationTypeSettings == null)
                return false;

            if (vrNotificationTypeSettings.ExtendedSettings == null)
                return false;

            AccountBalanceNotificationTypeSettings accountBalanceNotificationTypeSettings = vrNotificationTypeSettings.ExtendedSettings as AccountBalanceNotificationTypeSettings;
            if (accountBalanceNotificationTypeSettings == null)
                return false;

            return true;
        }
    }
}

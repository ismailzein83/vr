using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceNotificationTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountBalance_NotificationTypeExtendedSettingsConfig";

        public string Editor { get; set; }
    }
}

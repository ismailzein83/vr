using System;
using System.Collections.Generic;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Notification.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceNotificationTypeManager
    {
        public IEnumerable<AccountBalanceNotificationTypeExtendedSettingsConfig> GetAccountBalanceNotificationTypeExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountBalanceNotificationTypeExtendedSettingsConfig>(AccountBalanceNotificationTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        public AccountBalanceNotificationTypeSettings GetAccountBalanceNotificationTypeSettings(Guid notificationTypeId)
        {
            return new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings<AccountBalanceNotificationTypeSettings>(notificationTypeId);
        }
    }
}

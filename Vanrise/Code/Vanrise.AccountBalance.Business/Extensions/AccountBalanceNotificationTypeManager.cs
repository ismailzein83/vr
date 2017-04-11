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

        public string GetAccountColumnHeader(Guid notificationTypeId)
        {
            AccountBalanceNotificationTypeSettings accountBalanceNotificationTypeSettings = new VRNotificationTypeManager().GetVRNotificationTypeExtendedSettings<AccountBalanceNotificationTypeSettings>(notificationTypeId);
            return accountBalanceNotificationTypeSettings.AccountColumnHeader;
        }
    }
}

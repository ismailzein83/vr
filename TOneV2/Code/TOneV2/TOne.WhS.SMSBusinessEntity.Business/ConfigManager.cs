using System;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class ConfigManager
    {
        #region PublicMethods

        public ReceiverIdentification GetGeneralReceiverIndentification()
        {
            SMSImportMappingConfiguration smsImportMappingConfiguration = GetSMSImportMappingConfiguration();
            if (!smsImportMappingConfiguration.GeneralIdentification.HasValue)
                throw new NullReferenceException("smsImportMappingConfiguration.GeneralIdentification");

            return smsImportMappingConfiguration.GeneralIdentification.Value;
        }

        public ReceiverIdentification GetCustomerReceiverIdentification()
        {
            SMSImportMappingConfiguration smsImportMappingConfiguration = GetSMSImportMappingConfiguration();
            if (!smsImportMappingConfiguration.CustomerIdentification.HasValue)
                throw new NullReferenceException("smsImportMappingConfiguration.CustomerIdentification");

            return smsImportMappingConfiguration.CustomerIdentification.Value;
        }

        public ReceiverIdentification GetSupplierReceiverIdentification()
        {
            SMSImportMappingConfiguration smsImportMappingConfiguration = GetSMSImportMappingConfiguration();
            if (!smsImportMappingConfiguration.SupplierIdentification.HasValue)
                throw new NullReferenceException("smsImportMappingConfiguration.SupplierIdentification");

            return smsImportMappingConfiguration.SupplierIdentification.Value;
        }

        public ReceiverIdentification GetMobileNetworkReceiverIdentification()
        {
            SMSImportMappingConfiguration smsImportMappingConfiguration = GetSMSImportMappingConfiguration();
            if (!smsImportMappingConfiguration.MobileNetworkIdentification.HasValue)
                throw new NullReferenceException("smsImportMappingConfiguration.MobileNetworkIdentification");

            return smsImportMappingConfiguration.MobileNetworkIdentification.Value;
        }

        #endregion

        #region PrivateMethods

        private SMSImportMappingConfiguration GetSMSImportMappingConfiguration()
        {
            SMSImportSettings smsImportSettings = GetSMSImportSettings();
            smsImportSettings.ThrowIfNull("smsImportSettings.SMSImportMappingConfiguration");
            return smsImportSettings.SMSImportMappingConfiguration;
        }

        private SMSImportSettings GetSMSImportSettings()
        {
            SettingManager settingManager = new SettingManager();
            SMSImportSettings smsImportSettings = settingManager.GetSetting<SMSImportSettings>(Constants.SMSImportSettings);
            smsImportSettings.ThrowIfNull("smsImportSettings");

            return smsImportSettings;
        }

        #endregion
    }
}
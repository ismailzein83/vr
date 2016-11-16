using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ConfigManager
    {
        #region Public Methods

        public int GetOffPeakRateTypeId()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();

            if (setting.RateTypeConfiguration == null)
                throw new NullReferenceException("setting.RateTypeConfiguration");

            return setting.RateTypeConfiguration.OffPeakRateTypeId;
        }
        public int GetWeekendRateTypeId()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();

            if (setting.RateTypeConfiguration == null)
                throw new NullReferenceException("setting.RateTypeConfiguration");

            return setting.RateTypeConfiguration.WeekendRateTypeId;
        }
        public int GetHolidayRateTypeId()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();

            if (setting.RateTypeConfiguration == null)
                throw new NullReferenceException("setting.RateTypeConfiguration");

            return setting.RateTypeConfiguration.HolidayRateTypeId;
        }

        public CDPNIdentification GetGeneralCDPNIndentification()
        {
            SwitchCDRMappingConfiguration switchCDRMappingConfiguration = GetSwitchCDRMappingConfiguration();
            if (!switchCDRMappingConfiguration.GeneralIdentification.HasValue)
                throw new NullReferenceException("switchCDRMappingConfiguration.GeneralIdentification");

            return switchCDRMappingConfiguration.GeneralIdentification.Value;
        }
        public CDPNIdentification GetCustomerCDPNIndentification()
        {
            SwitchCDRMappingConfiguration switchCDRMappingConfiguration = GetSwitchCDRMappingConfiguration();
            if (!switchCDRMappingConfiguration.CustomerIdentification.HasValue)
                throw new NullReferenceException("switchCDRMappingConfiguration.CustomerIdentification");

            return switchCDRMappingConfiguration.CustomerIdentification.Value;
        }
        public CDPNIdentification GetSupplierCDPNIndentification()
        {
            SwitchCDRMappingConfiguration switchCDRMappingConfiguration = GetSwitchCDRMappingConfiguration();
            if (!switchCDRMappingConfiguration.SupplierIdentification.HasValue)
                throw new NullReferenceException("switchCDRMappingConfiguration.SupplierIdentification");

            return switchCDRMappingConfiguration.SupplierIdentification.Value;
        }
        public CDPNIdentification GetSaleZoneCDPNIndentification()
        {
            SwitchCDRMappingConfiguration switchCDRMappingConfiguration = GetSwitchCDRMappingConfiguration();
            if (!switchCDRMappingConfiguration.SaleZoneIdentification.HasValue)
                throw new NullReferenceException("switchCDRMappingConfiguration.SaleZoneIdentification");

            return switchCDRMappingConfiguration.SaleZoneIdentification.Value;
        }
        public CDPNIdentification GetSupplierZoneCDPNIndentification()
        {
            SwitchCDRMappingConfiguration switchCDRMappingConfiguration = GetSwitchCDRMappingConfiguration();
            if (!switchCDRMappingConfiguration.SupplierZoneIdentification.HasValue)
                throw new NullReferenceException("switchCDRMappingConfiguration.SupplierZoneIdentification");

            return switchCDRMappingConfiguration.SupplierZoneIdentification.Value;
        }

        public CachingExpirationIntervals GetCachingExpirationIntervals()
        {
            BusinessEntitySettingsData businessEntitySettingsData = GetBusinessEntitySettingsData();
            if (businessEntitySettingsData.CachingExpirationIntervals == null)
                throw new NullReferenceException("businessEntitySettingsData.CachingExpirationIntervals");

            return businessEntitySettingsData.CachingExpirationIntervals;
        }

        #endregion

        #region Private Method

        private BusinessEntityTechnicalSettingsData GetBusinessEntitySettingData()
        {
            SettingManager settingManager = new SettingManager();
            BusinessEntityTechnicalSettingsData setting = settingManager.GetSetting<BusinessEntityTechnicalSettingsData>(BusinessEntityTechnicalSettingsData.BusinessEntityTechnicalSettings);

            if (setting == null)
                throw new NullReferenceException("BusinessEntityTechnicalSettingsData");

            return setting;
        }

        private CDRMappingSettings GetCDRMappingSettings()
        {
            SettingManager settingManager = new SettingManager();
            CDRMappingSettings cdrMappingSettings = settingManager.GetSetting<CDRMappingSettings>(Constants.CDRMappingSettings);

            if (cdrMappingSettings == null)
                throw new NullReferenceException("cdrMappingSettings");

            return cdrMappingSettings;
        }
        private SwitchCDRMappingConfiguration GetSwitchCDRMappingConfiguration()
        {
            CDRMappingSettings cdrMappingSettings = GetCDRMappingSettings();
            if (cdrMappingSettings.SwitchCDRMappingConfiguration == null)
                throw new NullReferenceException("cdrMappingSettings.SwitchCDRMappingConfiguration");

            return cdrMappingSettings.SwitchCDRMappingConfiguration;
        }

        private BusinessEntitySettingsData GetBusinessEntitySettingsData()
        {
            SettingManager settingManager = new SettingManager();
            BusinessEntitySettingsData businessEntitySettingsData = settingManager.GetSetting<BusinessEntitySettingsData>(Constants.BusinessEntitySettingsData);

            if (businessEntitySettingsData == null)
                throw new NullReferenceException("businessEntitySettingsData");

            return businessEntitySettingsData;
        }

        #endregion
    }
}
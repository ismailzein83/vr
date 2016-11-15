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
            GeneralIdentification generalIdentification = GetGeneralIdentification();
            if (!generalIdentification.CDPNIdentification.HasValue)
                throw new NullReferenceException("generalIdentification.CDPNIdentification");

            return generalIdentification.CDPNIdentification.Value;
        }

        public CDPNIdentification GetCustomerCDPNIndentification()
        {
            CustomerIdentification customerIdentification = GetCustomerIdentification();
            if (!customerIdentification.CDPNIdentification.HasValue)
                throw new NullReferenceException("customerIdentification.CDPNIdentification");

            return customerIdentification.CDPNIdentification.Value;
        }

        public CDPNIdentification GetSupplierCDPNIndentification()
        {
            SupplierIdentification supplierIdentification = GetSupplierIdentification();
            if (!supplierIdentification.CDPNIdentification.HasValue)
                throw new NullReferenceException("supplierIdentification.CDPNIdentification");

            return supplierIdentification.CDPNIdentification.Value;
        }

        public CDPNIdentification GetSaleZoneCDPNIndentification()
        {
            SaleZoneIdentification saleZoneIdentification = GetSaleZoneIdentification();
            if (!saleZoneIdentification.CDPNIdentification.HasValue)
                throw new NullReferenceException("saleZoneIdentification.CDPNIdentification");

            return saleZoneIdentification.CDPNIdentification.Value;
        }

        public CDPNIdentification GetSupplierZoneCDPNIndentification()
        {
            SupplierZoneIdentification supplierZoneIdentification = GetSupplierZoneIdentification();
            if (!supplierZoneIdentification.CDPNIdentification.HasValue)
                throw new NullReferenceException("supplierZoneIdentification.CDPNIdentification");

            return supplierZoneIdentification.CDPNIdentification.Value;
        }

        public GeneralIdentification GetGeneralIdentification()
        {
            GeneralIdentification generalIdentification = GetSwitchCDRProcessConfiguration().GeneralIdentification;
            if (generalIdentification == null)
                throw new NullReferenceException("generalIdentification");

            return generalIdentification;
        }

        public CustomerIdentification GetCustomerIdentification()
        {
            CustomerIdentification customerIdentification = GetSwitchCDRProcessConfiguration().CustomerIdentification;
            if (customerIdentification == null)
                throw new NullReferenceException("customerIdentification");

            return customerIdentification;
        }

        public SupplierIdentification GetSupplierIdentification()
        {
            SupplierIdentification supplierIdentification = GetSwitchCDRProcessConfiguration().SupplierIdentification;
            if (supplierIdentification == null)
                throw new NullReferenceException("supplierIdentification");

            return supplierIdentification;
        }

        public SaleZoneIdentification GetSaleZoneIdentification()
        {
            SaleZoneIdentification saleZoneIdentification= GetSwitchCDRProcessConfiguration().SaleZoneIdentification;
            if (saleZoneIdentification == null)
                throw new NullReferenceException("saleZoneIdentification");

            return saleZoneIdentification;
        }

        public SupplierZoneIdentification GetSupplierZoneIdentification()
        {
            SupplierZoneIdentification supplierZoneIdentification = GetSwitchCDRProcessConfiguration().SupplierZoneIdentification;
            if (supplierZoneIdentification == null)
                throw new NullReferenceException("supplierZoneIdentification");

            return supplierZoneIdentification;
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

        private CDRImportSettings GetCDRImportSettings()
        {
            SettingManager settingManager = new SettingManager();
            CDRImportSettings cdrImportSettings = settingManager.GetSetting<CDRImportSettings>(Constants.CDRImportSettings);

            if (cdrImportSettings == null)
                throw new NullReferenceException("cdrImportSettings");

            return cdrImportSettings;
        }
        private SwitchCDRProcessConfiguration GetSwitchCDRProcessConfiguration()
        {
            CDRImportSettings cdrImportSettings = GetCDRImportSettings();
            if (cdrImportSettings.SwitchCDRProcessConfiguration == null)
                throw new NullReferenceException("cdrImportSettings.SwitchCDRProcessConfiguration");

            return cdrImportSettings.SwitchCDRProcessConfiguration;
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
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

        public Guid GetRuleDefinitionGuid()
        {
            CDRImportTechnicalSettings settingData = GetCdrImportTechnicalSetting();
            if (settingData.CdrImportTechnicalSettingData == null)
                throw new NullReferenceException("setting.CdrImportTechnicalSettingData");
            return settingData.CdrImportTechnicalSettingData.RuleDefinitionGuid;
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
        public int GetSaleAreaEffectiveDateDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.EffectiveDateDayOffset;
        }
        public int GetSaleAreaRetroactiveDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.RetroactiveDayOffset;
        }
        public int GetPurchaseAreaRetroactiveDayOffset()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.RetroactiveDayOffset;
        }

        public CustomerInvoiceSettings GetDefaultCustomerInvoiceSettings()
        {
            InvoiceSettings setting = GetInvoiceSettings();
            if (setting.CustomerInvoiceSettings == null)
                throw new NullReferenceException("setting.CustomerInvoiceSettings");
            foreach (var customerInvoiceSettings in setting.CustomerInvoiceSettings)
            {
                if (customerInvoiceSettings.IsDefault)
                    return customerInvoiceSettings;
            }
            throw new NullReferenceException("setting.CustomerInvoiceSettings");
        }

        public IEnumerable<CustomerInvoiceSettingInfo> GetCustomerInvoiceSettingInfo()
        {
            InvoiceSettings setting = GetInvoiceSettings();
            List<CustomerInvoiceSettingInfo> lstCustomerInvoiceSettingInfo = new List<CustomerInvoiceSettingInfo>();
            foreach (var customerInvoiceSetting in setting.CustomerInvoiceSettings)
            {
                CustomerInvoiceSettingInfo customerInvoiceSettingInfo = new CustomerInvoiceSettingInfo();
                customerInvoiceSettingInfo = CustomerInvoiceSettingInfoMapper(customerInvoiceSetting);
                lstCustomerInvoiceSettingInfo.Add(customerInvoiceSettingInfo);
            }

            return lstCustomerInvoiceSettingInfo;
        }

        public CustomerInvoiceSettings GetInvoiceSettingsbyGuid(Guid guid)
        {
            InvoiceSettings setting = GetInvoiceSettings();
            if (setting.CustomerInvoiceSettings == null)
                throw new NullReferenceException("setting.CustomerInvoiceSettings");
            foreach (var customerInvoiceSettings in setting.CustomerInvoiceSettings)
            {
                if (customerInvoiceSettings.InvoiceSettingId == guid)
                    return customerInvoiceSettings;
            }
            throw new NullReferenceException("setting.CustomerInvoiceSettings");
        }

        public Guid GetDefaultCustomerInvoiceTemplateMessageId()
        {
            CustomerInvoiceSettings customerInvoiceSettings = GetDefaultCustomerInvoiceSettings();
            if (customerInvoiceSettings == null)
                throw new NullReferenceException("defaultCustomerInvoice");
            return customerInvoiceSettings.DefaultEmailId;
        }
        #endregion

        #region Private Methods

        private CDRImportTechnicalSettings GetCdrImportTechnicalSetting()
        {
            SettingManager manager = new SettingManager();
            CDRImportTechnicalSettings cdrSettingData =
                manager.GetSetting<CDRImportTechnicalSettings>(Constants.CdrImportTechnicalSettings);
            if (cdrSettingData == null)
                throw new NullReferenceException("CDRImportTechnicalSettingData");
            return cdrSettingData;
        }
        private InvoiceSettings GetInvoiceSettings()
        {
            SettingManager settingManager = new SettingManager();
            InvoiceSettings invoiceSettings = settingManager.GetSetting<InvoiceSettings>(InvoiceSettings.SETTING_TYPE);

            if (invoiceSettings == null)
                throw new NullReferenceException("invoiceSettings");

            return invoiceSettings;
        }
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
        private SwitchCDRMappingConfiguration GetSwitchCDRMappingConfiguration()
        {
            CDRImportSettings cdrImportSettings = GetCDRImportSettings();
            if (cdrImportSettings.SwitchCDRMappingConfiguration == null)
                throw new NullReferenceException("cdrImportSettings.SwitchCDRMappingConfiguration");

            return cdrImportSettings.SwitchCDRMappingConfiguration;
        }
        private BusinessEntitySettingsData GetBusinessEntitySettingsData()
        {
            SettingManager settingManager = new SettingManager();
            BusinessEntitySettingsData businessEntitySettingsData = settingManager.GetSetting<BusinessEntitySettingsData>(Constants.BusinessEntitySettingsData);

            if (businessEntitySettingsData == null)
                throw new NullReferenceException("businessEntitySettingsData");

            return businessEntitySettingsData;
        }
        private T GetSettings<T>(string constant) where T : Vanrise.Entities.SettingData
        {
            var settingManager = new SettingManager();
            T settings = settingManager.GetSetting<T>(constant);
            if (settings == null)
                throw new NullReferenceException("settings");
            return settings;
        }
        private SaleAreaSettingsData GetSaleAreaSettings()
        {
            return GetSettings<SaleAreaSettingsData>(Constants.SaleAreaSettings);
        }
        private PurchaseAreaSettingsData GetPurchaseAreaSettings()
        {
            return GetSettings<PurchaseAreaSettingsData>(Constants.PurchaseAreaSettings);
        }

        #endregion

           

        #region Mappers

        private CustomerInvoiceSettingInfo CustomerInvoiceSettingInfoMapper(CustomerInvoiceSettings customerInvoiceSettings)
        {
            return new CustomerInvoiceSettingInfo()
            {
                InvoiceSettingId = customerInvoiceSettings.InvoiceSettingId,
                Name = customerInvoiceSettings.Name,
            };
        }
        #endregion
    }
}
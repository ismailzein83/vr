﻿using System;
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
        public PrimarySaleEntity GetPrimarySaleEntity()
        {
            SaleAreaSettingsData saleAreaSettingsData = GetSaleAreaSettingsData();
            return saleAreaSettingsData.PrimarySaleEntity;
        }
        public Guid GetCustomerRuleDefinitionId()
        {
            CDRImportTechnicalSettings settingData = GetCdrImportTechnicalSetting();
            if (settingData.CdrImportTechnicalSettingData == null)
                throw new NullReferenceException("setting.CDRTechnicalConfiguration");
            return settingData.CdrImportTechnicalSettingData.CustomerRuleDefinitionGuid;
        }
        public Guid GetSupplierRuleDefinitionId()
        {
            CDRImportTechnicalSettings settingData = GetCdrImportTechnicalSetting();
            if (settingData.CdrImportTechnicalSettingData == null)
                throw new NullReferenceException("setting.CDRTechnicalConfiguration");
            return settingData.CdrImportTechnicalSettingData.SupplierRuleDefinitionGuid;
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
        public decimal GetSaleAreaMaximumRate()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.MaximumRate;
        }
        public int GetSaleAreaRetroactiveDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.RetroactiveDayOffset;
        }
        public PriceListExtensionFormat GetSaleAreaPriceListExtensionFormatId()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PriceListExtensionFormat;
        }
        public int GetPurchaseAreaEffectiveDateDayOffset()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.EffectiveDateDayOffset;
        }
        public decimal GetPurchaseAreaMaximumRate()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.MaximumRate;
        }
        public int GetPurchaseAreaRetroactiveDayOffset()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.RetroactiveDayOffset;
        }
        public IEnumerable<VRDocumentItemDefinition> GetDocumentItemDefinitionsInfo()
        {
            BusinessEntityTechnicalSettingsData setting = GetBusinessEntitySettingData();

            if (setting.DocumentCategoryDefinition == null)
                throw new NullReferenceException("setting.DocumentCategoryDefinition");

            return setting.DocumentCategoryDefinition.ItemDefinitions;
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
        private SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            SettingManager manager = new SettingManager();
            SaleAreaSettingsData saleAreaSettingsData =
                manager.GetSetting<SaleAreaSettingsData>(Constants.SaleAreaSettings);
            if (saleAreaSettingsData == null)
                throw new NullReferenceException("SaleAreaSettings");
            return saleAreaSettingsData;
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
    }
}
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

        public decimal GetRoundedDefaultRate()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            var defaultRate = saleAreaSettings.PricingSettings.DefaultRate.Value;
            var longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            return Decimal.Round(defaultRate, longPrecision);
        }
        public int GetSaleAreaEffectiveDateDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.EffectiveDateDayOffset.Value;
        }
        public decimal GetSaleAreaMaximumRate()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.MaximumRate.Value;
        }
        public int GetSaleAreaRetroactiveDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.RetroactiveDayOffset.Value;
        }
        public int GetSaleAreaNewRateDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.NewRateDayOffset.Value;
        }
        public int GetSaleAreaIncreasedRateDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.IncreasedRateDayOffset.Value;
        }
        public int GetSaleAreaDecreasedRateDayOffset()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings.DecreasedRateDayOffset.Value;
        }


        public PriceListExtensionFormat GetSaleAreaPriceListExtensionFormatId()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings.PriceListExtensionFormat.Value;
        }
        public SalePriceListType GetSaleAreaPriceListType()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings.PriceListType.Value;
        }
        public int GetSaleAreaPriceListTemplateId()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings.DefaultSalePLTemplateId.Value;
        }
        public Guid GetSaleAreaPriceListMailTemplateId()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings.DefaultSalePLMailTemplateId.Value;
        }
        public bool GetSaleAreaCompressPriceListFileStatus()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings.CompressPriceListFile.Value;
        }


        public PricingSettings GetSaleAreaPricingSettings()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricingSettings;
        }
        public PricelistSettings GetSaleAreaPricelistSettings()
        {
            SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
            return saleAreaSettings.PricelistSettings;
        }

        public PricingSettings MergePricingSettings(PricingSettings pricingSettingsParent, PricingSettings pricingSettingsChild)
        {
            PricingSettings result = new PricingSettings();
            result.DefaultRate = pricingSettingsParent.DefaultRate;
            result.MaximumRate = pricingSettingsParent.MaximumRate;
            result.EffectiveDateDayOffset = pricingSettingsParent.EffectiveDateDayOffset;
            result.RetroactiveDayOffset = pricingSettingsParent.RetroactiveDayOffset;
            result.NewRateDayOffset = pricingSettingsParent.NewRateDayOffset;
            result.IncreasedRateDayOffset = pricingSettingsParent.IncreasedRateDayOffset;
            result.DecreasedRateDayOffset = pricingSettingsParent.DecreasedRateDayOffset;

            if (pricingSettingsChild == null)
                return result;

            if (pricingSettingsChild.DefaultRate.HasValue)
                result.DefaultRate = pricingSettingsChild.DefaultRate;

            if (pricingSettingsChild.MaximumRate.HasValue)
                result.MaximumRate = pricingSettingsChild.MaximumRate;

            if (pricingSettingsChild.EffectiveDateDayOffset.HasValue)
                result.EffectiveDateDayOffset = pricingSettingsChild.EffectiveDateDayOffset;

            if (pricingSettingsChild.RetroactiveDayOffset.HasValue)
                result.RetroactiveDayOffset = pricingSettingsChild.RetroactiveDayOffset;

            if (pricingSettingsChild.NewRateDayOffset.HasValue)
                result.NewRateDayOffset = pricingSettingsChild.NewRateDayOffset;

            if (pricingSettingsChild.IncreasedRateDayOffset.HasValue)
                result.IncreasedRateDayOffset = pricingSettingsChild.IncreasedRateDayOffset;

            if (pricingSettingsChild.DecreasedRateDayOffset.HasValue)
                result.DecreasedRateDayOffset = pricingSettingsChild.DecreasedRateDayOffset;

            return result;
        }

        public PricelistSettings MergePricelistSettings(PricelistSettings pricelistSettingsParent, PricelistSettings pricelistSettingsChild)
        {
            PricelistSettings result = new PricelistSettings();

            result.DefaultSalePLTemplateId = pricelistSettingsParent.DefaultSalePLTemplateId;
            result.DefaultSalePLMailTemplateId = pricelistSettingsParent.DefaultSalePLMailTemplateId;
            result.PriceListType = pricelistSettingsParent.PriceListType;
            result.PriceListExtensionFormat = pricelistSettingsParent.PriceListExtensionFormat;
            result.CompressPriceListFile = pricelistSettingsParent.CompressPriceListFile;

            if (pricelistSettingsChild == null)
                return result;

            if (pricelistSettingsChild.DefaultSalePLTemplateId.HasValue)
                result.DefaultSalePLTemplateId = pricelistSettingsChild.DefaultSalePLTemplateId;

            if (pricelistSettingsChild.DefaultSalePLMailTemplateId.HasValue)
                result.DefaultSalePLMailTemplateId = pricelistSettingsChild.DefaultSalePLMailTemplateId;

            if (pricelistSettingsChild.PriceListType.HasValue)
                result.PriceListType = pricelistSettingsChild.PriceListType;

            if (pricelistSettingsChild.PriceListExtensionFormat.HasValue)
                result.PriceListExtensionFormat = pricelistSettingsChild.PriceListExtensionFormat;

            if (pricelistSettingsChild.CompressPriceListFile.HasValue)
                result.CompressPriceListFile = pricelistSettingsChild.CompressPriceListFile;

            return result;
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
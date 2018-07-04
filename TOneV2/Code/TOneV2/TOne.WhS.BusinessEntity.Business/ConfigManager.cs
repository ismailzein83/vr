using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

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
		public Guid GetCustomerTariffRuleDefinitionId()
		{
			SaleAreaTechnicalSettings setting = GetSaleAreaTechnicalSetting();
			if (setting.SaleAreaTechnicalSettingData == null)
				throw new NullReferenceException("setting.SaleAreaTechnicalSettingData");
			return setting.SaleAreaTechnicalSettingData.TariffRuleDefinitionGuid;
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
		public int? GetSecondarySellingNumberPlanId()
		{
			CDRImportZoneIdentification cdrImportZoneIdentification = GetCDRImportZoneIdentification();
			if (cdrImportZoneIdentification == null)
				return null;

			return cdrImportZoneIdentification.SecondarySellingNumberPlanId;
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
		public decimal GetSaleAreaDefaultRate()
		{
			SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
			return saleAreaSettings.PricingSettings.DefaultRate.Value;
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
		public IncludeClosedEntitiesEnum GetSaleAreaIncludeClosedEntitiesStatus()
		{
			SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
			return saleAreaSettings.PricelistSettings.IncludeClosedEntities.Value;
		}
		public string GetSaleAreaPricelistFileNamePattern()
		{
			SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
			return saleAreaSettings.PricelistSettings.SalePricelistFileNamePattern;
		}

		public CodeChangeTypeDescriptions GetSaleAreaCodeChangeTypeSettings()
		{
			SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
			return saleAreaSettings.PricelistSettings.CodeChangeTypeDescriptions;
		}
		public RateChangeTypeDescriptions GetSaleAreaRateChangeTypeSettings()
		{
			SaleAreaSettingsData saleAreaSettings = GetSaleAreaSettings();
			return saleAreaSettings.PricelistSettings.RateChangeTypeDescriptions;
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

		public string GetFaultTicketsCustomerSerialNumberPattern()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.CustomerSetting.ThrowIfNull("faultTicketsSettingsData.CustomerSetting");
			faultTicketsSettingsData.CustomerSetting.SerialNumberPattern.ThrowIfNull("faultTicketsSettingsData.CustomerSetting.SerialNumberPattern");
			return faultTicketsSettingsData.CustomerSetting.SerialNumberPattern;
		}
		public Guid? GetFaultTicketsCustomerOpenMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.CustomerSetting.ThrowIfNull("faultTicketsSettingsData.CustomerSetting");
			return faultTicketsSettingsData.CustomerSetting.OpenMailTemplateId;
		}
		public Guid? GetFaultTicketsCustomerPendingMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.CustomerSetting.ThrowIfNull("faultTicketsSettingsData.CustomerSetting");
			return faultTicketsSettingsData.CustomerSetting.PendingMailTemplateId;
		}
		public Guid? GetFaultTicketsCustomerClosedMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.CustomerSetting.ThrowIfNull("faultTicketsSettingsData.CustomerSetting");
			return faultTicketsSettingsData.CustomerSetting.ClosedMailTemplateId;
		}
		public string GetFaultTicketsSupplierSerialNumberPattern()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.SupplierSetting.ThrowIfNull("faultTicketsSettingsData.SupplierSetting");
			faultTicketsSettingsData.SupplierSetting.SerialNumberPattern.ThrowIfNull("faultTicketsSettingsData.SupplierSetting.SerialNumberPattern");
			return faultTicketsSettingsData.SupplierSetting.SerialNumberPattern;
		}
		public Guid? GetFaultTicketsSupplierOpenMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.SupplierSetting.ThrowIfNull("faultTicketsSettingsData.SupplierSetting");
			return faultTicketsSettingsData.SupplierSetting.OpenMailTemplateId;
		}
		public Guid? GetFaultTicketsSupplierPendingMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.SupplierSetting.ThrowIfNull("faultTicketsSettingsData.SupplierSetting");
			return faultTicketsSettingsData.SupplierSetting.PendingMailTemplateId;
		}
		public Guid? GetFaultTicketsSupplierClosedMailTemplateId()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.SupplierSetting.ThrowIfNull("faultTicketsSettingsData.SupplierSetting");
			return faultTicketsSettingsData.SupplierSetting.ClosedMailTemplateId;
		}
		public long GetFaultTicketsCustomerSerialNumberInitialSequence()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.CustomerSetting.ThrowIfNull("faultTicketsSettingsData.CustomerSetting");
			return faultTicketsSettingsData.CustomerSetting.InitialSequence;
		}
		public long GetFaultTicketsSupplierSerialNumberInitialSequence()
		{
			FaultTicketsSettingsData faultTicketsSettingsData = GetFaultTicketsSettingsData();
			faultTicketsSettingsData.SupplierSetting.ThrowIfNull("faultTicketsSettingsData.SupplierSetting");
			return faultTicketsSettingsData.SupplierSetting.InitialSequence;
		}

		public PricingSettings MergePricingSettings(PricingSettings pricingSettingsParent, PricingSettings pricingSettingsChild)
		{
			PricingSettings result = new PricingSettings();
			result.DefaultRate = pricingSettingsParent.DefaultRate;
			result.MaximumRate = pricingSettingsParent.MaximumRate;
			result.EffectiveDateDayOffset = pricingSettingsParent.EffectiveDateDayOffset;
			result.RetroactiveDayOffset = pricingSettingsParent.RetroactiveDayOffset;
			result.NewRateDayOffset = pricingSettingsParent.NewRateDayOffset;
			result.EndCountryDayOffset = pricingSettingsParent.EndCountryDayOffset;
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

			if (pricingSettingsChild.EndCountryDayOffset.HasValue)
				result.EndCountryDayOffset = pricingSettingsChild.EndCountryDayOffset.Value;

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
			result.IncludeClosedEntities = pricelistSettingsParent.IncludeClosedEntities;
			result.CodeChangeTypeDescriptions = pricelistSettingsParent.CodeChangeTypeDescriptions;
			result.RateChangeTypeDescriptions = pricelistSettingsParent.RateChangeTypeDescriptions;
			result.SalePricelistFileNamePattern = pricelistSettingsParent.SalePricelistFileNamePattern;

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

			if (pricelistSettingsChild.IncludeClosedEntities.HasValue)
				result.IncludeClosedEntities = pricelistSettingsChild.IncludeClosedEntities;

			if (!string.IsNullOrEmpty(pricelistSettingsChild.SalePricelistFileNamePattern))
				result.SalePricelistFileNamePattern = pricelistSettingsChild.SalePricelistFileNamePattern;

			result.CodeChangeTypeDescriptions = MergeCodeChangeTypeDescriptions(pricelistSettingsParent.CodeChangeTypeDescriptions, pricelistSettingsChild.CodeChangeTypeDescriptions);
			result.RateChangeTypeDescriptions = MergeRateChangeTypeDescriptions(pricelistSettingsParent.RateChangeTypeDescriptions, pricelistSettingsChild.RateChangeTypeDescriptions);

			return result;
		}

		public CodeChangeTypeDescriptions MergeCodeChangeTypeDescriptions(CodeChangeTypeDescriptions parentCodeChangeTypeDescriptions, CodeChangeTypeDescriptions childCodeChangeTypeDescriptions)
		{
			CodeChangeTypeDescriptions result = new CodeChangeTypeDescriptions();
			result.NewCode = parentCodeChangeTypeDescriptions.NewCode;
			result.ClosedCode = parentCodeChangeTypeDescriptions.ClosedCode;
			result.NotChangedCode = parentCodeChangeTypeDescriptions.NotChangedCode;

			if (childCodeChangeTypeDescriptions == null)
				return result;
			if (!string.IsNullOrEmpty(childCodeChangeTypeDescriptions.NewCode))
				result.NewCode = childCodeChangeTypeDescriptions.NewCode;
			if (!string.IsNullOrEmpty(childCodeChangeTypeDescriptions.ClosedCode))
				result.ClosedCode = childCodeChangeTypeDescriptions.ClosedCode;
			if (!string.IsNullOrEmpty(childCodeChangeTypeDescriptions.NotChangedCode))
				result.NotChangedCode = childCodeChangeTypeDescriptions.NotChangedCode;

			return result;
		}
		public RateChangeTypeDescriptions MergeRateChangeTypeDescriptions(RateChangeTypeDescriptions parentRateChangeTypeDescriptions, RateChangeTypeDescriptions childRateChangeTypeDescriptions)
		{
			RateChangeTypeDescriptions result = new RateChangeTypeDescriptions();
			if (parentRateChangeTypeDescriptions != null)
			{
				result.NotChanged = parentRateChangeTypeDescriptions.NotChanged;
				result.NewRate = parentRateChangeTypeDescriptions.NewRate;
				result.IncreasedRate = parentRateChangeTypeDescriptions.IncreasedRate;
				result.DecreasedRate = parentRateChangeTypeDescriptions.DecreasedRate;
				result.DeletedRate = parentRateChangeTypeDescriptions.DeletedRate;
			}
			if (childRateChangeTypeDescriptions == null)
				return result;
			if (!string.IsNullOrEmpty(childRateChangeTypeDescriptions.NotChanged))
				result.NotChanged = childRateChangeTypeDescriptions.NotChanged;
			if (!string.IsNullOrEmpty(childRateChangeTypeDescriptions.NewRate))
				result.NewRate = childRateChangeTypeDescriptions.NewRate;
			if (!string.IsNullOrEmpty(childRateChangeTypeDescriptions.IncreasedRate))
				result.IncreasedRate = childRateChangeTypeDescriptions.IncreasedRate;
			if (!string.IsNullOrEmpty(childRateChangeTypeDescriptions.DecreasedRate))
				result.DecreasedRate = childRateChangeTypeDescriptions.DecreasedRate;
			if (!string.IsNullOrEmpty(childRateChangeTypeDescriptions.DeletedRate))
				result.DeletedRate = childRateChangeTypeDescriptions.DeletedRate;

			return result;
		}
        public bool GetCodeGroupVerification()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.CodeGroupVerfifcation;
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

		public long GetPurchaseMaximumCodeRange()
		{
			PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
			return purchaseAreaSettings.MaximumCodeRange;
		}
        public int GetPurchaseAcceptableIncreasedRate()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.AcceptableIncreasedRate;
        }
        public int GetPurchaseAcceptableDecreasedRate()
        {
            PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
            return purchaseAreaSettings.AcceptableDecreasedRate;
        }
		public int GetPurchaseAreaRetroactiveDayOffset()
		{
			PurchaseAreaSettingsData purchaseAreaSettings = GetPurchaseAreaSettings();
			return purchaseAreaSettings.RetroactiveDayOffset;
		}
		public List<PricelistTypeMapping> GetPurchasePricelistTypeMappingList()
		{
            AutoImportSPLSettings autoImportSettings = GetAutoImportSPLSettings();
            return autoImportSettings.PricelistTypeMappingList;
		}

		public SupplierAutoImportTemplate GetSupplierAutoImportTemplateByType(AutoImportEmailTypeEnum type)
		{
            AutoImportSPLSettings autoImportSettings = GetAutoImportSPLSettings();
            if (autoImportSettings.SupplierAutoImportTemplateList != null && autoImportSettings.SupplierAutoImportTemplateList.Count > 0)
			{
                return autoImportSettings.SupplierAutoImportTemplateList.FindRecord(item => item.TemplateType == type);
			}
			return null;
		}

		public InternalAutoImportTemplate GetInternalAutoImportTemplateByType(AutoImportEmailTypeEnum type)
		{
            AutoImportSPLSettings autoImportSettings = GetAutoImportSPLSettings();
            if (autoImportSettings.InternalAutoImportTemplateList != null && autoImportSettings.InternalAutoImportTemplateList.Count > 0)
			{
                return autoImportSettings.InternalAutoImportTemplateList.FindRecord(item => item.TemplateType == type);
			}
			return null;
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
		private SaleAreaTechnicalSettings GetSaleAreaTechnicalSetting()
		{
			SettingManager manager = new SettingManager();
			SaleAreaTechnicalSettings saleareaSettingData =
				manager.GetSetting<SaleAreaTechnicalSettings>(Constants.SaleAreaTechnicalSettings);
			if (saleareaSettingData == null)
				throw new NullReferenceException("SaleAreaTechnicalSettingsData");
			return saleareaSettingData;
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
		private CDRImportZoneIdentification GetCDRImportZoneIdentification()
		{
			CDRImportSettings cdrImportSettings = GetCDRImportSettings();
			if (cdrImportSettings.SwitchCDRMappingConfiguration == null)
				throw new NullReferenceException("cdrImportSettings.SwitchCDRMappingConfiguration");

			return cdrImportSettings.CDRImportZoneIdentification;
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
        private AutoImportSPLSettings GetAutoImportSPLSettings()
        {
            return GetSettings<AutoImportSPLSettings>(Constants.AutoImportSettings);
        }
		private FaultTicketsSettingsData GetFaultTicketsSettingsData()
		{
			SettingManager manager = new SettingManager();
			FaultTicketsSettingsData faultTicketsSettingsData =
				manager.GetSetting<FaultTicketsSettingsData>(Constants.FaultTicketsSettingsData);
			if (faultTicketsSettingsData == null)
				throw new NullReferenceException("FaultTicketsSettings");
			return faultTicketsSettingsData;
		}

		#endregion
	}
}
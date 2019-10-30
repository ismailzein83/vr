﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation;
using Vanrise.Rules;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager : BaseBusinessEntityManager, ICarrierAccountManager
    {
        #region ctor/Local Variables

        CarrierProfileManager _carrierProfileManager;
        SellingNumberPlanManager _sellingNumberPlanManager;
        SellingProductManager _sellingProductManager;
        LOBManager _lobManager;

        static CarrierAccountManager()
        {
            MappingRuleManager instance = new MappingRuleManager();
            instance.AddRuleCachingExpirationChecker(new CarrierAccountCachingExpirationChecker());
        }

        public CarrierAccountManager()
        {
            _carrierProfileManager = new CarrierProfileManager();
            _sellingNumberPlanManager = new SellingNumberPlanManager();
            _sellingProductManager = new SellingProductManager();
            _lobManager = new LOBManager();
        }

        #endregion

        #region Public Methods

        #region CarrierAccount
        public int GetAccountChannelsLimit(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException(String.Format("carrierAccount '{0}'", carrierAccountId));
            return carrierAccount.CarrierAccountSettings.ChannelsLimit;
        }
        public List<BankDetail> GetSupplierBankDetails(int carrierAccountId)
        {
            var supplier = GetCarrierAccount(carrierAccountId);
            supplier.ThrowIfNull("supplier", carrierAccountId);
            supplier.SupplierSettings.ThrowIfNull("supplier.SupplierSettings", carrierAccountId);
            if (supplier.SupplierSettings.SupplierBankDetails != null && supplier.SupplierSettings.SupplierBankDetails.Count > 0)
                return supplier.SupplierSettings.SupplierBankDetails;
            return _carrierProfileManager.GetSupplierBankDetails(supplier.CarrierProfileId);
        }

        public Dictionary<int, CarrierAccount> GetCachedCarrierAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierAccounts",
               () =>
               {
                   Dictionary<int, CarrierAccount> allCarrierAccounts = this.GetCachedCarrierAccountsWithDeleted();
                   Dictionary<int, CarrierAccount> carrierAccounts = new Dictionary<int, CarrierAccount>();
                   foreach (CarrierAccount item in allCarrierAccounts.Values)
                   {
                       if (!item.IsDeleted)
                           carrierAccounts.Add(item.CarrierAccountId, item);
                   }

                   return carrierAccounts;
               });
        }
        public Dictionary<string, List<CarrierAccount>> GetCachedSupplierAccountsByAutoImportEmail()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierAccountsByAutoImportEmail",
               () =>
               {
                   Dictionary<int, CarrierAccount> allCarrierAccounts = this.GetCachedCarrierAccounts();
                   var supplierAccountsByAutoImportEmail = new Dictionary<string, List<CarrierAccount>>();

                   foreach (CarrierAccount item in allCarrierAccounts.Values)
                   {
                       if (item.SupplierSettings == null || item.SupplierSettings.AutoImportSettings == null)
                           continue;

                       string email = item.SupplierSettings.AutoImportSettings.Email;
                       string code = item.SupplierSettings.AutoImportSettings.SubjectCode;

                       if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code))
                       {
                           email = email.ToLower();
                           code = code.ToLower();
                           List<CarrierAccount> allAccountsWithCommonEmail = supplierAccountsByAutoImportEmail.GetOrCreateItem(email);

                           foreach (var account in allAccountsWithCommonEmail)
                           {
                               if (account.SupplierSettings.AutoImportSettings.SubjectCode.ToLower() == code)
                                   throw new VRBusinessException(string.Format("Accounts of IDs '{0}' and '{1}' have the same Email '{2}' and Code '{3}'.", item.CarrierAccountId, account.CarrierAccountId, email, code));
                           }
                           allAccountsWithCommonEmail.Add(item);
                       }
                       else
                       {
                           if (item.SupplierSettings.AutoImportSettings.IsAutoImportActive)
                               throw new VRBusinessException(string.Format("Carrier account of ID '{0}' has activate auto import but do not have auto import email and subject code.", item.CarrierAccountId));
                       }
                   }
                   return supplierAccountsByAutoImportEmail;
               });
        }
        private Dictionary<int, CarrierAccount> GetCachedCarrierAccountsWithDeleted()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("AllCarrierAccounts",
               () =>
               {
                   ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                   IEnumerable<CarrierAccount> carrierAccounts = dataManager.GetCarrierAccounts();

                   Dictionary<int, CarrierAccount> dic = new Dictionary<int, CarrierAccount>();
                   CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

                   foreach (CarrierAccount item in carrierAccounts)
                   {
                       if (carrierProfileManager.IsCarrierProfileDeleted(item.CarrierProfileId))
                           item.IsDeleted = true;

                       dic.Add(item.CarrierAccountId, item);
                   }

                   return dic;
               });
        }

        public string GetSupplierAutoImportEmail(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);

            carrierAccount.ThrowIfNull("Carrier Account", carrierAccountId);
            carrierAccount.SupplierSettings.ThrowIfNull("CarrierAccount.SupplierSettings", carrierAccountId);

            var autoImportEmail = carrierAccount.SupplierSettings.AutoImportSettings.Email;

            if (string.IsNullOrEmpty(autoImportEmail))
                throw new DataIntegrityValidationException(string.Format("There is no auto import email for supplier with Id '{0}'.", carrierAccount.CarrierAccountId));

            return autoImportEmail;
        }
        public IEnumerable<ClientAccountInfo> GetClientAccountsInfo(CarrierAccountInfoFilter filter)
        {
            var carrierAccountsInfo = GetCarrierAccountInfo(filter);
            if (carrierAccountsInfo == null)
                return null;
            else
                return carrierAccountsInfo.MapRecords(ClientAccountInfoMapper);
        }

        public int GetSupplierEffectiveDateDayOffset(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);

            carrierAccount.ThrowIfNull("Carrier Account", carrierAccountId);
            carrierAccount.SupplierSettings.ThrowIfNull("CarrierAccount.CustomerSetting", carrierAccountId);

            var supplierEffectiveDateDayOffset = carrierAccount.SupplierSettings.EffectiveDateDayOffset;

            if (supplierEffectiveDateDayOffset.HasValue)
                return supplierEffectiveDateDayOffset.Value;

            ConfigManager configManager = new ConfigManager();
            return configManager.GetPurchaseAreaEffectiveDateDayOffset();
        }

        public decimal GetCustomerRoundedDefaultRate(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.DefaultRate != null)
                {
                    var longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
                    var defaultRate = customer.CustomerSettings.PricingSettings.DefaultRate.Value;
                    return Decimal.Round(defaultRate, longPrecision);
                }
            }

            return sellingProductManager.GetSellingProductRoundedDefaultRate(sellingProductId);
        }
        public decimal GetCustomerDefaultRate(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.DefaultRate != null)
                    return customer.CustomerSettings.PricingSettings.DefaultRate.Value;
            }

            return sellingProductManager.GetSellingProductDefaultRate(sellingProductId);
        }
        public decimal GetCustomerMaximumRate(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.MaximumRate != null)
                    return customer.CustomerSettings.PricingSettings.MaximumRate.Value;
            }

            return sellingProductManager.GetSellingProductMaximumRate(sellingProductId);
        }

        public int GetCustomerEffectiveDateDayOffset(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.EffectiveDateDayOffset != null)
                    return customer.CustomerSettings.PricingSettings.EffectiveDateDayOffset.Value;
            }

            return sellingProductManager.GetSellingProductEffectiveDateDayOffset(sellingProductId);
        }
        public int GetCustomerRetroactiveDayOffset(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.RetroactiveDayOffset != null)
                    return customer.CustomerSettings.PricingSettings.RetroactiveDayOffset.Value;
            }

            return sellingProductManager.GetSellingProductRetroactiveDayOffset(sellingProductId);
        }
        public int GetCustomerNewRateDayOffset(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.NewRateDayOffset != null)
                    return customer.CustomerSettings.PricingSettings.NewRateDayOffset.Value;
            }

            return sellingProductManager.GetSellingProductNewRateDayOffset(sellingProductId);
        }
        public int GetCustomerIncreasedRateDayOffset(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.IncreasedRateDayOffset != null)
                    return customer.CustomerSettings.PricingSettings.IncreasedRateDayOffset.Value;
            }

            return sellingProductManager.GetSellingProductIncreasedRateDayOffset(sellingProductId);
        }
        public int GetCustomerDecreasedRateDayOffset(int carrierAccountId)
        {
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = GetSellingProductId(carrierAccountId);

            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricingSettings.ThrowIfNull("Customer.CustomerSettings.PricingSettings", carrierAccountId);

            if (customer.CustomerSettings.PricingSettings != null)
            {
                if (customer.CustomerSettings.PricingSettings.DecreasedRateDayOffset != null)
                    return customer.CustomerSettings.PricingSettings.DecreasedRateDayOffset.Value;
            }

            return sellingProductManager.GetSellingProductDecreasedRateDayOffset(sellingProductId);
        }

        public string GetCustomerSubjectCode(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);
            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("customer.CustomerSettings", carrierAccountId);

            return customer.CustomerSettings.PricelistSettings != null
                ? customer.CustomerSettings.PricelistSettings.SubjectCode
                : null;
        }
        public PriceListExtensionFormat GetCustomerPriceListExtensionFormatId(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.PriceListExtensionFormat != null)
                    return customer.CustomerSettings.PricelistSettings.PriceListExtensionFormat.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).PriceListExtensionFormat.Value;
        }
        public SalePriceListType GetCustomerPriceListType(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.PriceListType != null)
                    return customer.CustomerSettings.PricelistSettings.PriceListType.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).PriceListType.Value;
        }
        public int GetCustomerPriceListTemplateId(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.DefaultSalePLTemplateId != null)
                    return customer.CustomerSettings.PricelistSettings.DefaultSalePLTemplateId.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).DefaultSalePLTemplateId.Value;
        }
        public Guid GetCustomerPriceListMailTemplateId(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.DefaultSalePLMailTemplateId != null)
                    return customer.CustomerSettings.PricelistSettings.DefaultSalePLMailTemplateId.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).DefaultSalePLMailTemplateId.Value;
        }
        public bool GetCustomerCompressPriceListFileStatus(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.CompressPriceListFile != null)
                    return customer.CustomerSettings.PricelistSettings.CompressPriceListFile.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).CompressPriceListFile.Value;
        }

        public IncludeClosedEntitiesEnum GetCustomerIncludeClosedEntitiesStatus(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);
            //customer.CustomerSettings.PricelistSettings.ThrowIfNull("Customer.CustomerSettings.PricelistSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (customer.CustomerSettings.PricelistSettings.IncludeClosedEntities != null)
                    return customer.CustomerSettings.PricelistSettings.IncludeClosedEntities.Value;
            }

            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).IncludeClosedEntities.Value;
        }

        public string GetCustomerPricelistFileNamePattern(int carrierAccountId)
        {
            var customer = GetCarrierAccount(carrierAccountId);

            customer.ThrowIfNull("Customer", carrierAccountId);
            customer.CustomerSettings.ThrowIfNull("Customer.CustomerSettings", carrierAccountId);

            if (customer.CustomerSettings.PricelistSettings != null)
            {
                if (!string.IsNullOrEmpty(customer.CustomerSettings.PricelistSettings.SalePricelistFileNamePattern))
                    return customer.CustomerSettings.PricelistSettings.SalePricelistFileNamePattern;
            }
            return GetCompanyPricelistSettingsByCustomerId(carrierAccountId).SalePricelistFileNamePattern;
        }

        public CodeChangeTypeDescriptions GetCustomerCodeChangeTypeSettings(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            var configManager = new ConfigManager();
            CodeChangeTypeDescriptions companyCodeChangeTypeDescriptions = GetCompanyPricelistSettingsByCustomerId(carrierAccountId).CodeChangeTypeDescriptions;

            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccountId);

            if (carrierAccount.CustomerSettings.PricelistSettings == null)
                return companyCodeChangeTypeDescriptions;
            return configManager.MergeCodeChangeTypeDescriptions(companyCodeChangeTypeDescriptions, carrierAccount.CustomerSettings.PricelistSettings.CodeChangeTypeDescriptions);
        }
        public RateChangeTypeDescriptions GetCustomerRateChangeTypeSettings(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            var configManager = new ConfigManager();
            RateChangeTypeDescriptions companyRateChangeTypeDescriptions = GetCompanyPricelistSettingsByCustomerId(carrierAccountId).RateChangeTypeDescriptions;

            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccountId);

            if (carrierAccount.CustomerSettings.PricelistSettings == null)
                return companyRateChangeTypeDescriptions;

            return configManager.MergeRateChangeTypeDescriptions(companyRateChangeTypeDescriptions, carrierAccount.CustomerSettings.PricelistSettings.RateChangeTypeDescriptions);
        }

        public PricingSettings GetCustomerPricingSettings(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);

            var sellingProductManager = new SellingProductManager();
            int sellingProductId = GetSellingProductId(carrierAccountId);
            var sellingProductPricingSettings = new PricingSettings();
            var configManager = new ConfigManager();

            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccountId);
            //carrierAccount.CustomerSettings.PricingSettings.ThrowIfNull("carrierAccount.CustomerSettings.PricingSettings", carrierAccountId);

            sellingProductPricingSettings = sellingProductManager.GetSellingProductPricingSettings(sellingProductId);

            return configManager.MergePricingSettings(sellingProductPricingSettings, carrierAccount.CustomerSettings.PricingSettings);
        }
        public PricelistSettings GetCustomerPricelistSettings(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            var configManager = new ConfigManager();
            var companyPricelistSettings = GetCompanyPricelistSettingsByCustomerId(carrierAccountId);

            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccountId);
            //carrierAccount.CustomerSettings.PricelistSettings.ThrowIfNull("carrierAccount.CustomerSettings.PricelistSettings", carrierAccountId);

            return configManager.MergePricelistSettings(companyPricelistSettings, carrierAccount.CustomerSettings.PricelistSettings);
        }
        public PurchasePricelistSettings GetSupplierPricelistSettings(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            var configManager = new ConfigManager();
            var companyPricelistSettings = GetCompanyPricelistSettingsBySupplierId(carrierAccountId);

            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.SupplierSettings.ThrowIfNull("carrierAccount.SupplierSettings", carrierAccountId);

            return configManager.MergePurchasePricelistSettings(companyPricelistSettings, carrierAccount.SupplierSettings.PricelistSettings);
        }
        public PurchasePricelistSettings GetCompanyPricelistSettingsBySupplierId(int carrierAccountId)
        {
            var companySetting = GetCompanySetting(carrierAccountId);
            return GetCompanyPurchasePricelistSettings(companySetting);

        }
        public PricelistSettings GetCompanyPricelistSettingsByCustomerId(int carrierAccountId)
        {
            var companySetting = GetCompanySetting(carrierAccountId);
            return GetCompanyPricelistSettings(companySetting);

        }
        public PurchasePricelistSettings GetCompanyPurchasePricelistSettings(CompanySetting companySetting)
        {
            var configManager = new ConfigManager();

            Vanrise.Common.Business.ConfigManager vanriseCommonBusinessConfigManager = new Vanrise.Common.Business.ConfigManager();
            CompanyPurchasePricelistSettings companyPricelistSettingsObj = vanriseCommonBusinessConfigManager.GetCompanyExtendedSettings<CompanyPurchasePricelistSettings>(companySetting);
            PurchasePricelistSettings companyPricelistSettings = (companyPricelistSettingsObj != null)
                ? companyPricelistSettingsObj.PricelistSettings
                : default(PurchasePricelistSettings);

            return configManager.MergePurchasePricelistSettings(configManager.GetPurchaseAreaPricelistSettings(), companyPricelistSettings);
        }
        public PricelistSettings GetCompanyPricelistSettings(CompanySetting companySetting)
        {
            var configManager = new ConfigManager();

            Vanrise.Common.Business.ConfigManager vanriseCommonBusinessConfigManager = new Vanrise.Common.Business.ConfigManager();
            CompanyPricelistSettings companyPricelistSettingsObj = vanriseCommonBusinessConfigManager.GetCompanyExtendedSettings<CompanyPricelistSettings>(companySetting);
            PricelistSettings companyPricelistSettings = (companyPricelistSettingsObj != null) ? companyPricelistSettingsObj.PricelistSettings : default(PricelistSettings);

            return configManager.MergePricelistSettings(configManager.GetSaleAreaPricelistSettings(), companyPricelistSettings);
        }

        public int GetCustomerTimeZoneId(int carrierAccountId, bool invoiceTimeZone = false)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierProfile.CustomerSettings", carrierAccountId);

            if (!carrierAccount.CustomerSettings.TimeZoneId.HasValue)
                return new CarrierProfileManager().GetCustomerTimeZoneId(carrierAccount.CarrierProfileId);

            if (!invoiceTimeZone)
                return carrierAccount.CustomerSettings.TimeZoneId.Value;

            if (!carrierAccount.CustomerSettings.InvoiceTimeZone)
                return new CarrierProfileManager().GetCustomerTimeZoneId(carrierAccount.CarrierProfileId);

            return carrierAccount.CustomerSettings.TimeZoneId.Value;
        }
        public int GetSupplierTimeZoneId(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException("carrierAccount");
            if (carrierAccount.SupplierSettings == null)
                throw new NullReferenceException("carrierProfile.SupplierSettings");
            if (carrierAccount.SupplierSettings.TimeZoneId.HasValue)
                return carrierAccount.SupplierSettings.TimeZoneId.Value;
            return new CarrierProfileManager().GetSupplierTimeZoneId(carrierAccount.CarrierProfileId);
        }
        public IEnumerable<CarrierAccount> GetCarriersByProfileId(int carrierProfileId, bool getCustomers, bool getSuppliers)
        {
            if (getCustomers && getSuppliers)
                return GetCachedCarrierAccounts().Values.Where(x => x.CarrierProfileId == carrierProfileId);
            else if (getCustomers)
                return GetAllCustomers().Where(x => x.CarrierProfileId == carrierProfileId);
            else if (getSuppliers)
                return GetAllSuppliers().Where(x => x.CarrierProfileId == carrierProfileId);
            return null;
        }
        public Vanrise.Entities.IDataRetrievalResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var allCarrierProfiles = carrierProfileManager.GetCarrierProfilesInfo();

            var allCarrierAccounts = GetCachedCarrierAccounts();

            Func<CarrierAccount, bool> filterExpression = (item) =>
              {
                  if (input.Query.Name != null && !IsMatchByName(input.Query.Name, item))
                      return false;
                  if (input.Query.CarrierProfilesIds != null && input.Query.CarrierProfilesIds.Count > 0 && !input.Query.CarrierProfilesIds.Contains(item.CarrierProfileId))
                      return false;
                  if (input.Query.CarrierAccountsIds != null && input.Query.CarrierAccountsIds.Count > 0 && !input.Query.CarrierAccountsIds.Contains(item.CarrierAccountId))
                      return false;
                  if (input.Query.ActivationStatusIds != null && input.Query.ActivationStatusIds.Count > 0 && !input.Query.ActivationStatusIds.Contains((int)item.CarrierAccountSettings.ActivationStatus))
                      return false;
                  if (input.Query.AccountsTypes != null && input.Query.AccountsTypes.Count > 0 && !input.Query.AccountsTypes.Contains(item.AccountType))
                      return false;
                  if (input.Query.SellingNumberPlanIds != null && input.Query.SellingNumberPlanIds.Count > 0 && !(item.AccountType != CarrierAccountType.Supplier && input.Query.SellingNumberPlanIds.Contains(item.SellingNumberPlanId)))
                      return false;
                  if (input.Query.SellingProductsIds != null && input.Query.SellingProductsIds.Count > 0 && !(item.AccountType != CarrierAccountType.Supplier && input.Query.SellingProductsIds.Contains(item.SellingProductId)))
                      return false;
                  if (input.Query.Services != null && input.Query.Services.Count > 0 && !(item.AccountType != CarrierAccountType.Customer && input.Query.Services.All(x => item.SupplierSettings.DefaultServices.Select(y => y.ServiceId).Contains(x))))
                      return false;
                  if (input.Query.LOBIds != null && !input.Query.LOBIds.Contains(item.LOBId))
                      return false;
                  if (input.Query.IsInterconnectSwitch == true && input.Query.IsInterconnectSwitch != item.CarrierAccountSettings.IsInterconnectSwitch)
                      return false;

                  if (input.Query.InvoiceTypes != null && input.Query.InvoiceTypes.Count > 0)
                  {
                      WHSCarrierFinancialAccountData financialAccountData;
                      WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
                      if (item.AccountType == CarrierAccountType.Customer || item.AccountType == CarrierAccountType.Exchange)
                      {
                          financialAccountManager.TryGetCustAccFinancialAccountData(item.CarrierAccountId, DateTime.Now, out financialAccountData);
                      }
                      else
                      {
                          financialAccountManager.TryGetSuppAccFinancialAccountData(item.CarrierAccountId, DateTime.Now, out financialAccountData);
                      }
                      if (financialAccountData == null || financialAccountData.FinancialAccount == null || !input.Query.InvoiceTypes.Contains(GetCarrierAccountInvoiceType(financialAccountData)))
                          return false;
                  }
                  if (input.Query.CompanySettingsIds != null && input.Query.CompanySettingsIds.Count > 0 && !input.Query.CompanySettingsIds.Contains(GetCompanySetting(item.CarrierAccountId).CompanySettingId))
                      return false;
                  return true;
              };
            var resultProcessingHandler = new ResultProcessingHandler<CarrierAccountDetail>()
            {
                ExportExcelHandler = new CarrierAccountDetailExportExcelHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(CarrierAccountLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierAccounts.ToBigResult(input, filterExpression, CarrierAccountDetailMapper), resultProcessingHandler);
        }
        public IEnumerable<CarrierAccount> GetAllCarriers()
        {
            return GetCachedCarrierAccounts().Values;
        }
        public CarrierAccount GetCarrierAccount(int carrierAccountId)
        {
            var CarrierAccounts = GetCachedCarrierAccounts();
            return CarrierAccounts.GetRecord(carrierAccountId);
        }

        public List<CarrierAccountInfo> GetCarrierAccountInfos(List<int> carrierAccountIds)
        {
            if (carrierAccountIds == null || carrierAccountIds.Count == 0)
                return null;

            var carrierAccounts = GetCachedCarrierAccounts();
            List<CarrierAccountInfo> carrierAccountInfos = new List<CarrierAccountInfo>();
            foreach (int carrierAccountId in carrierAccountIds)
                carrierAccountInfos.Add(CarrierAccountInfoMapper(GetCarrierAccount(carrierAccountId)));

            return carrierAccountInfos;
        }

        public string GetDescription(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            if (carrierAccountsIds.Count() > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (int carrierAccount in carrierAccountsIds)
                {
                    if (stringBuilder != null && stringBuilder.Length > 0)
                        stringBuilder.Append(",");
                    stringBuilder.AppendFormat("{0}", GetCarrierAccountName(carrierAccount));
                }
                return stringBuilder.ToString();
            }

            return string.Empty;
        }
        public IEnumerable<CarrierAccountInfo> GetCarrierAccountInfo(CarrierAccountInfoFilter filter)
        {
            Func<CarrierAccount, bool> filterPredicate = null;
            List<object> customObjects = null;

            if (filter != null)
            {
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(filter.SupplierFilterSettings);
                HashSet<int> filteredCustomerIds = CustomerGroupContext.GetFilteredCustomerIds(filter.CustomerFilterSettings);

                SellingProduct sellingProduct = null;
                IEnumerable<CustomerSellingProduct> customerSellingProductsEffectiveInFuture = null;
                if (filter.AssignableToSellingProductId.HasValue)
                {
                    sellingProduct = LoadSellingProduct(filter.AssignableToSellingProductId.Value);
                    customerSellingProductsEffectiveInFuture = LoadCustomerSellingProductsEffectiveInFuture();
                }

                // IEnumerable < AssignedCarrier > assignedCarriers = null;
                //if (filter.AssignableToUserId.HasValue)
                //{
                //    AccountManagerManager AccountManagerManager = new AccountManagerManager();
                //    assignedCarriers = AccountManagerManager.GetAssignedCarriers();
                //}

                if (filter.Filters != null)
                {
                    customObjects = new List<object>();
                    foreach (ICarrierAccountFilter carrierAccountFilter in filter.Filters)
                        customObjects.Add(null);
                }

                filterPredicate = (carr) =>
                {
                    if (filter.ExcludedCarrierAccountIds != null && filter.ExcludedCarrierAccountIds.Contains(carr.CarrierAccountId))
                        return false;

                    if (filter.GetExchangeCarriers && carr.AccountType != CarrierAccountType.Exchange)
                        return false;

                    if (!ShouldSelectCarrierAccount(carr, filter.GetCustomers, filter.GetSuppliers, filteredSupplierIds, filteredCustomerIds))
                        return false;

                    if (filter.AssignableToSellingProductId.HasValue && !IsAssignableToSellingProduct(carr, filter.AssignableToSellingProductId.Value, sellingProduct, customerSellingProductsEffectiveInFuture))
                        return false;

                    if (filter.LOBId.HasValue && !carr.LOBId.Equals(filter.LOBId.Value))
                        return false;

                    if (filter.SellingNumberPlanId.HasValue && carr.SellingNumberPlanId != filter.SellingNumberPlanId.Value)
                        return false;

                    if (filter.SellingProductId.HasValue && carr.SellingProductId != filter.SellingProductId.Value)
                        return false;

                    if (filter.CarrierProfileId.HasValue && carr.CarrierProfileId != filter.CarrierProfileId.Value)
                        return false;

                    if (filter.UserId.HasValue)
                    {
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        var carrierAccountIds = carrierProfileManager.GetCarrierProfileCarrierAccountsByUserId(filter.UserId.Value);
                        if (carrierAccountIds != null)
                        {
                            if (!carrierAccountIds.CarrierAccountIds.Contains(carr.CarrierAccountId))
                                return false;
                        }
                    }

                    carr.CarrierAccountSettings.ThrowIfNull("carr.CarrierAccountSettings", carr.CarrierAccountId);
                    if (filter.ActivationStatuses != null && filter.ActivationStatuses.Count() != 0 && !filter.ActivationStatuses.Contains(carr.CarrierAccountSettings.ActivationStatus))
                        return false;

                    //if (filter.AssignableToUserId.HasValue && !IsCarrierAccountAssignableToUser(carr, filter.GetCustomers, filter.GetSuppliers, assignedCarriers))
                    //    return false;

                    if (filter.Filters != null)
                    {
                        for (int i = 0; i < filter.Filters.Count(); i++)
                        {
                            var context = new CarrierAccountFilterContext() { CarrierAccount = carr, CustomObject = customObjects[i] };
                            if (filter.Filters.ElementAt(i).IsExcluded(context))
                                return false;
                            customObjects[i] = context.CustomObject;
                        }
                    }

                    return true;
                };
            }

            //TODO: fix this when we reach working with Account Manager module
            //if (filter != null && filter.AssignableToUserId != null)
            //    return GetCachedCarrierAccounts().MapRecords(AccountManagerCarrierMapper, filterPredicate).OrderBy(x => x.Name);
            //else
            return GetCachedCarrierAccounts().MapRecords(CarrierAccountInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }
        public IEnumerable<CarrierAccountInfo> GetCustomersBySellingNumberPlanId(int sellingNumberPlanId, bool onlyActive = false)
        {
            Func<CarrierAccount, bool> filterPredicate = (carr) =>
            {
                if (carr.AccountType == CarrierAccountType.Supplier || carr.SellingNumberPlanId.Value != sellingNumberPlanId)
                    return false;

                if (onlyActive && !IsCarrierAccountActive(carr))
                    return false;

                return true;
            };

            return GetCachedCarrierAccounts().MapRecords(CarrierAccountInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }
        public InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            var insertOperationOutput = new InsertCarrierAccountOperationOutput<CarrierAccountDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            var validationMessages = new List<string>();
            if (!ValidateCarrierAccountToAdd(carrierAccount, validationMessages))
            {
                insertOperationOutput.ValidationMessages = validationMessages;
                return insertOperationOutput;
            }

            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            carrierAccount.CreatedBy = loggedInUserId;
            carrierAccount.LastModifiedBy = loggedInUserId;

            var dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();

            if (!dataManager.Insert(carrierAccount, out var carrierAccountId))
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
                return insertOperationOutput;
            }

            if (IsSupplier(carrierAccount.AccountType))
            {
                var zoneServiceManager = new SupplierZoneServiceManager();
                zoneServiceManager.Insert(carrierAccountId, carrierAccount.SupplierSettings.DefaultServices);
            }

            new CarrierAccountStatusHistoryManager().AddAccountStatusHistory(carrierAccountId, carrierAccount.CarrierAccountSettings.ActivationStatus, null);

            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            carrierAccount.CarrierAccountId = carrierAccountId;
            VRActionLogger.Current.TrackAndLogObjectAdded(CarrierAccountLoggableEntity.Instance, carrierAccount);

            VREventManager vrEventManager = new VREventManager();
            vrEventManager.ExecuteEventHandlersAsync(new CarrierAccountStatusChangedEventPayload { CarrierAccountId = carrierAccountId });
            CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = carrierAccountDetail;

            return insertOperationOutput;
        }
        public UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccountToEdit carrierAccountToEdit)
        {
            UpdateCarrierAccountOperationOutput<CarrierAccountDetail> updateOperationOutput = new UpdateCarrierAccountOperationOutput<CarrierAccountDetail>();
            List<string> validationMessages;
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (TryUpdateCarrierAccount(carrierAccountToEdit, true, out validationMessages))
            {
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(this.GetCarrierAccount(carrierAccountToEdit.CarrierAccountId));

                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierAccountDetail;

            }
            else if (validationMessages.Count == 0)
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                updateOperationOutput.ValidationMessages = validationMessages;
            }

            return updateOperationOutput;
        }
        public bool TryUpdateCarrierAccount(CarrierAccountToEdit carrierAccountToEdit, bool withTracking, out List<string> validationMessages)
        {
            int carrierProfileId;
            bool isCarrierAccountStatusChanged = false;
            bool isCustomerRoutingStatusChanged = false;
            bool isSupplierRoutingStatusChanged = false;
            validationMessages = new List<string>();
            if (ValidateCarrierAccountToEdit(carrierAccountToEdit, out carrierProfileId, validationMessages))
            {
                ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                CarrierAccount cachedAccount = this.GetCarrierAccount(carrierAccountToEdit.CarrierAccountId);

                ActivationStatus previousActivationStatus = cachedAccount.CarrierAccountSettings.ActivationStatus;
                ActivationStatus activationStatus = carrierAccountToEdit.CarrierAccountSettings.ActivationStatus;

                isCarrierAccountStatusChanged = previousActivationStatus != activationStatus;
                if (cachedAccount.CustomerSettings != null && carrierAccountToEdit.CustomerSettings != null)
                {
                    isCustomerRoutingStatusChanged = cachedAccount.CustomerSettings.RoutingStatus != carrierAccountToEdit.CustomerSettings.RoutingStatus;
                }

                if (cachedAccount.SupplierSettings != null && carrierAccountToEdit.SupplierSettings != null)
                {
                    isSupplierRoutingStatusChanged = cachedAccount.SupplierSettings.RoutingStatus != carrierAccountToEdit.SupplierSettings.RoutingStatus;
                }


                carrierAccountToEdit.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();

                bool updateActionSucc = dataManager.Update(carrierAccountToEdit, carrierProfileId);

                if (CarrierAccountManager.IsSupplier(cachedAccount.AccountType))
                {
                    SupplierZoneServiceManager zoneServiceManager = new SupplierZoneServiceManager();
                    zoneServiceManager.UpdateSupplierDefaultService(carrierAccountToEdit.CarrierAccountId, carrierAccountToEdit.SupplierSettings.DefaultServices);
                }
                if (updateActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    CarrierAccount updatedCA = this.GetCarrierAccount(carrierAccountToEdit.CarrierAccountId);
                    if (withTracking)
                    {
                        var carrierAccount = GetCarrierAccount(carrierAccountToEdit.CarrierAccountId);
                        VRActionLogger.Current.TrackAndLogObjectUpdated(CarrierAccountLoggableEntity.Instance, updatedCA);
                    }
                    if (isCarrierAccountStatusChanged || isCustomerRoutingStatusChanged || isSupplierRoutingStatusChanged)
                    {
                        new CarrierAccountStatusHistoryManager().AddAccountStatusHistory(carrierAccountToEdit.CarrierAccountId, activationStatus, previousActivationStatus);

                        if (withTracking)
                        {
                            string actionDescription = string.Format("Status Changed from '{0}' to '{1}'", previousActivationStatus, activationStatus);
                            VRActionLogger.Current.LogObjectCustomAction(CarrierAccountLoggableEntity.Instance, "Status Changed", true, updatedCA, actionDescription);
                        }
                        VREventManager vrEventManager = new VREventManager();
                        vrEventManager.ExecuteEventHandlersAsync(new CarrierAccountStatusChangedEventPayload
                        {
                            CarrierAccountId = carrierAccountToEdit.CarrierAccountId,
                            IsCustomerRoutingStatusChanged = isCustomerRoutingStatusChanged,
                            IsSupplierRoutingStatusChanged = isSupplierRoutingStatusChanged,
                            IsCarrierAccountStatusChanged = isCarrierAccountStatusChanged
                        });
                    }
                    return true;
                }
            }
            return false;
        }
        public void UpdateCarrierAccountExtendedSetting<T>(int carrierAccountId, T extendedSettings) where T : class
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount.ExtendedSettings == null)
                carrierAccount.ExtendedSettings = new Dictionary<string, Object>();
            string extendedSettingName = typeof(T).FullName;

            Object exitingExtendedSettings;
            if (carrierAccount.ExtendedSettings.TryGetValue(extendedSettingName, out exitingExtendedSettings))
            {
                carrierAccount.ExtendedSettings[extendedSettingName] = extendedSettings;
            }
            else
            {
                carrierAccount.ExtendedSettings.Add(extendedSettingName, extendedSettings);
            }
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            if (dataManager.UpdateExtendedSettings(carrierAccountId, carrierAccount.ExtendedSettings))
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

        }
        public bool UpdateAccountRoutingStatus(int carrierAccountId, RoutingStatus routingStatus, bool withTracking)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId).VRDeepCopy();
            carrierAccount.CustomerSettings.RoutingStatus = routingStatus;
            List<string> validationMessages;
            return TryUpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount), withTracking, out validationMessages);
        }
        public int GetSellingProductId(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new DataIntegrityValidationException(string.Format("Carrier Account with Id {0} is not found", carrierAccountId));

            if (!IsCustomer(carrierAccount.AccountType))
                throw new InvalidOperationException(string.Format("Getting selling product id only works with carrier accounts of type customer. Carrier Account Id {0}", carrierAccountId));

            if (!carrierAccount.SellingProductId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to any selling product", carrierAccountId));

            return carrierAccount.SellingProductId.Value;
        }
        public IEnumerable<int> GetCarrierAccountIdsAssignedToSellingProduct(int sellingProductId)
        {
            var carrierAccountIds = new List<int>();
            var carrierAccountsAssignedToSellingProduct = GetCarrierAccountsAssignedToSellingProduct(sellingProductId);

            foreach (var carrierAccount in carrierAccountsAssignedToSellingProduct)
            {
                carrierAccountIds.Add(carrierAccount.CarrierAccountId);
            }

            return carrierAccountIds;
        }
        public IEnumerable<CarrierAccountInfo> GetCarrierAccountsAssignedToSellingProduct(int sellingProductId)
        {
            var carrierAccounts = new List<CarrierAccountInfo>();
            Dictionary<int, CarrierAccount> carrierAccountsById = GetCachedCarrierAccounts();

            foreach (CarrierAccount carrierAccount in carrierAccountsById.Values)
            {
                if (!IsCarrierAccountDeleted(carrierAccount.CarrierAccountId) && IsCarrierAccountActive(carrierAccount) && carrierAccount.SellingProductId.HasValue && carrierAccount.SellingProductId.Value == sellingProductId)
                    carrierAccounts.Add(CarrierAccountInfoMapper(carrierAccount));
            }

            return carrierAccounts.OrderBy(x => x.Name);
        }
        public List<int> GetCustomersIdsAssignedToSellingNumberPlanId(int sellingNumberPlanId)
        {
            return GetCustomersBySellingNumberPlanId(sellingNumberPlanId).Select(x => x.CarrierAccountId).ToList();
        }
        public CarrierAccount GetCarrierAccountHistoryDetailbyHistoryId(int carrierAccountHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var carrierAccount = s_vrObjectTrackingManager.GetObjectDetailById(carrierAccountHistoryId);
            return carrierAccount.CastWithValidate<CarrierAccount>("Carrier Account : historyId ", carrierAccountHistoryId);
        }
        public IEnumerable<RoutingCustomerInfoDetails> GetRoutingCustomerInfoDetailsByCustomersIds(IEnumerable<int> customerIds)
        {
            if (customerIds == null || customerIds.Count() == 0)
                return null;

            var dataByCustomer = new List<RoutingCustomerInfoDetails>();

            foreach (int customerId in customerIds)
            {
                int sellingProductId = GetSellingProductId(customerId);
                dataByCustomer.Add(new RoutingCustomerInfoDetails() { CustomerId = customerId, SellingProductId = sellingProductId });
            }

            return dataByCustomer;
        }
        public bool IsInterconnectSwitch(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CarrierAccountSettings.ThrowIfNull("carrierAccount.CarrierAccountSettings", carrierAccountId);

            return carrierAccount.CarrierAccountSettings.IsInterconnectSwitch;
        }
        public IEnumerable<int> GetCustomerIdsBySellingProductId(int sellingProductId)
        {
            return GetAllCustomers().MapRecords(x => x.CarrierAccountId, x => x.SellingProductId.Value == sellingProductId);
        }

        public List<SMSServiceType> GetCustomerSMSServiceTypes(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            return GetCustomerSMSServiceTypes(carrierAccount);
        }

        public List<SMSServiceType> GetSupplierSMSServiceTypes(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            return GetSupplierSMSServiceTypes(carrierAccount);
        }

        public List<SMSServiceType> GetCustomerSMSServiceTypes(CarrierAccount carrierAccount)
        {
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccount.CarrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccount.CarrierAccountId);

            List<SMSServiceType> smsServicetypes = new List<SMSServiceType>();

            return GetSMSServiceTypesEntities(carrierAccount.CustomerSettings.SMSServiceTypes);
        }

        public List<SMSServiceType> GetSupplierSMSServiceTypes(CarrierAccount carrierAccount)
        {
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccount.CarrierAccountId);
            carrierAccount.SupplierSettings.ThrowIfNull("carrierAccount.SupplierSettings", carrierAccount.CarrierAccountId);

            SMSServiceTypeManager smsServiceTypeManager = new SMSServiceTypeManager();
            List<SMSServiceType> smsServicetypes = new List<SMSServiceType>();

            return GetSMSServiceTypesEntities(carrierAccount.SupplierSettings.SMSServiceTypes);
        }

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="carrierAccountId"></param>
        /// <returns></returns>
        public bool IsCustomerValideForCDR(int carrierAccountId)
        {
            CarrierAccount carrierAccount = this.GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                return false;

            if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                return false;

            return true;
        }

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="carrierAccountId"></param>
        /// <returns></returns>
        public bool IsSupplierValideForCDR(int carrierAccountId)
        {
            CarrierAccount carrierAccount = this.GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                return false;

            if (carrierAccount.AccountType == CarrierAccountType.Customer)
                return false;

            return true;
        }

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="carrierAccountId"></param>
        /// <returns></returns>
        public bool IsCustomerPassThrough(int carrierAccountId)
        {
            CarrierAccount carrierAccount = this.GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                return false;

            if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                return false;

            CarrierAccountCustomerSettings carrierAccountCustomerSettings = carrierAccount.CustomerSettings;
            if (carrierAccountCustomerSettings == null)
                return false;

            return carrierAccountCustomerSettings.PassThroughCustomerRateEvaluator != null;
        }

        /// <summary>
        /// Used In DataTransformation
        /// </summary>
        /// <param name="carrierAccountId"></param>
        /// <returns></returns>
        public decimal? EvaluatePassThroughCustomerRate(int carrierAccountId, decimal? costRate, int? costCurrencyId)
        {
            CarrierAccount carrierAccount = this.GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            carrierAccount.CustomerSettings.ThrowIfNull("carrierAccount.CustomerSettings", carrierAccountId);
            carrierAccount.CustomerSettings.PassThroughCustomerRateEvaluator.ThrowIfNull("carrierAccount.CustomerSettings.PassThroughCustomerRateEvaluator", carrierAccountId);

            PassThroughEvaluateCustomerRateContext context = new PassThroughEvaluateCustomerRateContext() { CostRate = costRate, CostCurrencyId = costCurrencyId };
            return carrierAccount.CustomerSettings.PassThroughCustomerRateEvaluator.EvaluateCustomerRate(context);
        }

        #endregion

        #region ExtensionConfiguration
        public IEnumerable<CustomerGroupConfig> GetCustomersGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CustomerGroupConfig>(CustomerGroupConfig.EXTENSION_TYPE);
        }
        public IEnumerable<SupplierGroupConfig> GetSupplierGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SupplierGroupConfig>(SupplierGroupConfig.EXTENSION_TYPE);
        }
        public IEnumerable<SuppliersWithZonesGroupSettingsConfig> GetSuppliersWithZonesGroupsTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SuppliersWithZonesGroupSettingsConfig>(SuppliersWithZonesGroupSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<CustomerGroupConfig> GetCustomerGroupTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<CustomerGroupConfig>(CustomerGroupConfig.EXTENSION_TYPE);
        }
        public IEnumerable<PassThroughCustomerRateEvaluatorExtensionConfig> GetPassThroughCustomerRateExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<PassThroughCustomerRateEvaluatorExtensionConfig>(PassThroughCustomerRateEvaluatorExtensionConfig.EXTENSION_TYPE);
        }

        #endregion

        #region RoutingCarriers
        public IEnumerable<RoutingCustomerInfo> GetRoutingActiveCustomers()
        {
            IEnumerable<CarrierAccount> carrierAccounts = GetCarrierAccountsByType(true, false, null, null);

            Func<CarrierAccount, bool> filterExpression = (item) => (item.CarrierAccountSettings != null && item.CustomerSettings != null && (item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active || item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Testing) && item.CustomerSettings.RoutingStatus == RoutingStatus.Enabled);
            return carrierAccounts.MapRecords(RoutingCustomerInfoMapper, filterExpression);
        }

        public IEnumerable<RoutingSupplierInfo> GetRoutingActiveSuppliers()
        {
            IEnumerable<CarrierAccount> carrierAccounts = GetCarrierAccountsByType(false, true, null, null);
            Func<CarrierAccount, bool> filterExpression = (item) => (item.CarrierAccountSettings != null && item.SupplierSettings != null && (item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active || item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Testing) && item.SupplierSettings.RoutingStatus == RoutingStatus.Enabled);
            return carrierAccounts.MapRecords(RoutingSupplierInfolMapper, filterExpression);

        }
        #endregion

        #region Special Methods
        public int GetSellingNumberPlanId(int carrierAccountId, CarrierAccountType carrierAccountType = CarrierAccountType.Customer)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException(String.Format("carrierAccount '{0}'", carrierAccountId));
            if (!IsCustomer(carrierAccount.AccountType))
                throw new Exception(String.Format("Carrier Account '{0}' is not Customer", carrierAccountId));
            if (!carrierAccount.SellingNumberPlanId.HasValue)
                throw new NullReferenceException(String.Format("carrierAccount.SellingNumberPlanId. CarrierAccountId '{0}'", carrierAccountId));
            return carrierAccount.SellingNumberPlanId.Value;
        }
        public int? GetCustomerSellingNumberPlanId(int customerId)
        {
            var customer = GetCarrierAccount(customerId);
            if (customer == null || customer.SellingNumberPlanId == null)
                return null;
            else
                return customer.SellingNumberPlanId;
        }
        public int GetCarrierAccountCurrencyId(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAcount", carrierAccountId);
            return GetCarrierAccountCurrencyId(carrierAccount);
        }

        public int GetCarrierAccountCurrencyId(CarrierAccount carrierAccount)
        {
            if (carrierAccount == null)
                throw new NullReferenceException("carrierAccount");
            if (carrierAccount.CarrierAccountSettings == null)
                throw new NullReferenceException("carrierAccount.CarrierAccountSettings");
            if (carrierAccount.CarrierAccountSettings.CurrencyId != null)
                return carrierAccount.CarrierAccountSettings.CurrencyId;
            return new CarrierProfileManager().GetCarrierProfileCurrencyId(carrierAccount.CarrierProfileId);
        }
        public bool IsCarrierAccountDeleted(int carrierAccountId)
        {
            var carrierAccounts = GetCachedCarrierAccountsWithDeleted();
            CarrierAccount carrierAccount = carrierAccounts.GetRecord(carrierAccountId);

            if (carrierAccount == null)
                throw new DataIntegrityValidationException(string.Format("carrierAccount with Id {0} is not found", carrierAccountId));

            return carrierAccount.IsDeleted;
        }
        public bool IsCarrierAccountActive(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            return IsCarrierAccountActive(carrierAccount);
        }

        public bool IsCarrierAccountActive(CarrierAccount carrierAccount)
        {
            carrierAccount.CarrierAccountSettings.ThrowIfNull("carrierAccount.CarrierAccountSettings", carrierAccount.CarrierAccountId);
            return IsStatusActive(carrierAccount.CarrierAccountSettings.ActivationStatus);
        }

        public bool IsStatusActive(ActivationStatus status)
        {
            return status != ActivationStatus.Inactive;
        }

        public int? GetCarrierProfileId(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount != null)
                return carrierAccount.CarrierProfileId;
            return null;
        }
        public T GetExtendedSettings<T>(int carrierAccountId) where T : class
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            return carrierAccount != null ? GetExtendedSettings<T>(carrierAccount) : default(T);
        }
        public T GetExtendedSettings<T>(CarrierAccount carrierAccount) where T : class
        {
            string extendedSettingName = typeof(T).FullName;
            Object exitingExtendedSettings;
            if (carrierAccount.ExtendedSettings != null)
            {
                carrierAccount.ExtendedSettings.TryGetValue(extendedSettingName, out exitingExtendedSettings);
                if (exitingExtendedSettings != null)
                    return exitingExtendedSettings as T;
                else return default(T);
            }
            else
                return default(T);
        }

        #endregion

        #region Settings
        public CompanySetting GetCompanySetting(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException(string.Format("carrierAccount carrierAccountId: {0}", carrierAccountId));
            if (carrierAccount.CarrierAccountSettings == null)
                throw new NullReferenceException(string.Format("carrierAccount.CarrierAccountSettings carrierAccountId: {0}", carrierAccountId));
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            if (carrierAccount.CarrierAccountSettings.CompanySettingId.HasValue)
            {
                return configManager.GetCompanySettingById(carrierAccount.CarrierAccountSettings.CompanySettingId.Value);
            }
            else
            {
                return new CarrierProfileManager().GetCompanySetting(carrierAccount.CarrierProfileId);
            }
        }
        public Dictionary<Guid, InvoiceReportFile> GetCompanySettingInvoiceReportFiles(int carrierAccountId)
        {
            return GetCompanySetting(carrierAccountId).InvoiceReportFiles;
        }
        public IEnumerable<Guid> GetBankDetails(int carrierAccountId)
        {
            var carrierProfileId = GetCarrierProfileId(carrierAccountId);
            if (carrierProfileId.HasValue)
            {
                var carrierProfile = _carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);
                if (carrierProfile != null && carrierProfile.Settings != null && carrierProfile.Settings.BankDetailsIds != null && carrierProfile.Settings.BankDetailsIds.Count > 0)
                {
                    return carrierProfile.Settings.BankDetailsIds;
                }
            }
            var companySettings = GetCompanySetting(carrierAccountId);
            return companySettings.BankDetails;
        }
        #endregion

        #endregion

        #region ICarrierAccountManager Members

        public List<CarrierAccount> GetAllSuppliers()
        {
            return GetAllCarriers().FindAllRecords(ca => IsSupplier(ca.AccountType)).ToList();
        }

        public List<CarrierAccount> GetAllCustomers()
        {
            return GetAllCarriers().FindAllRecords(ca => IsCustomer(ca.AccountType)).ToList();
        }

        public List<CarrierAccount> GetSuppliers(IEnumerable<int> suppliersIds)
        {
            throw new NotImplementedException();
        }

        public string GetCarrierAccountName(int carrierAccountId, ModuleName? moduleName)
        {
            if (moduleName.HasValue && Helper.ShouldFilterCarrierAccount(moduleName.Value))
            {
                List<int> currentUserCarrierAccountIds = null;
                bool isAccountManager = new AccountManagerAssignmentManager().TryGetCurrentUserEffectiveNowCarrierAccountIds(out currentUserCarrierAccountIds);

                if (isAccountManager && (currentUserCarrierAccountIds == null || !currentUserCarrierAccountIds.Contains(carrierAccountId)))
                    return "N/A";
            }

            return GetCarrierAccountName(carrierAccountId);
        }

        public string GetCarrierAccountName(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            return GetCarrierAccountName(carrierAccount);
        }

        public string GetCarrierAccountName(CarrierAccount carrierAccount)
        {
            if (carrierAccount == null)
                return null;
            string profileName = _carrierProfileManager.GetCarrierProfileName(carrierAccount.CarrierProfileId);
            return GetCarrierAccountName(profileName, carrierAccount.NameSuffix);
        }

        #endregion

        #region Validation Methods

        private bool ValidateCarrierAccountToAdd(CarrierAccount carrierAccount, List<string> validationMessages)
        {
            var carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
            if (carrierProfile == null)
                throw new DataIntegrityValidationException(String.Format("CarrierProfile '{0}' does not exist", carrierAccount.CarrierProfileId));

            if (carrierAccount.SellingNumberPlanId.HasValue)
            {
                if (carrierAccount.AccountType == CarrierAccountType.Supplier)
                    throw new DataIntegrityValidationException(String.Format("Supplier cannot be associated with SellingNumberPlan '{0}'", carrierAccount.SellingNumberPlanId.Value));

                var sellingNumberPlanManager = new SellingNumberPlanManager();
                SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(carrierAccount.SellingNumberPlanId.Value);
                if (sellingNumberPlan == null)
                    throw new DataIntegrityValidationException(String.Format("SellingNumberPlan '{0}' does not exist", carrierAccount.SellingNumberPlanId.Value));
            }
            else
            {
                if (CarrierAccountManager.IsCustomer(carrierAccount.AccountType))
                    throw new DataIntegrityValidationException(String.Format("{0} must be associated with a SellingNumberPlan", carrierAccount.AccountType.ToString()));
            }

            if (carrierAccount.SupplierSettings != null && carrierAccount.SupplierSettings.AutoImportSettings != null)
            {
                string email = carrierAccount.SupplierSettings.AutoImportSettings.Email;
                string code = carrierAccount.SupplierSettings.AutoImportSettings.SubjectCode;
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code))
                {
                    var supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
                    CarrierAccount duplicatedCarrierAccount = GetSupplierByAutomaticEmailAndSubjectCode(email, code);
                    if (duplicatedCarrierAccount != null)
                    {
                        validationMessages.Add(string.Format("Cannot set auto import email {0} and subject code {1} because they are already set for carrier account '{2}'.", email, code, GetCarrierAccountName(duplicatedCarrierAccount.CarrierAccountId)));
                        return false;
                    }
                }
            }

            return ValidateCarrierAccount(carrierAccount.NameSuffix, carrierAccount.CarrierAccountSettings, carrierAccount.SellingProductId, validationMessages);
        }

        private bool ValidateCarrierAccountToEdit(CarrierAccountToEdit carrierAccount, out int carrierProfileId, List<string> validationMessages)
        {
            carrierProfileId = -1;
            var carrierAccountEntity = this.GetCarrierAccount(carrierAccount.CarrierAccountId);
            if (carrierAccountEntity == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not exist", carrierAccount.CarrierAccountId));

            var carrierProfile = _carrierProfileManager.GetCarrierProfile(carrierAccountEntity.CarrierProfileId);
            if (carrierProfile == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not belong to a CarrierProfile", carrierAccount.CarrierAccountId));

            if (carrierAccountEntity.CarrierAccountSettings.CurrencyId != carrierAccount.CarrierAccountSettings.CurrencyId)
            {
                var areEffectiveOrFutureCountriesSoldToCustomer = new CustomerCountryManager().AreEffectiveOrFutureCountriesSoldToCustomer(carrierAccount.CarrierAccountId, DateTime.Now);
                if (areEffectiveOrFutureCountriesSoldToCustomer)
                    validationMessages.Add("Cannot modify selling product cannot in case of customer has sold countries");
            }
            if (carrierAccountEntity.SellingProductId != carrierAccount.SellingProductId)
            {
                var areEffectiveOrFutureCountriesSoldToCustomer = new CustomerCountryManager().AreEffectiveOrFutureCountriesSoldToCustomer(carrierAccount.CarrierAccountId, DateTime.Now);
                if (areEffectiveOrFutureCountriesSoldToCustomer)
                    validationMessages.Add("Cannot modify currency cannot in case of customer has sold countries");
            }

            carrierProfileId = carrierProfile.CarrierProfileId;

            if (carrierAccount.SupplierSettings != null && carrierAccount.SupplierSettings.AutoImportSettings != null)
            {
                string email = carrierAccount.SupplierSettings.AutoImportSettings.Email;
                string code = carrierAccount.SupplierSettings.AutoImportSettings.SubjectCode;
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code))
                {
                    var supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
                    CarrierAccount duplicatedCarrierAccount = GetSupplierByAutomaticEmailAndSubjectCode(email, code);
                    if (duplicatedCarrierAccount != null && duplicatedCarrierAccount.CarrierAccountId != carrierAccountEntity.CarrierAccountId)
                    {
                        validationMessages.Add(string.Format("Cannot set auto import email {0} and subject code {1} because they are already set for carrier account '{2}'.", email, code, GetCarrierAccountName(duplicatedCarrierAccount.CarrierAccountId)));
                        return false;
                    }
                }
            }

            return ValidateCarrierAccount(carrierAccount.NameSuffix, carrierAccount.CarrierAccountSettings, carrierAccount.SellingProductId, validationMessages);
        }

        public CarrierAccount GetSupplierByAutomaticEmailAndSubjectCode(string email, string subjectCode)
        {
            Dictionary<string, List<CarrierAccount>> supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
            List<CarrierAccount> suppliersHavingSpecifiedEmail = supplierAccounts.GetRecord(email.ToLower());
            if (suppliersHavingSpecifiedEmail != null)
            {
                foreach (var account in suppliersHavingSpecifiedEmail)
                {
                    if (subjectCode.ToLower() == account.SupplierSettings.AutoImportSettings.SubjectCode.ToLower())
                        return account;
                }
            }
            return null;
        }

        public CarrierAccount GetMatchedSupplier(string from, string subject)
        {
            Dictionary<string, List<CarrierAccount>> supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
            List<CarrierAccount> suppliersHavingSpecifiedEmail = supplierAccounts.GetRecord(from.ToLower());
            if (suppliersHavingSpecifiedEmail != null)
            {
                foreach (var account in suppliersHavingSpecifiedEmail)
                {
                    if (account.SupplierSettings.AutoImportSettings.IsAutoImportActive && subject.ToLower().Contains(account.SupplierSettings.AutoImportSettings.SubjectCode.ToLower()))
                        return account;
                }
            }
            return null;
        }

        private bool ValidateCarrierAccount(string caNameSuffix, CarrierAccountSettings caSettings, int? sellingProductId, List<string> validationMessages)
        {
            //if (String.IsNullOrWhiteSpace(caNameSuffix))
            //    throw new MissingArgumentValidationException("CarrierAccount.NameSuffix"); // bug: 2164

            if (caSettings == null)
                throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings");

            //if (String.IsNullOrWhiteSpace(caSettings.Mask))
            //    throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings.Mask");

            var currencyManager = new CurrencyManager();
            var currencyId = caSettings.CurrencyId;
            Currency currency = currencyManager.GetCurrency(currencyId);
            if (currency == null)
                throw new DataIntegrityValidationException(String.Format("Currency '{0}' does not exist", caSettings.CurrencyId));

            if (sellingProductId.HasValue)
            {
                var sellingProductCurrencyId = new SellingProductManager().GetSellingProductCurrencyId(sellingProductId.Value);

                if (currencyId != sellingProductCurrencyId)
                {
                    validationMessages.Add("Carrier account and selling product must have the same currency");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        private ClientAccountInfo ClientAccountInfoMapper(CarrierAccountInfo account)
        {
            var clientAccountInfo = new ClientAccountInfo
            {
                AccountId = account.CarrierAccountId,
                Name = account.Name,
                CurrencyId = account.CurrencyId
            };
            if (account.AccountType == CarrierAccountType.Exchange)
                clientAccountInfo.CarrierAccountType = ClientAccountType.Exchange;
            else if (account.AccountType == CarrierAccountType.Customer)
                clientAccountInfo.CarrierAccountType = ClientAccountType.Customer;
            else if (account.AccountType == CarrierAccountType.Supplier)
                clientAccountInfo.CarrierAccountType = ClientAccountType.Supplier;
            if (account.ActivationStatus == ActivationStatus.Active)
                clientAccountInfo.ActivationStatus = ClientActivationStatus.Active;
            else if (account.ActivationStatus == ActivationStatus.Inactive)
                clientAccountInfo.ActivationStatus = ClientActivationStatus.Inactive;
            else if (account.ActivationStatus == ActivationStatus.Testing)
                clientAccountInfo.ActivationStatus = ClientActivationStatus.Testing;
            return clientAccountInfo;
        }
        private IEnumerable<CarrierAccount> GetCarrierAccountsByIds(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            var carrierAccounts = this.GetCarrierAccountsByType(getCustomers, getSuppliers, null, null);
            Func<CarrierAccount, bool> filterExpression = null;

            if (carrierAccountsIds != null)
                filterExpression = (item) => (carrierAccountsIds.Contains(item.CarrierAccountId));

            return carrierAccounts.FindAllRecords(filterExpression);
        }

        private bool ShouldSelectCarrierAccount(CarrierAccount carrierAccount, bool getCustomers, bool getSuppliers)
        {
            return ShouldSelectCarrierAccount(carrierAccount, getCustomers, getSuppliers, null, null);
        }

        private bool ShouldSelectCarrierAccount(CarrierAccount carrierAccount, bool getCustomers, bool getSuppliers, HashSet<int> filteredSupplierIds, HashSet<int> filteredCustomerIds)
        {
            bool isSupplier = IsSupplier(carrierAccount.AccountType);
            bool isCustomer = IsCustomer(carrierAccount.AccountType);
            if (getCustomers && getSuppliers)
                return true;
            if (getCustomers && !isCustomer)
                return false;
            if (getSuppliers && !isSupplier)
                return false;
            if (isSupplier && filteredSupplierIds != null && !filteredSupplierIds.Contains(carrierAccount.CarrierAccountId))
                return false;
            if (isCustomer && filteredCustomerIds != null && !filteredCustomerIds.Contains(carrierAccount.CarrierAccountId))
                return false;
            return true;
        }

        public static bool IsCustomer(CarrierAccountType carrierAccountType)
        {
            return carrierAccountType == CarrierAccountType.Customer || carrierAccountType == CarrierAccountType.Exchange;
        }

        public static bool IsSupplier(CarrierAccountType carrierAccountType)
        {
            return carrierAccountType == CarrierAccountType.Supplier || carrierAccountType == CarrierAccountType.Exchange;
        }

        private bool IsMatchByName(string accountName, CarrierAccount carrierAccount)
        {
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile itemProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);

            if (itemProfile == null)
                throw new DataIntegrityValidationException(string.Format("Carrier Account with Id {0} is not linked to any profile", carrierAccount.CarrierAccountId));

            string carrierAccountName = GetCarrierAccountName(itemProfile.Name, carrierAccount.NameSuffix);
            if (carrierAccountName.ToLower().Contains(accountName.ToLower()))
                return true;

            return false;
        }

        private IEnumerable<CarrierAccount> GetCarrierAccountsByType(bool getCustomers, bool getSuppliers, SupplierFilterSettings supplierFilterSettings, CustomerFilterSettings customerFilterSettings)
        {
            Dictionary<int, CarrierAccount> carrierAccounts = GetCachedCarrierAccounts();
            List<CarrierAccount> filteredList = null;

            if (carrierAccounts != null)
            {
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                HashSet<int> filteredCustomerIds = CustomerGroupContext.GetFilteredCustomerIds(customerFilterSettings);

                filteredList = new List<CarrierAccount>();

                foreach (CarrierAccount carr in carrierAccounts.Values)
                {
                    if (ShouldSelectCarrierAccount(carr, getCustomers, getSuppliers, filteredSupplierIds, filteredCustomerIds))
                        filteredList.Add(carr);
                }
            }

            return filteredList;
        }

        private SellingProduct LoadSellingProduct(int sellingProductId)
        {
            SellingProductManager sellingProductManager = new SellingProductManager();
            SellingProduct sellingProduct = sellingProductManager.GetSellingProduct(sellingProductId);
            if (sellingProduct == null)
                throw new NullReferenceException(String.Format("SellingProduct '{0}'", sellingProductId));

            return sellingProduct;
        }

        private IEnumerable<CustomerSellingProduct> LoadCustomerSellingProductsEffectiveInFuture()
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            return customerSellingProductManager.GetEffectiveInFutureCustomerSellingProduct();
        }

        private bool IsAssignableToSellingProduct(CarrierAccount carrierAccount, int sellingProductId, SellingProduct sellingProduct,
          IEnumerable<CustomerSellingProduct> customerSellingProductsEffectiveInFuture)
        {
            //if (!ShouldSelectCarrierAccount(carrierAccount, true, false))
            //    return false;

            if (carrierAccount.SellingNumberPlanId.Value != sellingProduct.SellingNumberPlanId)
                return false;

            if (customerSellingProductsEffectiveInFuture.Any(x => x.CustomerId == carrierAccount.CarrierAccountId))
                return false;

            return true;
        }

        public static string GetCarrierAccountName(string profileName, string nameSuffix)
        {
            return string.Format("{0}{1}", profileName, string.IsNullOrEmpty(nameSuffix) ? string.Empty : " (" + nameSuffix + ")");
        }

        private CarrierAccountToEdit ConvertCarrierAccountToEdit(CarrierAccount carrierAccount)
        {
            return new CarrierAccountToEdit
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                SourceId = carrierAccount.SourceId,
                SupplierSettings = carrierAccount.SupplierSettings,
                CarrierAccountSettings = carrierAccount.CarrierAccountSettings,
                CustomerSettings = carrierAccount.CustomerSettings,
                NameSuffix = carrierAccount.NameSuffix,
                CreatedTime = carrierAccount.CreatedTime,
                SellingProductId = carrierAccount.SellingProductId
            };
        }

        private CarrierAccountInvoiceType GetCarrierAccountInvoiceType(WHSCarrierFinancialAccountData financialAccountData)

        {
            if (financialAccountData.FinancialAccount.CarrierAccountId.HasValue)
                return CarrierAccountInvoiceType.Account;
            else
                return CarrierAccountInvoiceType.Profile;
        }

        private List<SMSServiceType> GetSMSServiceTypesEntities(List<CarrierAccountSMSServiceType> carrierAccountSMSServiceTypes)
        {
            List<SMSServiceType> smsServiceTypes = new List<SMSServiceType>();
            SMSServiceTypeManager smsServiceTypeManager = new SMSServiceTypeManager();

            if (carrierAccountSMSServiceTypes != null && carrierAccountSMSServiceTypes.Count > 0)
            {
                foreach (var crAccountSMSServiceType in carrierAccountSMSServiceTypes)
                {
                    SMSServiceType smsServiceType = smsServiceTypeManager.GetSMSServiceTypeById(crAccountSMSServiceType.SMSServiceTypeId);
                    if (smsServiceType != null)
                        smsServiceTypes.Add(smsServiceType);
                }
            }
            return smsServiceTypes;
        }
        #endregion

        #region Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _carrierProfileLastCheck;

            ICarrierAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierAccountsUpdated(ref _updateHandle)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<CarrierProfileManager.CacheManager>().IsCacheExpired(ref _carrierProfileLastCheck);
            }
        }

        public class CarrierAccountCachingExpirationChecker : RuleCachingExpirationChecker
        {
            DateTime? _cacheLastCheck;

            public override bool IsRuleDependenciesCacheExpired()
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref _cacheLastCheck);
            }
        }

        private class CarrierAccountDetailExportExcelHandler : ExcelExportHandler<CarrierAccountDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CarrierAccountDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Carrier Accounts",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Account Name", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Profile Name", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Account Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Activation Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Selling Number Plan", Width = 40 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Services" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.CarrierAccountId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CarrierAccountName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.CarrierProfileName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.AccountTypeDescription });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ActivationStatusDescription });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SellingNumberPlanName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.ServicesNames });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        public class CarrierAccountLoggableEntity : VRLoggableEntityBase
        {
            public static CarrierAccountLoggableEntity Instance = new CarrierAccountLoggableEntity();

            private CarrierAccountLoggableEntity()
            {

            }

            static CarrierAccountManager s_carrierAccountManager = new CarrierAccountManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_CarrierAccount"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Carrier Account"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_CarrierAccount_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                CarrierAccount carrierAccount = context.Object.CastWithValidate<CarrierAccount>("context.Object");
                return carrierAccount.CarrierAccountId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                CarrierAccount carrierAccount = context.Object.CastWithValidate<CarrierAccount>("context.Object");
                return s_carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
            }
        }

        #endregion

        #region  Mappers

        private CarrierAccountInfo CarrierAccountInfoMapper(CarrierAccount carrierAccount)
        {
            return new CarrierAccountInfo()
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Name = GetCarrierAccountName(_carrierProfileManager.GetCarrierProfileName(carrierAccount.CarrierProfileId), carrierAccount.NameSuffix),
                AccountType = carrierAccount.AccountType,
                ActivationStatus = carrierAccount.CarrierAccountSettings.ActivationStatus,
                SellingNumberPlanId = carrierAccount.SellingNumberPlanId,
                CurrencyId = GetCarrierAccountCurrencyId(carrierAccount)
            };
        }

        private RoutingCustomerInfo RoutingCustomerInfoMapper(CarrierAccount carrierAccount)
        {
            RoutingCustomerInfo routingCustomerInfo = new RoutingCustomerInfo();

            routingCustomerInfo.CustomerId = carrierAccount.CarrierAccountId;
            routingCustomerInfo.SellingNumberPlanId = carrierAccount.SellingNumberPlanId.Value;
            return routingCustomerInfo;
        }

        private RoutingSupplierInfo RoutingSupplierInfolMapper(CarrierAccount carrierAccount)
        {
            RoutingSupplierInfo routingSupplierInfo = new RoutingSupplierInfo();
            routingSupplierInfo.SupplierId = carrierAccount.CarrierAccountId;
            return routingSupplierInfo;
        }

        //private AccountManagerCarrier AccountManagerCarrierMapper(CarrierAccount carrierAccount)
        //{
        //    AccountManagerManager accountManagerManager = new AccountManagerManager();
        //    IEnumerable<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers();
        //    var assignedCarrierAccount = assignedCarriers.FindRecord(x => x.CarrierAccountId == carrierAccount.CarrierAccountId);
        //    return new AccountManagerCarrier()
        //    {
        //        CarrierAccountId = carrierAccount.CarrierAccountId,
        //        Name = GetCarrierAccountName(carrierAccount.CarrierAccountId),
        //        CarrierType = carrierAccount.AccountType,
        //        IsCustomerAvailable = (CarrierAccountManager.IsCustomer(carrierAccount.AccountType)) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Customer),
        //        IsSupplierAvailable = (CarrierAccountManager.IsSupplier(carrierAccount.AccountType)) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Supplier)
        //    };
        //}

        private CarrierAccountDetail CarrierAccountDetailMapper(CarrierAccount carrierAccount)
        {
            CarrierAccountDetail carrierAccountDetail = new CarrierAccountDetail();
            ZoneServiceConfigManager ZoneServiceConfigManager = new ZoneServiceConfigManager();
            carrierAccountDetail.Entity = carrierAccount;

            var carrierProfile = _carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);

            if (carrierProfile != null)
            {
                carrierAccountDetail.CarrierProfileName = carrierProfile.Name;
                carrierAccountDetail.CarrierAccountName = GetCarrierAccountName(carrierProfile.Name, carrierAccountDetail.Entity.NameSuffix);
            }

            carrierAccountDetail.AccountTypeDescription = carrierAccount.AccountType.ToString();

            carrierAccountDetail.LOBName = _lobManager.GetLOBName(carrierAccount.LOBId);

            if (carrierAccount.SellingNumberPlanId.HasValue)
                carrierAccountDetail.SellingNumberPlanName = _sellingNumberPlanManager.GetSellingNumberPlanName(carrierAccount.SellingNumberPlanId.Value);

            if (carrierAccount.SellingProductId.HasValue)
                carrierAccountDetail.SellingProductName = _sellingProductManager.GetSellingProductName(carrierAccount.SellingProductId.Value);

            if (carrierAccount.CarrierAccountSettings != null)
                carrierAccountDetail.ActivationStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(carrierAccount.CarrierAccountSettings.ActivationStatus);
            if ((carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange) && carrierAccount.SupplierSettings != null && carrierAccount.SupplierSettings.DefaultServices.Count > 0)
            {
                carrierAccountDetail.Services = carrierAccount.SupplierSettings.DefaultServices.Select(x => x.ServiceId).ToList();
                carrierAccountDetail.ServicesNames = ZoneServiceConfigManager.GetZoneServicesNames(carrierAccountDetail.Services);
                carrierAccountDetail.ServicesWeight = ZoneServiceConfigManager.GetZoneServicesWeight(carrierAccountDetail.Services);
            }
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            WHSCarrierFinancialAccountData financialAccountData;
            if (carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange)
            {
                //financialAccountManager.GetCustInvoiceData(carrierAccount.CarrierAccountId, DateTime.Now, out invoiceSettingName, out invoiceTypeName);
                financialAccountManager.TryGetCustAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out financialAccountData);

            }
            else
            {
                financialAccountManager.TryGetSuppAccFinancialAccountData(carrierAccount.CarrierAccountId, DateTime.Now, out financialAccountData);
                //financialAccountManager.GetSuppInvoiceData(carrierAccount.CarrierAccountId, DateTime.Now, out invoiceSettingName, out invoiceTypeName);
            }

            if (financialAccountData != null)
            {

                carrierAccountDetail.InvoiceTypeDescription = GetCarrierAccountInvoiceType(financialAccountData).ToString();
                carrierAccountDetail.InvoiceSettingName = financialAccountManager.GetFinancialInvoiceSettingName(financialAccountData.FinancialAccount.FinancialAccountDefinitionId, financialAccountData.FinancialAccount.FinancialAccountId.ToString(), financialAccountData.InvoiceData.InvoiceTypeId);
            }
            var companySettings = GetCompanySetting(carrierAccount.CarrierAccountId);
            if (companySettings != null)
                carrierAccountDetail.CompanySettingName = companySettings.CompanyName;
            return carrierAccountDetail;
        }

        #endregion

        #region IBusinessEntityManager

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCarrierAccount(Convert.ToInt32(context.EntityId));
        }

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allCarrierAccounts = GetAllCarriers();
            if (allCarrierAccounts == null)
                return null;
            else
                return allCarrierAccounts.Select(itm => itm as dynamic).ToList();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCarrierAccountName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var carrierAccount = context.Entity as CarrierAccount;
            return carrierAccount.CarrierAccountId;
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            Func<CarrierAccount, bool> filter;
            switch (context.ParentEntityDefinition.Name)
            {
                case CarrierProfile.BUSINESSENTITY_DEFINITION_NAME: filter = (ca) => ca.CarrierProfileId == context.ParentEntityId; break;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
            return GetCachedCarrierAccounts().FindAllRecords(filter).MapRecords(ca => ca.CarrierAccountId as dynamic);
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            var carrierAccount = context.Entity as CarrierAccount;
            if (carrierAccount == null)
                throw new NullReferenceException("carrierAccount");
            switch (context.ParentEntityDefinition.Name)
            {
                case CarrierProfile.BUSINESSENTITY_DEFINITION_NAME: return carrierAccount.CarrierProfileId;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsStillAvailable(IBusinessEntityIsStillAvailableContext context)
        {
            context.ThrowIfNull("context");
            return !IsCarrierAccountDeleted(Convert.ToInt32(context.EntityId));
        }

        #endregion
    }

    public class CarrierAccountFilterPersonalizationItem : EntityPersonalizationExtendedSetting
    {
        public string Name { get; set; }

        public List<int> CarrierProfilesIds { get; set; }

        public List<CarrierAccountType> AccountsTypes { get; set; }

        public List<ActivationStatus> ActivationStatuses { get; set; }

        public List<int> SellingNumberPlanIds { get; set; }

        public List<int> SellingProductsIds { get; set; }

        public List<int> Services { get; set; }
    }
}
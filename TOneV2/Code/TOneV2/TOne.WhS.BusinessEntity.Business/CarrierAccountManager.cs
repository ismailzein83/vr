using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Caching;
using Vanrise.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager : BaseBusinessEntityManager, ICarrierAccountManager
    {
        #region ctor/Local Variables

        CarrierProfileManager _carrierProfileManager;
        SellingNumberPlanManager _sellingNumberPlanManager;
        SellingProductManager _sellingProductManager;

        public CarrierAccountManager()
        {
            _carrierProfileManager = new CarrierProfileManager();
            _sellingNumberPlanManager = new SellingNumberPlanManager();
            _sellingProductManager = new SellingProductManager();
        }

        #endregion

        #region Public Methods

        #region CarrierAccount

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

        public Dictionary<string, CarrierAccount> GetCachedSupplierAccountsByAutoImportEmail()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierAccountsByAutoImportEmail",
               () =>
               {
                   Dictionary<int, CarrierAccount> allCarrierAccounts = this.GetCachedCarrierAccounts();
                   Dictionary<string, CarrierAccount> supplierAccountsByAutoImportEmail = new Dictionary<string, CarrierAccount>();
                   foreach (CarrierAccount item in allCarrierAccounts.Values)
                   {
                       if (item.CarrierAccountSettings == null || item.CarrierAccountSettings.PriceListSettings == null)
                           continue;

                       string email = item.CarrierAccountSettings.PriceListSettings.Email;
                       if (!string.IsNullOrEmpty(email))
                       {
                           CarrierAccount duplicatedAccount = supplierAccountsByAutoImportEmail.GetRecord(email);
                           if (duplicatedAccount != null)
                               throw new VRBusinessException(string.Format("Email {0} is duplicated for accounts {1} and {2}", email, GetCarrierAccountName(item.CarrierAccountId), GetCarrierAccountName(duplicatedAccount.CarrierAccountId)));
                           supplierAccountsByAutoImportEmail.Add(email, item);
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

        public PricelistSettings GetCompanyPricelistSettingsByCustomerId(int carrierAccountId)
        {
            var companySetting = GetCompanySetting(carrierAccountId);
            return GetCompanyPricelistSettings(companySetting);

        }
        public PricelistSettings GetCompanyPricelistSettings(CompanySetting companySetting)
        {
            var configManager = new ConfigManager();

            Vanrise.Common.Business.ConfigManager vanriseCommonBusinessConfigManager = new Vanrise.Common.Business.ConfigManager();
            CompanyPricelistSettings companyPricelistSettingsObj = vanriseCommonBusinessConfigManager.GetCompanyExtendedSettings<CompanyPricelistSettings>(companySetting);
            PricelistSettings companyPricelistSettings = (companyPricelistSettingsObj != null) ? companyPricelistSettingsObj.PricelistSettings : default(PricelistSettings);

            return configManager.MergePricelistSettings(configManager.GetSaleAreaPricelistSettings(), companyPricelistSettings);
        }

        public int GetCustomerTimeZoneId(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException("carrierAccount");
            if (carrierAccount.CustomerSettings == null)
                throw new NullReferenceException("carrierProfile.CustomerSettings");
            if (carrierAccount.CustomerSettings.InvoiceTimeZone && carrierAccount.CustomerSettings.TimeZoneId.HasValue)
                return carrierAccount.CustomerSettings.TimeZoneId.Value;
            return new CarrierProfileManager().GetCustomerTimeZoneId(carrierAccount.CarrierProfileId);
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
                (input.Query.Name == null || IsMatchByName(input.Query.Name, item))
                &&
                (input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(item.CarrierProfileId))
                &&
                (input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(item.CarrierAccountId))
                &&
                (input.Query.ActivationStatusIds == null || input.Query.ActivationStatusIds.Contains((int)item.CarrierAccountSettings.ActivationStatus))
                &&
                (input.Query.AccountsTypes == null || input.Query.AccountsTypes.Contains(item.AccountType))
                &&
                (input.Query.SellingNumberPlanIds == null || (item.AccountType == CarrierAccountType.Supplier || input.Query.SellingNumberPlanIds.Contains(item.SellingNumberPlanId)))
                 &&
                (input.Query.SellingProductsIds == null || (item.AccountType == CarrierAccountType.Supplier || input.Query.SellingProductsIds.Contains(item.SellingProductId)))
                  &&
                (input.Query.Services == null || (item.AccountType == CarrierAccountType.Customer || input.Query.Services.All(x => item.SupplierSettings.DefaultServices.Select(y => y.ServiceId).Contains(x))));

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

                IEnumerable<AssignedCarrier> assignedCarriers = null;
                if (filter.AssignableToUserId.HasValue)
                {
                    AccountManagerManager AccountManagerManager = new AccountManagerManager();
                    assignedCarriers = AccountManagerManager.GetAssignedCarriers();
                }

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

                    if (carr.CarrierAccountSettings != null && carr.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
                        return false;

                    if (!ShouldSelectCarrierAccount(carr, filter.GetCustomers, filter.GetSuppliers, filteredSupplierIds, filteredCustomerIds))
                        return false;

                    if (filter.AssignableToSellingProductId.HasValue && !IsAssignableToSellingProduct(carr, filter.AssignableToSellingProductId.Value, sellingProduct, customerSellingProductsEffectiveInFuture))
                        return false;

                    if (filter.SellingNumberPlanId.HasValue && carr.SellingNumberPlanId != filter.SellingNumberPlanId.Value)
                        return false;

                    if (filter.SellingProductId.HasValue && carr.SellingProductId != filter.SellingProductId.Value )
                        return false;

                    if (filter.AssignableToUserId.HasValue && !IsCarrierAccountAssignableToUser(carr, filter.GetCustomers, filter.GetSuppliers, assignedCarriers))
                        return false;
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
            if (filter != null && filter.AssignableToUserId != null)
                return GetCachedCarrierAccounts().MapRecords(AccountManagerCarrierMapper, filterPredicate).OrderBy(x => x.Name);
            else
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
            ValidateCarrierAccountToAdd(carrierAccount);

            InsertOperationOutput<CarrierAccountDetail> insertOperationOutput = new InsertOperationOutput<CarrierAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierAccountId = -1;

            if (CarrierAccountManager.IsCustomer(carrierAccount.AccountType) && carrierAccount.SellingNumberPlanId == null)
                throw new ArgumentNullException("Missing SellingNumberPlanId");

            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            carrierAccount.CreatedBy = loggedInUserId;
            carrierAccount.LastModifiedBy = loggedInUserId;

            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierAccount, out carrierAccountId);
            bool isDefaultServiceInsertedSuccessfully = true;

            if (CarrierAccountManager.IsSupplier(carrierAccount.AccountType))
            {
                SupplierZoneServiceManager zoneServiceManager = new SupplierZoneServiceManager();
                isDefaultServiceInsertedSuccessfully = zoneServiceManager.Insert(carrierAccountId, carrierAccount.SupplierSettings.DefaultServices);
            }

            if (insertActionSucc)
                new CarrierAccountStatusHistoryManager().AddAccountStatusHistory(carrierAccountId, carrierAccount.CarrierAccountSettings.ActivationStatus, null);

            if (insertActionSucc && isDefaultServiceInsertedSuccessfully)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                carrierAccount.CarrierAccountId = carrierAccountId;
                VRActionLogger.Current.TrackAndLogObjectAdded(CarrierAccountLoggableEntity.Instance, carrierAccount);

                VREventManager vrEventManager = new VREventManager();
                vrEventManager.ExecuteEventHandlersAsync(new CarrierAccountStatusChangedEventPayload { CarrierAccountId = carrierAccountId });
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = carrierAccountDetail;

            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;


            return insertOperationOutput;
        }
        public UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccountToEdit carrierAccountToEdit)
        {
            UpdateOperationOutput<CarrierAccountDetail> updateOperationOutput = new UpdateOperationOutput<CarrierAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            if (TryUpdateCarrierAccount(carrierAccountToEdit, true))
            {
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(this.GetCarrierAccount(carrierAccountToEdit.CarrierAccountId));

                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierAccountDetail;

            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }
        public bool TryUpdateCarrierAccount(CarrierAccountToEdit carrierAccountToEdit, bool withTracking)
        {
            int carrierProfileId;

            ValidateCarrierAccountToEdit(carrierAccountToEdit, out carrierProfileId);
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            CarrierAccount cachedAccount = this.GetCarrierAccount(carrierAccountToEdit.CarrierAccountId);

            ActivationStatus previousActivationStatus = cachedAccount.CarrierAccountSettings.ActivationStatus;
            ActivationStatus activationStatus = carrierAccountToEdit.CarrierAccountSettings.ActivationStatus;

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
                if (previousActivationStatus != activationStatus)
                {
                    new CarrierAccountStatusHistoryManager().AddAccountStatusHistory(carrierAccountToEdit.CarrierAccountId, activationStatus, previousActivationStatus);

                    if (withTracking)
                    {
                        string actionDescription = string.Format("Status Changed from '{0}' to '{1}'", previousActivationStatus, activationStatus);
                        VRActionLogger.Current.LogObjectCustomAction(CarrierAccountLoggableEntity.Instance, "Status Changed", true, updatedCA, actionDescription);
                    }
                    VREventManager vrEventManager = new VREventManager();
                    vrEventManager.ExecuteEventHandlersAsync(new CarrierAccountStatusChangedEventPayload { CarrierAccountId = carrierAccountToEdit.CarrierAccountId });
                }

                return true;
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
        public bool UpdateCustomerRoutingStatus(int carrierAccountId, RoutingStatus routingStatus, bool withTracking)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            carrierAccount.CustomerSettings.RoutingStatus = routingStatus;
            return TryUpdateCarrierAccount(ConvertCarrierAccountToEdit(carrierAccount), withTracking);
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

        public IEnumerable<int> GetCustomerIdsBySellingProductId(int sellingProductId)
        {
            return GetAllCustomers().MapRecords(x => x.CarrierAccountId, x => x.SellingProductId.Value == sellingProductId);
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
        public int GetAccountNominalCapacity(int carrierAccountId)
        {
            var carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount == null)
                throw new NullReferenceException(String.Format("carrierAccount '{0}'", carrierAccountId));
            return carrierAccount.CarrierAccountSettings.NominalCapacity;
        }
        public int GetAccountsTotalNominalCapacity(IEnumerable<int> carrierAccountIds)
        {
            int totalNominalCapacity = 0;
            foreach (var accountId in carrierAccountIds.Distinct())
            {
                totalNominalCapacity += GetAccountNominalCapacity(accountId);
            }
            return totalNominalCapacity;
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
        public IEnumerable<Guid> GetBankDetails(int carrierAccountId)
        {
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

        private void ValidateCarrierAccountToAdd(CarrierAccount carrierAccount)
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


            if (carrierAccount.CarrierAccountSettings != null && carrierAccount.CarrierAccountSettings.PriceListSettings != null)
            {
                string email = carrierAccount.CarrierAccountSettings.PriceListSettings.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    var supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
                    CarrierAccount duplicatedCarrierAccount = supplierAccounts.GetRecord(email);
                    if (duplicatedCarrierAccount != null)
                        throw new VRBusinessException(string.Format("Cannot set auto import email {0} because it is already set for carrier account {1}.", email, GetCarrierAccountName(duplicatedCarrierAccount.CarrierAccountId)));
                }
            }

            ValidateCarrierAccount(carrierAccount.NameSuffix, carrierAccount.CarrierAccountSettings);
        }

        private void ValidateCarrierAccountToEdit(CarrierAccountToEdit carrierAccount, out int carrierProfileId)
        {
            var carrierAccountEntity = this.GetCarrierAccount(carrierAccount.CarrierAccountId);
            if (carrierAccountEntity == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not exist", carrierAccount.CarrierAccountId));

            var carrierProfile = _carrierProfileManager.GetCarrierProfile(carrierAccountEntity.CarrierProfileId);
            if (carrierProfile == null)
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' does not belong to a CarrierProfile", carrierAccount.CarrierAccountId));

            carrierProfileId = carrierProfile.CarrierProfileId;

            if (carrierAccount.CarrierAccountSettings != null && carrierAccount.CarrierAccountSettings.PriceListSettings != null)
            {
                string email = carrierAccount.CarrierAccountSettings.PriceListSettings.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    var supplierAccounts = GetCachedSupplierAccountsByAutoImportEmail();
                    CarrierAccount duplicatedCarrierAccount = supplierAccounts.GetRecord(email);
                    if (duplicatedCarrierAccount != null && duplicatedCarrierAccount.CarrierAccountId != carrierAccountEntity.CarrierAccountId)
                        throw new VRBusinessException(string.Format("Cannot set auto import email {0} because it is already set for carrier account {1}.", email, GetCarrierAccountName(duplicatedCarrierAccount.CarrierAccountId)));
                }
            }

            ValidateCarrierAccount(carrierAccount.NameSuffix, carrierAccount.CarrierAccountSettings);
        }

        private void ValidateCarrierAccount(string caNameSuffix, CarrierAccountSettings caSettings)
        {
            //if (String.IsNullOrWhiteSpace(caNameSuffix))
            //    throw new MissingArgumentValidationException("CarrierAccount.NameSuffix"); // bug: 2164

            if (caSettings == null)
                throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings");

            //if (String.IsNullOrWhiteSpace(caSettings.Mask))
            //    throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings.Mask");

            var currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrency(caSettings.CurrencyId);
            if (currency == null)
                throw new DataIntegrityValidationException(String.Format("Currency '{0}' does not exist", caSettings.CurrencyId));
        }

        #endregion

        #region Private Methods

        private IEnumerable<CarrierAccount> GetCarrierAccountsByIds(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            var carrierAccounts = this.GetCarrierAccountsByType(getCustomers, getSuppliers, null, null);
            Func<CarrierAccount, bool> filterExpression = null;

            if (carrierAccountsIds != null)
                filterExpression = (item) => (carrierAccountsIds.Contains(item.CarrierAccountId));

            return carrierAccounts.FindAllRecords(filterExpression);
        }

        private bool IsCarrierAccountAssignableToUser(CarrierAccount carrierAccount, bool getCustomers, bool getSuppliers, IEnumerable<AssignedCarrier> assignedCarriers)
        {
            if (carrierAccount.AccountType == CarrierAccountType.Exchange && assignedCarriers.Where(x => x.CarrierAccountId == carrierAccount.CarrierAccountId).Count() > 1)
                return false;

            if (carrierAccount.AccountType != CarrierAccountType.Exchange && assignedCarriers.Any(y => y.CarrierAccountId == carrierAccount.CarrierAccountId))
                return false;

            return true;
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

        private static string GetCarrierAccountName(string profileName, string nameSuffix)
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

        private AccountManagerCarrier AccountManagerCarrierMapper(CarrierAccount carrierAccount)
        {
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            IEnumerable<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers();
            var assignedCarrierAccount = assignedCarriers.FindRecord(x => x.CarrierAccountId == carrierAccount.CarrierAccountId);
            return new AccountManagerCarrier()
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Name = GetCarrierAccountName(carrierAccount.CarrierAccountId),
                CarrierType = carrierAccount.AccountType,
                IsCustomerAvailable = (CarrierAccountManager.IsCustomer(carrierAccount.AccountType)) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Customer),
                IsSupplierAvailable = (CarrierAccountManager.IsSupplier(carrierAccount.AccountType)) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Supplier)
            };
        }

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
                if (financialAccountData.FinancialAccount.CarrierAccountId.HasValue)
                {
                    carrierAccountDetail.InvoiceTypeDescription = "Account";
                }
                else
                {
                    carrierAccountDetail.InvoiceTypeDescription = "Profile";
                }
                carrierAccountDetail.InvoiceSettingName = financialAccountManager.GetFinancialInvoiceSettingName(financialAccountData.FinancialAccount.FinancialAccountDefinitionId, financialAccountData.FinancialAccount.FinancialAccountId.ToString(), financialAccountData.InvoiceData.InvoiceTypeId);
            }
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

        #endregion
    }

    public class CarrierAccountFilterPersonalizationItem : EntityPersonalizationItemSetting
    {
        public override string Title
        {
            get { return "Filter"; }
        }

        public string Name { get; set; }

        public List<int> CarrierProfilesIds { get; set; }
        
        public List<CarrierAccountType> AccountsTypes { get; set; }
        
        public List<ActivationStatus> ActivationStatuses { get; set; }

        public List<int> SellingNumberPlanIds { get; set; }

        public List<int> SellingProductsIds { get; set; }

        public List<int> Services { get; set; }
    }
}
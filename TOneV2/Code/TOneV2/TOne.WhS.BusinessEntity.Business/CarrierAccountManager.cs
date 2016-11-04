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

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager : IBusinessEntityManager, ICarrierAccountManager
    {
        #region ctor/Local Variables
        CarrierProfileManager _carrierProfileManager;
        SellingNumberPlanManager _sellingNumberPlanManager;
        public CarrierAccountManager()
        {
            _carrierProfileManager = new CarrierProfileManager();
            _sellingNumberPlanManager = new SellingNumberPlanManager();
        }

        #endregion

        #region Public Methods

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

        public IEnumerable<CarrierAccount> GetCarriersByProfileId(int carrierProfileId, bool getCustomers, bool getSuppliers)
        {
            if (getCustomers)
                return GetAllCustomers().Where(x => x.CarrierProfileId == carrierProfileId);
            if (getSuppliers)
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
                (input.Query.Services == null || (item.AccountType == CarrierAccountType.Customer || input.Query.Services.All(x => item.SupplierSettings.DefaultServices.Select(y => y.ServiceId).Contains(x))));

            var resultProcessingHandler = new ResultProcessingHandler<CarrierAccountDetail>()
            {
                ExportExcelHandler = new CarrierAccountDetailExportExcelHandler()
            };

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

                filterPredicate = (carr) =>
                    {
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

                        if (filter.AssignableToUserId.HasValue && !IsCarrierAccountAssignableToUser(carr, filter.GetCustomers, filter.GetSuppliers, assignedCarriers))
                            return false;

                        return true;
                    };
            }

            //TODO: fix this when we reach working with Account Manager module
            if (filter != null && filter.AssignableToUserId != null)
                return GetCachedCarrierAccounts().MapRecords(AccountManagerCarrierMapper, filterPredicate).OrderBy(x => x.Name);
            else
                return GetCachedCarrierAccounts().MapRecords(CarrierAccountInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }

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

        public TOne.Entities.InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            ValidateCarrierAccountToAdd(carrierAccount);

            TOne.Entities.InsertOperationOutput<CarrierAccountDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierAccountId = -1;

            if (CarrierAccountManager.IsCustomer(carrierAccount.AccountType) && carrierAccount.SellingNumberPlanId == null)
                throw new ArgumentNullException("Missing SellingNumberPlanId");

            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierAccount, out carrierAccountId);
            bool isDefaultServiceInsertedSuccessfully = true;

            if (carrierAccount.AccountType != CarrierAccountType.Customer)
            {
                SupplierZoneServiceManager zoneServiceManager = new SupplierZoneServiceManager();
                SupplierDefaultService supplierZoneService = new SupplierDefaultService()
                {
                    EffectiveServices = carrierAccount.SupplierSettings.DefaultServices,
                    ReceivedServices = carrierAccount.SupplierSettings.DefaultServices,
                    BED = DateTime.Today,
                    SupplierId = carrierAccountId
                };

               isDefaultServiceInsertedSuccessfully = zoneServiceManager.Insert(supplierZoneService);
            }


            if (insertActionSucc && isDefaultServiceInsertedSuccessfully)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                carrierAccount.CarrierAccountId = carrierAccountId;
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = carrierAccountDetail;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;


            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccountToEdit carrierAccount)
        {
            int carrierProfileId;
            ValidateCarrierAccountToEdit(carrierAccount, out carrierProfileId);

            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();

            bool updateActionSucc = dataManager.Update(carrierAccount, carrierProfileId);
            TOne.Entities.UpdateOperationOutput<CarrierAccountDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            SupplierZoneServiceManager zoneServiceManager = new SupplierZoneServiceManager();
            SupplierDefaultService supplierZoneService = zoneServiceManager.GetSupplierDefaultServiceBySupplier(carrierAccount.CarrierAccountId, DateTime.Today);

            if (supplierZoneService == null)
                throw new DataIntegrityValidationException(string.Format("Supplier {0} does not have default services", carrierAccount.NameSuffix));

           
            if(!zoneServiceManager.HasSameServices(supplierZoneService.ReceivedServices, carrierAccount.SupplierSettings.DefaultServices))
            { 
                SupplierDefaultService supplierZoneServiceNew = new SupplierDefaultService()
                {
                    EffectiveServices = carrierAccount.SupplierSettings.DefaultServices,
                    ReceivedServices = carrierAccount.SupplierSettings.DefaultServices,
                    BED = DateTime.Today,
                    SupplierId = carrierAccount.CarrierAccountId
                };
               zoneServiceManager.CloseOverlappedDefaultService(supplierZoneService.SupplierZoneServiceId, supplierZoneServiceNew, DateTime.Today);
            }

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(this.GetCarrierAccount(carrierAccount.CarrierAccountId));
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierAccountDetail;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

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

        public int? GetCustomerSellingNumberPlanId(int customerId)
        {
            var customer = GetCarrierAccount(customerId);
            if (customer == null || customer.SellingNumberPlanId == null)
                return null;
            else
                return customer.SellingNumberPlanId;
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetCarrierAccountName(Convert.ToInt32(context.EntityId));
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
            if (carrierAccount == null)
                throw new NullReferenceException("carrierAccount");
            if (carrierAccount.CarrierAccountSettings == null)
                throw new NullReferenceException("carrierAccount.CarrierAccountSettings");
            return carrierAccount.CarrierAccountSettings.CurrencyId;
        }

        public bool IsCarrierAccountDeleted(int carrierAccountId)
        {
            var carrierAccounts = GetCachedCarrierAccountsWithDeleted();
            CarrierAccount carrierAccount = carrierAccounts.GetRecord(carrierAccountId);

            if (carrierAccount == null)
                throw new DataIntegrityValidationException(string.Format("carrierAccount with Id {0} is not found", carrierAccountId));

            return carrierAccount.IsDeleted;
        }

        public int? GetCarrierProfileId(int carrierAccountId)
        {
            CarrierAccount carrierAccount = GetCarrierAccount(carrierAccountId);
            if (carrierAccount != null)
                return carrierAccount.CarrierProfileId;
            return null;
        }

        #endregion

        #region ICarrierAccountManager Memebers

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
            ValidateCarrierAccount(carrierAccount.NameSuffix, carrierAccount.CarrierAccountSettings);
        }

        private void ValidateCarrierAccount(string caNameSuffix, CarrierAccountSettings caSettings)
        {
            //if (String.IsNullOrWhiteSpace(caNameSuffix))
            //    throw new MissingArgumentValidationException("CarrierAccount.NameSuffix"); // bug: 2164

            if (caSettings == null)
                throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings");

            if (String.IsNullOrWhiteSpace(caSettings.Mask))
                throw new MissingArgumentValidationException("CarrierAccount.CarrierAccountSettings.Mask");

            var currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrency(caSettings.CurrencyId);
            if (currency == null)
                throw new DataIntegrityValidationException(String.Format("Currency '{0}' does not exist", caSettings.CurrencyId));
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region  Mappers

        private CarrierAccountInfo CarrierAccountInfoMapper(CarrierAccount carrierAccount)
        {
            return new CarrierAccountInfo()
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Name = GetCarrierAccountName(_carrierProfileManager.GetCarrierProfileName(carrierAccount.CarrierProfileId), carrierAccount.NameSuffix),
                AccountType = carrierAccount.AccountType,
                SellingNumberPlanId = carrierAccount.SellingNumberPlanId
            };
        }

        private RoutingCustomerInfo RoutingCustomerInfoMapper(CarrierAccount carrierAccount)
        {
            RoutingCustomerInfo routingCustomerInfo = new RoutingCustomerInfo();

            routingCustomerInfo.CustomerId = carrierAccount.CarrierAccountId;

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

            if (carrierAccount.SellingNumberPlanId != null)
            {
                var sellingNumberPlan = _sellingNumberPlanManager.GetSellingNumberPlan((int)carrierAccount.SellingNumberPlanId);
                if (sellingNumberPlan != null)
                    carrierAccountDetail.SellingNumberPlanName = sellingNumberPlan.Name;
            }

            if (carrierAccount.CarrierAccountSettings != null)
                carrierAccountDetail.ActivationStatusDescription = Vanrise.Common.Utilities.GetEnumDescription(carrierAccount.CarrierAccountSettings.ActivationStatus);
            if ((carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange) && carrierAccount.SupplierSettings != null && carrierAccount.SupplierSettings.DefaultServices.Count > 0)
            {
                carrierAccountDetail.Services = carrierAccount.SupplierSettings.DefaultServices.Select(x => x.ServiceId).ToList();
                carrierAccountDetail.ServicesNames = ZoneServiceConfigManager.GetZoneServicesNames(carrierAccountDetail.Services);
            }


            return carrierAccountDetail;
        }


        private class CarrierAccountDetailExportExcelHandler : ExcelExportHandler<CarrierAccountDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CarrierAccountDetail> context)
            {
                if (context.BigResult == null || context.BigResult.Data == null)
                    return;

                var sheet = new ExportExcelSheet();
                sheet.SheetName = "Carrier Accounts";

                sheet.Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Account Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Profile Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Account Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Activation Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Selling Number Plan" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Services" });

                sheet.Rows = new List<ExportExcelRow>();
                foreach (var record in context.BigResult.Data)
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

                context.MainSheet = sheet;
            }
        }

        #endregion

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCarrierAccount(context.EntityId);
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var allCarrierAccounts = GetAllCarriers();
            if (allCarrierAccounts == null)
                return null;
            else
                return allCarrierAccounts.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            Func<CarrierAccount, bool> filter;
            switch (context.ParentEntityDefinition.Name)
            {
                case CarrierProfile.BUSINESSENTITY_DEFINITION_NAME: filter = (ca) => ca.CarrierProfileId == context.ParentEntityId; break;
                default: throw new NotImplementedException(String.Format("Business Entity Definition Name '{0}'", context.ParentEntityDefinition.Name));
            }
            return GetCachedCarrierAccounts().FindAllRecords(filter).MapRecords(ca => ca.CarrierAccountId as dynamic);
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
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


        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}

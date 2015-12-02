using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            var allCarrierAccounts = GetCachedCarrierAccounts();

            Func<CarrierAccount, bool> filterExpression = (item) =>
                 (input.Query.Name == null || item.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(item.CarrierProfileId))
                  &&
                 (input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(item.CarrierAccountId))
                   &&
                 (input.Query.AccountsTypes == null || input.Query.AccountsTypes.Contains(item.AccountType));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierAccounts.ToBigResult(input, filterExpression, CarrierAccountDetailMapper));
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
            var carrierAccounts = this.GetCarrierAccountsByIds(carrierAccountsIds, true, false);
            if (carrierAccounts != null)
                return string.Join(", ", carrierAccounts.Select(x => x.Name));

            return string.Empty;
        }

        public IEnumerable<CarrierAccountInfo> GetCarrierAccountInfo(CarrierAccountInfoFilter filter)
        {
            IEnumerable<CarrierAccount> carrierAccounts = null;

            if (filter != null)
            {
                if (filter.AssignableToSellingProductId != null)
                    carrierAccounts = this.GetNotAssignableCustomersToSellingProduct((int)filter.AssignableToSellingProductId);
                else if (filter.AssignableToUserId != null){
                    carrierAccounts = this.GetCarriersAssignableToUserId(filter.GetCustomers, filter.GetSuppliers).ToList();
                   return  carrierAccounts.MapRecords(AccountManagerCarrierMapper);
                }
                   
                else
                    carrierAccounts = this.GetCarrierAccountsByType(filter.GetCustomers, filter.GetSuppliers, filter.SupplierFilterSettings, filter.CustomerFilterSettings);
            }
            else
            {
                var cachedCarrierAccounts = GetCachedCarrierAccounts();
                if (cachedCarrierAccounts != null)
                    carrierAccounts = cachedCarrierAccounts.Values;
            }

            return carrierAccounts.MapRecords(CarrierAccountInfoMapper);
        }

        private IEnumerable<CarrierAccount> GetCarriersAssignableToUserId(bool getCustomers, bool getSuppliers)
        {
            AccountManagerManager AccountManagerManager = new AccountManagerManager();
            IEnumerable<CarrierAccount> carriers = GetCarrierAccountsByType(getCustomers, getSuppliers, null, null);
            IEnumerable<AssignedCarrier> assignedCarriers = AccountManagerManager.GetAssignedCarriers();

            Func<CarrierAccount, bool> filterExpression = (carrierAccount) =>
            {
                if( carrierAccount.AccountType == CarrierAccountType.Exchange  &&  assignedCarriers.Where(z=>z.CarrierAccountId==carrierAccount.CarrierAccountId).Count()>1)
                    return false;
                if (carrierAccount.AccountType != CarrierAccountType.Exchange && assignedCarriers.Any(y => y.CarrierAccountId == carrierAccount.CarrierAccountId))
                    return false;
                return true;
   
            };

            return carriers.FindAllRecords(filterExpression);
        }

        public List<Vanrise.Entities.TemplateConfig> GetCustomersGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerGroupConfigType);
        }

        public List<Vanrise.Entities.TemplateConfig> GetSupplierGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SupplierGroupConfigType);
        }
        public List<Vanrise.Entities.TemplateConfig> GetSuppliersWithZonesGroupsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SuppliersWithZonesGroupSettingsConfigType);
        }

        public List<Vanrise.Entities.TemplateConfig> GetCustomerGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerGroupConfigType);
        }
        public TOne.Entities.InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            TOne.Entities.InsertOperationOutput<CarrierAccountDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CarrierAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierAccountId = -1;

            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierAccount, out carrierAccountId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                carrierAccount.CarrierAccountId = carrierAccountId;
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = carrierAccountDetail;
            }

            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();

            bool updateActionSucc = dataManager.Update(carrierAccount);
            TOne.Entities.UpdateOperationOutput<CarrierAccountDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CarrierAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var allCarrierAccounts = GetCachedCarrierAccounts();

                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierAccountDetail;
            }

            return updateOperationOutput;
        }

        public int GetSellingNumberPlanId(int carrierAccountId, CarrierAccountType carrierAccountType)
        {
            if (carrierAccountType == CarrierAccountType.Supplier)
                return -1;

            return this.GetCarrierAccount(carrierAccountId).CustomerSettings.SellingNumberPlanId;
        }

        public IEnumerable<RoutingCustomerInfo> GetRoutingActiveCustomers()
        {
            IEnumerable<CarrierAccount> carrierAccounts = GetCarrierAccountsByType(true, false, null, null);

            Func<CarrierAccount, bool> filterExpression = (item) => (item.CarrierAccountSettings != null && item.CustomerSettings != null && item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active && item.CustomerSettings.RoutingStatus == RoutingStatus.Enabled);
            return carrierAccounts.MapRecords(RoutingCustomerInfoMapper, filterExpression);
        }
        public IEnumerable<RoutingSupplierInfo> GetRoutingActiveSuppliers()
        {
            IEnumerable<CarrierAccount> carrierAccounts = GetCarrierAccountsByType(false, true, null, null);
            Func<CarrierAccount, bool> filterExpression = (item) => (item.CarrierAccountSettings != null && item.SupplierSettings != null && item.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active && item.SupplierSettings.RoutingStatus == RoutingStatus.Enabled);
            return carrierAccounts.MapRecords(RoutingSupplierInfolMapper, filterExpression);
        
        }

        public IEnumerable<CarrierAccount> GetNotAssignableCustomersToSellingProduct(int sellingProductId)
        {
           SellingProductManager sellingProductManager = new SellingProductManager();
           SellingProduct sellingProduct = sellingProductManager.GetSellingProduct(sellingProductId);
           var cachedCarrierAccounts = GetCarrierAccountsByType(true, false, null, null);
           CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
           IEnumerable<CustomerSellingProduct> customerSellingProducts = customerSellingProductManager.GetEffectiveCustomerSellingProduct();
           return cachedCarrierAccounts.Where(x => x.CustomerSettings.SellingNumberPlanId == sellingProduct.SellingNumberPlanId
               && (!customerSellingProducts.Any(y => y.CustomerId == x.CarrierAccountId )));
         
        }


        #region Private Members

        Dictionary<int, CarrierAccount> GetCachedCarrierAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierAccounts",
               () =>
               {
                   ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                   IEnumerable<CarrierAccount> carrierAccounts = dataManager.GetCarrierAccounts();
                   return carrierAccounts.ToDictionary(kvp => kvp.CarrierAccountId, kvp => kvp);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICarrierAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierAccountsUpdated(ref _updateHandle);
            }
        }

        private CarrierAccountInfo CarrierAccountInfoMapper(CarrierAccount carrierAccount)
        {
            return new CarrierAccountInfo()
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Name = carrierAccount.Name,
            };
        }

        private IEnumerable<CarrierAccount> GetCarrierAccountsByIds(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            var carrierAccounts = this.GetCarrierAccountsByType(getCustomers, getSuppliers, null, null);
            Func<CarrierAccount, bool> filterExpression = null;

            if (carrierAccountsIds != null)
                filterExpression = (item) => (carrierAccountsIds.Contains(item.CarrierAccountId));

            return carrierAccounts.FindAllRecords(filterExpression);
        }

        private IEnumerable<CarrierAccount> GetCarrierAccountsByType(bool getCustomers, bool getSuppliers, SupplierFilterSettings supplierFilterSettings, CustomerFilterSettings customerFilterSettings)
        {
            Dictionary<int, CarrierAccount> carrierAccounts = GetCachedCarrierAccounts();

            HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
            HashSet<int> filteredCustomerIds = CustomerGroupContext.GetFilteredCustomerIds(customerFilterSettings);
            Func<CarrierAccount, bool> filterExpression = (carrierAccount) =>
                {
                    bool isSupplier = carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange;
                    bool isCustomer = carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange;
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
                };
            return carrierAccounts.FindAllRecords(filterExpression);
        }

        private CarrierAccountDetail CarrierAccountDetailMapper(CarrierAccount carrierAccount)
        {
            CarrierAccountDetail carrierAccountDetail = new CarrierAccountDetail();

            carrierAccountDetail.Entity = carrierAccount;

            CarrierProfileManager manager = new CarrierProfileManager();
            var carrierProfiles = manager.GetCachedCarrierProfiles();
            var carrierProfile = carrierProfiles.FindRecord(itm => itm.Value.CarrierProfileId == carrierAccount.CarrierProfileId);
            if (carrierProfile.Value != null)
            {
                carrierAccountDetail.CarrierProfileName = carrierProfile.Value.Name;
            }
            carrierAccountDetail.AccountTypeDescription = carrierAccount.AccountType.ToString();

            return carrierAccountDetail;
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
            var assignedCarrierAccount=assignedCarriers.FindRecord(x=>x.CarrierAccountId==carrierAccount.CarrierAccountId);
            return new AccountManagerCarrier()
            {
                CarrierAccountId = carrierAccount.CarrierAccountId,
                Name = carrierAccount.Name,
                CarrierType = carrierAccount.AccountType,
                IsCustomerAvailable = (carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Customer),
                IsSupplierAvailable = (carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange) && (assignedCarrierAccount == null || assignedCarrierAccount.RelationType != CarrierAccountType.Supplier),
            };
        }

        #endregion

        public int? GetCustomerSellingNumberPlanId(int customerId)
        {
            var customer = GetCarrierAccount(customerId);
            if (customer == null || customer.CustomerSettings == null)
                return null;
            else
                return customer.CustomerSettings.SellingNumberPlanId;
        }
    }
}

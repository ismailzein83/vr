using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            var allCarrierAccounts = GetCachedCarrierAccounts();

            Func<CarrierAccountDetail, bool> filterExpression = (item) =>
                 (input.Query.Name == null || item.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(item.CarrierProfileId))
                  &&
                 (input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(item.CarrierAccountId))
                   &&
                 (input.Query.AccountsTypes == null || input.Query.AccountsTypes.Contains(item.AccountType));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierAccounts.ToBigResult(input, filterExpression));     
        }

        public List<CarrierAccount> GetAllCustomers()
        {
            throw new NotImplementedException();
        }
        
        public CarrierAccountDetail GetCarrierAccount(int carrierAccountId)
        {
            List<CarrierAccountDetail> CarrierAccountsDetails = GetCachedCarrierAccounts();
            var carrierAccount = CarrierAccountsDetails.FindRecord(x => x.CarrierAccountId == carrierAccountId);
            return carrierAccount;
        }
        
        public string GetDescription(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            IEnumerable<CarrierAccountDetail> carrierAccounts = this.GetCarrierAccountsByIds(carrierAccountsIds, true, false);
            if(carrierAccounts != null)
                return string.Join(", ", carrierAccounts.Select(x => x.Name));

            return string.Empty;
        }

        public IEnumerable<CarrierAccountInfo> GetCarrierAccountInfo(CarrierAccountInfoFilter filter)
        {
            IEnumerable<CarrierAccountDetail> carrierAccountsDetails = null;

            if(filter != null)
            {
                carrierAccountsDetails = this.GetCarrierAccountsByType(filter.GetCustomers, filter.GetSuppliers, filter.SupplierFilterSettings, filter.CustomerFilterSettings);
            }
            else
            {
                carrierAccountsDetails = this.GetCachedCarrierAccounts();
            }
            
            return carrierAccountsDetails.MapRecords(CarrierAccountInfoMapper);
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
                var allCarrierAccounts = GetCachedCarrierAccounts();
                CarrierAccountDetail carrierAccountDetail = allCarrierAccounts.FindRecord(x => x.CarrierAccountId == carrierAccountId);
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
                var allCarrierAccounts = GetCachedCarrierAccounts();

                CarrierAccountDetail carrierAccountDetail = allCarrierAccounts.FindRecord(x => x.CarrierAccountId == carrierAccount.CarrierAccountId);
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

        #region Private Members

        List<CarrierAccountDetail> GetCachedCarrierAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierAccounts",
               () =>
               {
                   ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                   return dataManager.GetCarrierAccounts();
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

        private CarrierAccountInfo CarrierAccountInfoMapper(CarrierAccountDetail carrierAccountDetail)
        {
            return new CarrierAccountInfo()
            {
                CarrierAccountId=carrierAccountDetail.CarrierAccountId,
                Name = carrierAccountDetail.Name,
            };
        }

        private IEnumerable<CarrierAccountDetail> GetCarrierAccountsByIds(IEnumerable<int> carrierAccountsIds, bool getCustomers, bool getSuppliers)
        {
            IEnumerable<CarrierAccountDetail> carrierAccountsDetails = this.GetCarrierAccountsByType(getCustomers, getSuppliers, null, null);
            Func<CarrierAccountDetail, bool> filterExpression = null;

            if(carrierAccountsIds != null)
                filterExpression = (item) => (carrierAccountsIds.Contains(item.CarrierAccountId));

            return carrierAccountsDetails.FindAllRecords(filterExpression);
        }

        private IEnumerable<CarrierAccountDetail> GetCarrierAccountsByType(bool getCustomers, bool getSuppliers, SupplierFilterSettings supplierFilterSettings, CustomerFilterSettings customerFilterSettings)
        {
            IEnumerable<CarrierAccountDetail> carrierAccountsDetails = GetCachedCarrierAccounts();
            
            HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
            HashSet<int> filteredCustomerIds = CustomerGroupContext.GetFilteredCustomerIds(customerFilterSettings);
            Func<CarrierAccountDetail, bool> filterExpression = (carrierAccount) =>
                {
                    bool isSupplier = carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange;
                    bool isCustomer = carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange;
                    if (getCustomers && !isCustomer)
                        return false;
                    if (getSuppliers && !isSupplier)
                        return false;
                    if (isSupplier && filteredSupplierIds != null && filteredSupplierIds.Contains(carrierAccount.CarrierAccountId))
                        return false;
                    if (isCustomer && filteredCustomerIds != null && filteredCustomerIds.Contains(carrierAccount.CarrierAccountId))
                        return false;
                    return true;
                };

            return carrierAccountsDetails.FindAllRecords(filterExpression);
        }

        #endregion
    }
}

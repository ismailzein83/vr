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

            Func<CarrierAccountDetail, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(prod.CarrierProfileId))
                  &&
                 (input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(prod.CarrierAccountId))
                   &&
                 (input.Query.AccountsTypes == null || input.Query.AccountsTypes.Contains(prod.AccountType));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierAccounts.ToBigResult(input, filterExpression));     
        }
        public List<CarrierAccount> GetAllCustomers()
        {
            throw new NotImplementedException();
        }
        public CarrierAccountDetail GetCarrierAccount(int carrierAccountId)
        {
            List<CarrierAccountDetail> CarrierAccountsDetails = GetCachedCarrierAccounts();
            return CarrierAccountsDetails.FindRecord(x => x.CarrierAccountId == carrierAccountId);
        }
        public IEnumerable<CarrierAccountInfo> GetCarrierAccounts(bool getCustomers, bool getSuppliers)
        {
            List<CarrierAccountType> CarrierAccountsType = new List<CarrierAccountType>();

            if (getCustomers)
            {
                CarrierAccountsType.Add(CarrierAccountType.Customer);
            }
            if (getSuppliers)
            {
                CarrierAccountsType.Add(CarrierAccountType.Supplier);
            }
            List<CarrierAccountDetail> CarrierAccountsDetails = GetCachedCarrierAccounts();
            return CarrierAccountsDetails.MapRecords(CarrierAccountInfoMapper, x => x.AccountType == CarrierAccountType.Exchange || CarrierAccountsType.Contains(x.AccountType));
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

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Demo.Module.Business
{
    public class CarrierAccountManager
    {

        #region ctor/Local Variables
        CarrierProfileManager _carrierProfileManager;
        public CarrierAccountManager()
        {
            _carrierProfileManager = new CarrierProfileManager();
        }

        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            var allCarrierAccounts = GetCachedCarrierAccounts();

            Func<CarrierAccount, bool> filterExpression = (item) =>
                 (input.Query.Name == null || item.NameSuffix.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(item.CarrierProfileId))
                  &&
                 (input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(item.CarrierAccountId));
               

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCarrierAccounts.ToBigResult(input, filterExpression, CarrierAccountDetailMapper));
        }
        public CarrierAccount GetCarrierAccount(int carrierAccountId)
        {
            var CarrierAccounts = GetCachedCarrierAccounts();
            return CarrierAccounts.GetRecord(carrierAccountId);
        }

        public Vanrise.Entities.InsertOperationOutput<CarrierAccountDetail> AddCarrierAccount(CarrierAccount carrierAccount)
        {
            Vanrise.Entities.InsertOperationOutput<CarrierAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CarrierAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierAccountId = -1;

            ICarrierAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(carrierAccount, out carrierAccountId);
            if (insertActionSucc)
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
        public Vanrise.Entities.UpdateOperationOutput<CarrierAccountDetail> UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            ICarrierAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();

            bool updateActionSucc = dataManager.Update(carrierAccount);
            Vanrise.Entities.UpdateOperationOutput<CarrierAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CarrierAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                CarrierAccountDetail carrierAccountDetail = CarrierAccountDetailMapper(carrierAccount);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = carrierAccountDetail;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        Dictionary<int, CarrierAccount> GetCachedCarrierAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierAccounts",
               () =>
               {
                   ICarrierAccountDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                   IEnumerable<CarrierAccount> carrierAccounts = dataManager.GetCarrierAccounts();
                   return carrierAccounts.ToDictionary(kvp => kvp.CarrierAccountId, kvp => kvp);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICarrierAccountDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierAccountsUpdated(ref _updateHandle);
            }
        }
        private static string GetCarrierAccountName(string profileName, string nameSuffix)
        {
            return string.Format("{0}{1}", profileName, string.IsNullOrEmpty(nameSuffix) ? string.Empty : " (" + nameSuffix + ")");
        }

        #endregion

        #region  Mappers

        private CarrierAccountDetail CarrierAccountDetailMapper(CarrierAccount carrierAccount)
        {
            CarrierAccountDetail carrierAccountDetail = new CarrierAccountDetail();
            carrierAccountDetail.Entity = carrierAccount;

            var carrierProfile = _carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);

            if (carrierProfile != null)
            {
                carrierAccountDetail.CarrierProfileName = carrierProfile.Name;
                carrierAccountDetail.CarrierAccountName = GetCarrierAccountName(carrierProfile.Name, carrierAccountDetail.Entity.NameSuffix);
            }

            return carrierAccountDetail;
        }
        #endregion


    }
}

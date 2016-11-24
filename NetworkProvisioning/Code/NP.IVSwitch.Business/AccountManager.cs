using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

 

namespace NP.IVSwitch.Business
{
    public class AccountManager
    {
        #region Public Methods
        public Account GetAccount(int accountId)
        {
            Dictionary<int, Account> cachedAccount = this.GetCachedAccount();
            return cachedAccount.GetRecord(accountId);
        }

        public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        {
             //Get Carrier by id
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(input.Query.CarrierAccountId.GetValueOrDefault());

            //AccountExtended accountExtended = new AccountExtended();
            //if (carrierAccount.ExtendedSettings != null)
            //{
            //    Dictionary<string, object> temp = carrierAccount.ExtendedSettings;
            //    accountExtended = (AccountExtended)temp["NP_IVSwitch_ExtendedSettings"];
            //}
 
            var allAccounts = this.GetCachedAccount();
            Func<Account, bool> filterExpression = (x) => (input.Query.Name == null || x.FirstName.ToLower().Contains(input.Query.Name.ToLower()))
                                                            &&
                                                          (input.Query.AccountTypes == null || input.Query.AccountTypes.Contains(x.TypeId));
                                                    //      &&
                                                      //    (accountExtended.AccountId != null || accountExtended.AccountId.Contains(x.AccountId));
                                                         
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccounts.ToBigResult(input, filterExpression, AccountDetailMapper));
        }

      

        public Vanrise.Entities.InsertOperationOutput<AccountDetail> AddAccount(Account accountItem)
        {
            

             
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();

            int accountId = -1;

 
            if (dataManager.Insert(accountItem,out  accountId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = AccountDetailMapper(this.GetAccount(accountId));
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            //Update ExtendedSettings of carrierAccount
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            AccountExtended accountIdList = new AccountExtended();
            accountIdList.AccountId = new List<int>();
            accountIdList.AccountId.Add(accountId);
            carrierAccountManager.UpdateCarrierAccountExtendedSetting(accountItem.CarrierId, "NP_IVSwitch_ExtendedSettings", accountIdList);

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(Account accountItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();

 
            if (dataManager.Update(accountItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = AccountDetailMapper(this.GetAccount(accountItem.AccountId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAccountDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, Account> GetCachedAccount()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccount",
                () =>
                {
                    IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
                    return dataManager.GetAccounts().ToDictionary(x => x.AccountId, x => x);
                });
        }

        #endregion

        #region Mappers

        public AccountDetail AccountDetailMapper(Account account)
        {
            AccountDetail accountDetail = new AccountDetail()
            {
                Entity = account,
                CurrentStateDescription = Vanrise.Common.Utilities.GetEnumDescription<State>(account.CurrentState),
                TypeDescription = Vanrise.Common.Utilities.GetEnumDescription<AccountType>(account.TypeId),
            };

            return accountDetail;
        }

        #endregion
    }
}

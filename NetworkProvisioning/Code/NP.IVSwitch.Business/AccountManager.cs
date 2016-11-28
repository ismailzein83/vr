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
        //public Account GetAccount(int accountId)
        //{
        //    Dictionary<int, Account> cachedAccount = this.GetCachedAccount();
        //    return cachedAccount.GetRecord(accountId);
        //}

        public Account GetAccountInfoFromProfile(CarrierProfile carrierProfile, bool customer)
        {
             //CarrierAccountType { Exchange = 1, Supplier = 2, Customer = 3 }
             //   AccountType {Vendor = 1, Customer = 2}

            AccountType accountType;
            if (customer == true)
                accountType = AccountType.Customer;
            else
                accountType = AccountType.Vendor;

            Account account = new Account();

            account.FirstName = carrierProfile.Settings.Company;
            account.LastName = carrierProfile.Settings.Company;
            account.CompanyName = carrierProfile.Settings.Company;
            account.ContactDisplay = carrierProfile.Settings.Company;
            account.LogAlias = carrierProfile.Settings.Company;
            account.WebSite = carrierProfile.Settings.Website;
            account.Email = Guid.NewGuid().ToString() + "@guid.com";   // carrierProfile.Settings.Contacts.Find(x => x.Type.ToString().Equals("TechnicalEmail")).Description; //technical contact
            account.TypeId = accountType;




            return account;
        }
        //public IDataRetrievalResult<AccountDetail> GetFilteredAccounts(DataRetrievalInput<AccountQuery> input)
        //{
                    
  
        //    var allAccounts = this.GetCachedAccount();
        //    Func<Account, bool> filterExpression = (x) => (input.Query.Name == null || x.FirstName.ToLower().Contains(input.Query.Name.ToLower()))
        //                                                    &&
        //                                                  (input.Query.AccountTypes == null || input.Query.AccountTypes.Contains(x.TypeId));                                                 
                                                         
        //    return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccounts.ToBigResult(input, filterExpression, AccountDetailMapper));
        //}



        public int AddAccount(Account accountItem)
        {
            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();

            int accountId = -1; 
            dataManager.Insert(accountItem,out  accountId);

            return accountId;
        }

        //public Vanrise.Entities.UpdateOperationOutput<AccountDetail> UpdateAccount(Account accountItem)
        //{
        //    var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<AccountDetail>();

        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();

 
        //    if (dataManager.Update(accountItem))
        //    {
        //        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = AccountDetailMapper(this.GetAccount(accountItem.AccountId));
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}

        #endregion

        //#region Private Classes

        //private class CacheManager : Vanrise.Caching.BaseCacheManager
        //{
        //    IAccountDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
        //    protected override bool IsTimeExpirable { get { return true; } }

        //}
        //#endregion

        //#region Private Methods

        //Dictionary<int, Account> GetCachedAccount()
        //{
        //    return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccount",
        //        () =>
        //        {
        //            IAccountDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IAccountDataManager>();
        //            return dataManager.GetAccounts().ToDictionary(x => x.AccountId, x => x);
        //        });
        //}

        //#endregion

        //#region Mappers

        //public AccountDetail AccountDetailMapper(Account account)
        //{
        //    AccountDetail accountDetail = new AccountDetail()
        //    {
        //        Entity = account,
        //        CurrentStateDescription = Vanrise.Common.Utilities.GetEnumDescription<State>(account.CurrentState),
        //        TypeDescription = Vanrise.Common.Utilities.GetEnumDescription<AccountType>(account.TypeId),
        //    };

        //    return accountDetail;
        //}

      

      //  #endregion
    }
}

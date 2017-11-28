using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Data;
using Vanrise.AccountManager.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace Vanrise.AccountManager.Business
{
   public class AccountManagerManager
   {
       UserManager userManager = new UserManager();
       #region Public Methods
       public Vanrise.Entities.IDataRetrievalResult<AccountManagerDetail> GetFilteredAccountManagers(Vanrise.Entities.DataRetrievalInput<AccountManagerQuery> input)
       {
           var allAccountManagers = this.GetCachedAccountManagers();

           Func<Vanrise.AccountManager.Entities.AccountManager, bool> filterExpression = (prod) =>
           {
               if(input.Query.AccountManagerDefinitionId != prod.AccountManagerDefinitionId)
               {
                   return false;
               }
               if (input.Query.UserIds != null && !input.Query.UserIds.Contains(prod.UserId) )
                   return false;
               return true;
           };
           var x = Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccountManagers.ToBigResult(input, filterExpression, AccountManagerDetailMapper)); 
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccountManagers.ToBigResult(input, filterExpression, AccountManagerDetailMapper));
       }
       public IEnumerable<AccountManagerInfo> GetAccountManagerInfo(AccountManagerFilter filter)
       {
           var accountManagers = this.GetCachedAccountManagers();
           Func<Vanrise.AccountManager.Entities.AccountManager, bool> filterExpression = (accountManager) =>
           {
               if (filter != null)
               {
                   if (accountManager.AccountManagerDefinitionId != filter.AccountManagerDefinitionId)
                       return false;
               }
               return true;
           };
           return accountManagers.MapRecords(AccountManagerInfoMapper, filterExpression).OrderBy(x => x.UserName);
       }
       public string GetAccountManagerName(long accountManagerId)
       {
           var accountManager = GetAccountManager(accountManagerId);
           accountManager.ThrowIfNull("accountManager", accountManagerId);
           return userManager.GetUserName(accountManager.UserId);
       }
       public Vanrise.Entities.InsertOperationOutput<AccountManagerDetail> AddAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           int accountManagerId = -1;
           InsertOperationOutput<AccountManagerDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<AccountManagerDetail>();
           insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
           insertOperationOutput.InsertedObject = null;
           IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
           bool insertActionSucc = dataManager.AddAccountManager(accountManager, out accountManagerId);
           if (insertActionSucc)
           {
               accountManager.AccountManagerId = accountManagerId;
               insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
               insertOperationOutput.InsertedObject = AccountManagerDetailMapper(accountManager);
               Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
           }
           else
               insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
           return insertOperationOutput;
       }
       public Vanrise.Entities.UpdateOperationOutput<AccountManagerDetail> UpdateAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           UpdateOperationOutput<AccountManagerDetail> updateOperationOutput = new UpdateOperationOutput<AccountManagerDetail>();
           updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
           updateOperationOutput.UpdatedObject = null;
           IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
           bool updateActionSucc = dataManager.UpdateAccountManager(accountManager);
           if (updateActionSucc)
           {
               updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
               updateOperationOutput.UpdatedObject = AccountManagerDetailMapper(accountManager);
               Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
           }
           else
           {
               updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
           }

           return updateOperationOutput;
       }
       public bool AreUsersAssignableToAccountManager(int userId)
       {
           var accountManagers = this.GetCachedAccountManagers();
           foreach (var accountManager in accountManagers)
           {
               if (accountManager.Value.UserId == userId)
                   return false;
           }
           return true;
       }
       public Vanrise.AccountManager.Entities.AccountManager GetAccountManager(long accountManagerId)
       {
           var allAccountManagers = this.GetCachedAccountManagers();
           return allAccountManagers.GetRecord(accountManagerId);
       }
       public Guid GetAccountManagerDefinitionId(long accountManagerId)
       {
           var accountManager = GetAccountManager( accountManagerId);
           accountManager.ThrowIfNull("accountManager", accountManagerId);
           return accountManager.AccountManagerDefinitionId;
       }
       public IEnumerable<AccountManagerDefinitionConfig> GetAccountManagerDefinitionConfigs()
       {
           ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
           return manager.GetExtensionConfigurations<AccountManagerDefinitionConfig>(AccountManagerDefinitionConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
       }
       #endregion

       #region Mappers
       private AccountManagerDetail AccountManagerDetailMapper(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           return  new AccountManagerDetail()
           {
               AccountManagerId = accountManager.AccountManagerId,
               UserName = userManager.GetUserName(accountManager.UserId),
               UserId = accountManager.UserId,
               AccountManagerDefinitionId = accountManager.AccountManagerDefinitionId
           };
          
       }
       private AccountManagerInfo AccountManagerInfoMapper(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           return new AccountManagerInfo
           {
               AccountManagerId = accountManager.AccountManagerId,
               UserName = userManager.GetUserName(accountManager.UserId)
           };
       }
     
       
       #endregion

       #region Private Classes
       private class CacheManager : Vanrise.Caching.BaseCacheManager
       {
           IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
           object _updateHandle;

           protected override bool ShouldSetCacheExpired(object parameter)
           {
               return dataManager.AreAccountManagersUpdated(ref _updateHandle);
           }
       }
       #endregion

       #region Private Methods
       Dictionary<long, Vanrise.AccountManager.Entities.AccountManager> GetCachedAccountManagers()
       {
           return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountManagers",
              () =>
              {
                  IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
                  IEnumerable<Vanrise.AccountManager.Entities.AccountManager> accountManagers = dataManager.GetAccountManagers();
                  return accountManagers.ToDictionary(cn => cn.AccountManagerId, cn => cn);
              });
       }
       #endregion
   }
}

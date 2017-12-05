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
       SecurityManager s_securityManager = new SecurityManager();
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
           VRActionLogger.Current.LogGetFilteredAction(new AccountManagerLoggableEntity(input.Query.AccountManagerDefinitionId), input);
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
               VRActionLogger.Current.TrackAndLogObjectAdded(new AccountManagerLoggableEntity(accountManager.AccountManagerDefinitionId), accountManager);
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
               VRActionLogger.Current.TrackAndLogObjectUpdated(new AccountManagerLoggableEntity(accountManager.AccountManagerDefinitionId), accountManager);
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
       public bool DoesUserHaveAccess(int userId, Guid accountManagerDefinitionId, Func<AccountManagerDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
       {
           AccountManagerDefinitionManager definitionManager = new AccountManagerDefinitionManager();
           var accountManagerBEDefinitionSettings = definitionManager.GetAccountManagerDefinitionSettings(accountManagerDefinitionId);
           if (accountManagerBEDefinitionSettings != null && accountManagerBEDefinitionSettings.Security != null && getRequiredPermissionSetting(accountManagerBEDefinitionSettings.Security) != null)
               return s_securityManager.IsAllowed(getRequiredPermissionSetting(accountManagerBEDefinitionSettings.Security), userId);
           else
               return true;
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
       public class AccountManagerLoggableEntity : VRLoggableEntityBase
       {
           Guid _accountManagerDefinitionId;
           static AccountManagerDefinitionManager s_accountManagerDefinitionManager = new AccountManagerDefinitionManager();
           static UserManager s_userManager = new UserManager();
           public AccountManagerLoggableEntity(Guid accountManagerDefinitionId)
           {
               _accountManagerDefinitionId = accountManagerDefinitionId;
           }

           public override string EntityUniqueName
           {
               get { return String.Format("VR_AccountManager_AccountManager_{0}", _accountManagerDefinitionId); }
           }

           public override string EntityDisplayName
           {
               get { return s_accountManagerDefinitionManager.GetAccountManagerDefinitionName(_accountManagerDefinitionId); }
           }

           public override string ViewHistoryItemClientActionName
           {
               get { return "VR_AccountManager_AccountManager_ViewHistoryItem"; }
           }


           public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
           {
               Vanrise.AccountManager.Entities.AccountManager accountManager = context.Object.CastWithValidate<Vanrise.AccountManager.Entities.AccountManager>("context.Object");
               return accountManager.AccountManagerId;
           }

           public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
           {
               Vanrise.AccountManager.Entities.AccountManager accountManager = context.Object.CastWithValidate<Vanrise.AccountManager.Entities.AccountManager>("context.Object");
               return s_userManager.GetUserName(accountManager.UserId);
           }

           public override string ModuleName
           {
               get { return "Business Entity"; }
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
       #region Security
       public bool DoesUserHaveViewAccess(Guid accountManagerDefinitionId)
       {
           int userId = SecurityContext.Current.GetLoggedInUserId();
           return DoesUserHaveAccess(userId, accountManagerDefinitionId, (sec) => sec.ViewRequiredPermission);
       }
       public bool DoesUserHaveAddAccess(Guid accountManagerDefinitionId)
       {
           int userId = SecurityContext.Current.GetLoggedInUserId();
           return DoesUserHaveAccess(userId, accountManagerDefinitionId, (sec) => sec.AddRequiredPermission);

       }
       public bool DoesUserHaveEditAccess(Guid accountManagerDefinitionId)
       {
           int userId = SecurityContext.Current.GetLoggedInUserId();
           return DoesUserHaveEditAccess(userId, accountManagerDefinitionId);
       }
       public bool DoesUserHaveEditAccess(int userId, Guid accountManagerDefinitionId)
       {
           return DoesUserHaveAccess(userId, accountManagerDefinitionId, (sec) => sec.EditRequiredPermission);
       }
       #endregion
   }
}

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
               if (input.Query.UserIds != null && !input.Query.UserIds.Contains(prod.UserId))
                   return false;
               return true;
           };
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccountManagers.ToBigResult(input, filterExpression, AccountManagerDetailMapper));
       }
       #endregion

       #region Mappers
       private AccountManagerDetail AccountManagerDetailMapper(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           var accountManagerDetail = new AccountManagerDetail()
           {
               UserName = userManager.GetUserName(accountManager.UserId),
           };
           return accountManagerDetail;
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
       Dictionary<int, Vanrise.AccountManager.Entities.AccountManager> GetCachedAccountManagers()
       {
           return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAccountManagers",
              () =>
              {
                  IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
                  IEnumerable<Vanrise.AccountManager.Entities.AccountManager> accountManagers = dataManager.GetAccountManagers();
                  return accountManagers.ToDictionary(cn => cn.UserId, cn => cn);
              });
       }
       #endregion
   }
}

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



namespace Vanrise.AccountManager.Business
{
   public class AMManager
    {
       public Vanrise.Entities.IDataRetrievalResult<AccountManagerDetail> GetFilteredAccountManagers(Vanrise.Entities.DataRetrievalInput<AccountManagerQuery> input)
       {
           var allAccountManagers = this.GetCachedAccountManagers();

           Func<Vanrise.AccountManager.Entities.AccountManager, bool> filterExpression = (prod) =>
            (input.Query.UserIds == null || input.Query.UserIds.Contains(prod.UserId));
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allAccountManagers.ToBigResult(input, filterExpression, AccountManagerDetailMapper));
       }
       private AccountManagerDetail AccountManagerDetailMapper(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           var accountManagerDetail = new AccountManagerDetail()
           {
               UserID = accountManager.UserId,
           };
           return accountManagerDetail;
       }
       private class CacheManager : Vanrise.Caching.BaseCacheManager
       {
           IAccountManagerDataManager dataManager = AccountManagerDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
           object _updateHandle;

           protected override bool ShouldSetCacheExpired(object parameter)
           {
               return dataManager.AreAccountManagersUpdated(ref _updateHandle);
           }
       }
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
    }
}

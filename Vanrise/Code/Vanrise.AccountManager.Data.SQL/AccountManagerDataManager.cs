using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.AccountManager.Entities;
namespace Vanrise.AccountManager.Data.SQL
{
   public class AccountManagerDataManager : BaseSQLDataManager, IAccountManagerDataManager
    {
       public AccountManagerDataManager()
           : base(GetConnectionStringName("VR_AccountManager_AccountManagerDBConnStringKey", "VR_AccountManager_AccountManagerDBConnString"))
    {}
       public List<Vanrise.AccountManager.Entities.AccountManager> GetAccountManagers()
       {
           return GetItemsSP("[VR_AccountManager].[sp_AccountManager_GetAll]", AccountManagerMapper);
       }
       public bool AreAccountManagersUpdated(ref object updateHandle)
       {
           return base.IsDataUpdated("VR_AccountManager.AccountManager", ref updateHandle);
       }
       Vanrise.AccountManager.Entities.AccountManager AccountManagerMapper(IDataReader reader)
       {
           return new Vanrise.AccountManager.Entities.AccountManager()
           {
               UserId = (int)reader["UserID"],
           };
       }
    }
}

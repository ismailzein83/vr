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
       public bool AddAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager, out int insertedId)
       {
           object acountManagerId;
           int recordsEffected = ExecuteNonQuerySP("[VR_AccountManager].[sp_AccountManager_Insert]", out acountManagerId, accountManager.AccountManagerDefinitionId, accountManager.UserId);
           bool insertedSuccesfully = (recordsEffected > 0);
           if (insertedSuccesfully)
               insertedId = (int)acountManagerId;
           else
               insertedId = 0;
           return insertedSuccesfully;

       }
       public bool UpdateAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager)
       {
           int recordsEffected = ExecuteNonQuerySP("[VR_AccountManager].[sp_AccountManager_Update]", accountManager.AccountManagerId, accountManager.UserId,accountManager.AccountManagerDefinitionId);
           return (recordsEffected > 0);
       }
       public bool AreAccountManagersUpdated(ref object updateHandle)
       {
           return base.IsDataUpdated("VR_AccountManager.AccountManager", ref updateHandle);
       }
       Vanrise.AccountManager.Entities.AccountManager AccountManagerMapper(IDataReader reader)
       {
           return new Vanrise.AccountManager.Entities.AccountManager()
           {
               AccountManagerDefinitionId = (Guid)reader["AccountManagerDefinitionId"],
               AccountManagerId = (long)reader["ID"],
               UserId = (int)reader["UserID"],
           };
       }
    }
}

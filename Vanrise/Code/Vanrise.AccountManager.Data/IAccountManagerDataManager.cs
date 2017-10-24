using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Data
{
    public interface IAccountManagerDataManager : IDataManager
    {
        List<Entities.AccountManager> GetAccountManagers();
        bool AreAccountManagersUpdated(ref object updateHandle);
        bool AddAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager, out int insertedId);
        bool UpdateAccountManager(Vanrise.AccountManager.Entities.AccountManager accountManager);
    }
}

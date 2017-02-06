using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountServiceDataManager:IDataManager
    {
        bool Insert(AccountService accountService, out long insertedId);
        bool Update(AccountService accountService);
        bool AreAccountServicesUpdated(ref object updateHandle);
        List<AccountService> GetAccountServices();
        bool UpdateStatus(long accountServiceId, Guid statusId);

    }
}

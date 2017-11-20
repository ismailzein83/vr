using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountStatusHistoryDataManager:IDataManager
    {
        void Insert(Guid accountDefinitionId, long accountId, Guid statusDefinitionId, Guid? previousStatusId, DateTime statusChangedDate);
        void DeleteAccountStatusHistories(List<long> accountStatusHistoryIdsToDelete);
        List<AccountStatusHistory> GetAccountStatusHistoryList(HashSet<AccountDefinition> accountDefinitions);
        IEnumerable<AccountStatusHistory> GetAccountStatusHistoriesAfterDate(Guid accountBEDefinitionId, List<long> accountIds, DateTime statusChangedDate);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
namespace Vanrise.GenericData.Business
{
    public struct BEDefinition
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public Object BusinessEntityId { get; set; }
        public string FieldName { get; set; }
    }

    public class BusinessEntityStatusHistoryManager
    {
        public bool InsertStatusHistory(Guid businessEntityDefinitionId, Object businessEntityId, string fieldName, Guid statusId)
        {
            IBusinessEntityStatusHistoryDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
            return dataManager.Insert(businessEntityDefinitionId, businessEntityId, fieldName, statusId, Guid.NewGuid());
        }
        //public IEnumerable<BusinessEntityStatusHistory> GetBusinessEntityStatusHistoryAfterDate(Guid businessEntityDefinitionId, Object businessEntittId, string fieldName, DateTime statusChangedDate)
        //{
        //    IBusinessEntityStatusHistoryDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityStatusHistoryDataManager>();
        //    return dataManager.GetBusinessEntityStatusHistoriesAfterDate(businessEntityDefinitionId, businessEntityIds, fieldName, statusChangedDate);
        //}

        //public Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> GetAccountStatusHistoryListByAccountDefinition(HashSet<AccountDefinition> accountDefinitions)
        //{
        //    IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
        //    List<AccountStatusHistory> accountStatusHistoryList = dataManager.GetAccountStatusHistoryList(accountDefinitions);
        //    if (accountStatusHistoryList == null)
        //        return null;

        //    Dictionary<AccountDefinition, List<AccountStatusHistory>> accountStatusHistoryListByAccountDefinition = new Dictionary<AccountDefinition, List<AccountStatusHistory>>();
        //    foreach (AccountStatusHistory accountStatusHistory in accountStatusHistoryList)
        //    {
        //        AccountDefinition accountDefinition = new AccountDefinition()
        //        {
        //            AccountBEDefinitionId = accountStatusHistory.AccountBEDefinitionId,
        //            AccountId = accountStatusHistory.AccountId
        //        };
        //        List<AccountStatusHistory> tempAccountStatusHistoryList = accountStatusHistoryListByAccountDefinition.GetOrCreateItem(accountDefinition, () => { return new List<AccountStatusHistory>(); });
        //        tempAccountStatusHistoryList.Add(accountStatusHistory);
        //    }
        //    return accountStatusHistoryListByAccountDefinition.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(historyItem => historyItem.StatusChangedDate));
        //}
    }
}

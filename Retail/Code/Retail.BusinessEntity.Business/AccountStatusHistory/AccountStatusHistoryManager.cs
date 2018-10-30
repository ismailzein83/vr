﻿using System;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System.Linq;

namespace Retail.BusinessEntity.Business
{
    public class AccountStatusHistoryManager
    {
        public void AddAccountStatusHistory(Guid accountDefinitionId, long accountId, Guid statusDefinitionId, Guid? previousStatusId, DateTime statusChangedDate)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            dataManager.Insert(accountDefinitionId, accountId, statusDefinitionId, previousStatusId, statusChangedDate);
        }

        public void DeleteAccountStatusHistories(List<long> accountStatusHistoryIdsToDelete)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            dataManager.DeleteAccountStatusHistories(accountStatusHistoryIdsToDelete);
        }
        public Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> GetAccountStatusHistoryListByAccountDefinition(HashSet<AccountDefinition> accountDefinitions)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            List<AccountStatusHistory> accountStatusHistoryList = dataManager.GetAccountStatusHistoryList(accountDefinitions);
            if (accountStatusHistoryList == null)
                return null;

            Dictionary<AccountDefinition, List<AccountStatusHistory>> accountStatusHistoryListByAccountDefinition = new Dictionary<AccountDefinition, List<AccountStatusHistory>>();
            foreach (AccountStatusHistory accountStatusHistory in accountStatusHistoryList)
            {
                AccountDefinition accountDefinition = new AccountDefinition()
                {
                    AccountBEDefinitionId = accountStatusHistory.AccountBEDefinitionId,
                    AccountId = accountStatusHistory.AccountId
                };
                List<AccountStatusHistory> tempAccountStatusHistoryList = accountStatusHistoryListByAccountDefinition.GetOrCreateItem(accountDefinition, () => { return new List<AccountStatusHistory>(); });
                tempAccountStatusHistoryList.Add(accountStatusHistory);
            }
            return accountStatusHistoryListByAccountDefinition.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(historyItem => historyItem.StatusChangedDate));
        }

        public Guid GetAccountStatus(Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> accountStatusHistoryListByAccountDefinition, Guid accountBEDefinitionId, Account account, DateTime chargeDay)
        {
            account.ThrowIfNull("account");

            if (accountStatusHistoryListByAccountDefinition == null || accountStatusHistoryListByAccountDefinition.Count == 0)
                return account.StatusId;

            IOrderedEnumerable<AccountStatusHistory> accountStatusHistoryList = accountStatusHistoryListByAccountDefinition.GetRecord(new AccountDefinition() { AccountBEDefinitionId = accountBEDefinitionId, AccountId = account.AccountId });
            if (accountStatusHistoryList == null || accountStatusHistoryList.Count() == 0)
                return account.StatusId;

            AccountStatusHistory previousAccountStatusHistory = null;
            foreach (AccountStatusHistory accountStatusHistory in accountStatusHistoryList.OrderByDescending(itm => itm.StatusChangedDate).ThenByDescending(itm => itm.AccountStatusHistoryId))
            {
                if (accountStatusHistory.StatusChangedDate <= chargeDay)
                    return accountStatusHistory.StatusId;

                previousAccountStatusHistory = accountStatusHistory;
            }

            if (previousAccountStatusHistory != null)
            {
                if (previousAccountStatusHistory.PreviousStatusId.HasValue)
                    return previousAccountStatusHistory.PreviousStatusId.Value;
                else
                    return previousAccountStatusHistory.StatusId;
            }

            return account.StatusId;
        }

        public Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> GetAccountStatusHistoryListByAccountDefinition(Guid accountBEDefinition, List<long> accountIds)
        {
            HashSet<AccountDefinition> accountDefinitions = new HashSet<AccountDefinition>();
            if (accountIds != null)
            {
                foreach (var accountId in accountIds)
                {
                    accountDefinitions.Add(new AccountDefinition
                    {
                        AccountBEDefinitionId = accountBEDefinition,
                        AccountId = accountId
                    });
                }
            }
            return GetAccountStatusHistoryListByAccountDefinition(accountDefinitions);
        }


        public IEnumerable<AccountStatusHistory> GetAccountStatusHistoriesAfterDate(Guid accountBEDefinitionId, List<long> accountIds, DateTime statusChangedDate)
        {
            IAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountStatusHistoryDataManager>();
            return dataManager.GetAccountStatusHistoriesAfterDate(accountBEDefinitionId, accountIds, statusChangedDate);
        }
    }
}

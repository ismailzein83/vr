using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountManager
    {
        static AccountBEManager s_accountManager = new AccountBEManager();
        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
      
        #region Public Methods
        public IDataRetrievalResult<FinancialAccountDetail> GetFilteredFinancialAccounts(DataRetrievalInput<FinancialAccountQuery> input)
        {
            var cachedFinancialAccounts = GetFinancialAccounts(input.Query.AccountBEDefinitionId, input.Query.AccountId, false);
            Func<FinancialAccountData, bool> filterExpression = (financialAccountData) =>
            {
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, cachedFinancialAccounts.ToBigResult(input, filterExpression, FinancialAccountDetailMapper));
        }
        public IOrderedEnumerable<FinancialAccountData> GetFinancialAccounts(Guid accountDefinitionId, long accountId, bool withInherited)
        {
            var cachedFinancialAccounts = withInherited ? GetCachedFinancialAccountsWithInherited(accountDefinitionId) : GetCachedFinancialAccounts(accountDefinitionId);
            return cachedFinancialAccounts.GetRecord(accountId);
        }
        public bool TryGetFinancialAccount(Guid accountDefinitionId, long accountId, bool withInherited, DateTime effectiveOn, out FinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<FinancialAccountData> financialAccounts = GetFinancialAccounts(accountDefinitionId, accountId, withInherited);
            if (financialAccounts != null)
            {
                financialAccountData = financialAccounts.FindRecord(itm => itm.FinancialAccount.IsEffective(effectiveOn));
                return financialAccountData != null;
            }
            else
            {
                financialAccountData = null;
                return false;
            }
        }
        public Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> AddFinancialAccount(FinancialAccountToInsert financialAccountToInsert)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            var accountBEFinancialAccountsSettings = GetAccountBEFinancialAccountsSettings(financialAccountToInsert.AccountBEDefinitionId, financialAccountToInsert.AccountId);
            accountBEFinancialAccountsSettings.LastTakenSequenceNumber++;
            financialAccountToInsert.FinancialAccount.SequenceNumber = accountBEFinancialAccountsSettings.LastTakenSequenceNumber;
            accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccountToInsert.FinancialAccount);

            if(s_accountManager.UpdateAccountExtendedSetting(financialAccountToInsert.AccountBEDefinitionId,financialAccountToInsert.AccountId,accountBEFinancialAccountsSettings))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccountToInsert.FinancialAccount);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> UpdateFinancialAccount(FinancialAccountToEdit financialAccountToEdit)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            
            var accountBEFinancialAccountsSettings = GetAccountBEFinancialAccountsSettings(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId);
            var financialAccount = accountBEFinancialAccountsSettings.FinancialAccounts.FindRecord(x => x.SequenceNumber == financialAccountToEdit.FinancialAccount.SequenceNumber);
            accountBEFinancialAccountsSettings.FinancialAccounts.Remove(financialAccount);
            accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccountToEdit.FinancialAccount);
            if (s_accountManager.UpdateAccountExtendedSetting(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, accountBEFinancialAccountsSettings))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(financialAccountToEdit.FinancialAccount);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public FinancialAccountRuntimeEditor GetFinancialAccountEditorRuntime(Guid accountBEDefinitionId, long accountId, int sequenceNumber)
        {
            IOrderedEnumerable<FinancialAccountData> financialAccounts = GetFinancialAccounts(accountBEDefinitionId, accountId, false);
            if (financialAccounts != null)
            {
                FinancialAccountData financialAccountData = financialAccounts.FindRecord(itm => itm.FinancialAccount.SequenceNumber == sequenceNumber);
                if (financialAccountData == null)
                    return null;
                return new FinancialAccountRuntimeEditor
                {
                    FinancialAccount = financialAccountData.FinancialAccount
                };
            }
            return null;
        }

        #endregion

        #region Private Methods
        private AccountBEFinancialAccountsSettings GetAccountBEFinancialAccountsSettings(Guid accountBEDefinitionId, long accountId)
        {
            var accountBEFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(accountBEDefinitionId, accountId);
            if (accountBEFinancialAccountsSettings == null)
                accountBEFinancialAccountsSettings = new AccountBEFinancialAccountsSettings();
            if (accountBEFinancialAccountsSettings.FinancialAccounts == null)
                accountBEFinancialAccountsSettings.FinancialAccounts = new List<FinancialAccount>();
            return accountBEFinancialAccountsSettings;
        }
        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccounts(Guid accountDefinitionId)
        {
            return GetCacheManager().GetOrCreateObject("GetCachedFinancialAccounts", accountDefinitionId,
                () =>
                {
                    Dictionary<long, IOrderedEnumerable<FinancialAccountData>> allFinancialAccountsData = new Dictionary<long, IOrderedEnumerable<FinancialAccountData>>();
                    var allAccounts = s_accountManager.GetCachedAccounts(accountDefinitionId);
                    if (allAccounts != null)
                    {
                        foreach (var account in allAccounts.Values)
                        {
                            AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(account);
                            if (accountFinancialAccountsSettings != null && accountFinancialAccountsSettings.FinancialAccounts != null)
                            {
                                List<FinancialAccountData> financialAccountsData = accountFinancialAccountsSettings.FinancialAccounts.Select(itm => CreateFinancialAccountData(itm, account)).ToList();
                                allFinancialAccountsData.Add(account.AccountId, financialAccountsData.OrderByDescending(itm => itm.FinancialAccount.EED.HasValue ? itm.FinancialAccount.EED.Value : DateTime.MaxValue));
                            }
                        }
                    }
                    return allFinancialAccountsData;
                });
        }
        private FinancialAccountData CreateFinancialAccountData(FinancialAccount financialAccount, Account account)
        {
            return new FinancialAccountData
            {
                FinancialAccount = financialAccount,
                Account = account
            };
        }
        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccountsWithInherited(Guid accountDefinitionId)
        {
            return GetCacheManager().GetOrCreateObject("GetCachedFinancialAccountsWithInherited", accountDefinitionId,
                () =>
                {
                    Dictionary<long, List<FinancialAccountData>> allFinancialAccountsData = new Dictionary<long, List<FinancialAccountData>>();
                    var allAccountNodes = s_accountManager.GetCacheAccountTreeNodes(accountDefinitionId);
                    if(allAccountNodes != null)
                    {
                        foreach (var accountNode in allAccountNodes.Values)
                        {
                            if (!accountNode.Account.ParentAccountId.HasValue)//start from only root nodes
                                AddAccountNodeFinancialAccounts(accountNode, allFinancialAccountsData);
                        }
                    }

                    return allFinancialAccountsData.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(faItm => faItm.FinancialAccount.EED.HasValue ? faItm.FinancialAccount.EED.Value : DateTime.MaxValue));
                });
        }
        private void AddAccountNodeFinancialAccounts(AccountTreeNode accountNode,  Dictionary<long, List<FinancialAccountData>> allFinancialAccountsData)
        {
            List<FinancialAccountData> financialAccounts = allFinancialAccountsData.GetOrCreateItem(accountNode.Account.AccountId);
            AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(accountNode.Account);
            if (accountFinancialAccountsSettings != null && accountFinancialAccountsSettings.FinancialAccounts != null && accountFinancialAccountsSettings.FinancialAccounts.Count > 0)
            {
                financialAccounts.AddRange(accountFinancialAccountsSettings.FinancialAccounts.Select(itm => CreateFinancialAccountData(itm, accountNode.Account)));
            }

            if (accountNode.ChildNodes != null && accountNode.ChildNodes.Count > 0)
            {
                foreach (var childNode in accountNode.ChildNodes)
                {
                    if (financialAccounts.Count > 0)
                    {
                        List<FinancialAccountData> childFinancialAccounts = allFinancialAccountsData.GetOrCreateItem(childNode.Account.AccountId);
                        childFinancialAccounts.AddRange(financialAccounts.Select(itm =>
                            new FinancialAccountData
                            {
                                FinancialAccount = itm.FinancialAccount,
                                Account = itm.Account,
                                CreditLimit = itm.CreditLimit,
                                IsInherited = true
                            }).ToList());
                    }
                    AddAccountNodeFinancialAccounts(childNode, allFinancialAccountsData);
                }
            }
        }
        private Vanrise.Caching.BaseCacheManager<Guid> GetCacheManager()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>();
        }
        private FinancialAccountDetail FinancialAccountDetailMapper(FinancialAccountData financialAccountData)
        {
            return FinancialAccountDetailMapper(financialAccountData.FinancialAccount);
        }
        private FinancialAccountDetail FinancialAccountDetailMapper(FinancialAccount financialAccount)
        {
            return new FinancialAccountDetail
            {
                SequenceNumber = financialAccount.SequenceNumber,
                BED = financialAccount.BED,
                EED = financialAccount.EED,
                FinancialAccountDefinitionName = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionName(financialAccount.FinancialAccountDefinitionId)
            };
        }
        #endregion
    }
}

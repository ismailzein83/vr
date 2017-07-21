using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountManager
    {
        static AccountBEManager s_accountManager = new AccountBEManager();
        static AccountBEDefinitionManager s_accountBEDefinitionManager = new AccountBEDefinitionManager();
        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
      
        #region Public Methods

        public void UpdateAccountStatus(Guid accountDefinitionId, long accountId)
        {
            AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(accountDefinitionId, accountId);
            if (accountFinancialAccountsSettings != null && accountFinancialAccountsSettings.FinancialAccounts != null)
            {
                foreach (var financialAccount in accountFinancialAccountsSettings.FinancialAccounts)
                {
                    UpdateAccountStatus(accountDefinitionId, accountId, financialAccount);
                }
            }
        }

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
        public IEnumerable<FinancialAccountData> GetFinancialAccountsWithInheritedAndChilds(Guid accountDefinitionId, long accountId)
        {
            var allFinancialAccountsData = GetFinancialAccounts(accountDefinitionId, accountId, true).ToList();
            var accounts = s_accountManager.GetChildAccounts(accountDefinitionId, accountId, true);
            foreach( var account in accounts)
            {
                AccountBEFinancialAccountsSettings accountFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(account);
                if (accountFinancialAccountsSettings != null && accountFinancialAccountsSettings.FinancialAccounts != null && accountFinancialAccountsSettings.FinancialAccounts.Count > 0)
                {
                    allFinancialAccountsData.AddRange(accountFinancialAccountsSettings.FinancialAccounts.Select(itm => CreateFinancialAccountData(itm, account)));
                }
            }
            return allFinancialAccountsData;
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
            string message = null;
            if (CheckFinancialAccountOverlaping(financialAccountToInsert.AccountBEDefinitionId, financialAccountToInsert.AccountId, financialAccountToInsert.FinancialAccount, out  message))
            {
                var accountBEFinancialAccountsSettings = GetAccountBEFinancialAccountsSettings(financialAccountToInsert.AccountBEDefinitionId, financialAccountToInsert.AccountId);
                AddFinancialAccountToExtSettings(financialAccountToInsert.FinancialAccount, accountBEFinancialAccountsSettings);

                if (s_accountManager.UpdateAccountExtendedSetting(financialAccountToInsert.AccountBEDefinitionId, financialAccountToInsert.AccountId, accountBEFinancialAccountsSettings))
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccountToInsert.FinancialAccount);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }else
            {
                insertOperationOutput.Message = message;
            }
            return insertOperationOutput;
        }
        
        public AccountBEFinancialAccountsSettings CreateAccountFinancialAccountExtSettings()
        {
            var accountBEFinancialAccountsSettings = new AccountBEFinancialAccountsSettings();
            accountBEFinancialAccountsSettings.FinancialAccounts = new List<FinancialAccount>();
            return accountBEFinancialAccountsSettings;
        }

        public void AddFinancialAccountToExtSettings(FinancialAccount financialAccount, AccountBEFinancialAccountsSettings accountBEFinancialAccountsSettings)
        {
            accountBEFinancialAccountsSettings.LastTakenSequenceNumber++;
            financialAccount.SequenceNumber = accountBEFinancialAccountsSettings.LastTakenSequenceNumber;
            accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccount);
        }

        public Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> UpdateFinancialAccount(FinancialAccountToEdit financialAccountToEdit)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            string errorrMessage = null;
            if (CheckFinancialAccountOverlaping(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, financialAccountToEdit.FinancialAccount, out  errorrMessage))
            {
                var accountBEFinancialAccountsSettings = GetAccountBEFinancialAccountsSettings(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId);
                var financialAccount = accountBEFinancialAccountsSettings.FinancialAccounts.FindRecord(x => x.SequenceNumber == financialAccountToEdit.FinancialAccount.SequenceNumber);
                accountBEFinancialAccountsSettings.FinancialAccounts.Remove(financialAccount);
                accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccountToEdit.FinancialAccount);

                if (UpdateAccountEffectiveDate(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, financialAccountToEdit.FinancialAccount, out errorrMessage))
                {
                    if (s_accountManager.UpdateAccountExtendedSetting(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, accountBEFinancialAccountsSettings))
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(financialAccountToEdit.FinancialAccount);
                    }
                    else
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                    }

                }
            }
            if (errorrMessage != null)
            {
                updateOperationOutput.Message = errorrMessage;
                updateOperationOutput.ShowExactMessage = true;
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

        public bool CheckAllowAddFinancialAccounts(Guid accountDefinitionId, long accountId)
        {
            var financialAccountsData =  GetFinancialAccountsWithInheritedAndChilds( accountDefinitionId,  accountId);
            foreach (var financialAccount in financialAccountsData)
            {
                if (!financialAccount.FinancialAccount.EED.HasValue)
                {
                    return false;
                }
            }
            return true;
        }

        public FinancialAccountRuntimeData GetAccountFinancialInfo(Guid accountDefinitionId, long accountId, DateTime effectiveOn)
        {
            var financialAccountLocatorContext = new Retail.BusinessEntity.Business.FinancialAccountLocatorContext();
            financialAccountLocatorContext.AccountDefinitionId = accountDefinitionId;
            financialAccountLocatorContext.AccountId = accountId;
            financialAccountLocatorContext.EffectiveOn = effectiveOn;

            FinancialAccountLocator financialAccountLocator = GetFinancialAccountLocator(accountDefinitionId, accountId, effectiveOn);
            financialAccountLocator.ThrowIfNull("financialAccountLocator");

            if (financialAccountLocator.TryGetFinancialAccountId(financialAccountLocatorContext))
            {
                return new FinancialAccountRuntimeData()
                {
                    FinancialAccountId = financialAccountLocatorContext.FinancialAccountId,
                    BalanceAccountId = financialAccountLocatorContext.BalanceAccountId,
                    BalanceAccountTypeId = financialAccountLocatorContext._balanceAccountTypeId
                };
            }

            return null;
        }

        public FinancialAccountLocator GetFinancialAccountLocator(Guid accountDefinitionId, long accountId, DateTime effectiveOn)
        {
            var accountBEDefinitionSettings = s_accountBEDefinitionManager.GetAccountBEDefinitionSettings(accountDefinitionId);
            accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings", accountDefinitionId);
            return accountBEDefinitionSettings.FinancialAccountLocator != null ? accountBEDefinitionSettings.FinancialAccountLocator : new DefaultFinancialAccountLocator();
        }
        public IEnumerable<FinancialAccountInfo> GetFinancialAccountsInfo(Guid accountBEDefinitionId ,FinancialAccountInfoFilter filter)
        {
            List<FinancialAccountInfo> financialAccountsInfo = new List<FinancialAccountInfo>();
            if (filter != null && filter.AccountIds != null)
            {
                foreach (var accountId in filter.AccountIds)
                { 
                    var cachedFinancialAccounts = GetFinancialAccounts(accountBEDefinitionId, accountId, false);
                    Func<FinancialAccountData, bool> filterExpression = (financialAccountData) =>
                    {
                        if (filter.Status.HasValue)
                        {
                            switch (filter.Status.Value)
                            {
                                case VRAccountStatus.Active:
                                    if (filter.IsEffectiveInFuture.HasValue)
                                    {
                                        if (filter.IsEffectiveInFuture.Value)
                                        {
                                            if (financialAccountData.FinancialAccount.EED.HasValue && financialAccountData.FinancialAccount.EED < DateTime.Now)
                                                return false;
                                        }
                                    }
                                    if (filter.EffectiveDate.HasValue)
                                    {
                                        if (financialAccountData.FinancialAccount.BED > filter.EffectiveDate.Value || (financialAccountData.FinancialAccount.EED.HasValue && financialAccountData.FinancialAccount.EED.Value < filter.EffectiveDate.Value))
                                            return false;
                                    }
                                    break;
                                case VRAccountStatus.InActive:
                                    break;
                            }
                        }
                        return true;
                    };
                    financialAccountsInfo.AddRange(cachedFinancialAccounts.MapRecords(FinancialAccountInfoMapper, filterExpression));

                }
            }
            return financialAccountsInfo;
        }

        public string GetFinancialAccountId(long accountId, int sequenceNumber)
        {
            return string.Format("{0}_{1}", accountId, sequenceNumber);
        }
        public FinancialAccountData GetFinancialAccountData(Guid accountBEDefinitionId, string financialAccountId)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.GetRecord(financialAccountId);
        }
        public IEnumerable<string> GetAllFinancialAccountsIds(Guid accountBEDefinitionId)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.Keys;
        }
        public IEnumerable<long> GetAccountIdsByFinancialAccountIds(Guid accountBEDefinitionId, List<string> financialAccountIds)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.MapRecords(x=> x.Value.Account.AccountId, y=> financialAccountIds.Contains(y.Key));
        }


        #endregion

        #region Private Validation Methods
       

        private bool CheckFinancialAccountOverlaping(Guid accountDefinitionId, long accountId,FinancialAccount mainFinancialAccount,out string message)
        {

            if (mainFinancialAccount.EED.HasValue && mainFinancialAccount.EED.Value < new DateTime())
            {
                message = "EED must not be less than today.";
                return false;
            }
            var financialAccountsData = GetFinancialAccountsWithInheritedAndChilds(accountDefinitionId, accountId);
            bool result = true;
            CheckFinancialAccountOverlaping(mainFinancialAccount, financialAccountsData, out message, out result);
            return result;
        }
        private void CheckFinancialAccountOverlaping(FinancialAccount mainFinancialAccount, IEnumerable<FinancialAccountData> financialAccounts, out string message, out bool result)
        {
            foreach (var financialAccount in financialAccounts)
            {
                if (financialAccount.FinancialAccount.SequenceNumber != mainFinancialAccount.SequenceNumber)
                {
                    if (mainFinancialAccount.IsOverlappedWith(financialAccount.FinancialAccount))
                    {
                        message = string.Format("Financial account must not overlap.");
                        result = false;
                        return;
                    }
                }
            }
            message = null;
            result = true;
        }
        #endregion
         
        #region Private Methods
        private AccountBEFinancialAccountsSettings GetAccountBEFinancialAccountsSettings(Guid accountBEDefinitionId, long accountId)
        {
            var accountBEFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(accountBEDefinitionId, accountId);
            if (accountBEFinancialAccountsSettings == null)
            {
                accountBEFinancialAccountsSettings = CreateAccountFinancialAccountExtSettings();
            }
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
            var financialAccountData = new FinancialAccountData
            {
                FinancialAccountId = GetFinancialAccountId(account.AccountId, financialAccount.SequenceNumber),
                FinancialAccount = financialAccount,
                Account = account
            };
            financialAccount.ExtendedSettings.ThrowIfNull("financialAccount.ExtendedSettings", financialAccountData.FinancialAccountId);
            var fillExtraDataContext = new FinancialAccountFillExtraDataContext
            {
                FinancialAccountData = financialAccountData
            };
            financialAccount.ExtendedSettings.FillExtraData(fillExtraDataContext);
            return financialAccountData;
        }
        private Dictionary<string, FinancialAccountData> GetCachedFinancialAccountDataByFinancialAccountId(Guid accountDefinitionId)
        {
            return GetCacheManager().GetOrCreateObject("GetCachedFinancialAccountDataByFinancialAccountId", accountDefinitionId,
                () =>
                {
                    var cachedFinancialAccounts = GetCachedFinancialAccounts(accountDefinitionId);
                    Dictionary<string, FinancialAccountData> financialAccountDataByFinancialAccountId = new Dictionary<string, FinancialAccountData>();
                    foreach (var cachedFinancialAccount in cachedFinancialAccounts)
                    {
                        if (cachedFinancialAccount.Value != null)
                        {
                            foreach (var financialAccountData in cachedFinancialAccount.Value)
                            {
                                financialAccountDataByFinancialAccountId.Add(financialAccountData.FinancialAccountId, financialAccountData);
                            }
                        }
                    }
                    return financialAccountDataByFinancialAccountId;
                });
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
                                FinancialAccountId = itm.FinancialAccountId,
                                CreditLimit = itm.CreditLimit,
                                CreditLimitCurrencyId = itm.CreditLimitCurrencyId,
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
            var financialAccountDetail = FinancialAccountDetailMapper(financialAccountData.FinancialAccount);
            financialAccountDetail.FinancialAccountId = financialAccountData.FinancialAccountId;
            return financialAccountDetail;
        }
        private FinancialAccountDetail FinancialAccountDetailMapper(FinancialAccount financialAccount)
        {
            return new FinancialAccountDetail
            {
                SequenceNumber = financialAccount.SequenceNumber,
                BED = financialAccount.BED,
                EED = financialAccount.EED,
                FinancialAccountDefinitionName = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionName(financialAccount.FinancialAccountDefinitionId),
                BalanceAccountTypeId = s_financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId)
            };
        }
        private FinancialAccountInfo FinancialAccountInfoMapper(FinancialAccountData financialAccountData)
        {
            return new FinancialAccountInfo
            {
                FinancialAccountId = financialAccountData.FinancialAccountId,
                Description = string.Format("{0} ({1})",s_accountManager.GetAccountName(financialAccountData.Account),GetFinancialAccountDescription(financialAccountData.FinancialAccount)),
                IsEffectiveAndActive = IsFinancialAccountEffectiveAndActive(financialAccountData.FinancialAccount)
            };
        }
        private bool IsFinancialAccountEffectiveAndActive(FinancialAccount financialAccount)
        {
            return (financialAccount.BED <= DateTime.Now && !financialAccount.EED.HasValue || financialAccount.EED > DateTime.Now);
        }
        private string GetFinancialAccountDescription(FinancialAccount financialAccount)
        {
            StringBuilder description = new StringBuilder();
            description.Append(s_financialAccountDefinitionManager.GetFinancialAccountDefinitionName(financialAccount.FinancialAccountDefinitionId));
            description.Append(" ");
            if(financialAccount.BED > DateTime.Now)
            {
                description.Append("Future");
            }
            else if (financialAccount.EED.HasValue && financialAccount.EED.Value < DateTime.Now)
            {
                var dateTimeFormat = Utilities.GetDateTimeFormat(DateTimeType.Date);
                description.AppendFormat("{0} -> {1}", financialAccount.BED.ToString(dateTimeFormat), financialAccount.EED.Value.ToString(dateTimeFormat));
            }
           return description.ToString();
        }

        private bool UpdateAccountStatus(Guid accountBEDefinitionId, long accountId, FinancialAccount financialAccount)
        {
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.InActive;
            if(s_accountManager.IsAccountActive(accountBEDefinitionId, accountId))
                vrAccountStatus = VRAccountStatus.Active;
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            var financialAccountId = GetFinancialAccountId(accountId, financialAccount.SequenceNumber);
            var result = false;
            if (financialAccountDefinitionSettings.InvoiceTypeId.HasValue)
            {
              result= invoiceAccountManager.TryUpdateInvoiceAccountStatus(financialAccountDefinitionSettings.InvoiceTypeId.Value, financialAccountId,  vrAccountStatus, false);
              
            }
            if (financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
            {
               result = new LiveBalanceManager().TryUpdateLiveBalanceStatus(financialAccountId, financialAccountDefinitionSettings.BalanceAccountTypeId.Value ,vrAccountStatus, false);
            }

            return result;
        }

        private bool UpdateAccountEffectiveDate(Guid accountBEDefinitionId, long accountId, FinancialAccount financialAccount, out string errorMessage)
        {
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            var financialAccountId = GetFinancialAccountId(accountId, financialAccount.SequenceNumber);
            var result = false;
            errorMessage = null;
            if (financialAccountDefinitionSettings.InvoiceTypeId.HasValue)
            {
                result = invoiceAccountManager.TryUpdateInvoiceAccountEffectiveDate(financialAccountDefinitionSettings.InvoiceTypeId.Value, financialAccountId, financialAccount.BED, financialAccount.EED, out errorMessage);
            }
            if (result && financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
            {
                result = new LiveBalanceManager().TryUpdateLiveBalanceEffectiveDate(financialAccountId, financialAccountDefinitionSettings.BalanceAccountTypeId.Value, financialAccount.BED, financialAccount.EED);
            }

            return result;
        }
        #endregion

        #region Private Classes

        public class FinancialAccountFillExtraDataContext : IFinancialAccountFillExtraDataContext
        {
            public FinancialAccountData FinancialAccountData
            {
                get;
                set;
            }
        }


        #endregion
    }
}

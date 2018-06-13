using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Notification.Business;
using Vanrise.AccountBalance.Business.Extensions;
using Retail.BusinessEntity.APIEntities;
using Vanrise.Invoice.Business;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountManager
    {
        static AccountBEManager s_accountManager = new AccountBEManager();
        static AccountBEDefinitionManager s_accountBEDefinitionManager = new AccountBEDefinitionManager();
        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
        static InvoiceManager s_invoiceManager = new InvoiceManager();
        static InvoiceAccountManager s_invoiceAccountManager = new InvoiceAccountManager();
        static LiveBalanceManager s_liveBalanceManager = new LiveBalanceManager();


        #region Main Methods
        public IDataRetrievalResult<FinancialAccountDetail> GetFilteredFinancialAccounts(DataRetrievalInput<FinancialAccountQuery> input)
        {
            var cachedFinancialAccounts = GetFinancialAccounts(input.Query.AccountBEDefinitionId, input.Query.AccountId, false);
            Func<FinancialAccountData, bool> filterExpression = (financialAccountData) =>
            {
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, cachedFinancialAccounts.ToBigResult(input, filterExpression, FinancialAccountDetailMapper));
        }

        public IEnumerable<FinancialAccountInfo> GetFinancialAccountsInfo(Guid accountBEDefinitionId, FinancialAccountInfoFilter filter)
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

                        if (filter.Filters != null)
                        {
                            FinancialAccountFilterContext context = new FinancialAccountFilterContext
                            {
                                AccountBEDefinitionId = accountBEDefinitionId,
                                AccountId = accountId,
                                FinancialAccountId = financialAccountData.FinancialAccountId
                            };
                            if (!filter.Filters.Any(y => y.IsMatched(context)))
                                return false;
                        }
                        return true;
                    };
                    financialAccountsInfo.AddRange(cachedFinancialAccounts.MapRecords(FinancialAccountInfoMapper, filterExpression));

                }
            }
            return financialAccountsInfo;
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
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(financialAccountToInsert.AccountBEDefinitionId);

                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccountToInsert.FinancialAccount);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            else
            {
                insertOperationOutput.Message = message;
            }
            return insertOperationOutput;
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

                if (TryUpdateFinancialAccount(financialAccountToEdit, out errorrMessage))
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(financialAccountToEdit.AccountBEDefinitionId);

                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(financialAccountToEdit.FinancialAccount);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }

            }
            if (errorrMessage != null)
            {
                updateOperationOutput.Message = errorrMessage;
                updateOperationOutput.ShowExactMessage = true;
            }
            return updateOperationOutput;
        }

        #endregion

        #region Public Methods

        public bool CheckAllowAddFinancialAccounts(Guid accountDefinitionId, long accountId)
        {
            var accountBEManager = new AccountBEManager();
            var accountClassifications = accountBEManager.GetAccountClassifications(accountDefinitionId, accountId);
            var financialAccountsData = GetFinancialAccountsWithInheritedAndChildsByAccount(accountDefinitionId, accountId);

            if (accountClassifications != null && accountClassifications.Count > 0)
            {
                foreach (var classification in accountClassifications)
                {
                    var financialAccounts = financialAccountsData.GetRecord(classification);
                    if (financialAccounts == null || financialAccounts.Count() == 0 || financialAccounts.All(x => x.FinancialAccount.EED.HasValue))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        public IEnumerable<VRTaxItemDetail> GetFinancialAccountTaxItemDetails(Guid invoiceTypeId, Guid accountDefinitionId, string financialAccountId)
        {
            var financialAccountData = GetFinancialAccountData(accountDefinitionId, financialAccountId);
            return s_accountManager.GetTaxItemDetails(invoiceTypeId, accountDefinitionId, financialAccountData.Account.AccountId);
        }
        public FinancialAccountRuntimeData GetAccountFinancialInfo(Guid accountDefinitionId, long accountId, DateTime effectiveOn, string classification)
        {
            var financialAccountLocatorContext = new Retail.BusinessEntity.Business.FinancialAccountLocatorContext();
            financialAccountLocatorContext.AccountDefinitionId = accountDefinitionId;
            financialAccountLocatorContext.AccountId = accountId;
            financialAccountLocatorContext.EffectiveOn = effectiveOn;
            financialAccountLocatorContext.Classification = classification;

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

        public bool TryGetBalanceAccountCreditLimit(Guid accountTypeId, string balanceAccountId, out Guid accountBEDefinitionId, out long accountId, out FinancialAccountData financialAccountData, out Decimal creditLimit, out int currencyId)
        {
            ResolveBalanceAccountId(accountTypeId, balanceAccountId, out accountBEDefinitionId, out accountId, out financialAccountData);
            if (financialAccountData != null)
            {
                if (financialAccountData.CreditLimit.HasValue)
                {
                    creditLimit = financialAccountData.CreditLimit.Value;
                    currencyId = financialAccountData.CreditLimitCurrencyId.Value;
                    return true;
                }
            }
            else
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                IAccountPayment accountPayment;

                if (!accountBEManager.HasAccountPayment(accountBEDefinitionId, accountId, false, out accountPayment))
                    throw new NullReferenceException(String.Format("accountPayment. Account '{0}'", accountId));

                ProductManager productManager = new ProductManager();
                var product = productManager.GetProduct(accountPayment.ProductId);
                product.ThrowIfNull("product", accountPayment.ProductId);
                product.Settings.ThrowIfNull("product.Settings", accountPayment.ProductId);

                IPostpaidProductSettings postpaidProductSettings = product.Settings.ExtendedSettings as IPostpaidProductSettings;
                if (postpaidProductSettings != null && postpaidProductSettings.CreditLimit.HasValue)
                {
                    creditLimit = postpaidProductSettings.CreditLimit.Value;
                    currencyId = product.Settings.PricingCurrencyId;
                    return true;
                }
            }
            creditLimit = default(Decimal);
            currencyId = default(int);
            return false;
        }


        public IEnumerable<long> GetAccountIdsByFinancialAccountIds(Guid accountBEDefinitionId, List<string> financialAccountIds)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.MapRecords(x => x.Value.Account.AccountId, y => financialAccountIds.Contains(y.Key));
        }

        public void ReflectStatusToInvoiceAndBalanceAccounts(Guid accountBEDefinitionId, VRAccountStatus vrAccountStatus, IEnumerable<FinancialAccountData> financialAccounts,
VRAccountStatus vrInvoiceAccountStatus, VRAccountStatus vrBalanceAccountStatus)
        {
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    Guid? balanceAccountTypeId = financialAccount.BalanceAccountTypeId;

                    if (balanceAccountTypeId.HasValue)
                        s_liveBalanceManager.TryUpdateLiveBalanceStatus(financialAccount.FinancialAccountId, balanceAccountTypeId.Value, vrBalanceAccountStatus, false);

                    DateTime? eedToSet = null;
                    if (financialAccount.InvoiceTypeIds != null)
                    {
                        foreach (var invoiceTypeId in financialAccount.InvoiceTypeIds)
                        {
                            s_invoiceAccountManager.TryUpdateInvoiceAccountStatus(invoiceTypeId, financialAccount.FinancialAccountId, vrInvoiceAccountStatus, false);

                            if (vrAccountStatus == VRAccountStatus.InActive)
                            {
                                var lastInvoiceToDate = s_invoiceManager.GetLastInvoiceToDate(invoiceTypeId, financialAccount.FinancialAccountId);
                                if (lastInvoiceToDate.HasValue && (!eedToSet.HasValue || eedToSet.Value < lastInvoiceToDate.Value))
                                    eedToSet = lastInvoiceToDate.Value.AddDays(1).Date;
                            }
                        }
                    }
                    if (vrAccountStatus == VRAccountStatus.InActive)
                    {
                        CloseFinancialAccount(accountBEDefinitionId, vrAccountStatus, financialAccount, eedToSet, balanceAccountTypeId);
                    }
                }
            }
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

        public Guid GetAccountBEDefinitionIdByBalanceAccountTypeId(Guid accountTypeId)
        {
            var retailAccountBalanceSetting = GetSubscriberAccountBalanceSetting(accountTypeId);
            return retailAccountBalanceSetting.AccountBEDefinitionId;
        }

        public FinancialAccountData GetFinancialAccountData(Guid accountBEDefinitionId, string financialAccountId)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.GetRecord(financialAccountId);
        }

        public string GetFinancialAccountId(long accountId, int sequenceNumber)
        {
            return string.Format("{0}_{1}", accountId, sequenceNumber);
        }

        public Guid GetAccountBEDefinitionIdByInvoiceTypeId(Guid invoiceTypeId)
        {
            return GetRetailInvoiceSetting(invoiceTypeId).AccountBEDefinitionId;
        }

        public IEnumerable<string> GetAllFinancialAccountsIds(Guid accountBEDefinitionId)
        {
            var cachedFinancialAccountsData = GetCachedFinancialAccountDataByFinancialAccountId(accountBEDefinitionId);
            return cachedFinancialAccountsData.Keys;
        }

        public SubscriberAccountBalanceSetting GetSubscriberAccountBalanceSetting(Guid accountTypeId)
        {
            Vanrise.AccountBalance.Business.AccountTypeManager balanceAccountTypeManager = new Vanrise.AccountBalance.Business.AccountTypeManager();
            Vanrise.AccountBalance.Entities.AccountTypeSettings accountTypeSettings = balanceAccountTypeManager.GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ExtendedSettings.CastWithValidate<SubscriberAccountBalanceSetting>("accountTypeSettings.ExtendedSettings");
        }

        public BaseRetailInvoiceTypeSettings GetRetailInvoiceSetting(Guid invoiceTypeId)
        {
            var invoiceTypeManager = new Vanrise.Invoice.Business.InvoiceTypeManager();
            var invoiceTypeExtendedSettings = invoiceTypeManager.GetInvoiceTypeExtendedSettings(invoiceTypeId);
            return invoiceTypeExtendedSettings.CastWithValidate<BaseRetailInvoiceTypeSettings>("invoiceTypeExtendedSettings");
        }

        public bool TryGetFinancialAccount(Guid accountDefinitionId, long accountId, bool withInherited, DateTime effectiveOn, string classification, out FinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<FinancialAccountData> financialAccounts = GetFinancialAccounts(accountDefinitionId, accountId, withInherited, classification);// shouldbe for for one classification
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

        public IOrderedEnumerable<FinancialAccountData> GetFinancialAccountsWithChildren(Guid accountDefinitionId, long accountId)
        {
            return GetCachedFinancialAccountsWithChildren(accountDefinitionId).GetRecord(accountId);
        }

        public IOrderedEnumerable<FinancialAccountData> GetFinancialAccounts(Guid accountDefinitionId, long accountId, bool withInherited)
        {
            var cachedFinancialAccounts = withInherited ? GetCachedFinancialAccountsWithInherited(accountDefinitionId) : GetCachedFinancialAccounts(accountDefinitionId);
            return cachedFinancialAccounts.GetRecord(accountId);
        }

        public IOrderedEnumerable<FinancialAccountData> GetFinancialAccounts(Guid accountDefinitionId, long accountId, bool withInherited, string classification)
        {
            var cachedFinancialAccounts = withInherited ? GetCachedFinancialAccountsWithInheritedByClassification(accountDefinitionId, classification) : GetCachedFinancialAccountsByClassification(accountDefinitionId, classification);
            return cachedFinancialAccounts.GetRecord(accountId);
        }

        public void AddFinancialAccountToExtSettings(FinancialAccount financialAccount, AccountBEFinancialAccountsSettings accountBEFinancialAccountsSettings)
        {
            accountBEFinancialAccountsSettings.LastTakenSequenceNumber++;
            financialAccount.SequenceNumber = accountBEFinancialAccountsSettings.LastTakenSequenceNumber;
            accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccount);
        }
       
        public AccountBEFinancialAccountsSettings CreateAccountFinancialAccountExtSettings()
        {
            var accountBEFinancialAccountsSettings = new AccountBEFinancialAccountsSettings();
            accountBEFinancialAccountsSettings.FinancialAccounts = new List<FinancialAccount>();
            return accountBEFinancialAccountsSettings;
        }
    
        public FinancialAccountLocator GetFinancialAccountLocator(Guid accountDefinitionId, long accountId, DateTime effectiveOn)
        {
            var accountBEDefinitionSettings = s_accountBEDefinitionManager.GetAccountBEDefinitionSettings(accountDefinitionId);
            accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings", accountDefinitionId);
            return accountBEDefinitionSettings.FinancialAccountLocator != null ? accountBEDefinitionSettings.FinancialAccountLocator : new DefaultFinancialAccountLocator();
        }

        public Guid GetBalanceAccountTypeIdByAlertRuleTypeId(Guid alertRuleTypeId)
        {
            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
            AccountBalanceAlertRuleTypeSettings balanceRuleTypeSettings = alertRuleTypeManager.GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(alertRuleTypeId);
            balanceRuleTypeSettings.ThrowIfNull("balanceRuleTypeSettings", alertRuleTypeId);

            return balanceRuleTypeSettings.AccountTypeId;
        }

        public Dictionary<string, IOrderedEnumerable<FinancialAccountData>> GetFinancialAccountsWithInheritedAndChildsByAccount(Guid accountDefinitionId, long accountId)
        {
            return GetFinancialAccountsWithInheritedAndChilds(accountDefinitionId).GetRecord(accountId);
        }

        #endregion

        #region Resolving Methods
     
        public void ResolveInvoiceAccountId(Guid invoiceTypeId, string invoiceAccountId, out Guid accountBEDefinitionId, out long accountId, out FinancialAccountData financialAccountData)
        {
            accountBEDefinitionId = GetAccountBEDefinitionIdByInvoiceTypeId(invoiceTypeId);
            financialAccountData = GetFinancialAccountData(accountBEDefinitionId, invoiceAccountId);
            if (financialAccountData != null)
            {
                accountId = financialAccountData.Account.AccountId;
            }
            else
            {
                accountId = invoiceAccountId.TryParseWithValidate<long>(long.TryParse);
            }
        }
   
        public void ResolveBalanceAccountId(Guid balanceAccountTypeId, string balanceAccountId, out Guid accountBEDefinitionId, out long accountId, out FinancialAccountData financialAccountData)
        {
            accountBEDefinitionId = GetAccountBEDefinitionIdByBalanceAccountTypeId(balanceAccountTypeId);
            financialAccountData = GetFinancialAccountData(accountBEDefinitionId, balanceAccountId);
            if (financialAccountData != null)
            {
                accountId = financialAccountData.Account.AccountId;
            }
            else
            {
                accountId = balanceAccountId.TryParseWithValidate<long>(long.TryParse);
            }
        }

        #endregion

        #region Private Methods

        internal bool TryUpdateFinancialAccount(FinancialAccountToEdit financialAccountToEdit, out string errorrMessage)
        {
            var accountBEFinancialAccountsSettings = GetAccountBEFinancialAccountsSettings(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId);
            var financialAccount = accountBEFinancialAccountsSettings.FinancialAccounts.FindRecord(x => x.SequenceNumber == financialAccountToEdit.FinancialAccount.SequenceNumber);
            accountBEFinancialAccountsSettings.FinancialAccounts.Remove(financialAccount);
            accountBEFinancialAccountsSettings.FinancialAccounts.Add(financialAccountToEdit.FinancialAccount);
            if (UpdateAccountEffectiveDate(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, financialAccountToEdit.FinancialAccount, out errorrMessage))
            {
                return s_accountManager.UpdateAccountExtendedSetting(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, accountBEFinancialAccountsSettings);
            }
            return false;
        }

        private AccountBEFinancialAccountsSettings GetAccountBEFinancialAccountsSettings(Guid accountBEDefinitionId, long accountId)
        {
            var accountBEFinancialAccountsSettings = s_accountManager.GetExtendedSettings<AccountBEFinancialAccountsSettings>(accountBEDefinitionId, accountId);
            if (accountBEFinancialAccountsSettings == null)
            {
                accountBEFinancialAccountsSettings = CreateAccountFinancialAccountExtSettings();
            }
            return accountBEFinancialAccountsSettings;
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
            if (financialAccount.BED > DateTime.Now)
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

        private bool UpdateAccountEffectiveDate(Guid accountBEDefinitionId, long accountId, FinancialAccount financialAccount, out string errorMessage)
        {
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            var financialAccountId = GetFinancialAccountId(accountId, financialAccount.SequenceNumber);
            var result = false;
            errorMessage = null;
            if (financialAccountDefinitionSettings.InvoiceTypes != null)
            {
                foreach (var invoiceType in financialAccountDefinitionSettings.InvoiceTypes)
                {
                    result = invoiceAccountManager.TryUpdateInvoiceAccountEffectiveDate(invoiceType.InvoiceTypeId, financialAccountId, financialAccount.BED, financialAccount.EED, out errorMessage);
                    if (!result)
                        return result;
                }
            }else
            {
                result = true;
            }
            if (result && financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
            {
                result = new LiveBalanceManager().TryUpdateLiveBalanceEffectiveDate(financialAccountId, financialAccountDefinitionSettings.BalanceAccountTypeId.Value, financialAccount.BED, financialAccount.EED);
            }

            return result;
        }
     
        private void CloseFinancialAccount(Guid accountBEDefinitionId, VRAccountStatus vrAccountStatus, FinancialAccountData financialAccount, DateTime? eedToSet, Guid? balanceAccountTypeId)
        {
            if (!financialAccount.FinancialAccount.EED.HasValue || financialAccount.FinancialAccount.EED.Value > DateTime.Today)
            {
                if (!eedToSet.HasValue && balanceAccountTypeId.HasValue)
                {
                    var lastTransactionDate = new BillingTransactionManager().GetLastTransactionDate(balanceAccountTypeId.Value, financialAccount.FinancialAccountId);
                    if (lastTransactionDate.HasValue)
                        eedToSet = lastTransactionDate.Value;
                }
                if (!eedToSet.HasValue)
                    eedToSet = DateTime.Today;
                if (financialAccount.FinancialAccount.BED > eedToSet)
                    eedToSet = financialAccount.FinancialAccount.BED;
                var financialAccountToEdit = new FinancialAccountToEdit
                {
                    AccountBEDefinitionId = accountBEDefinitionId,
                    AccountId = financialAccount.Account.AccountId,
                    FinancialAccount = new FinancialAccount
                    {
                        SequenceNumber = financialAccount.FinancialAccount.SequenceNumber,
                        ExtendedSettings = financialAccount.FinancialAccount.ExtendedSettings,
                        BED = financialAccount.FinancialAccount.BED,
                        EED = eedToSet,
                        FinancialAccountDefinitionId = financialAccount.FinancialAccount.FinancialAccountDefinitionId
                    }
                };
                string errorMessage = null;
                if (UpdateAccountEffectiveDate(financialAccountToEdit.AccountBEDefinitionId, financialAccountToEdit.AccountId, financialAccountToEdit.FinancialAccount, out  errorMessage))
                    TryUpdateFinancialAccount(financialAccountToEdit, out errorMessage);
            }
        }



        #endregion

        #region Validation Methods

        private bool CheckFinancialAccountOverlaping(Guid accountDefinitionId, long accountId, FinancialAccount mainFinancialAccount, out string message)
        {
            message = null;
            var financialAccountDefinition = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(mainFinancialAccount.FinancialAccountDefinitionId);
            var financialAccountClassifications = financialAccountDefinition.ApplicableClassifications;
            var financialAccountsData = GetFinancialAccountsWithInheritedAndChildsByAccount(accountDefinitionId, accountId);
            bool result = true;
            foreach (var classification in financialAccountDefinition.ApplicableClassifications)
            {
                var financialAccounts = financialAccountsData.GetRecord(classification);
                if (financialAccounts != null)
                {
                    CheckFinancialAccountOverlaping(mainFinancialAccount, financialAccounts, out message, out result);
                    if (!result)
                    {
                        return result;
                    }
                }
            }
            return result;
        }


        private void CheckFinancialAccountOverlaping(FinancialAccount mainFinancialAccount, IOrderedEnumerable<FinancialAccountData> financialAccounts, out string message, out bool result)
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

        #region Cached Methods

        private Dictionary<string, FinancialAccountData> GetCachedFinancialAccountDataByFinancialAccountId(Guid accountDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccountDataByFinancialAccountId", accountDefinitionId,
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

        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccountsWithChildren(Guid accountDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccountsWithChildren", accountDefinitionId,
                () =>
                {
                    Dictionary<long, List<FinancialAccountData>> financialAccountsDataWithChildren = new Dictionary<long, List<FinancialAccountData>>();
                    var cachedDirectFinancialAccounts = GetCachedFinancialAccounts(accountDefinitionId);
                    var allAccountNodes = s_accountManager.GetCacheAccountTreeNodes(accountDefinitionId);
                    if (cachedDirectFinancialAccounts != null)
                    {
                        allAccountNodes.ThrowIfNull("allAccountNodes");
                        foreach (var accountFinancialAccountData in cachedDirectFinancialAccounts)
                        {
                            long accountId = accountFinancialAccountData.Key;
                            IEnumerable<FinancialAccountData> financialAccounts = accountFinancialAccountData.Value;
                            financialAccountsDataWithChildren.GetOrCreateItem(accountId).AddRange(financialAccounts);
                            var accountNode = allAccountNodes.GetRecord(accountId);
                            accountNode.ThrowIfNull("accountNode", accountId);
                            var parentAccountNode = accountNode.ParentNode;
                            while (parentAccountNode != null)
                            {
                                financialAccountsDataWithChildren.GetOrCreateItem(parentAccountNode.Account.AccountId).AddRange(financialAccounts);
                                parentAccountNode = parentAccountNode.ParentNode;
                            }
                        }
                    }
                    return financialAccountsDataWithChildren.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(faItm => faItm.FinancialAccount.EED.HasValue ? faItm.FinancialAccount.EED.Value : DateTime.MaxValue));
                });
        }

        private Dictionary<long, Dictionary<string, IOrderedEnumerable<FinancialAccountData>>> GetFinancialAccountsWithInheritedAndChilds(Guid accountDefinitionId)
        {
             return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetFinancialAccountsWithInheritedAndChilds", accountDefinitionId,
                () =>
                {
                    var allFinancialAccountsData = GetCachedFinancialAccountsWithInherited(accountDefinitionId);
                    var financialAccountsChilds = GetCachedFinancialAccountsWithChildren(accountDefinitionId);
                    
                    Dictionary<long,Dictionary<string, List<FinancialAccountData>>>financialAccountDataDic = new Dictionary<long,Dictionary<string, List<FinancialAccountData>>>();
                    
                    AddfinancialAccountDataToDictionary(financialAccountDataDic, allFinancialAccountsData);
                    
                    AddfinancialAccountDataToDictionary(financialAccountDataDic, financialAccountsChilds);
                    
                    return financialAccountDataDic.ToDictionary(x=>x.Key, x=> x.Value.ToDictionary(y=>y.Key,y=>y.Value.OrderByDescending(faItm => faItm.FinancialAccount.EED.HasValue ? faItm.FinancialAccount.EED.Value : DateTime.MaxValue)));
                });
        }
       
        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccounts(Guid accountDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccounts", accountDefinitionId,
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

        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccountsByClassification(Guid accountDefinitionId, string classification)
        {
            string cacheKey = String.Concat("GetCachedFinancialAccountsByClassification_", classification);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheKey, accountDefinitionId,
                () =>
                {
                    Dictionary<long, IOrderedEnumerable<FinancialAccountData>> allFinancialAccountsData = GetCachedFinancialAccounts(accountDefinitionId);
                    Dictionary<long, IOrderedEnumerable<FinancialAccountData>> neededFinancialAccounts = new Dictionary<long, IOrderedEnumerable<FinancialAccountData>>();
                    foreach(var entry in allFinancialAccountsData)
                    {
                        List<FinancialAccountData> entryNeededFinAccounts = null;
                        foreach(var financialAccountData in entry.Value)
                        {
                            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                            financialAccountDefinitionSettings.ApplicableClassifications.ThrowIfNull("financialAccountDefinitionSettings.ApplicableClassifications", financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                            if(financialAccountDefinitionSettings.ApplicableClassifications.Contains(classification))
                            {
                                if (entryNeededFinAccounts == null)
                                    entryNeededFinAccounts = new List<FinancialAccountData>();
                                entryNeededFinAccounts.Add(financialAccountData);
                            }
                        }
                        if(entryNeededFinAccounts != null)
                            neededFinancialAccounts.Add(entry.Key, entryNeededFinAccounts.OrderByDescending(itm => itm.FinancialAccount.EED.HasValue ? itm.FinancialAccount.EED.Value : DateTime.MaxValue));
                    }
                    return neededFinancialAccounts;
                });
        }

        private void AddfinancialAccountDataToDictionary(Dictionary<long, Dictionary<string, List<FinancialAccountData>>> financialAccountDataDic, Dictionary<long, IOrderedEnumerable<FinancialAccountData>> financialAccountData)
        {
            if (financialAccountData != null)
            {
                foreach (var account in financialAccountData)
                {

                    var financialAccountsByClassification = financialAccountDataDic.GetOrCreateItem(account.Key);
                    if (account.Value != null)
                    {
                        foreach (var financialAccount in account.Value)
                        {
                            var financialAccountDefinition = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccount.FinancialAccountDefinitionId);
                            financialAccountDefinition.ThrowIfNull("financialAccountDefinition");
                            financialAccountDefinition.ApplicableClassifications.ThrowIfNull("financialAccountDefinition.ApplicableClassifications");
                            foreach (var classification in financialAccountDefinition.ApplicableClassifications)
                            {
                                var financialAccounts = financialAccountsByClassification.GetOrCreateItem(classification);
                                financialAccounts.Add(financialAccount);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccountsWithInherited(Guid accountDefinitionId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccountsWithInherited", accountDefinitionId,
                () =>
                {
                    Dictionary<long, List<FinancialAccountData>> allFinancialAccountsData = new Dictionary<long, List<FinancialAccountData>>();
                    var allAccountNodes = s_accountManager.GetCacheAccountTreeNodes(accountDefinitionId);
                    if (allAccountNodes != null)
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

        private Dictionary<long, IOrderedEnumerable<FinancialAccountData>> GetCachedFinancialAccountsWithInheritedByClassification(Guid accountDefinitionId, string classification)
        {
            string cacheKey = String.Concat("GetCachedFinancialAccountsWithInheritedByClassification_", classification);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheKey, accountDefinitionId,
                () =>
                {
                    Dictionary<long, IOrderedEnumerable<FinancialAccountData>> allFinancialAccountsData = GetCachedFinancialAccountsWithInherited(accountDefinitionId);
                    Dictionary<long, IOrderedEnumerable<FinancialAccountData>> neededFinancialAccounts = new Dictionary<long, IOrderedEnumerable<FinancialAccountData>>();
                    foreach (var entry in allFinancialAccountsData)
                    {
                        List<FinancialAccountData> entryNeededFinAccounts = null;
                        foreach (var financialAccountData in entry.Value)
                        {
                            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                            financialAccountDefinitionSettings.ApplicableClassifications.ThrowIfNull("financialAccountDefinitionSettings.ApplicableClassifications", financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                            if (financialAccountDefinitionSettings.ApplicableClassifications.Contains(classification))
                            {
                                if (entryNeededFinAccounts == null)
                                    entryNeededFinAccounts = new List<FinancialAccountData>();
                                entryNeededFinAccounts.Add(financialAccountData);
                            }
                        }
                        if (entryNeededFinAccounts != null)
                            neededFinancialAccounts.Add(entry.Key, entryNeededFinAccounts.OrderByDescending(itm => itm.FinancialAccount.EED.HasValue ? itm.FinancialAccount.EED.Value : DateTime.MaxValue));
                    }
                    return neededFinancialAccounts;
                });
        }

        private void AddAccountNodeFinancialAccounts(AccountTreeNode accountNode, Dictionary<long, List<FinancialAccountData>> allFinancialAccountsData)
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
                                BalanceAccountTypeId = itm.BalanceAccountTypeId,
                                InvoiceTypeIds = itm.InvoiceTypeIds,
                                IsInherited = true
                            }).ToList());
                    }
                    AddAccountNodeFinancialAccounts(childNode, allFinancialAccountsData);
                }
            }
        }

        private FinancialAccountData CreateFinancialAccountData(FinancialAccount financialAccount, Account account)
        {
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            var financialAccountData = new FinancialAccountData
            {
                FinancialAccountId = GetFinancialAccountId(account.AccountId, financialAccount.SequenceNumber),
                FinancialAccount = financialAccount,
                Account = account,
                BalanceAccountTypeId = financialAccountDefinitionSettings.BalanceAccountTypeId,
                InvoiceTypeIds = financialAccountDefinitionSettings.InvoiceTypes != null ? financialAccountDefinitionSettings.InvoiceTypes.MapRecords(x => x.InvoiceTypeId).ToList() : null
            };

            financialAccount.ExtendedSettings.ThrowIfNull("financialAccount.ExtendedSettings", financialAccountData.FinancialAccountId);
            var fillExtraDataContext = new FinancialAccountFillExtraDataContext
            {
                FinancialAccountData = financialAccountData
            };
            financialAccount.ExtendedSettings.FillExtraData(fillExtraDataContext);
            return financialAccountData;
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
     
        private class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        {
            object _updateHandle;
            DateTime? _accountBECacheLastCheck;

            DateTime? _creditClassCacheLastCheck;

            protected override bool ShouldSetCacheExpired(Guid accountBEDefinitionId)
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<AccountBEManager.CacheManager>().IsCacheExpired(accountBEDefinitionId, ref _accountBECacheLastCheck)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<CreditClassManager.CacheManager>().IsCacheExpired(ref _creditClassCacheLastCheck);
            }
        }

        #endregion

        #region Mappers
     
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
                Description = string.Format("{0} ({1})", s_accountManager.GetAccountName(financialAccountData.Account), GetFinancialAccountDescription(financialAccountData.FinancialAccount)),
                IsEffectiveAndActive = IsFinancialAccountEffectiveAndActive(financialAccountData.FinancialAccount)
            };
        }
     
        #endregion

        #region Client API
    
        public List<ClientInvoiceAccountInfo> GetClientInvoiceAccounts(Guid invoiceTypeId, long accountId)
        {
            Guid accountBEDefinitionId = GetAccountBEDefinitionIdByInvoiceTypeId(invoiceTypeId);
            List<ClientInvoiceAccountInfo> invoiceAccounts = new List<ClientInvoiceAccountInfo>();
            if (s_accountBEDefinitionManager.IsFinancialAccountModuleUsed(accountBEDefinitionId))
            {
                var financialAccounts = GetFinancialAccountsWithChildren(accountBEDefinitionId, accountId);
                if (financialAccounts != null)
                {

                    foreach (var financialAccount in financialAccounts)
                    {
                        var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccount.FinancialAccountDefinitionId);
                        if (financialAccountDefinitionSettings.InvoiceTypes != null)
                        {
                            foreach (var invoiceType in financialAccountDefinitionSettings.InvoiceTypes)
                            {
                                invoiceAccounts.Add(new ClientInvoiceAccountInfo
                                {
                                    InvoiceAccountId = financialAccount.FinancialAccountId,
                                    Name = s_accountManager.GetAccountName(financialAccount.Account)
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                invoiceAccounts.Add(new ClientInvoiceAccountInfo
                {
                    InvoiceAccountId = accountId.ToString(),
                    Name = s_accountManager.GetAccountName(accountBEDefinitionId, accountId)
                });
            }
            return invoiceAccounts;
        }
    
        public List<ClientBalanceAccountInfo> GetClientBalanceAccounts(Guid balanceAccountTypeId, long accountId)
        {
            Guid accountBEDefinitionId = GetAccountBEDefinitionIdByBalanceAccountTypeId(balanceAccountTypeId);
            List<ClientBalanceAccountInfo> balanceAccounts = new List<ClientBalanceAccountInfo>();
            if (s_accountBEDefinitionManager.IsFinancialAccountModuleUsed(accountBEDefinitionId))
            {
                var financialAccounts = GetFinancialAccountsWithChildren(accountBEDefinitionId, accountId);
                if (financialAccounts != null)
                {

                    foreach (var financialAccount in financialAccounts)
                    {
                        var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccount.FinancialAccountDefinitionId);
                        if (financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
                        {
                            balanceAccounts.Add(new ClientBalanceAccountInfo
                            {
                                BalanceAccountId = financialAccount.FinancialAccountId,
                                Name = s_accountManager.GetAccountName(financialAccount.Account)
                            });
                        }

                    }
                }
            }
            else
            {
                balanceAccounts.Add(new ClientBalanceAccountInfo
                {
                    BalanceAccountId = accountId.ToString(),
                    Name = s_accountManager.GetAccountName(accountBEDefinitionId, accountId)
                });
            }
            return balanceAccounts;
        }
    
        #endregion

    }
}

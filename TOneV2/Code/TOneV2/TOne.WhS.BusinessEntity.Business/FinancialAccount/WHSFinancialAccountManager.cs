﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class WHSFinancialAccountManager : IBusinessEntityManager
    {
        #region Fields

        static CarrierProfileManager s_carrierProfileManager = new CarrierProfileManager();
        static CarrierAccountManager s_carrierAccountManager = new CarrierAccountManager();
        static WHSFinancialAccountDefinitionManager s_financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
        static LiveBalanceManager s_liveBalanceManager = new LiveBalanceManager();
        static InvoiceManager s_invoiceManager = new InvoiceManager();
        static InvoiceAccountManager s_invoiceAccountManager = new InvoiceAccountManager();

        #endregion

        #region Public Methods

        public IDataRetrievalResult<WHSFinancialAccountDetail> GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<WHSFinancialAccountQuery> input)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();

            Func<WHSFinancialAccount, bool> filterExpression = (prod) =>
            {
                if (input.Query.CarrierAccountId.HasValue)
                {
                    if (!prod.CarrierAccountId.HasValue)
                        return false;
                    if (input.Query.CarrierAccountId.Value != prod.CarrierAccountId.Value)
                        return false;
                }
                if (input.Query.CarrierProfileId.HasValue)
                {
                    if (!prod.CarrierProfileId.HasValue)
                        return false;
                    if (input.Query.CarrierProfileId.Value != prod.CarrierProfileId.Value)
                        return false;
                }
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allFinancialAccounts.ToBigResult(input, filterExpression, FinancialAccountDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<WHSFinancialAccountDetail> AddFinancialAccount(WHSFinancialAccountToAdd financialAccountToAdd)
        {
            Vanrise.Entities.InsertOperationOutput<WHSFinancialAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<WHSFinancialAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            string message;
            if (CanAddFinancialAccount(financialAccountToAdd.FinancialAccount, out message))
            {
                int financialAccountId;

                IWHSFinancialAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IWHSFinancialAccountDataManager>();
                bool insertActionSucc = dataManager.Insert(financialAccountToAdd.FinancialAccount, out financialAccountId);
                if (insertActionSucc)
                {
                    if (financialAccountToAdd.InvoiceSettingId.HasValue)
                    {
                        LinkPartnerToInvoiceSetting(Guid.NewGuid(), financialAccountToAdd.InvoiceSettingId.Value, financialAccountId.ToString());
                    }
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    financialAccountToAdd.FinancialAccount.FinancialAccountId = financialAccountId;
                    insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccountToAdd.FinancialAccount);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            insertOperationOutput.Message = message;
            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<WHSFinancialAccountDetail> UpdateFinancialAccount(WHSFinancialAccountToEdit financialAccountToEdit)
        {

            Vanrise.Entities.UpdateOperationOutput<WHSFinancialAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<WHSFinancialAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            string message;

            if (CanUpdateFinancialAccount(financialAccountToEdit, out message))
            {
                if (TryReflectEffectiveDateToInvoiceAndBalanceAccounts(financialAccountToEdit, out message))
                {
                    IWHSFinancialAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IWHSFinancialAccountDataManager>();
                    bool updateActionSucc = dataManager.Update(financialAccountToEdit);
                    if (updateActionSucc)
                    {
                        if(financialAccountToEdit.InvoiceSettingId.HasValue)
                        {
                            Guid partnerInvoiceSettingId = financialAccountToEdit.PartnerInvoiceSettingId.HasValue ? financialAccountToEdit.PartnerInvoiceSettingId.Value : Guid.NewGuid();
                            LinkPartnerToInvoiceSetting(partnerInvoiceSettingId, financialAccountToEdit.InvoiceSettingId.Value, financialAccountToEdit.FinancialAccountId.ToString());
                        }
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                        updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(this.GetFinancialAccount(financialAccountToEdit.FinancialAccountId));
                    }
                    else
                    {
                        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                        updateOperationOutput.Message = "Account not exists";
                    }
                }
            }
            updateOperationOutput.Message = message;
            return updateOperationOutput;
        }

        public WHSFinancialAccount GetFinancialAccount(int financialAccountId)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();
            return allFinancialAccounts.GetRecord(financialAccountId);
        }
        public WHSFinancialAccountRuntimeEditor GetFinancialAccountRuntimeEditor(int financialAccountId)
        {
            var financialAccount =  GetFinancialAccount(financialAccountId);
            financialAccount.ThrowIfNull("financialAccount",financialAccountId);
             WHSFinancialAccountRuntimeEditor financialAccountRuntimeEditor = new WHSFinancialAccountRuntimeEditor{
                FinancialAccount = financialAccount
            };

            WHSFinancialAccountDefinitionManager financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var financialAccountDefinitionSetting = financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
           
            if(financialAccountDefinitionSetting.InvoiceTypeId.HasValue)
            {
                var partnerInvoiceSetting = new PartnerInvoiceSettingManager().GetPartnerInvoiceSettingByPartnerId(financialAccountId.ToString());
                if(partnerInvoiceSetting != null)
                {
                    financialAccountRuntimeEditor.InvoiceSettingId = partnerInvoiceSetting.InvoiceSettingID;
                    financialAccountRuntimeEditor.PartnerInvoiceSettingId = partnerInvoiceSetting.PartnerInvoiceSettingId;
                }
            }
            return financialAccountRuntimeEditor;
        }
        public WHSCarrierFinancialAccountData GetCustCarrierFinancialByFinAccId(int financialAccountId)
        {
            WHSCarrierFinancialAccountData carrierFinancialAccountData = GetCachedCustCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public WHSCarrierFinancialAccountData GetSuppCarrierFinancialByFinAccId(int financialAccountId)
        {
            WHSCarrierFinancialAccountData carrierFinancialAccountData = GetCachedSuppCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public bool TryGetCustAccFinancialAccountData(int customerAccountId, DateTime effectiveOn, out WHSCarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<WHSCarrierFinancialAccountData> carrierFinancialAccounts = GetCachedCustCarrierFinancialsByCarrAccId().GetRecord(customerAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.FinancialAccount.IsEffective(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }
        public bool TryGetSuppAccFinancialAccountData(int supplierAccountId, DateTime effectiveOn, out WHSCarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<WHSCarrierFinancialAccountData> carrierFinancialAccounts = GetCachedSuppCarrierFinancialsByCarrAccId().GetRecord(supplierAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.FinancialAccount.IsEffective(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }

        public IEnumerable<WHSFinancialAccountInfo> GetFinancialAccountsInfo(WHSFinancialAccountInfoFilter filter)
        {
            Dictionary<int, WHSFinancialAccount> allFinancialAccounts = GetCachedFinancialAccounts();

            Func<WHSFinancialAccount, bool> filterFunc = null;

            if (filter != null)
            {
                filterFunc = (financialAccount) =>
                {
                    if (filter.FinancialAccountDefinitionId.HasValue && financialAccount.FinancialAccountDefinitionId != filter.FinancialAccountDefinitionId.Value)
                            return false;
                    if(filter.InvoiceTypeId.HasValue)
                    {
                        var invoiceTypeId = s_financialAccountDefinitionManager.GetInvoiceTypeId(financialAccount.FinancialAccountDefinitionId);
                        if (invoiceTypeId != filter.InvoiceTypeId)
                            return false;
                    }
                    if (filter.BalanceAccountTypeId.HasValue)
                    {
                        var balanceAccountTypeId = s_financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
                        if (balanceAccountTypeId != filter.BalanceAccountTypeId)
                            return false;
                    }
                    if (filter.CarrierType.HasValue)
                    {
                        switch (filter.CarrierType.Value)
                        {
                            case WHSFinancialAccountCarrierType.Profile: if (!financialAccount.CarrierProfileId.HasValue) return false; break;
                            case WHSFinancialAccountCarrierType.Account: if (!financialAccount.CarrierAccountId.HasValue) return false; break;

                        }
                    }
                    if (filter.Status.HasValue)
                    {
                        switch (filter.Status.Value)
                        {
                            case VRAccountStatus.Active: if (!IsFinancialAccountActive(financialAccount)) return false; break;
                            case VRAccountStatus.InActive: if (IsFinancialAccountActive(financialAccount)) return false; break;
                        }
                    }
                    if (filter.EffectiveDate.HasValue && !financialAccount.IsEffective(filter.EffectiveDate.Value))
                        return false;
                    if (filter.IsEffectiveInFuture.HasValue && filter.IsEffectiveInFuture.Value)
                    {
                        DateTime today = DateTime.Today;
                        if (financialAccount.EED.HasValue && financialAccount.EED.Value < DateTime.Today)
                            return false;
                    }
                    return true;
                };
            }

            return allFinancialAccounts.MapRecords(FinancialAccountInfoMapper, filterFunc);
        }

        public IEnumerable<WHSFinancialAccount> GetFinancialAccountsByCarrierAccountId(int carrierAccountId)
        {
            var financialAccounts = GetCachedFinancialAccounts();
            return financialAccounts.Values.FindAllRecords(x => x.CarrierAccountId.HasValue && x.CarrierAccountId.Value == carrierAccountId);
        }

        public IEnumerable<WHSFinancialAccount> GetFinancialAccountsByCarrierProfileId(int carrierProfileId)
        {
            var financialAccounts = GetCachedFinancialAccounts();
            return financialAccounts.Values.FindAllRecords(x => x.CarrierProfileId.HasValue && x.CarrierProfileId.Value == carrierProfileId);
        }

        public List<WHSFinancialAccount> GetFinancialAccountsByDefinitionId(Guid financialAccountDefinitionId)
        {
            return GetCachedFinancialAccountsByDefinitionId().GetRecord(financialAccountDefinitionId);
        }

        public void GetMatchExistingFinancialAccounts(int? carrierAccountId, int? carrierProfileId,
            out List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts, out List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts)
        {
            int carrierProfileIdToValidate;
            HashSet<int> carrierAccountIdsToValidate = new HashSet<int>();
            if (carrierAccountId.HasValue)
            {
                carrierAccountIdsToValidate.Add(carrierAccountId.Value);
                int? accountProfileId = s_carrierAccountManager.GetCarrierProfileId(carrierAccountId.Value);
                if (!accountProfileId.HasValue)
                    throw new NullReferenceException(String.Format("accountProfileId. Carrier Account '{0}'", carrierAccountId.Value));
                carrierProfileIdToValidate = accountProfileId.Value;
            }
            else if (carrierProfileId.HasValue)
            {
                carrierProfileIdToValidate = carrierProfileId.Value;
                var profileAccounts = s_carrierAccountManager.GetCarriersByProfileId(carrierProfileId.Value, true, true);
                if (profileAccounts != null)
                    carrierAccountIdsToValidate = new HashSet<int>(profileAccounts.Select(itm => itm.CarrierAccountId));
            }
            else
            {
                throw new NullReferenceException("carrierAccountId & carrierProfileId");
            }
            matchExistingCustomerFinancialAccounts = new List<WHSCarrierFinancialAccountData>();
            matchExistingSupplierFinancialAccounts = new List<WHSCarrierFinancialAccountData>();
            Func<WHSFinancialAccount, bool> isFinancialAccountMatchWithExisting = (existingFA) =>
            {
                if (existingFA.CarrierProfileId.HasValue && existingFA.CarrierProfileId.Value == carrierProfileIdToValidate)
                    return true;
                if (existingFA.CarrierAccountId.HasValue && carrierAccountIdsToValidate.Contains(existingFA.CarrierAccountId.Value))
                    return true;
                return false;
            };


            Dictionary<int, WHSCarrierFinancialAccountData> existingCustomerFinancialAccountData = GetCachedCustCarrierFinancialsByFinAccId();
            if (existingCustomerFinancialAccountData != null)
                matchExistingCustomerFinancialAccounts.AddRange(existingCustomerFinancialAccountData.Values.Where(itm => isFinancialAccountMatchWithExisting(itm.FinancialAccount)));

            Dictionary<int, WHSCarrierFinancialAccountData> existingSupplierFinancialAccountData = GetCachedSuppCarrierFinancialsByFinAccId();
            if (existingSupplierFinancialAccountData != null)
                matchExistingSupplierFinancialAccounts.AddRange(existingSupplierFinancialAccountData.Values.Where(itm => isFinancialAccountMatchWithExisting(itm.FinancialAccount)));
        }

        public bool CanSelectFinAccDefInAddOrUpdate(int? financialAccountId, Guid financialAccountDefinitionId, int? carrierAccountId, int? carrierProfileId,
            List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts, List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts)
        {
            if(financialAccountId.HasValue)//In Update, only same definition could be selected
            {
                var existingFinancialAccount = GetFinancialAccount(financialAccountId.Value);
                existingFinancialAccount.ThrowIfNull("existingFinancialAccount", financialAccountId.Value);
                return existingFinancialAccount.FinancialAccountDefinitionId == financialAccountDefinitionId;
            }
            else
            {
                string message;
                return CanAddUpdateFinancialAccount(null, financialAccountDefinitionId, carrierAccountId, carrierProfileId, true, null, null, 
                    matchExistingCustomerFinancialAccounts, matchExistingSupplierFinancialAccounts, out message);
            }
        }

        public bool CanAddFinancialAccountToCarrier(int? carrierAccountId, int? carrierProfileId)
        {
            var financialAccountDefinitions = s_financialAccountDefinitionManager.GetAllFinancialAccountDefinitions();
            if (financialAccountDefinitions != null && financialAccountDefinitions.Count > 0)
            {
                List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts;
                List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts;

                GetMatchExistingFinancialAccounts(carrierAccountId, carrierProfileId, out matchExistingCustomerFinancialAccounts, out matchExistingSupplierFinancialAccounts);
                foreach (var financialAccountDefinition in financialAccountDefinitions.Values)
                {
                    if (CanSelectFinAccDefInAddOrUpdate(null, financialAccountDefinition.BusinessEntityDefinitionId, carrierAccountId, carrierProfileId,
                        matchExistingCustomerFinancialAccounts, matchExistingSupplierFinancialAccounts))
                        return true;
                }
            }
            return false;
        }

        public void ReflectStatusToInvoiceAndBalanceAccounts(VRAccountStatus vrAccountStatus, IEnumerable<WHSFinancialAccount> financialAccounts)
        {
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    string financialAccountIdAsString = financialAccount.FinancialAccountId.ToString();
                    var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
                    Guid? invoiceTypeId = financialAccountDefinitionSettings.InvoiceTypeId;
                    Guid? balanceAccountTypeId = financialAccountDefinitionSettings.BalanceAccountTypeId;
                    if (balanceAccountTypeId.HasValue)
                        s_liveBalanceManager.TryUpdateLiveBalanceStatus(financialAccountIdAsString, balanceAccountTypeId.Value, vrAccountStatus, false);
                    if (invoiceTypeId.HasValue)
                        s_invoiceAccountManager.TryUpdateInvoiceAccountStatus(invoiceTypeId.Value, financialAccountIdAsString, vrAccountStatus, false);
                    if (vrAccountStatus == VRAccountStatus.InActive && (!financialAccount.EED.HasValue || financialAccount.EED.Value > DateTime.Today))
                    {
                        DateTime? eedToSet = null;
                        if (invoiceTypeId.HasValue)
                        {
                            var lastInvoiceToDate = s_invoiceManager.GetLastInvoiceToDate(invoiceTypeId.Value, financialAccountIdAsString);
                            if (lastInvoiceToDate.HasValue)
                                eedToSet = lastInvoiceToDate.Value.AddDays(1).Date;
                        }
                        if (!eedToSet.HasValue && balanceAccountTypeId.HasValue)
                        {
                            var lastTransactionDate = new BillingTransactionManager().GetLastTransactionDate(balanceAccountTypeId.Value, financialAccountIdAsString);
                            if (lastTransactionDate.HasValue)
                                eedToSet = lastTransactionDate.Value;
                        }
                        if (!eedToSet.HasValue)
                            eedToSet = DateTime.Today;
                        var financialAccountToEdit = new WHSFinancialAccountToEdit
                        {
                            FinancialAccountId = financialAccount.FinancialAccountId,
                            Settings = financialAccount.Settings,
                            BED = financialAccount.BED,
                            EED = eedToSet
                        };
                        UpdateFinancialAccount(financialAccountToEdit);
                    }                    
                }
            }
        }

        private bool TryReflectEffectiveDateToInvoiceAndBalanceAccounts(WHSFinancialAccountToEdit financialAccountToEdit, out string errorMessage)
        {
            errorMessage = null;
            var existingFinancialAccount = GetFinancialAccount(financialAccountToEdit.FinancialAccountId);
            existingFinancialAccount.ThrowIfNull("financialAccount", financialAccountToEdit.FinancialAccountId);

            var financialAccountDefinitionSettings = GetDefinitionWithValidate(existingFinancialAccount);
            string financialAccountIdAsString = financialAccountToEdit.FinancialAccountId.ToString();
            if (financialAccountDefinitionSettings.InvoiceTypeId.HasValue)
            {
                if (!s_invoiceAccountManager.TryUpdateInvoiceAccountEffectiveDate(financialAccountDefinitionSettings.InvoiceTypeId.Value, financialAccountIdAsString, financialAccountToEdit.BED, financialAccountToEdit.EED, out errorMessage))
                    return false;
            }
            if(financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
            {
                if (!s_liveBalanceManager.TryUpdateLiveBalanceEffectiveDate(financialAccountIdAsString, financialAccountDefinitionSettings.BalanceAccountTypeId.Value, financialAccountToEdit.BED, financialAccountToEdit.EED))
                    return false;
            }
            return true;
        }

        public string GetAccountCurrencyName(int? carrierProfileId, int? carrierAccountId)
        {
            int currencyId = -1;
            if (carrierProfileId.HasValue)
                currencyId = new CarrierProfileManager().GetCarrierProfileCurrencyId(carrierProfileId.Value);
            else
                currencyId = new CarrierAccountManager().GetCarrierAccountCurrencyId(carrierAccountId.Value);
            CurrencyManager currencyManager = new CurrencyManager();
            return currencyManager.GetCurrencySymbol(currencyId);
        }


        public int GetSupplierTimeZoneId(int financialAccountId)
        {
            var financialAccount = GetFinancialAccount(financialAccountId);
            financialAccount.ThrowIfNull("financialAccount", financialAccountId);
            if(financialAccount.CarrierAccountId.HasValue)
            {
                return s_carrierAccountManager.GetSupplierTimeZoneId(financialAccount.CarrierAccountId.Value);
            }else
            {
                return s_carrierProfileManager.GetSupplierTimeZoneId(financialAccount.CarrierProfileId.Value);
            }
        }
        public int GetCustomerTimeZoneId(int financialAccountId)
        {
            var financialAccount = GetFinancialAccount(financialAccountId);
            financialAccount.ThrowIfNull("financialAccount", financialAccountId);
            if (financialAccount.CarrierAccountId.HasValue)
            {
                return s_carrierAccountManager.GetCustomerTimeZoneId(financialAccount.CarrierAccountId.Value);
            }
            else
            {
                return s_carrierProfileManager.GetCustomerTimeZoneId(financialAccount.CarrierProfileId.Value);
            }
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IWHSFinancialAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<IWHSFinancialAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreFinancialAccountsUpdated(ref _updateHandle);
            }
        }

        private class WHSFinancialAccountFillCustomerExtraDataContext : IWHSFinancialAccountFillCustomerExtraDataContext
        {
            public WHSCarrierFinancialAccountData FinancialAccountData
            {
                get;
                set;
            }
        }

        private class WHSFinancialAccountFillSupplierExtraDataContext : IWHSFinancialAccountFillSupplierExtraDataContext
        {
            public WHSCarrierFinancialAccountData FinancialAccountData
            {
                get;
                set;
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, WHSFinancialAccount> GetCachedFinancialAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccounts",
               () =>
               {
                   IWHSFinancialAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<IWHSFinancialAccountDataManager>();
                   IEnumerable<WHSFinancialAccount> financialAccounts = dataManager.GetFinancialAccounts();
                   return financialAccounts.ToDictionary(fa => fa.FinancialAccountId, fa => fa);
               });
        }

        Dictionary<Guid, List<WHSFinancialAccount>> GetCachedFinancialAccountsByDefinitionId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccountsByDefinitionId",
                 () =>
                 {
                     var financialAccountsByDefinitionId = new Dictionary<Guid, List<WHSFinancialAccount>>();

                     Dictionary<int, WHSFinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();

                     if (cachedFinancialAccounts != null)
                     {
                         foreach (var financialAccount in cachedFinancialAccounts.Values)
                         {
                             financialAccountsByDefinitionId.GetOrCreateItem(financialAccount.FinancialAccountDefinitionId).Add(financialAccount);
                         }
                     }

                     return financialAccountsByDefinitionId;
                 });
        }

        Dictionary<int, WHSCarrierFinancialAccountData> GetCachedCustCarrierFinancialsByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierFinancialsByFinAccId",
             () =>
             {
                 var carrierDataByFinancialAccountId = new Dictionary<int, WHSCarrierFinancialAccountData>();

                 Dictionary<int, WHSFinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();
                 if (cachedFinancialAccounts != null)
                 {
                     foreach (var financialAccount in cachedFinancialAccounts.Values)
                     {
                         WHSCarrierFinancialAccountData financialAccountData;
                         if (TryCreateCustomerFinancialAccountData(financialAccount, out financialAccountData))
                         {
                             carrierDataByFinancialAccountId.Add(financialAccount.FinancialAccountId, financialAccountData);
                         }
                     }
                 }
                 return carrierDataByFinancialAccountId;
             });
        }

        Dictionary<int, WHSCarrierFinancialAccountData> GetCachedSuppCarrierFinancialsByFinAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierFinancialsByFinAccId",
             () =>
             {
                 var carrierDataByFinancialAccountId = new Dictionary<int, WHSCarrierFinancialAccountData>();

                 Dictionary<int, WHSFinancialAccount> cachedFinancialAccounts = GetCachedFinancialAccounts();
                 if (cachedFinancialAccounts != null)
                 {
                     foreach (var financialAccount in cachedFinancialAccounts.Values)
                     {
                         WHSCarrierFinancialAccountData financialAccountData;
                         if (TryCreateSupplierFinancialAccountData(financialAccount, out financialAccountData))
                         {
                             carrierDataByFinancialAccountId.Add(financialAccount.FinancialAccountId, financialAccountData);
                         }
                     }
                 }
                 return carrierDataByFinancialAccountId;
             });
        }

        Dictionary<int, IOrderedEnumerable<WHSCarrierFinancialAccountData>> GetCachedCustCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierFinancialsByCarrAccId",
            () =>
            {
                Dictionary<int, List<WHSCarrierFinancialAccountData>> financialAccountsDataDict = new Dictionary<int, List<WHSCarrierFinancialAccountData>>();
                Dictionary<int, WHSCarrierFinancialAccountData> customerFinancialAccountsData = GetCachedCustCarrierFinancialsByFinAccId();

                if (customerFinancialAccountsData != null)
                {
                    foreach (var financialAccountData in customerFinancialAccountsData.Values)
                    {
                        if (financialAccountData.FinancialAccount.CarrierAccountId.HasValue)
                        {
                            financialAccountsDataDict.GetOrCreateItem(financialAccountData.FinancialAccount.CarrierAccountId.Value).Add(financialAccountData);
                        }
                        else // so financialAccount.CarrierProfileId.HasValue = true
                        {
                            var customerAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccountData.FinancialAccount.CarrierProfileId.Value, true, false);
                            if (customerAccounts != null)
                            {
                                foreach (var customerAccount in customerAccounts)
                                {
                                    financialAccountsDataDict.GetOrCreateItem(customerAccount.CarrierAccountId).Add(financialAccountData);
                                }
                            }
                        }
                    }
                }

                return financialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccountData => financialAccountData.FinancialAccount.BED));
            });
        }

        Dictionary<int, IOrderedEnumerable<WHSCarrierFinancialAccountData>> GetCachedSuppCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierFinancialsByCarrAccId",
            () =>
            {
                Dictionary<int, List<WHSCarrierFinancialAccountData>> financialAccountsDataDict = new Dictionary<int, List<WHSCarrierFinancialAccountData>>();
                Dictionary<int, WHSCarrierFinancialAccountData> supplierFinancialAccountsData = GetCachedSuppCarrierFinancialsByFinAccId();

                if (supplierFinancialAccountsData != null)
                {
                    foreach (var financialAccountData in supplierFinancialAccountsData.Values)
                    {
                        if (financialAccountData.FinancialAccount.CarrierAccountId.HasValue)
                        {
                            financialAccountsDataDict.GetOrCreateItem(financialAccountData.FinancialAccount.CarrierAccountId.Value).Add(financialAccountData);
                        }
                        else // so financialAccount.CarrierProfileId.HasValue = true
                        {
                            var supplierAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccountData.FinancialAccount.CarrierProfileId.Value, false, true);
                            if (supplierAccounts != null)
                            {
                                foreach (var supplierAccount in supplierAccounts)
                                {
                                    financialAccountsDataDict.GetOrCreateItem(supplierAccount.CarrierAccountId).Add(financialAccountData);
                                }
                            }
                        }
                    }
                }

                return financialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccountData => financialAccountData.FinancialAccount.BED));
            });
        }

        private bool TryCreateCustomerFinancialAccountData(WHSFinancialAccount financialAccount, out WHSCarrierFinancialAccountData financialAccountData)
        {
            ValidateFinancialAccount(financialAccount);

            WHSFinancialAccountDefinitionSettings definitionSettings = GetDefinitionWithValidate(financialAccount);

            if (!definitionSettings.ExtendedSettings.IsApplicableToCustomer)
            {
                financialAccountData = null;
                return false;
            }
            else
            {
                int currencyId;
                if (financialAccount.CarrierAccountId.HasValue)
                    currencyId = s_carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                else // so financialAccount.CarrierProfileId.HasValue = true                        
                    currencyId = s_carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);
                financialAccountData = new WHSCarrierFinancialAccountData()
                {
                    FinancialAccount = financialAccount,
                    CurrencyId = currencyId
                };
                if (definitionSettings.BalanceAccountTypeId.HasValue)
                {
                    financialAccountData.AccountBalanceData = new WHSCarrierFinancialAccountBalanceData
                    {
                        AccountTypeId = definitionSettings.BalanceAccountTypeId.Value,

                    };
                }
                if (definitionSettings.InvoiceTypeId.HasValue)
                {
                    financialAccountData.InvoiceData = new WHSCarrierFinancialAccountInvoiceData
                    {
                        InvoiceTypeId = definitionSettings.InvoiceTypeId.Value
                    };
                }
                var fillExtraDataContext = new WHSFinancialAccountFillCustomerExtraDataContext
                {
                    FinancialAccountData = financialAccountData
                };
                financialAccount.Settings.ExtendedSettings.FillCustomerExtraData(fillExtraDataContext);
                return true;
            }
        }

        private bool TryCreateSupplierFinancialAccountData(WHSFinancialAccount financialAccount, out WHSCarrierFinancialAccountData financialAccountData)
        {
            ValidateFinancialAccount(financialAccount);

            WHSFinancialAccountDefinitionSettings definitionSettings = GetDefinitionWithValidate(financialAccount);

            if (!definitionSettings.ExtendedSettings.IsApplicableToSupplier)
            {
                financialAccountData = null;
                return false;
            }
            else
            {
                int currencyId;
                if (financialAccount.CarrierAccountId.HasValue)
                    currencyId = s_carrierAccountManager.GetCarrierAccountCurrencyId(financialAccount.CarrierAccountId.Value);
                else // so financialAccount.CarrierProfileId.HasValue = true                        
                    currencyId = s_carrierProfileManager.GetCarrierProfileCurrencyId(financialAccount.CarrierProfileId.Value);
                financialAccountData = new WHSCarrierFinancialAccountData()
                {
                    FinancialAccount = financialAccount,
                    CurrencyId = currencyId
                };
                if (definitionSettings.BalanceAccountTypeId.HasValue)
                {
                    financialAccountData.AccountBalanceData = new WHSCarrierFinancialAccountBalanceData
                    {
                        AccountTypeId = definitionSettings.BalanceAccountTypeId.Value
                    };
                }
                if (definitionSettings.InvoiceTypeId.HasValue)
                {
                    financialAccountData.InvoiceData = new WHSCarrierFinancialAccountInvoiceData
                    {
                        InvoiceTypeId = definitionSettings.InvoiceTypeId.Value
                    };
                }
                var fillExtraDataContext = new WHSFinancialAccountFillSupplierExtraDataContext
                {
                    FinancialAccountData = financialAccountData
                };
                financialAccount.Settings.ExtendedSettings.FillSupplierExtraData(fillExtraDataContext);
                return true;
            }
        }

        private void ValidateFinancialAccount(WHSFinancialAccount financialAccount)
        {
            financialAccount.ThrowIfNull("financialAccount");
            financialAccount.Settings.ThrowIfNull("financialAccount.Settings", financialAccount.FinancialAccountId);
            financialAccount.Settings.ExtendedSettings.ThrowIfNull("financialAccount.Settings.ExtendedSettings", financialAccount.FinancialAccountId);
            if (!financialAccount.CarrierAccountId.HasValue && !financialAccount.CarrierProfileId.HasValue)
                throw new NullReferenceException(string.Format("financialAccount.CarrierAccountId & financialAccount.CarrierProfileId for financial Account Id: {0}", financialAccount.FinancialAccountId));
        }

        private WHSFinancialAccountDefinitionSettings GetDefinitionWithValidate(WHSFinancialAccount financialAccount)
        {
            return GetDefinitionWithValidate(financialAccount.FinancialAccountDefinitionId);
        }

        private WHSFinancialAccountDefinitionSettings GetDefinitionWithValidate(Guid financialAccountDefinitionId)
        {
            WHSFinancialAccountDefinitionSettings definitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
            definitionSettings.ExtendedSettings.ThrowIfNull("definitionSettings.ExtendedSettings", financialAccountDefinitionId);
            return definitionSettings;
        }

        private bool LinkPartnerToInvoiceSetting(Guid partnerInvoiceSettingId, Guid invoiceSettingId, string partnerId)
        {
            PartnerInvoiceSettingManager partnerInvoiceSettingManager = new Vanrise.Invoice.Business.PartnerInvoiceSettingManager();
            return  partnerInvoiceSettingManager.LinkPartnerToInvoiceSetting(partnerInvoiceSettingId,partnerId, invoiceSettingId);
        }

        #endregion

        #region Private Validation Methods
        
        private bool CanAddUpdateFinancialAccount(int? financialAccountId, Guid financialAccountDefinitionId, int? carrierAccountId, int? carrierProfileId, bool beforeAddingNewFinancialAccount, DateTime? bed, DateTime? eed,
            List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts, List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts, out string message)
        {
            if (!beforeAddingNewFinancialAccount)
            {
                if (!bed.HasValue)
                    throw new ArgumentNullException("bed && beforeAddingNewFinancialAccount");
            }
            bool isApplicableToCustomer;
            bool isApplicableToSupplier;
            if (!IsFinancialAccountDefinitionApplicableToCarrier(financialAccountDefinitionId, carrierAccountId, carrierProfileId, out isApplicableToCustomer, out isApplicableToSupplier))
            {
                message = "Financial Account Definition is not compatible with Carrier";
                return false;
            }

            Func<WHSFinancialAccount, bool> isFinancialAccountConflictedWithExisting = (existingFA) =>
            {
                if (financialAccountId.HasValue && financialAccountId.Value == existingFA.FinancialAccountId)
                    return false;
                if (beforeAddingNewFinancialAccount)
                {
                    if (!existingFA.EED.HasValue)
                        return true;
                }
                else
                {
                    if (Utilities.AreTimePeriodsOverlapped(existingFA.BED, existingFA.EED, bed.Value, eed))
                        return true;
                }
                return false;
            };
            if (isApplicableToCustomer && matchExistingCustomerFinancialAccounts != null)
            {
                if (matchExistingCustomerFinancialAccounts.Any(itm => isFinancialAccountConflictedWithExisting(itm.FinancialAccount)))
                {
                    message = "Financial account overlaps with existing one";
                    return false;
                }
            }
            if (isApplicableToSupplier && matchExistingSupplierFinancialAccounts != null)
            {
                if (matchExistingSupplierFinancialAccounts.Any(itm => isFinancialAccountConflictedWithExisting(itm.FinancialAccount)))
                {
                    message = "Financial account overlaps with existing one";
                    return false;
                }
            }
            message = null;
            return true;
        }

        private bool CanAddFinancialAccount(WHSFinancialAccount financialAccount, out string message)
        {
            ValidateFinancialAccount(financialAccount);
            if (!IsFinancialAccountActive(financialAccount))
            {
                message = "Financial account is inactive.";
                return false;
            }

            List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts;
            List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts;

            GetMatchExistingFinancialAccounts(financialAccount.CarrierAccountId, financialAccount.CarrierProfileId, out matchExistingCustomerFinancialAccounts, out matchExistingSupplierFinancialAccounts);

            if (!CanAddUpdateFinancialAccount(financialAccount.FinancialAccountId, financialAccount.FinancialAccountDefinitionId, financialAccount.CarrierAccountId, financialAccount.CarrierProfileId,
                false, financialAccount.BED, financialAccount.EED, matchExistingCustomerFinancialAccounts, matchExistingSupplierFinancialAccounts, out message))
                return false;
            message = null;
            return true;
        }

        private bool CanUpdateFinancialAccount(WHSFinancialAccountToEdit financialAccountToEdit, out string message)
        {
            financialAccountToEdit.Settings.ThrowIfNull("financialAccountToEdit.Settings", financialAccountToEdit.FinancialAccountId);
            financialAccountToEdit.Settings.ExtendedSettings.ThrowIfNull("financialAccountToEdit.Settings.ExtendedSettings", financialAccountToEdit.FinancialAccountId);
            var existingFinancialAccount = GetFinancialAccount(financialAccountToEdit.FinancialAccountId);
            existingFinancialAccount.ThrowIfNull("financialAccount", financialAccountToEdit.FinancialAccountId);

            List<WHSCarrierFinancialAccountData> matchExistingCustomerFinancialAccounts;
            List<WHSCarrierFinancialAccountData> matchExistingSupplierFinancialAccounts;

            GetMatchExistingFinancialAccounts(existingFinancialAccount.CarrierAccountId, existingFinancialAccount.CarrierProfileId, out matchExistingCustomerFinancialAccounts, out matchExistingSupplierFinancialAccounts);

            if (!CanAddUpdateFinancialAccount(financialAccountToEdit.FinancialAccountId, existingFinancialAccount.FinancialAccountDefinitionId, existingFinancialAccount.CarrierAccountId, existingFinancialAccount.CarrierProfileId,
                false, financialAccountToEdit.BED, financialAccountToEdit.EED, matchExistingCustomerFinancialAccounts, matchExistingSupplierFinancialAccounts, out message))
                return false;
            message = null;
            return true;
        }

        private bool IsFinancialAccountActive(WHSFinancialAccount financialAccount)
        {
            ValidateFinancialAccount(financialAccount);
            if (financialAccount.CarrierAccountId.HasValue)
            {
                return !s_carrierAccountManager.IsCarrierAccountDeleted(financialAccount.CarrierAccountId.Value) && s_carrierAccountManager.IsCarrierAccountActive(financialAccount.CarrierAccountId.Value);
            }
            else
            {
                WHSFinancialAccountDefinitionSettings definitionSettings = GetDefinitionWithValidate(financialAccount);
                return !s_carrierProfileManager.IsCarrierProfileDeleted(financialAccount.CarrierProfileId.Value) && s_carrierProfileManager.EvaluateCarrierProfileStatus(financialAccount.CarrierProfileId.Value, definitionSettings.ExtendedSettings.IsApplicableToCustomer, definitionSettings.ExtendedSettings.IsApplicableToSupplier) != CarrierProfileActivationStatus.InActive;
            }
        }

        private bool IsFinancialAccountDefinitionApplicableToCarrier(Guid financialAccountDefinitionId, int? carrierAccountId, int? carrierProfileId, out bool isApplicableToCustomer, out bool isApplicableToSupplier)
        {
            WHSFinancialAccountDefinitionSettings definitionSettings = GetDefinitionWithValidate(financialAccountDefinitionId);
            isApplicableToCustomer = definitionSettings.ExtendedSettings.IsApplicableToCustomer;
            isApplicableToSupplier = definitionSettings.ExtendedSettings.IsApplicableToSupplier;
            if (carrierAccountId.HasValue)
            {
                CarrierAccount carrierAccount = s_carrierAccountManager.GetCarrierAccount(carrierAccountId.Value);
                carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId.Value);
                if (isApplicableToCustomer && carrierAccount.AccountType != CarrierAccountType.Customer && carrierAccount.AccountType != CarrierAccountType.Exchange)
                    return false;
                if (isApplicableToSupplier && carrierAccount.AccountType != CarrierAccountType.Supplier && carrierAccount.AccountType != CarrierAccountType.Exchange)
                    return false;
                return true;
            }
            else
            {
                IEnumerable<CarrierAccount> profileAccounts = s_carrierAccountManager.GetCarriersByProfileId(carrierProfileId.Value, isApplicableToCustomer, isApplicableToSupplier);
                if (profileAccounts == null)
                    return false;
                bool anyActiveCustomer = false;
                bool anyActiveSupplier = false;
                foreach (var ca in profileAccounts)
                {
                    if (s_carrierAccountManager.IsCarrierAccountActive(ca))
                    {
                        switch (ca.AccountType)
                        {
                            case CarrierAccountType.Customer: anyActiveCustomer = true; break;
                            case CarrierAccountType.Supplier: anyActiveSupplier = true; break;
                            case CarrierAccountType.Exchange:
                                anyActiveCustomer = true;
                                anyActiveSupplier = true;
                                break;
                        }
                    }
                }
                if (isApplicableToCustomer && !anyActiveCustomer)
                    return false;
                if (isApplicableToSupplier && !anyActiveSupplier)
                    return false;
                return true;
            }
        }

        #endregion

        #region Mappers

        private WHSFinancialAccountInfo FinancialAccountInfoMapper(WHSFinancialAccount financialAccount)
        {           
            
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            var financialAccountInfo = new WHSFinancialAccountInfo()
            {
                FinancialAccountId = financialAccount.FinancialAccountId,
                EffectiveStatus = GetFinancialAccountEffectiveStatus(financialAccount.BED, financialAccount.EED),
                BalanceAccountTypeId = financialAccountDefinitionSettings.BalanceAccountTypeId,
                InvoiceTypeId = financialAccountDefinitionSettings.InvoiceTypeId
            };
            if (financialAccount.CarrierProfileId.HasValue)
            {
                financialAccountInfo.CarrierType = WHSFinancialAccountCarrierType.Profile;
                financialAccountInfo.Name = s_carrierProfileManager.GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
                string profilePrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Profile);
                financialAccountInfo.Description = string.Format("{1} ({0})", profilePrefix, financialAccountInfo.Name);
            }
            else
            {
                financialAccountInfo.CarrierType = WHSFinancialAccountCarrierType.Account;
                financialAccountInfo.Name = s_carrierAccountManager.GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
                string accountPrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Account);
                financialAccountInfo.Description = string.Format("{1} ({0})", accountPrefix, financialAccountInfo.Name);
            }
            return financialAccountInfo;
        }
        private WHSFinancialAccountEffectiveStatus GetFinancialAccountEffectiveStatus(DateTime financialAccountBED, DateTime? financialAccountEED)
        {
            DateTime today = DateTime.Today;

            if (financialAccountEED.HasValue)
            {
                if (financialAccountBED > today)
                    return WHSFinancialAccountEffectiveStatus.Future;
                else if (financialAccountBED == today)
                    return WHSFinancialAccountEffectiveStatus.Current;
                else
                {
                    return (financialAccountEED.Value > today) ? WHSFinancialAccountEffectiveStatus.Current : WHSFinancialAccountEffectiveStatus.Recent;
                }
            }
            else
            {
                return (financialAccountBED > today) ? WHSFinancialAccountEffectiveStatus.Future : WHSFinancialAccountEffectiveStatus.Current;
            }
        }
        private WHSFinancialAccountDetail FinancialAccountDetailMapper(WHSFinancialAccount financialAccount)
        {
            var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            return new WHSFinancialAccountDetail
            {
                Entity = financialAccount,
                AccountTypeDescription = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionName(financialAccount.FinancialAccountDefinitionId),
                IsActive = IsFinancialAccountActive(financialAccount),
                BalanceAccountTypeId =financialAccountDefinitionSettings.BalanceAccountTypeId ,
                InvoiceTypeId = financialAccountDefinitionSettings.InvoiceTypeId,
            };
        }
        #endregion

        #region IBusinessEntityManager

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var currentBEAccounts = GetFinancialAccountsByDefinitionId(context.EntityDefinitionId);
            return currentBEAccounts != null ? currentBEAccounts.Select(itm => itm as dynamic).ToList() : null;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

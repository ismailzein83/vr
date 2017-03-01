using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Data;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountManager
    {
        #region Public Methods

        public IDataRetrievalResult<FinancialAccountDetail> GetFilteredFinancialAccounts(Vanrise.Entities.DataRetrievalInput<FinancialAccountQuery> input)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();

            Func<FinancialAccount, bool> filterExpression = (prod) =>
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

        public Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> AddFinancialAccount(FinancialAccount financialAccount)
        {
            Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<FinancialAccountDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int financialAccountId = -1;

            IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
            bool insertActionSucc = dataManager.Insert(financialAccount, out financialAccountId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                financialAccount.FinancialAccountId = financialAccountId;
                insertOperationOutput.InsertedObject = FinancialAccountDetailMapper(financialAccount);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> UpdateFinancialAccount(FinancialAccount financialAccount)
        {
            IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();

            bool updateActionSucc = dataManager.Update(financialAccount);
            Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<FinancialAccountDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = FinancialAccountDetailMapper(financialAccount);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public FinancialAccount GetFinancialAccount(int financialAccountId)
        {
            var allFinancialAccounts = GetCachedFinancialAccounts();
            return allFinancialAccounts.GetRecord(financialAccountId);
        }

        public CarrierFinancialAccountData GetCustCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedCustCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public CarrierFinancialAccountData GetSuppCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedSuppCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public bool TryGetCustAccFinancialAccountData(int customerAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedCustCarrierFinancialsByCarrAccId().GetRecord(customerAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }

        public bool TryGetSuppAccFinancialAccountData(int supplierAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedSuppCarrierFinancialsByCarrAccId().GetRecord(supplierAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IFinancialAccountDataManager _dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreFinancialAccountsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, FinancialAccount> GetCachedFinancialAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedFinancialAccounts",
               () =>
               {
                   IFinancialAccountDataManager dataManager = AccountBalanceManagerFactory.GetDataManager<IFinancialAccountDataManager>();
                   IEnumerable<FinancialAccount> financialAccounts = dataManager.GetFinancialAccounts();
                   return financialAccounts.ToDictionary(fa => fa.FinancialAccountId, fa => fa);
               });
        }

        Dictionary<Guid, List<FinancialAccount>> GetCachedFinancialAccountsByAccBalTypeId()
        {
            throw new NotImplementedException();
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedCustCarrierFinancialsByFinAccId()
        {
            throw new NotImplementedException();
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedSuppCarrierFinancialsByFinAccId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each customer account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedCustCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustCarrierFinancialsByCarrAccId",
            () =>
            {
                List<CarrierFinancialAccountData> customerFinancialAccountsData;
                Dictionary<int, List<CarrierFinancialAccountData>> customerFinancialAccountsDataDict = new Dictionary<int, List<CarrierFinancialAccountData>>();
                Dictionary<int, FinancialAccount> financialAccounts = GetCachedFinancialAccounts();

                foreach (var financialAccount in financialAccounts.Values)
                {
                    if (financialAccount.Settings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.Settings.ExtendedSettings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    FinancialAccountIsCustomerAccountContext context = new FinancialAccountIsCustomerAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                    if (!financialAccount.Settings.ExtendedSettings.IsCustomerAccount(context))// IsCustomerAccount will set UsageTransactionTypeId on context
                        continue;

                    if (!financialAccount.CarrierAccountId.HasValue && !financialAccount.CarrierProfileId.HasValue)
                        throw new NullReferenceException(string.Format("financialAccount.CarrierAccountId & financialAccount.CarrierProfileId for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.CarrierAccountId.HasValue)
                    {
                        customerFinancialAccountsData = customerFinancialAccountsDataDict.GetOrCreateItem(financialAccount.CarrierAccountId.Value);
                        customerFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId));
                    }
                    else // so financialAccount.CarrierProfileId.HasValue = true
                    {
                        var customerAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                        if (customerAccounts != null)
                        {
                            foreach (var customerAccount in customerAccounts)
                            {
                                customerFinancialAccountsData = customerFinancialAccountsDataDict.GetOrCreateItem(customerAccount.CarrierAccountId);
                                customerFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId));
                            }
                        }
                    }
                }

                return customerFinancialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccount => financialAccount.BED));
            });
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each supplier account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedSuppCarrierFinancialsByCarrAccId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSuppCarrierFinancialsByCarrAccId",
            () =>
            {
                List<CarrierFinancialAccountData> supplierFinancialAccountsData;
                Dictionary<int, List<CarrierFinancialAccountData>> supplierFinancialAccountsDataDict = new Dictionary<int, List<CarrierFinancialAccountData>>();
                Dictionary<int, FinancialAccount> financialAccounts = GetCachedFinancialAccounts();

                foreach (var financialAccount in financialAccounts.Values)
                {
                    if (financialAccount.Settings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.Settings.ExtendedSettings == null)
                        throw new NullReferenceException(string.Format("financialAccount.Settings.ExtendedSettings for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    FinancialAccountIsSupplierAccountContext context = new FinancialAccountIsSupplierAccountContext() { AccountTypeId = financialAccount.Settings.AccountTypeId };

                    if (!financialAccount.Settings.ExtendedSettings.IsSupplierAccount(context))// IsSupplierAccount will set UsageTransactionTypeId on context
                        continue;

                    if (!financialAccount.CarrierAccountId.HasValue && !financialAccount.CarrierProfileId.HasValue)
                        throw new NullReferenceException(string.Format("financialAccount.CarrierAccountId & financialAccount.CarrierProfileId for financial Account Id: {0}", financialAccount.FinancialAccountId));

                    if (financialAccount.CarrierAccountId.HasValue)
                    {
                        supplierFinancialAccountsData = supplierFinancialAccountsDataDict.GetOrCreateItem(financialAccount.CarrierAccountId.Value);
                        supplierFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId));
                    }
                    else // so financialAccount.CarrierProfileId.HasValue = true
                    {
                        var supplierAccounts = new CarrierAccountManager().GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, false, true);
                        foreach (var supplierAccount in supplierAccounts)
                        {
                            supplierFinancialAccountsData = supplierFinancialAccountsDataDict.GetOrCreateItem(supplierAccount.CarrierAccountId);
                            supplierFinancialAccountsData.Add(CreateCarrierFinancialAccountData(financialAccount, context.UsageTransactionTypeId));
                        }
                    }
                }

                return supplierFinancialAccountsDataDict.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(financialAccount => financialAccount.BED));
            });
        }

        private CarrierFinancialAccountData CreateCarrierFinancialAccountData(FinancialAccount financialAccount, Guid usageTransactionTypeId)
        {
            return new CarrierFinancialAccountData()
            {
                AccountTypeId = financialAccount.Settings.AccountTypeId,
                FinancialAccountId = financialAccount.FinancialAccountId,
                UsageTransactionTypeId = usageTransactionTypeId,
                BED = financialAccount.BED,
                EED = financialAccount.EED
            };
        }

        #endregion

        #region Mapper
        FinancialAccountDetail FinancialAccountDetailMapper(FinancialAccount financialAccount)
        {
            return new FinancialAccountDetail
            {
                Entity = financialAccount,
                AccountTypeDescription = new AccountTypeManager().GetAccountTypeName(financialAccount.Settings.AccountTypeId)
            };
        }
        #endregion
    }
}

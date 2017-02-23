using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceManager : Vanrise.AccountBalance.Entities.IAccountManager
    {
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        #region Public Methods

        public void GetAccountOrProfileId(string balanceEntityId, out int? carrierAccountId, out int? carrierProfileId)
        {
            carrierAccountId = null;
            carrierProfileId = null;
            int financialAccountId = ParseFinancialAccountId(balanceEntityId);
            FinancialAccount financialAccount = new FinancialAccountManager().GetFinancialAccount(financialAccountId);
            if (financialAccount.CarrierProfileId.HasValue)
            {
                carrierProfileId = financialAccount.CarrierProfileId.Value;
            }
            else
            {
                carrierAccountId = financialAccount.CarrierAccountId.Value;
            }
        }

        public void GetAccountOrProfile(string balanceEntityId, out CarrierAccount carrierAccount, out CarrierProfile carrierProfile)
        {
            carrierAccount = null;
            carrierProfile = null;
            int? carrierAccountId;
            int? carrierProfileId;
            GetAccountOrProfileId(balanceEntityId, out carrierAccountId, out carrierProfileId);
            if (carrierProfileId.HasValue)
            {
                carrierProfile = new CarrierProfileManager().GetCarrierProfile(carrierProfileId.Value);
            }
            else
            {
                carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId.Value);
            }
        }

        public Decimal GetCustomerCreditLimit(string balanceEntityId)
        {
            int financeAccountId = ParseFinancialAccountId(balanceEntityId);
            CarrierFinancialAccountData financialAccountData = s_financialAccountManager.GetCustCarrierFinancialByFinAccId(financeAccountId);
            if (!financialAccountData.CreditLimit.HasValue)
                throw new NullReferenceException(String.Format("financialAccountData.CreditLimit '{0}'", balanceEntityId));
            return financialAccountData.CreditLimit.Value;
        }

        public Decimal GetSupplierCreditLimit(string balanceEntityId)
        {
            int financeAccountId = ParseFinancialAccountId(balanceEntityId);
            CarrierFinancialAccountData financialAccountData = s_financialAccountManager.GetSuppCarrierFinancialByFinAccId(financeAccountId);
            if (!financialAccountData.CreditLimit.HasValue)
                throw new NullReferenceException(String.Format("financialAccountData.CreditLimit '{0}'", balanceEntityId));
            return financialAccountData.CreditLimit.Value;
        }

        public void ConvertCDRToBalanceUpdatePayloads(dynamic mainCDR, Action<BalanceUpdatePayload> createBalanceUpdatePayload)
        {
            DateTime attemptTime = mainCDR.AttemptDateTime;
            int? customerId = mainCDR.CustomerId;
            if (customerId.HasValue)
            {
                Decimal? saleAmount = mainCDR.SaleNet;
                if (saleAmount.HasValue && saleAmount.Value > 0)
                {
                    CarrierFinancialAccountData financialAccountData;
                    if (s_financialAccountManager.TryGetCustAccFinancialAccountData(customerId.Value, attemptTime, out financialAccountData))
                    {
                        createBalanceUpdatePayload(new BalanceUpdatePayload
                            {
                                AccountTypeId = financialAccountData.AccountTypeId,
                                TransactionTypeId = financialAccountData.UsageTransactionTypeId,
                                AccountId = financialAccountData.FinancialAccountId.ToString(),
                                Value = saleAmount.Value,
                                EffectiveOn = attemptTime,
                                CurrencyId = mainCDR.SaleCurrencyId.Value
                            });
                    }
                }
            }
            int? supplierId = mainCDR.SupplierId;
            if (supplierId.HasValue)
            {
                Decimal? costAmount = mainCDR.CostNet;
                if (costAmount.HasValue && costAmount.Value > 0)
                {
                    CarrierFinancialAccountData financialAccountData;
                    if (s_financialAccountManager.TryGetSuppAccFinancialAccountData(customerId.Value, attemptTime, out financialAccountData))
                    {
                        createBalanceUpdatePayload(new BalanceUpdatePayload
                        {
                            AccountTypeId = financialAccountData.AccountTypeId,
                            TransactionTypeId = financialAccountData.UsageTransactionTypeId,
                            AccountId = financialAccountData.FinancialAccountId.ToString(),
                            Value = costAmount.Value,
                            EffectiveOn = attemptTime,
                            CurrencyId = mainCDR.CostCurrencyId.Value
                        });
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private int ParseFinancialAccountId(string balanceEntityId)
        {
            int financialAccountId;
            if (!int.TryParse(balanceEntityId, out financialAccountId))
                throw new Exception(String.Format("balanceEntityId '{0}' is not a valid int", balanceEntityId));
            return financialAccountId;
        }

        #endregion

        #region IAccountManager

        dynamic Vanrise.AccountBalance.Entities.IAccountManager.GetAccount(Vanrise.AccountBalance.Entities.IAccountContext context)
        {
            CarrierAccount carrierAccount;
            CarrierProfile carrierProfile;
            GetAccountOrProfile(context.AccountId, out carrierAccount, out carrierProfile);
            if (carrierProfile != null)
                return carrierProfile;
            else
                return carrierAccount;
        }

        Vanrise.AccountBalance.Entities.AccountInfo Vanrise.AccountBalance.Entities.IAccountManager.GetAccountInfo(Vanrise.AccountBalance.Entities.IAccountInfoContext context)
        {
            CarrierAccount carrierAccount;
            CarrierProfile carrierProfile;
            GetAccountOrProfile(context.AccountId, out carrierAccount, out carrierProfile);
            if (carrierProfile != null)
            {
                var carrierProfileManager = new CarrierProfileManager();
                return new Vanrise.AccountBalance.Entities.AccountInfo
                {
                    Name = carrierProfileManager.GetCarrierProfileName(carrierProfile),
                    CurrencyId = carrierProfileManager.GetCarrierProfileCurrencyId(carrierProfile)
                };
            }
            else
            {
                var carrierAccountManager = new CarrierAccountManager();
                return new Vanrise.AccountBalance.Entities.AccountInfo
                {
                    Name = carrierAccountManager.GetCarrierAccountName(carrierAccount),
                    CurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(carrierAccount)
                };
            }
        }

        #endregion

    }
        public class BalanceUpdatePayload
        {
            public Guid AccountTypeId { get; set; }

            public Guid TransactionTypeId { get; set; }

            public string AccountId { get; set; }

            public DateTime EffectiveOn { get; set; }

            public Decimal Value { get; set; }

            public int CurrencyId { get; set; }
        }
}
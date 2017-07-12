using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;
namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceManager : Vanrise.AccountBalance.Entities.IAccountManager
    {
        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();

        #region Public Methods

        public void GetAccountOrProfileId(string balanceEntityId, out int? carrierAccountId, out int? carrierProfileId, out DateTime? bed, out DateTime? eed,out VRAccountStatus status)
        {
            carrierAccountId = null;
            carrierProfileId = null;
            int financialAccountId = ParseFinancialAccountId(balanceEntityId);
            FinancialAccount financialAccount = new FinancialAccountManager().GetFinancialAccount(financialAccountId);
            bed = financialAccount.BED;
            eed = financialAccount.EED;
            status = VRAccountStatus.Active;
           
            if (financialAccount.CarrierProfileId.HasValue)
            {
                carrierProfileId = financialAccount.CarrierProfileId.Value;
            }
            else
            {
                carrierAccountId = financialAccount.CarrierAccountId.Value;
            }
        }

        public void GetAccountOrProfile(string balanceEntityId, out CarrierAccount carrierAccount, out CarrierProfile carrierProfile, out DateTime? bed, out DateTime? eed, out VRAccountStatus status)
        {
            carrierAccount = null;
            carrierProfile = null;
            int? carrierAccountId;
            int? carrierProfileId;
            GetAccountOrProfileId(balanceEntityId, out carrierAccountId, out carrierProfileId, out bed,out eed, out status);
            if (carrierProfileId.HasValue)
            {
                carrierProfile = new CarrierProfileManager().GetCarrierProfile(carrierProfileId.Value);
                carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId.Value);
            }
            else
            {
                carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId.Value);
                carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId.Value);
            }
        }

        public Decimal GetCustomerCreditLimit(string balanceEntityId, out int carrierCurrencyId)
        {
            int financeAccountId = ParseFinancialAccountId(balanceEntityId);
            CarrierFinancialAccountData financialAccountData = s_financialAccountManager.GetCustCarrierFinancialByFinAccId(financeAccountId);
            
            if (!financialAccountData.CreditLimit.HasValue)
                throw new Vanrise.Entities.VRBusinessException(String.Format("Credit Limit must have a value for the Financial Account: '{0}'", balanceEntityId));

            carrierCurrencyId = financialAccountData.CarrierCurrencyId;
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

        public bool CheckFinancialAccountTranasactions(Guid accountTypeId ,int financialAccountId)
        {
            LiveBalanceManager liveBalanceManager = new LiveBalanceManager();
            return liveBalanceManager.CheckIfAccountHasTransactions(accountTypeId, financialAccountId.ToString());
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
             int financialAccountId = ParseFinancialAccountId(context.AccountId);
             return new FinancialAccountManager().GetFinancialAccount(financialAccountId);
        }

        Vanrise.AccountBalance.Entities.AccountInfo Vanrise.AccountBalance.Entities.IAccountManager.GetAccountInfo(Vanrise.AccountBalance.Entities.IAccountInfoContext context)
        {
            CarrierAccount carrierAccount;
            CarrierProfile carrierProfile;
            DateTime? bed;
            DateTime? eed;
            VRAccountStatus status;
            GetAccountOrProfile(context.AccountId, out carrierAccount, out carrierProfile,out bed, out eed,out status);
            if (carrierProfile != null)
            {
                var carrierProfileManager = new CarrierProfileManager();
                return new Vanrise.AccountBalance.Entities.AccountInfo
                {
                    Name = carrierProfileManager.GetCarrierProfileName(carrierProfile),
                    CurrencyId = carrierProfileManager.GetCarrierProfileCurrencyId(carrierProfile),
                    BED = bed,
                    EED= eed,
                    Status = status,
                    IsDeleted = false
                };
            }
            else
            {
                var carrierAccountManager = new CarrierAccountManager();
                return new Vanrise.AccountBalance.Entities.AccountInfo
                {
                    Name = carrierAccountManager.GetCarrierAccountName(carrierAccount),
                    CurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(carrierAccount),
                    BED = bed,
                    EED = eed,
                    Status = status,
                    IsDeleted = false
                };
            }
        }

        #endregion
    }
}
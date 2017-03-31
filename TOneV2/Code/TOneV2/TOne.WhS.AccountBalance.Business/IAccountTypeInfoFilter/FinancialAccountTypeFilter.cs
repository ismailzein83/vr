using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountTypeFilter : IAccountTypeInfoFilter
    {
        public int? CarrierProfileId { get; set; }
        public int? CarrierAccountId { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsMatched(IAccountTypeInfoFilterContext context)
        {
            var accountBalanceSetting = context.AccountType.Settings.ExtendedSettings as AccountBalanceSettings;
            if (accountBalanceSetting == null)
                return false;
            FinancialValidationData financialValidationData = context.CustomObject as FinancialValidationData;
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();

            if (financialValidationData == null)
            {
                financialValidationData = financialAccountManager.LoadFinancialValidationData(this.CarrierProfileId,this.CarrierAccountId,0);
                context.CustomObject = financialValidationData;
            }
            if(this.CarrierAccountId.HasValue)
            {
                return financialAccountManager.CheckFinancialCarrierAccountValidation(context.AccountType.VRComponentTypeId, accountBalanceSetting, financialValidationData.FinancialCarrierAccount.CarrierAccount, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierAccount.FinancialAccounts, this.IsEditMode);
            }
            else if (this.CarrierProfileId.HasValue)
            {
                return financialAccountManager.CheckFinancialCarrierProfileValidation(context.AccountType.VRComponentTypeId, accountBalanceSetting, financialValidationData.FinancialCarrierProfile.ProfileCarrierAccounts, financialValidationData.ProfileFinancialAccounts, financialValidationData.FinancialCarrierProfile.FinancialAccountsByAccount, this.IsEditMode);
            }
            return true;
        }
    }
   
}

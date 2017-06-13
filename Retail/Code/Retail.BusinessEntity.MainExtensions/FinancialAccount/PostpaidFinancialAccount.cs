using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.FinancialAccount
{
    public class PostpaidFinancialAccount : FinancialAccountExtendedSettings
    {
        public int CreditClassId { get; set; }

        static CreditClassManager s_creditClassManager = new CreditClassManager();
        public override void FillExtraData(IFinancialAccountFillExtraDataContext context)
        {
            context.FinancialAccountData.ThrowIfNull("context.FinancialAccountData");
            CreditClass creditClass = s_creditClassManager.GetCreditClass(this.CreditClassId);
            creditClass.ThrowIfNull("creditClass", this.CreditClassId);
            creditClass.Settings.ThrowIfNull("creditClass.Settings", this.CreditClassId);
            context.FinancialAccountData.CreditLimit = creditClass.Settings.BalanceLimit;
            context.FinancialAccountData.CreditLimitCurrencyId = creditClass.Settings.CurrencyId;
        }
    }
}

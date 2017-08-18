using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingSettings : WHSFinancialAccountExtendedSettings
    {
        public Decimal CustomerCreditLimit { get; set; }

        public Decimal SupplierCreditLimit { get; set; }

        public override void FillCustomerExtraData(IWHSFinancialAccountFillCustomerExtraDataContext context)
        {
            context.FinancialAccountData.ThrowIfNull("context.FinancialAccountData");
            if (context.FinancialAccountData.AccountBalanceData != null)
                context.FinancialAccountData.AccountBalanceData.CreditLimit = this.CustomerCreditLimit;
        }

        public override void FillSupplierExtraData(IWHSFinancialAccountFillSupplierExtraDataContext context)
        {
            context.FinancialAccountData.ThrowIfNull("context.FinancialAccountData");
            if (context.FinancialAccountData.AccountBalanceData != null)
                context.FinancialAccountData.AccountBalanceData.CreditLimit = this.SupplierCreditLimit;
        }
    }
}

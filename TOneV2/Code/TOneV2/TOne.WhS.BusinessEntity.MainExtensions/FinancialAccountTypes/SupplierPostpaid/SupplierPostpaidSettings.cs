using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPostpaid
{
    public class SupplierPostpaidSettings : WHSFinancialAccountExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public override void FillSupplierExtraData(IWHSFinancialAccountFillSupplierExtraDataContext context)
        {
            context.FinancialAccountData.ThrowIfNull("context.FinancialAccountData");
            if (context.FinancialAccountData.AccountBalanceData != null)
                context.FinancialAccountData.AccountBalanceData.CreditLimit = this.CreditLimit;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.CustomerPostpaid
{
    public class CustomerPostpaidSettings : WHSFinancialAccountExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public override void FillCustomerExtraData(IWHSFinancialAccountFillCustomerExtraDataContext context)
        {
            context.FinancialAccountData.ThrowIfNull("context.FinancialAccountData");
            if (context.FinancialAccountData.AccountBalanceData != null)
                context.FinancialAccountData.AccountBalanceData.CreditLimit = this.CreditLimit;
        }
    }
}

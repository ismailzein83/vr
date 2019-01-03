using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Business;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public enum AccountCreditLimitField { CreditLimit = 0, CreditLimitCurrency = 1, ConvertedCreditLimit = 2 }
    public class AccountCreditLimitPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("73E1C37B-BB45-442E-B7DA-19BA1597E0D8"); } }
        public AccountCreditLimitField AccountCreditLimitField { get; set; }
        public bool UseDescription { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            AccountCreditLimit accountCreditLimit = context.Object as AccountCreditLimit;
            accountCreditLimit.ThrowIfNull("accountCreditLimit");
            switch (AccountCreditLimitField)
            {
                case AccountCreditLimitField.ConvertedCreditLimit:
                    if(accountCreditLimit.CreditLimit.HasValue && accountCreditLimit.CreditLimitCurrencyId.HasValue && accountCreditLimit.AccountCurrencyId.HasValue)
                    {
                        var creditClassManager = new CreditClassManager();
                        return creditClassManager.GetConvertedCreditClassBalanceLimit(accountCreditLimit.CreditLimit.Value, accountCreditLimit.CreditLimitCurrencyId.Value, accountCreditLimit.AccountCurrencyId.Value);
                    }
                    break;
                case AccountCreditLimitField.CreditLimit:
                    if(accountCreditLimit.CreditLimit.HasValue)
                        return accountCreditLimit.CreditLimit.Value;
                    break;
                case AccountCreditLimitField.CreditLimitCurrency:
                    if (accountCreditLimit.CreditLimitCurrencyId.HasValue)
                    {
                        if (UseDescription)
                            return new CurrencyManager().GetCurrencyName(accountCreditLimit.CreditLimitCurrencyId.Value);
                        else
                            return accountCreditLimit.CreditLimitCurrencyId.Value;
                    }
                    break;
            }
            return null;
        }
    }
}

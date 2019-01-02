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
    public enum CreditClassField { CreditLimit = 0, CreditLimitCurrency = 1, ConvertedCreditLimit = 2 }
    public class CreditClassPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("73E1C37B-BB45-442E-B7DA-19BA1597E0D8"); } }
        public CreditClassField CreditClassField { get; set; }
        public bool UseDescription { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CreditClass creditClass = context.Object as CreditClass;
            creditClass.ThrowIfNull("creditClass");
            creditClass.Settings.ThrowIfNull("creditClass.Settings");
            switch (CreditClassField)
            {
                case CreditClassField.ConvertedCreditLimit:
                    return new CreditClassManager().GetConvertedCreditClassBalanceLimit(creditClass.Settings.BalanceLimit, creditClass.Settings.CurrencyId);
                case CreditClassField.CreditLimit:
                    return creditClass.Settings.BalanceLimit;
                case CreditClassField.CreditLimitCurrency:
                    if (UseDescription)
                        return new CurrencyManager().GetCurrencyName(creditClass.Settings.CurrencyId);
                    else
                        return creditClass.Settings.CurrencyId;
            }
            return null;
        }
    }
}

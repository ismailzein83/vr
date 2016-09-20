using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public enum FinancialField { CreditClass = 0, CurrencyID = 1 }
    public class FinancialRetailAccountPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("2fba38a1-b755-4089-a0bb-0258e598a7ee"); } }

        public FinancialField FinancialField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            RetailAccountObjectType retailAccountObjectType = context.ObjectType as RetailAccountObjectType;
            if (retailAccountObjectType == null)
                throw new NullReferenceException("retailAccountObjectType");
            var account = context.Object as Account;
            if (account == null)
                throw new NullReferenceException("account");

            foreach (var part in account.Settings.Parts.Values)
            {
                var actionpartSetting = part.Settings as IAccountPayment;
                if (actionpartSetting != null)
                {
                    switch (this.FinancialField)
                    {
                        case FinancialField.CreditClass:
                            return actionpartSetting.CreditClassId;
                        case FinancialField.CurrencyID:
                            return actionpartSetting.CurrencyId;
                    }
                }
            }
            return null;
        }
    }
}

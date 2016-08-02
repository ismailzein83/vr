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
        public FinancialField FinancialField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            RetailAccountObjectType retailAccountObjectType = context.ObjectType as RetailAccountObjectType;
            if (retailAccountObjectType == null)
                throw new NullReferenceException("retailAccountObjectType");
            string fieldName = null;
            switch (this.FinancialField)
            {
                case FinancialField.CreditClass: fieldName = FinancialField.CreditClass.ToString(); break;
                case FinancialField.CurrencyID: fieldName = FinancialField.CurrencyID.ToString(); break;
            }
            return context.Object[fieldName];
        }
    }
}

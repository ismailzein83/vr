using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public abstract class TransactionTaxRuleSettings
    {
        public abstract Guid ConfigId { get; }

        protected abstract void Execute(ITransactionTaxRuleContext context);

        public void ApplyTransactionTaxRule(ITransactionTaxRuleContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }

    public class OverallTransactionTaxRuleSettings : TransactionTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("F7E9938D-A0F9-45FE-B21F-193023B94BC0"); } }

        public int TaxPercentage { get; set; }
       
        protected override void Execute(ITransactionTaxRuleContext context)
        {
            if (context.TotalAmount.HasValue)
                context.TotalTaxValue = context.TotalAmount.Value * TaxPercentage / 100;
            else context.TotalTaxValue = null;
        }
        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Overall Transaction Tax Percentage: {0}", TaxPercentage));
            return description.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleSettings
    {
        public ResidualPrepaidTaxRuleSettings ResidualSettings { get; set; }
        public TransactionTaxRuleSettings TopUpSettings { get; set; }
        public string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            if (ResidualSettings != null)
            { }

            if (TopUpSettings != null)
            {
                if (description.Length > 0)
                    description.Append(" ");
                description.Append(TopUpSettings.GetDescription(context));
            }

            return description.ToString();
        }

        public void ApplyPrepaidTaxRule(IPrepaidTaxRuleContext context)
        {
            if (ResidualSettings != null && context.ResidualContext != null)
            { }

            if (TopUpSettings != null && context.TopUpContext != null)
            {
                TopUpSettings.ApplyTransactionTaxRule(context.TopUpContext);
            }
        }
    }

    public class ResidualPrepaidTaxRuleSettings
    {
    }
}

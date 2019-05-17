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
        public decimal? ResidualPercentage { get; set; }
        public decimal? TopUpsPercentage { get; set; }
        public string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            if(ResidualPercentage.HasValue)
                description.Append(string.Format("Overall Prepaid Residual Percentage: {0}.", ResidualPercentage.Value));
            if(TopUpsPercentage.HasValue)
                description.Append(string.Format("Overall Prepaid Top-Ups Percentage: {0}", TopUpsPercentage.Value));
            return description.ToString();
        }
        public void ApplyTaxRule(IPrepaidTaxRuleContext context)
        {
            if (context.TotalResidualAmount.HasValue && ResidualPercentage.HasValue)
                context.TotalTaxValue = context.TotalResidualAmount.Value * ResidualPercentage.Value / 100;
            if (context.TotalTopUpsAmount.HasValue && TopUpsPercentage.HasValue)
                context.TotalTaxValue += context.TotalTopUpsAmount.Value * TopUpsPercentage.Value / 100;
        }
    }
}

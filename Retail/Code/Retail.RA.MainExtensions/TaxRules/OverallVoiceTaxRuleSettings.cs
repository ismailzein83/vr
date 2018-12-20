using System;
using System.Collections.Generic;
using System.Text;
using Retail.RA.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.MainExtensions
{
    public class OverallVoiceTaxRuleSettingsA : VoiceTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("F3DFF6E3-3061-495F-99C6-7DFDEBD401EC"); } }

        public int TaxPercentage { get; set; }

        protected override void Execute(IVoiceTaxRuleContext context)
        {
            if (context.TotalAmount.HasValue)
                context.TotalTaxValue = context.TotalAmount.Value * TaxPercentage / 100;
            else context.TotalTaxValue = null;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> settingsValuesByName = new Dictionary<string, object>();
            settingsValuesByName.Add("Tax Percentage", TaxPercentage);
            return settingsValuesByName;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Overall Tax Percentage: {0}", TaxPercentage));
            return description.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Retail.RA.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.MainExtensions
{
    public class OverallSMSTaxRuleSettings : SMSTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("00E25E63-9C8A-46C0-955E-4A7382FB1918"); } }

        public int TaxPercentage { get; set; }

        protected override void Execute(ISMSTaxRuleContext context)
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
            description.Append(String.Format("Overall SMS Tax Percentage: {0}", TaxPercentage));
            return description.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public abstract class VoiceTaxRuleSettings
    {
        public abstract Guid ConfigId { get; }

        protected abstract void Execute(IVoiceTaxRuleContext context);

        public void ApplyVoiceTaxRule(IVoiceTaxRuleContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }

    public class PerMinuterVoiceTaxRuleSettings : VoiceTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("FC96DCD0-F1F4-45A0-88E4-569A6BC0FA13"); } }

        public decimal TaxPerMinute { get; set; }

        protected override void Execute(IVoiceTaxRuleContext context)
        {
            context.TotalTaxValue = (context.DurationInSeconds / 60) * TaxPerMinute;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> settingsValuesByName = new Dictionary<string, object>();
            settingsValuesByName.Add("Tax per minute", TaxPerMinute);
            return settingsValuesByName;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Tax per minute: {0}", TaxPerMinute));
            return description.ToString();
        }
    }

    public class OverallVoiceTaxRuleSettings : VoiceTaxRuleSettings
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
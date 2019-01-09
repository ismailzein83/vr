using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public abstract class SMSTaxRuleSettings
    {
        public abstract Guid ConfigId { get; }

        protected abstract void Execute(ISMSTaxRuleContext context);

        public void ApplySMSTaxRule(ISMSTaxRuleContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }

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

    public class PerSMSTaxRuleSettings : SMSTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("E9A50C64-A4D2-4704-BCC3-E48752AA69E6"); } }

        public decimal TaxPerSMS { get; set; }

        protected override void Execute(ISMSTaxRuleContext context)
        {
            context.TotalTaxValue = context.NumberOfSMSs * TaxPerSMS;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> settingsValuesByName = new Dictionary<string, object>();
            settingsValuesByName.Add("Tax per SMS", TaxPerSMS);
            return settingsValuesByName;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Tax per SMS: {0}", TaxPerSMS));
            return description.ToString();
        }
    }
}
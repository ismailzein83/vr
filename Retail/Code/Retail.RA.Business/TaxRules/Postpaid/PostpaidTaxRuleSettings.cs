using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PostpaidTaxRuleSettings
    {
        public PostpaidVoiceTaxRuleSettings VoiceSettings { get; set; }
        public PostpaidSMSTaxRuleSettings SMSSettings { get; set; }
        public PostpaidTransactionTaxRuleSettings TransactionSettings { get; set; }

        public string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            if (VoiceSettings != null)
            {
                if (VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.Overall && VoiceSettings.TaxPercentage.HasValue)
                {
                    description.Append(string.Format("Overall Postpaid Voice Tax Percentage: {0}.", VoiceSettings.TaxPercentage.Value));
                }
                else if (VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.PerMinute && VoiceSettings.TaxPerMinute.HasValue)
                {
                    if (description.Length > 0)
                        description.Append(" ");
                    description.Append(string.Format("Postpaid Voice Tax Per Minute: {0}.", VoiceSettings.TaxPerMinute.Value));
                }
            }
            if (SMSSettings != null)
            {
                if (SMSSettings.ChargeType == PostpaidSMSTaxChargeType.Overall && SMSSettings.TaxPercentage.HasValue)
                {
                    if (description.Length > 0)
                        description.Append(" ");
                    description.Append(string.Format("Overall Postpaid SMS Tax Percentage: {0}.", SMSSettings.TaxPercentage.Value));
                }
                else if (SMSSettings.ChargeType == PostpaidSMSTaxChargeType.PerSMS && SMSSettings.TaxPerSMS.HasValue)
                {
                    if (description.Length > 0)
                        description.Append(" ");
                    description.Append(string.Format("Postpaid SMS Tax Per SMS: {0}.", SMSSettings.TaxPerSMS.Value));
                }
            }
            if (TransactionSettings != null)
            {
                if (TransactionSettings.TaxPercentage.HasValue)
                {
                    if (description.Length > 0)
                        description.Append(" ");
                    description.Append(string.Format("Overall Postpaid Transaction Tax Percentage: {0}.", TransactionSettings.TaxPercentage.Value));
                }
            }
            return description.ToString();
        }

        public void ApplyPostpaidTaxRule(IPostpaidTaxRuleContext context)
        {
            if (VoiceSettings != null && context.VoiceContext != null)
            {
                if (VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.Overall && context.VoiceContext.TotalAmount.HasValue && VoiceSettings.TaxPercentage.HasValue)
                {
                    context.VoiceContext.TotalTaxValue = context.VoiceContext.TotalAmount.Value * VoiceSettings.TaxPercentage.Value / 100;
                }
                else if (VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.PerMinute && context.VoiceContext.DurationInSeconds.HasValue && VoiceSettings.TaxPerMinute.HasValue)
                {
                    context.VoiceContext.TotalTaxValue = (context.VoiceContext.DurationInSeconds.Value / 60) * VoiceSettings.TaxPerMinute.Value;
                }
            }
            if (SMSSettings != null && context.SMSContext != null)
            {
                if (SMSSettings.ChargeType == PostpaidSMSTaxChargeType.Overall && context.SMSContext.TotalAmount.HasValue && SMSSettings.TaxPercentage.HasValue)
                {
                    context.SMSContext.TotalTaxValue = context.SMSContext.TotalAmount.Value * SMSSettings.TaxPercentage.Value / 100;
                }
                else if (SMSSettings.ChargeType == PostpaidSMSTaxChargeType.PerSMS && context.SMSContext.NumberOfSMS.HasValue && SMSSettings.TaxPerSMS.HasValue)
                {
                    context.SMSContext.TotalTaxValue = context.SMSContext.NumberOfSMS.Value * SMSSettings.TaxPerSMS.Value;
                }
            }
            if (TransactionSettings != null && context.TransactionContext != null)
            {
                if (TransactionSettings.TaxPercentage.HasValue && context.TransactionContext.TotalAmount.HasValue)
                {
                    context.TransactionContext.TotalTaxValue = context.TransactionContext.TotalAmount.Value * TransactionSettings.TaxPercentage.Value / 100;
                }
            }
        }
    }

    public enum PostpaidVoiceTaxChargeType { Overall = 0, PerMinute = 1}
    public class PostpaidVoiceTaxRuleSettings
    {
        public PostpaidVoiceTaxChargeType ChargeType { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxPerMinute { get; set; }
    }

    public enum PostpaidSMSTaxChargeType { Overall = 0, PerSMS = 1}
    public class PostpaidSMSTaxRuleSettings
    {
        public PostpaidSMSTaxChargeType ChargeType { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxPerSMS { get; set; }
    }

    public class PostpaidTransactionTaxRuleSettings
    {
        public decimal? TaxPercentage { get; set; }
    }
}
   

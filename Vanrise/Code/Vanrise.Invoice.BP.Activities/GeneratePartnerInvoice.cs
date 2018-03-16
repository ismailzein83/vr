using System;
using System.Activities;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business.Context;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Invoice.BP.Arguments;
using System.Linq;

namespace Vanrise.Invoice.BP.Activities
{
    public sealed class GeneratePartnerInvoice : BaseCodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<PartnerInvoiceGenerationDraft> PartnerInvoiceGenerationDraft { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsAutomatic { get; set; }
        public InArgument<InvoiceGapAction> InvoiceGapAction { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> IssueDate { get; set; }

        [RequiredArgument]
        public OutArgument<bool> GenerationErrorOccured { get; set; }

        [RequiredArgument]
        public OutArgument<Exception> GenerationErrorException { get; set; }
        [RequiredArgument]
        public InArgument<int> RemainingInvoices { get; set; }
        [RequiredArgument]
        public OutArgument<int> ProccessedInvoices { get; set; }
        [RequiredArgument]
        public OutArgument<int> SucceededInvoices { get; set; }

        #endregion

        protected override void VRExecute(IBaseCodeActivityContext context)
        {

            PartnerInvoiceGenerationDraft partnerInvoiceGenerationDraft = context.ActivityContext.GetValue(this.PartnerInvoiceGenerationDraft);
            partnerInvoiceGenerationDraft.ThrowIfNull("partnerInvoiceGenerationDraft");

            var partnerId = partnerInvoiceGenerationDraft.PartnerId;
            var invoiceTypeId = partnerInvoiceGenerationDraft.InvoiceTypeId;
            var isAutomatic = context.ActivityContext.GetValue(this.IsAutomatic);
            var issueDate = context.ActivityContext.GetValue(this.IssueDate);
            var invoiceGapAction = context.ActivityContext.GetValue(this.InvoiceGapAction);

            InvoiceManager invoiceManager = new InvoiceManager();
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();

            var remainingInvoices = context.ActivityContext.GetValue(this.RemainingInvoices);

            int succeededInvoices = 0;
            int proccessedInvoices = 0;

            if (partnerId != null)
            {
                GenerateInvoiceInput generateInvoiceInput = new GenerateInvoiceInput
                {
                    InvoiceTypeId = invoiceTypeId,
                    IsAutomatic = isAutomatic,
                    IssueDate = issueDate,
                    PartnerId = partnerId,
                    Items = new List<GenerateInvoiceInputItem>()
                };

                var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType.Settings", invoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType.Settings.ExtendedSettings", invoiceTypeId);

                var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                invoiceTypePartnerManager.ThrowIfNull("invoiceTypePartnerManager");

                PartnerManager partnerManager = new PartnerManager();
                var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);
                partnerSetting.ThrowIfNull("partnerSetting", partnerId);
                partnerSetting.ThrowIfNull(" partnerSetting.InvoiceSetting", partnerId);


                BillingPeriodInvoiceSettingPart billingPeriodInvoiceSettingPart = invoiceTypePartnerManager.GetInvoicePartnerSettingPart<BillingPeriodInvoiceSettingPart>(
                    new InvoicePartnerSettingPartContext
                    {
                        InvoiceSettingId = partnerSetting.InvoiceSetting.InvoiceSettingId,
                        InvoiceTypeId = invoiceTypeId,
                        PartnerId = partnerId
                    });

                if (partnerInvoiceGenerationDraft.Items != null && partnerInvoiceGenerationDraft.Items.Count > 0)
                {

                    proccessedInvoices = partnerInvoiceGenerationDraft.Items.Count;
                    this.ProccessedInvoices.Set(context.ActivityContext, proccessedInvoices);


                    DateTime partnerIssueDate = partnerInvoiceGenerationDraft.Items.Max(x => x.To).AddDays(1);
                    List<BillingInterval> billingPeriods = null;
                    if (billingPeriodInvoiceSettingPart != null && billingPeriodInvoiceSettingPart.FollowBillingPeriod)
                    {
                        billingPeriods = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, partnerIssueDate);
                    }

                    for (var i = 0; i < partnerInvoiceGenerationDraft.Items.Count; i++)
                    {
                        var item = partnerInvoiceGenerationDraft.Items[i];

                        if (billingPeriodInvoiceSettingPart != null && billingPeriodInvoiceSettingPart.FollowBillingPeriod)
                        {
                            if (billingPeriods == null || !billingPeriods.Any(x => x.FromDate == item.From && x.ToDate == item.To))
                            {
                                WriteErrorMessages(partnerInvoiceGenerationDraft.Items, partnerInvoiceGenerationDraft.PartnerName, "Invalid Billing Period", item.InvoiceGenerationDraftId, remainingInvoices, context);
                                return;
                            }
                        }
                        if (partnerInvoiceGenerationDraft.Items.Any(x => item.InvoiceGenerationDraftId != x.InvoiceGenerationDraftId && Utilities.AreTimePeriodsOverlapped(x.From, x.To, item.From, item.To)))
                        {
                            WriteErrorMessages(partnerInvoiceGenerationDraft.Items, partnerInvoiceGenerationDraft.PartnerName, "'Overlapped Billing Period", item.InvoiceGenerationDraftId, remainingInvoices, context);
                            return;
                        }

                        var invoiceGenerationGap = invoiceManager.CheckGeneratedInvoicePeriodGaP(item.From, invoiceTypeId, partnerId);
                        if (invoiceGenerationGap.HasValue)
                        {
                            switch (invoiceGapAction)
                            {
                                case Entities.InvoiceGapAction.GenerateInvoice:
                                    break;
                                case Entities.InvoiceGapAction.SkipInvoice:
                                    WriteErrorMessages(partnerInvoiceGenerationDraft.Items, partnerInvoiceGenerationDraft.PartnerName, string.Format("invoice must be generated from date {0:yyyy-MM-dd}'", invoiceGenerationGap.Value), item.InvoiceGenerationDraftId, remainingInvoices, context);
                                    return;
                            }
                        }
                        generateInvoiceInput.Items.Add(new GenerateInvoiceInputItem
                        {
                            CustomSectionPayload = item.CustomPayload,
                            FromDate = item.From,
                            ToDate = item.To
                        });

                    }
                }


                if (generateInvoiceInput.Items.Count > 0)
                {
                    var generatedInvoices = invoiceManager.GenerateInvoice(generateInvoiceInput);
                    if (generatedInvoices != null)
                    {

                        for (var i = 0; i < generatedInvoices.Count; i++)
                        {
                            var generatedInvoice = generatedInvoices[i];
                            if (generatedInvoice.IsSucceeded)
                            {
                                context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Information, "Invoice generated for '{0}' from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Remaining Invoices: {3}", partnerInvoiceGenerationDraft.PartnerName, generatedInvoice.FromDate, generatedInvoice.ToDate, remainingInvoices - (i + 1));
                                if (isAutomatic)
                                {
                                    AutomaticInvoiceActionsPart automaticInvoiceActionsPart = invoiceTypePartnerManager.GetInvoicePartnerSettingPart<AutomaticInvoiceActionsPart>(
                                        new InvoicePartnerSettingPartContext
                                        {
                                            InvoiceSettingId = partnerSetting.InvoiceSetting.InvoiceSettingId,
                                            InvoiceTypeId = invoiceTypeId,
                                            PartnerId = partnerId
                                        });
                                    if (automaticInvoiceActionsPart != null && automaticInvoiceActionsPart.Actions != null)
                                    {
                                        try
                                        {
                                            invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.AutomaticInvoiceActions");

                                            foreach (var action in automaticInvoiceActionsPart.Actions)
                                            {

                                                var automaticInvoiceAction = invoiceType.Settings.AutomaticInvoiceActions.FindRecord(x => x.AutomaticInvoiceActionId == action.AutomaticInvoiceActionId);
                                                automaticInvoiceAction.ThrowIfNull("automaticInvoiceAction");

                                                AutomaticActionRuntimeSettingsContext automaticActionContext = new Business.Context.AutomaticActionRuntimeSettingsContext
                                                {
                                                    Invoice = generatedInvoice.Invoice,
                                                    DefinitionSettings = automaticInvoiceAction.Settings
                                                };
                                                action.Settings.Execute(automaticActionContext);
                                                if (automaticActionContext.ErrorMessage != null)
                                                {
                                                    context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Warning, "{0} Account '{1}'. Remaining Invoices: {2}", automaticActionContext.ErrorMessage, partnerInvoiceGenerationDraft.PartnerName, remainingInvoices - (i + 1));
                                                }
                                                else
                                                {
                                                    context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Information, "Invoice action '{0}' executed successfully for account '{1}'. Remaining Invoices: {2}", automaticInvoiceAction.Title, partnerInvoiceGenerationDraft.PartnerName, remainingInvoices - (i + 1));
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            this.GenerationErrorOccured.Set(context.ActivityContext, true);
                                            this.GenerationErrorException.Set(context.ActivityContext, ex);
                                            this.SucceededInvoices.Set(context.ActivityContext, succeededInvoices);
                                            return;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (generatedInvoice.Message != null)
                                {
                                    context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Warning, "Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : {3}. Remaining Invoices: {4}", partnerInvoiceGenerationDraft.PartnerName, generatedInvoice.FromDate, generatedInvoice.ToDate, generatedInvoice.Message.Message, remainingInvoices - (i + 1));
                                }
                            }
                        }
                    }

                }

                this.SucceededInvoices.Set(context.ActivityContext, succeededInvoices);
            }
        }

        private void WriteErrorMessages(List<PartnerInvoiceGenerationDraftItem> items,string partnerName, string errorMessage, long invoiceGenerationDraftId, int remainingInvoices, IBaseCodeActivityContext context)
        {
            var generateInvoiceOutputs = new List<GenerateInvoiceOutput>();

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if(item.InvoiceGenerationDraftId == invoiceGenerationDraftId)
                {
                    context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Warning, "Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : '{3}'. Remaining Invoices: {4}", partnerName, item.From, item.To, errorMessage, --remainingInvoices);
                }
                else
                {
                    context.ActivityContext.WriteBusinessTrackingMsg(LogEntryType.Warning, "Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : 'Stopped due to an error occurred in the generation of related invoice'. Remaining Invoices: {3}", partnerName, item.From, item.To, --remainingInvoices);
                }
               
            }
        }
    }
}
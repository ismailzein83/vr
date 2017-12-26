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
namespace Vanrise.Invoice.BP.Activities
{
    public sealed class GeneratePartnerInvoice : BaseCodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<InvoiceGenerationDraft> InvoiceGenerationDraft { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsAutomatic { get; set; }
        public InArgument<InvoiceGapAction> InvoiceGapAction { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> IssueDate { get; set; }

        [RequiredArgument]
        public OutArgument<bool> Succeeded { get; set; }

        [RequiredArgument]
        public OutArgument<List<InvoiceGenerationMessageOutput>> Messages { get; set; }

        [RequiredArgument]
        public OutArgument<bool> GenerationErrorOccured { get; set; }

        [RequiredArgument]
        public OutArgument<Exception> GenerationErrorException { get; set; }


        #endregion

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            InvoiceGenerationDraft invoiceGenerationDraft = context.ActivityContext.GetValue(this.InvoiceGenerationDraft);
            var partnerId = invoiceGenerationDraft.PartnerId;
            var invoiceTypeId = invoiceGenerationDraft.InvoiceTypeId;
            var isAutomatic = context.ActivityContext.GetValue(this.IsAutomatic);
            var issueDate = context.ActivityContext.GetValue(this.IssueDate);
            var invoiceGapAction = context.ActivityContext.GetValue(this.InvoiceGapAction);
            List<InvoiceGenerationMessageOutput> messages = new List<InvoiceGenerationMessageOutput>();
            InvoiceManager invoiceManager = new InvoiceManager();
            InvoiceSettingManager invoiceSettingManager = new Business.InvoiceSettingManager();
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
           
            if (partnerId != null)
            {
                var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
                var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                PartnerManager partnerManager = new PartnerManager();
                var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);

                //if (isAutomatic)
                //{
                //    AutomaticInvoiceSettingPart automaticInvoiceSettingPart = invoiceTypePartnerManager.GetInvoicePartnerSettingPart<AutomaticInvoiceSettingPart>(
                //    new InvoicePartnerSettingPartContext
                //    {
                //        InvoiceSettingId = partnerSetting.InvoiceSetting.InvoiceSettingId,
                //        InvoiceTypeId = invoiceTypeId,
                //        PartnerId = partnerId
                //    });

                //    if (automaticInvoiceSettingPart == null || !automaticInvoiceSettingPart.IsEnabled)
                //    {
                //        this.Succeeded.Set(context.ActivityContext, false);
                //        this.LogEntryType.Set(context.ActivityContext, Vanrise.Entities.LogEntryType.Warning);
                //        this.Message.Set(context.ActivityContext, string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : 'Automatic invoice not enabled for this partner'", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To));
                //        return;
                //    }
                //}

                BillingPeriodInvoiceSettingPart billingPeriodInvoiceSettingPart = invoiceTypePartnerManager.GetInvoicePartnerSettingPart<BillingPeriodInvoiceSettingPart>(
                    new InvoicePartnerSettingPartContext
                    {
                        InvoiceSettingId = partnerSetting.InvoiceSetting.InvoiceSettingId,
                        InvoiceTypeId = invoiceTypeId,
                        PartnerId = partnerId
                    });

                if (billingPeriodInvoiceSettingPart != null && billingPeriodInvoiceSettingPart.FollowBillingPeriod)
                {
                    DateTime partnerIssueDate = invoiceGenerationDraft.To.Date.AddDays(1);
                    var billingPeriod = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, partnerIssueDate);
                    if (billingPeriod == null || billingPeriod.FromDate != invoiceGenerationDraft.From || billingPeriod.ToDate != invoiceGenerationDraft.To)
                    {
                        this.Succeeded.Set(context.ActivityContext, false);
                        messages.Add(new InvoiceGenerationMessageOutput
                        {
                            LogEntryType = Vanrise.Entities.LogEntryType.Warning,
                            Message = string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : 'Invalid Billing Period'", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To)
                        });
                        this.Messages.Set(context.ActivityContext, messages);
                        return;
                    }
                }
                var invoiceGenerationGap = invoiceManager.CheckGeneratedInvoicePeriodGaP(invoiceGenerationDraft.From,invoiceTypeId,partnerId);
                if (invoiceGenerationGap.HasValue)
                {
                    switch (invoiceGapAction)
                    {
                        case Entities.InvoiceGapAction.GenerateInvoice:
                            break;
                        case Entities.InvoiceGapAction.SkipInvoice:
                            messages.Add(new InvoiceGenerationMessageOutput
                            {
                                LogEntryType = Vanrise.Entities.LogEntryType.Warning,
                                Message = string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : 'invoice must be generated from date {3:yyyy-MM-dd}.'", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To, invoiceGenerationGap.Value)
                            });
                            this.Messages.Set(context.ActivityContext, messages);
                            return;
                    }
                }
                bool succeeded;
                var generatedInvoice = invoiceManager.GenerateInvoice(new Entities.GenerateInvoiceInput
                {
                    InvoiceTypeId = invoiceTypeId,
                    IssueDate = issueDate,
                    PartnerId = partnerId,
                    FromDate = invoiceGenerationDraft.From,
                    ToDate = invoiceGenerationDraft.To,
                    IsAutomatic = isAutomatic,
                    CustomSectionPayload = invoiceGenerationDraft.CustomPayload
                });

                if (generatedInvoice.Result == InsertOperationResult.Succeeded)
                {
                    succeeded = true;

                    messages.Add(new InvoiceGenerationMessageOutput
                    {
                        LogEntryType = Vanrise.Entities.LogEntryType.Information,
                        Message = string.Format("Invoice generated for '{0}' from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To)
                    });
                   
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
                                        Invoice = generatedInvoice.InsertedObject.Entity,
                                        DefinitionSettings = automaticInvoiceAction.Settings
                                    };
                                    action.Settings.Execute(automaticActionContext);
                                    if (automaticActionContext.ErrorMessage != null)
                                    {
                                        messages.Add(new InvoiceGenerationMessageOutput
                                        {
                                            LogEntryType = Vanrise.Entities.LogEntryType.Warning,
                                            Message = string.Format("{0} Account '{1}'", automaticActionContext.ErrorMessage, invoiceGenerationDraft.PartnerName)
                                        });
                                    }
                                 }
                            }
                            catch(Exception ex)
                            {
                                this.Messages.Set(context.ActivityContext, messages);
                                this.GenerationErrorOccured.Set(context.ActivityContext, true);
                                this.GenerationErrorException.Set(context.ActivityContext, ex);
                                return;
                            }
                        }
                    }
                   
                }
                else
                {
                    succeeded = false;
                    messages.Add(new InvoiceGenerationMessageOutput
                    {
                        LogEntryType = Vanrise.Entities.LogEntryType.Warning,
                        Message = string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : {3}", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To, generatedInvoice.Message)
                    });
                }

                this.Succeeded.Set(context.ActivityContext, succeeded);
                this.Messages.Set(context.ActivityContext, messages);
            }
        }
    }
}
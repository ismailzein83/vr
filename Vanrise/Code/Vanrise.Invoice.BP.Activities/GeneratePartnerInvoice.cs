using System;
using System.Activities;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business.Context;

namespace Vanrise.Invoice.BP.Activities
{
    public sealed class GeneratePartnerInvoice : BaseCodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<InvoiceGenerationDraft> InvoiceGenerationDraft { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsAutomatic { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> IssueDate { get; set; }

        [RequiredArgument]
        public OutArgument<bool> Succeeded { get; set; }

        [RequiredArgument]
        public OutArgument<string> Message { get; set; }

        [RequiredArgument]
        public OutArgument<LogEntryType> LogEntryType { get; set; }

        #endregion

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            InvoiceGenerationDraft invoiceGenerationDraft = context.ActivityContext.GetValue(this.InvoiceGenerationDraft);
            var partnerId = invoiceGenerationDraft.PartnerId;
            var invoiceTypeId = invoiceGenerationDraft.InvoiceTypeId;
            var isAutomatic = context.ActivityContext.GetValue(this.IsAutomatic);
            var issueDate = context.ActivityContext.GetValue(this.IssueDate);

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
                    DateTime partnerIssueDate = invoiceGenerationDraft.To.AddDays(1);
                    var billingPeriod = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, partnerIssueDate);
                    if (billingPeriod == null || billingPeriod.FromDate != invoiceGenerationDraft.From || billingPeriod.ToDate != invoiceGenerationDraft.To)
                    {
                        this.Succeeded.Set(context.ActivityContext, false);
                        this.LogEntryType.Set(context.ActivityContext, Vanrise.Entities.LogEntryType.Warning);
                        this.Message.Set(context.ActivityContext, string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : 'Invalid Billing Period'", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To));
                        return;
                    }
                }

                bool succeeded;
                LogEntryType logentryType;
                string message;

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
                            foreach (var action in automaticInvoiceActionsPart.Actions)
                            {
                                AutomaticSendEmailActionRuntimeSettingsContext automaticSendEmailActionContext = new Business.Context.AutomaticSendEmailActionRuntimeSettingsContext
                                {
                                    Invoice = generatedInvoice.InsertedObject.Entity,
                                    AutomaticInvoiceActionId = action.AutomaticInvoiceActionId
                                };
                                action.Settings.Execute(automaticSendEmailActionContext);
                            };
                        }
                    }
                    succeeded = true;
                    logentryType = Vanrise.Entities.LogEntryType.Information;
                    message = string.Format("Invoice generated for '{0}' from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To);
                }
                else
                {
                    succeeded = false;
                    logentryType = Vanrise.Entities.LogEntryType.Warning;
                    message = string.Format("Invoice not generated for {0} from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}. Reason : {3}", invoiceGenerationDraft.PartnerName, invoiceGenerationDraft.From, invoiceGenerationDraft.To, generatedInvoice.Message);
                }

                this.Succeeded.Set(context.ActivityContext, succeeded);
                this.LogEntryType.Set(context.ActivityContext, logentryType);
                this.Message.Set(context.ActivityContext, message);
            }
        }
    }
}
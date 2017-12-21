using System;
using System.Activities;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;
namespace Vanrise.Invoice.BP.Activities
{
    public sealed class GenerateAutomaticInvoiceGenerationDrafts : BaseCodeActivity
    {
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceTypeId { get; set; }

        [RequiredArgument]
        public InArgument<int> EndDateOffsetFromToday { get; set; }

        [RequiredArgument]
        public InArgument<int> IssueDateOffsetFromToday { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool?> IsEffectiveInFuture { get; set; }

        [RequiredArgument]
        public InArgument<VRAccountStatus> Status { get; set; }

        [RequiredArgument]
        public InArgument<PartnerGroup> PartnerGroup { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> IssueDate { get; set; }

        [RequiredArgument]
        public OutArgument<Guid> InvoiceGenerationIdentifier { get; set; }

        [RequiredArgument]
        public OutArgument<InvoiceGenerationDraftOutput> InvoiceGenerationDraftOutput { get; set; }

        #endregion

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            DateTime todayDate = DateTime.Today;
            int issueDateOffsetFromToday = this.IssueDateOffsetFromToday.Get(context.ActivityContext);
            DateTime issueDate = todayDate.AddDays(-issueDateOffsetFromToday);

            int endDateOffsetFromToday = this.EndDateOffsetFromToday.Get(context.ActivityContext);
            DateTime maximumToDate = DateTime.Today.AddDays(-endDateOffsetFromToday);

            Guid invoiceGenerationIdentifier = Guid.NewGuid();

            InvoiceGenerationDraftQuery query = new InvoiceGenerationDraftQuery()
            {
                InvoiceTypeId = this.InvoiceTypeId.Get(context.ActivityContext),
                EffectiveDate = this.EffectiveDate.Get(context.ActivityContext),
                IsEffectiveInFuture = this.IsEffectiveInFuture.Get(context.ActivityContext),
                Status = this.Status.Get(context.ActivityContext),
                PartnerGroup = this.PartnerGroup.Get(context.ActivityContext),
                Period = InvoicePartnerPeriod.FollowBillingCycle,
                FromDate = null,
                ToDate = null,
                MaximumToDate = maximumToDate,
                IssueDate = issueDate,
                IsAutomatic = true,
                InvoiceGenerationIdentifier = invoiceGenerationIdentifier
            };
            InvoiceGenerationDraftOutput invoiceGenerationDraftOutput = new InvoiceGenerationDraftManager().GenerateFilteredInvoiceGenerationDrafts(query);

            if (invoiceGenerationDraftOutput.Result == InvoiceGenerationDraftResult.Succeeded && invoiceGenerationDraftOutput.InvalidPartnerMessages != null)
            {
                foreach (string message in invoiceGenerationDraftOutput.InvalidPartnerMessages)
                {
                    context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Warning, message);
                }
            }

            this.IssueDate.Set(context.ActivityContext, issueDate);
            this.InvoiceGenerationIdentifier.Set(context.ActivityContext, invoiceGenerationIdentifier);
            this.InvoiceGenerationDraftOutput.Set(context.ActivityContext, invoiceGenerationDraftOutput);
        }
    }
}

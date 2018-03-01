using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions;
using System.IO;
namespace Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions
{
    public class RecreateInvoiceBulkActionRuntimeSettings : AutomaticInvoiceActionRuntimeSettings
    {
        public bool IncludeSentInvoices { get; set; }
        public bool IncludePaidInvoices { get; set; }
        public override bool ShouldWaitImport { get { return true; } }
        public override void Execute(IAutomaticActionRuntimeSettingsContext context)
        {
            string errorMessage;
            bool isErrorOccured;

            if (ValidateCanRecreate(context.Invoice, out  errorMessage, out isErrorOccured))
            {
                ExecuteRecreateAction(context);
            }else
            {
                context.IsErrorOccured = isErrorOccured;
                context.ErrorMessage = errorMessage;
            }
        }

        bool ValidateCanRecreate(Entities.Invoice invoice, out string errorMessage, out bool isErrorOccured)
        {
            errorMessage = null;
            isErrorOccured = false;
            if (invoice.PaidDate.HasValue && !IncludePaidInvoices)
            {
                errorMessage = "Cannot recreate invoice. Reason: 'Invoice' already paid.";
                return false;
            }
            if (invoice.SentDate.HasValue && !IncludeSentInvoices)
            {
                errorMessage = "Cannot recreate invoice. Reason: 'Invoice' already sent.";
                return false;
            }
            return true;
        }
        private void ExecuteRecreateAction(IAutomaticActionRuntimeSettingsContext context)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.Invoice.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", context.Invoice.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            InvoiceManager invoiceManager = new InvoiceManager();
            var getGenerationCustomPayloadContext = new GetGenerationCustomPayloadContext
            {
                InvoiceGenerationInfo = new List<InvoiceGenerationInfo> {
                    new InvoiceGenerationInfo
                    {
                       PartnerId = context.Invoice.PartnerId,
                       FromDate = context.Invoice.FromDate,
                       ToDate = context.Invoice.ToDate,
                    },
                },
                InvoiceTypeId = context.Invoice.InvoiceTypeId,
                MaximumTo = context.Invoice.FromDate,
                MinimumFrom = context.Invoice.ToDate,
            };
            if (invoiceType.Settings.ExtendedSettings != null && invoiceType.Settings.ExtendedSettings.GenerationCustomSection != null)
            {
                invoiceType.Settings.ExtendedSettings.GenerationCustomSection.EvaluateGenerationCustomPayload(getGenerationCustomPayloadContext);
            }

            var invoiceGenerationInfo = getGenerationCustomPayloadContext.InvoiceGenerationInfo.FirstOrDefault();
            invoiceGenerationInfo.ThrowIfNull("invoiceGenerationInfo");

            var regenerateOutput = invoiceManager.ReGenerateInvoice(new GenerateInvoiceInput
            {
                CustomSectionPayload = invoiceGenerationInfo.CustomPayload,
                FromDate = context.Invoice.FromDate,
                InvoiceId = context.Invoice.InvoiceId,
                InvoiceTypeId = context.Invoice.InvoiceTypeId,
                IssueDate = context.Invoice.IssueDate,
                PartnerId = context.Invoice.PartnerId,
                ToDate = context.Invoice.ToDate
            });
            if (regenerateOutput.Result == UpdateOperationResult.Failed)
            {
                context.ErrorMessage = regenerateOutput.Message;
                context.IsErrorOccured = true;
            }
        }
    }
}

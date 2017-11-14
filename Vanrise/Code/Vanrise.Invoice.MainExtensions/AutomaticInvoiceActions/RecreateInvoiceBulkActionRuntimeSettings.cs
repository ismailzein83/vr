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
        public override void Execute(IAutomaticActionRuntimeSettingsContext context)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.Invoice.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", context.Invoice.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            InvoiceManager invoiceManager = new InvoiceManager();
            var  getGenerationCustomPayloadContext  = new GetGenerationCustomPayloadContext {
                InvoiceTypeId= context.Invoice.InvoiceTypeId,
                PartnerId=context.Invoice.PartnerId
            };
            dynamic customSectionPayload = null;
            if( invoiceType.Settings.ExtendedSettings != null &&  invoiceType.Settings.ExtendedSettings.GenerationCustomSection != null)
            {
                 customSectionPayload = invoiceType.Settings.ExtendedSettings.GenerationCustomSection.GetGenerationCustomPayload(getGenerationCustomPayloadContext);
            }
           var regenerateOutput= invoiceManager.ReGenerateInvoice(new GenerateInvoiceInput
            {
                CustomSectionPayload = customSectionPayload,
                FromDate= context.Invoice.FromDate,
                InvoiceId= context.Invoice.InvoiceId,
                InvoiceTypeId= context.Invoice.InvoiceTypeId,
                IssueDate= context.Invoice.IssueDate,
                PartnerId= context.Invoice.PartnerId,
                ToDate= context.Invoice.ToDate
            });
           if (regenerateOutput.Result == UpdateOperationResult.Failed)
           {
               context.ErrorMessage = regenerateOutput.Message;
           }
        }
    }
}

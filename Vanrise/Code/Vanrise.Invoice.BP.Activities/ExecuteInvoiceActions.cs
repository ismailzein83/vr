using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Queueing;
using Vanrise.Common;
namespace Vanrise.Invoice.BP.Activities
{
    #region Argument Classes

    public class ExecuteInvoiceActionsInput
    {
        public BaseQueue<Entities.Invoice> InputQueue { get; set; }
        public List<InvoiceBulkActionRuntime> InvoiceBulkActions { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }

    #endregion

    public class ExecuteInvoiceActions : DependentAsyncActivity<ExecuteInvoiceActionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<Entities.Invoice>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<InvoiceBulkActionRuntime>> InvoiceBulkActions { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InvoiceTypeId { get; set; }
        #endregion
        protected override void DoWork(ExecuteInvoiceActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (invoiceQueue) =>
                        {
                            counter++;
                            ExecuteInvoiceActionsMethod(invoiceQueue, inputArgument.InvoiceTypeId, inputArgument.InvoiceBulkActions, handle);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }
        protected override ExecuteInvoiceActionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteInvoiceActionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                InvoiceBulkActions = this.InvoiceBulkActions.Get(context),
                InvoiceTypeId = this.InvoiceTypeId.Get(context),
            };
        }
        private void ExecuteInvoiceActionsMethod(Entities.Invoice invoiceQueue, Guid invoiceTypeId, List<InvoiceBulkActionRuntime> invoiceBulkActions, AsyncActivityHandle handle)
        {
            var invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            invoiceType.Settings.InvoiceBulkActions.ThrowIfNull("invoiceType.Settings.InvoiceBulkActions");
            PartnerManager partnerManager = new PartnerManager();
            var partnerName = partnerManager.GetPartnerName(invoiceQueue.InvoiceTypeId, invoiceQueue.PartnerId);
            if (invoiceBulkActions != null)
            {
                try
                {
                    foreach (var invoiceBulkAction in invoiceBulkActions)
                    {
                        var invoiceBulkActionDefinition = invoiceType.Settings.InvoiceBulkActions.FindRecord(x => x.InvoiceBulkActionId == invoiceBulkAction.InvoiceBulkActionId);
                        invoiceBulkActionDefinition.ThrowIfNull("invoiceBulkActionDefinition");

                        var actionContext = new AutomaticActionRuntimeSettingsContext
                        {
                            Invoice = invoiceQueue,
                            DefinitionSettings = invoiceBulkActionDefinition.Settings
                        };
                        invoiceBulkAction.Settings.Execute(actionContext);
                        if (actionContext.ErrorMessage != null)
                            handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, string.Format("{0}", actionContext.ErrorMessage));
                        else
                            handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "{0} completed successfully for invoice for account '{1}' having serial number '{2}' .", invoiceBulkActionDefinition.Title, partnerName, invoiceQueue.SerialNumber);
                    }
                }
                catch (Exception ex)
                {
                    handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Error, string.Format("Error occured while executing action for '{0}' having serial number '{1}' , Reason: {2}", partnerName, invoiceQueue.SerialNumber, ex.Message));
                    handle.SharedInstanceData.WriteHandledException(ex, true);
                }
            }
        }
    }
}

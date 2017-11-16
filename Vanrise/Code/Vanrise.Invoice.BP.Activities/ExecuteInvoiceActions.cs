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
using Vanrise.Invoice.BP.Arguments;
namespace Vanrise.Invoice.BP.Activities
{
    #region Argument Classes

    public class ExecuteInvoiceActionsInput
    {
        public BaseQueue<Entities.Invoice> InputQueue { get; set; }
        public List<InvoiceBulkActionRuntime> InvoiceBulkActions { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public HandlingErrorOption HandlingErrorOption { get; set; }

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
     
        [RequiredArgument]
        public InArgument<HandlingErrorOption> HandlingErrorOption { get; set; }
        #endregion
        protected override void DoWork(ExecuteInvoiceActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(inputArgument.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", inputArgument.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            invoiceType.Settings.InvoiceBulkActions.ThrowIfNull("invoiceType.Settings.InvoiceBulkActions");
            List<InvoiceBulkActionPreparedEntity> invoiceBulkActionPreparedEntities = new List<InvoiceBulkActionPreparedEntity>();
            if (inputArgument.InvoiceBulkActions != null)
            {
                foreach (var invoiceBulkAction in inputArgument.InvoiceBulkActions)
                {
                    var invoiceBulkActionDefinition = invoiceType.Settings.InvoiceBulkActions.FindRecord(x => x.InvoiceBulkActionId == invoiceBulkAction.InvoiceBulkActionId);
                    invoiceBulkActionDefinition.ThrowIfNull("invoiceBulkActionDefinition");
                    invoiceBulkActionPreparedEntities.Add(new InvoiceBulkActionPreparedEntity
                    {
                        InvoiceBulkActionDefinition = invoiceBulkActionDefinition,
                        InvoiceBulkActionRuntime = invoiceBulkAction
                    });
                }
            }


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
                            ExecuteInvoiceActionsMethod(invoiceQueue, invoiceBulkActionPreparedEntities,inputArgument.HandlingErrorOption, handle);
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
                HandlingErrorOption = this.HandlingErrorOption.Get(context),
            };
        }
        private void ExecuteInvoiceActionsMethod(Entities.Invoice invoiceQueue, List<InvoiceBulkActionPreparedEntity> invoiceBulkActionPreparedEntities, HandlingErrorOption handlingErrorOption, AsyncActivityHandle handle)
        {
            PartnerManager partnerManager = new PartnerManager();
            var partnerName = partnerManager.GetPartnerName(invoiceQueue.InvoiceTypeId, invoiceQueue.PartnerId);
            try
            {

                foreach (var invoiceBulkActionPreparedEntity in invoiceBulkActionPreparedEntities)
                {
                    var actionContext = new AutomaticActionRuntimeSettingsContext
                    {
                        Invoice = invoiceQueue,
                        DefinitionSettings = invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Settings
                    };
                    invoiceBulkActionPreparedEntity.InvoiceBulkActionRuntime.Settings.Execute(actionContext);
                    if (actionContext.ErrorMessage != null)
                    {
                        throw new Exception(actionContext.ErrorMessage);
                    }
                    else
                        handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "{0} completed successfully for invoice for account '{1}' having serial number '{2}' .", invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Title, partnerName, invoiceQueue.SerialNumber);
                }
            }
            catch (Exception ex)
            {
                switch (handlingErrorOption)
                {
                    case Entities.HandlingErrorOption.Skip:
                        handle.SharedInstanceData.WriteBusinessHandledException(Utilities.WrapException(ex, string.Format("'{0}' of serial number '{1}'.", partnerName, invoiceQueue.SerialNumber)));
                        break;
                    case Entities.HandlingErrorOption.Stop:
                        handle.SharedInstanceData.WriteBusinessHandledException(Utilities.WrapException(ex, string.Format("'{0}' of serial number '{1}'.", partnerName, invoiceQueue.SerialNumber)), true);
                        throw ex;
                }
            }

        }

        internal class InvoiceBulkActionPreparedEntity
        {
            public InvoiceBulkAction InvoiceBulkActionDefinition { get; set; }
            public InvoiceBulkActionRuntime InvoiceBulkActionRuntime { get; set; }
        }
    }
  
}

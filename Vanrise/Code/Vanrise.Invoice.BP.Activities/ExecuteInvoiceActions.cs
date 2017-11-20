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
using Vanrise.Entities;
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


            foreach (var invoiceBulkActionPreparedEntity in invoiceBulkActionPreparedEntities)
            {
                var actionContext = new AutomaticActionRuntimeSettingsContext
                {
                    Invoice = invoiceQueue,
                    DefinitionSettings = invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Settings
                };

                try
                {
                    invoiceBulkActionPreparedEntity.InvoiceBulkActionRuntime.Settings.Execute(actionContext);
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format("{0} not executed for invoice '{1}' of account '{2}'.", invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Title, invoiceQueue.SerialNumber, partnerName);
                    var exception = Utilities.WrapException(ex, errorMessage);
                    switch (handlingErrorOption)
                    {
                        case Entities.HandlingErrorOption.Skip:
                            handle.SharedInstanceData.WriteBusinessHandledException(exception);
                            continue;
                        case Entities.HandlingErrorOption.Stop:
                            throw exception;
                    }
                }


                if (actionContext.ErrorMessage != null)
                {
                    string errorMessage = string.Format("{0} not executed for invoice '{1}' of account '{2}'. Reason '{3}'.", invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Title, invoiceQueue.SerialNumber, partnerName, actionContext.ErrorMessage);
                    if (!actionContext.IsErrorOccured)
                    {
                        handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, errorMessage);
                    }
                    else
                    {
                        switch (handlingErrorOption)
                        {
                            case Entities.HandlingErrorOption.Skip:
                                handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Warning, errorMessage);
                                break;
                            case Entities.HandlingErrorOption.Stop:
                                throw new VRBusinessException(errorMessage);
                        }
                    }
                }
                else
                    handle.SharedInstanceData.WriteBusinessTrackingMsg(Vanrise.Entities.LogEntryType.Information, "{0} completed successfully for invoice '{1}' of account '{2}'.", invoiceBulkActionPreparedEntity.InvoiceBulkActionDefinition.Title, invoiceQueue.SerialNumber, partnerName);

            }
        }

        internal class InvoiceBulkActionPreparedEntity
        {
            public InvoiceBulkAction InvoiceBulkActionDefinition { get; set; }
            public InvoiceBulkActionRuntime InvoiceBulkActionRuntime { get; set; }
        }
    }
  
}

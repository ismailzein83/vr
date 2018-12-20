﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.BP.Arguments;
using Vanrise.Invoice.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceBulkActionsBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            context.BPInstance.ThrowIfNull("context.BPInstance");
            new HoldRequestManager().DeleteHoldRequestByBPInstanceId(context.BPInstance.ProcessInstanceID);
        }

        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            InvoiceBulkActionProcessInput invoiceBulkActionProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<InvoiceBulkActionProcessInput>("context.IntanceToRun.InputArgument");


            var bulkActionTypes = new InvoiceTypeManager().GetInvoiceBulkActionsByBulkActionId(invoiceBulkActionProcessInput.InvoiceTypeId);

            if (invoiceBulkActionProcessInput != null && invoiceBulkActionProcessInput.InvoiceBulkActions != null && invoiceBulkActionProcessInput.InvoiceBulkActions.Count != 0)
            {
                foreach (var bulkAction in invoiceBulkActionProcessInput.InvoiceBulkActions)
                {
                    var invoiceBulkAction = bulkActionTypes.GetRecord(bulkAction.InvoiceBulkActionId);
                    var bulkActionCheckAccessContext = new AutomaticInvoiceActionSettingsCheckAccessContext
                    {
                        UserId = context.IntanceToRun.InitiatorUserId,
                        InvoiceBulkAction = invoiceBulkAction
                    };
                    if (!invoiceBulkAction.Settings.DoesUserHaveAccess(bulkActionCheckAccessContext))
                        context.Reason = String.Format("'{0}' Action. Reason : You do not have access.", invoiceBulkAction.Title);
                    return false;
                }
            }

            DateTime invoiceMinimumFrom = invoiceBulkActionProcessInput.MinimumFrom;
            DateTime invoiceMaximumTo = invoiceBulkActionProcessInput.MaximumTo.AddDays(1);

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                InvoiceBulkActionProcessInput startedInvoiceBulkActionBPArg = startedBPInstance.InputArgument as InvoiceBulkActionProcessInput;
                if (startedInvoiceBulkActionBPArg != null)
                {
                    if(startedInvoiceBulkActionBPArg.InvoiceTypeId == invoiceBulkActionProcessInput.InvoiceTypeId)
                    {
                        DateTime instanceMinimumFrom = startedInvoiceBulkActionBPArg.MinimumFrom;
                        DateTime instanceMaximumTo = startedInvoiceBulkActionBPArg.MaximumTo.AddDays(1);
                        if (Utilities.AreTimePeriodsOverlapped(invoiceMinimumFrom, invoiceMaximumTo, instanceMinimumFrom, instanceMaximumTo))
                        {
                            context.Reason = String.Format("Another invoice action instance is running from {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", startedInvoiceBulkActionBPArg.MinimumFrom, startedInvoiceBulkActionBPArg.MaximumTo);
                            return false;
                        }
                    }
                  
                }
            }

            if(invoiceBulkActionProcessInput.InvoiceBulkActions == null)
            {
                context.Reason = String.Format("There are no actions to execute.");
                return false;
            }

            bool shouldWaitImport = false;
            foreach(var action in invoiceBulkActionProcessInput.InvoiceBulkActions )
            {
                if(action.Settings.ShouldWaitImport)
                {
                    shouldWaitImport = true;
                    break;
                }
            }
            if(shouldWaitImport)
            {
                HoldRequestManager holdRequestManager = new HoldRequestManager();
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                InvoiceType invoiceType = invoiceTypeManager.GetInvoiceType(invoiceBulkActionProcessInput.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", invoiceBulkActionProcessInput.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", invoiceBulkActionProcessInput.InvoiceTypeId);
                if (invoiceType.Settings.ExecutionFlowDefinitionId.HasValue)
                {
                    HoldRequest existingHoldRequest = holdRequestManager.GetHoldRequest(context.IntanceToRun.ProcessInstanceID);
                    if (existingHoldRequest == null)
                    {
                        holdRequestManager.InsertHoldRequest(context.IntanceToRun.ProcessInstanceID, invoiceType.Settings.ExecutionFlowDefinitionId.Value, invoiceMinimumFrom, invoiceMaximumTo,
                          invoiceType.Settings.StagesToHoldNames, invoiceType.Settings.StagesToProcessNames, HoldRequestStatus.Pending);

                        context.Reason = "Waiting CDR Import";
                        return false;
                    }
                    else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
                    {
                        context.Reason = "Waiting CDR Import";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
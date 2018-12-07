using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.BP.Arguments;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Invoice.Business
{
    public class AutomaticInvoiceBulkActionBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            InvoiceBulkActionProcessInput reprocessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<InvoiceBulkActionProcessInput>("context.IntanceToRun.InputArgument");
            var bulkActionTypes = new InvoiceTypeManager().GetInvoiceBulkActionsByBulkActionId(reprocessInputArg.InvoiceTypeId);

            if (reprocessInputArg != null && reprocessInputArg.InvoiceBulkActions != null && reprocessInputArg.InvoiceBulkActions.Count != 0)
            {
                foreach (var bulkAction in reprocessInputArg.InvoiceBulkActions)
                {
                    var invoiceBulkAction = bulkActionTypes.GetRecord(bulkAction.InvoiceBulkActionId);
                    var bulkActionCheckAccessContext = new AutomaticInvoiceActionSettingsCheckAccessContext
                    {
                        UserId = ContextFactory.GetContext().GetLoggedInUserId(),
                        InvoiceBulkAction = invoiceBulkAction
                    };
                    if (!invoiceBulkAction.Settings.DoesUserHaveAccess(bulkActionCheckAccessContext))
                        context.Reason = String.Format("'{0}' Action. Reason : You do not have access.", invoiceBulkAction.Title);
                    return false;
                }
            }
            return true;
        }
    }
}

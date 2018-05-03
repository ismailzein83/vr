using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ApprovalGridFilterCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("18D88A5E-B148-4D2E-B2AE-F411757F5591"); }
        }
        public bool NeedApproval { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            //is not NeedApproval when it does not need approval or approved
            if (!NeedApproval) {
                if (!context.Invoice.NeedApproval.HasValue || (context.Invoice.NeedApproval.HasValue && !context.Invoice.NeedApproval.Value) || context.Invoice.ApprovedTime.HasValue)
                    return true;
                else
                    return false;
            }

            //NeedApproval when need approval and not approved
            else
            {
                if ((context.Invoice.NeedApproval.HasValue && context.Invoice.NeedApproval.Value) && !context.Invoice.ApprovedTime.HasValue)
                    return true;
                else
                    return false;
            }
        }
    }
}

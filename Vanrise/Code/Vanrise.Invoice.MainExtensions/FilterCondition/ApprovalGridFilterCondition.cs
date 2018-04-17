using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    class ApprovalGridFilterCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("B220DD18-D38E-4170-8E80-7508854C29F6"); }
        }
        public bool NeedApproval { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            //is not NeedApproval when it does not need approval or approved

            //NeedApproval when need approval and not approved
            throw new NotImplementedException();
        }
    }
}

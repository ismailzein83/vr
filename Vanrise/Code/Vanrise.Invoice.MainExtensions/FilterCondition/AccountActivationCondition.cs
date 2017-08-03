using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.MainExtensions
{
    public class AccountActivationCondition : InvoiceGridActionFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("5BC6CF2E-5A0F-47A4-A939-6137FB8E1E1A"); }
        }
        public bool IsActive { get; set; }
        public override bool IsFilterMatch(IInvoiceGridActionFilterConditionContext context)
        {
            context.InvoiceAccount.ThrowIfNull("context.InvoiceAccount");
            switch(context.InvoiceAccount.Status)
            {
                case Vanrise.Entities.VRAccountStatus.Active:
                    if (this.IsActive)
                        return true;
                    return false;
                case Vanrise.Entities.VRAccountStatus.InActive:
                    if (this.IsActive)
                        return false;
                    return true;
                default: return true;
            }
        }
    }
}

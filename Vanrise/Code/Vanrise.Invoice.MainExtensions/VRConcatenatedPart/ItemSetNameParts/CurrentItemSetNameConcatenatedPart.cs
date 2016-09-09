using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart
{
    public class CurrentItemSetNameConcatenatedPart : VRConcatenatedPartSettings<IInvoiceItemConcatenatedPartContext>
    {
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return context.CurrentItemSetName;
        }
    }
}

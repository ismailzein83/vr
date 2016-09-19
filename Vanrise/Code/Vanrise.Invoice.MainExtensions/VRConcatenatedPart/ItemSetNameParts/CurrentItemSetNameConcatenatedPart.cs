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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("17832921-2BD5-4D2B-B952-C45BDBA25B33"); } }
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return context.CurrentItemSetName;
        }
    }
}

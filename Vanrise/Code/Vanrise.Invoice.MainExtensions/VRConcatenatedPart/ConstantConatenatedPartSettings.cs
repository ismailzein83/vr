using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart
{
    public class ConstantConatenatedPartSettings : VRConcatenatedPartSettings<IInvoiceItemConcatenatedPartContext>
    {
        public string Constant { get; set; }
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return this.Constant;
        }
    }
}

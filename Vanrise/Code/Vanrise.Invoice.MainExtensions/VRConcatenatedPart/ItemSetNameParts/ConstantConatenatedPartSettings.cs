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
        public override Guid ConfigId { get { return  new Guid("9E0B783D-FB05-4173-A137-CF01F2CDC83A"); } }
        public string Constant { get; set; }
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return this.Constant;
        }
    }
}

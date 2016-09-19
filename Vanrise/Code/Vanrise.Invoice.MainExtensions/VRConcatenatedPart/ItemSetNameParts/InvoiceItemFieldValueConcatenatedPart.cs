using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart
{
    public class InvoiceItemFieldValueConcatenatedPart : VRConcatenatedPartSettings<IInvoiceItemConcatenatedPartContext>
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("EDA771DE-D1CC-4137-B2AD-1647D4C50B81"); } }
        public string FieldName { get; set; }
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return context.InvoiceItemDetails != null ? Vanrise.Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.InvoiceItemDetails) : null;
        }
    }
}

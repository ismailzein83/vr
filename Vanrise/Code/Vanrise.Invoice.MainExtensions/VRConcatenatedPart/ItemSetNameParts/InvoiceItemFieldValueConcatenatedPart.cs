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
        public override Guid ConfigId { get { return new Guid("EDA771DE-D1CC-4137-B2AD-1647D4C50B81"); } }
        public string FieldName { get; set; }
        public override string GetPartText(IInvoiceItemConcatenatedPartContext context)
        {
            return context.InvoiceItemDetails != null ? Convert.ToString(context.InvoiceItemDetails.GetType().GetProperty(this.FieldName).GetValue(context.InvoiceItemDetails, null)) : null;
            //Vanrise.Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.InvoiceItemDetails)
        }
    }
}

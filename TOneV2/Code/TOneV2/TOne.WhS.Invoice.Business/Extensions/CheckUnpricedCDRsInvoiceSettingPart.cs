using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CheckUnpricedCDRsInvoiceSettingPart : Vanrise.Invoice.Entities.InvoiceSettingPart
    {
        public bool IsEnabled { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("28840BD1-3C9A-4793-8A46-64992F823C43"); }
        }
    }
}

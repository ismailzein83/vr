using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
    public class IncludeCDRsInvoiceSettingPart : InvoiceSettingPart
    {
        public bool IncludeCDRs { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("809622AB-15B3-4398-8CB8-1EA96ED90CAB"); }
        }
    }
}

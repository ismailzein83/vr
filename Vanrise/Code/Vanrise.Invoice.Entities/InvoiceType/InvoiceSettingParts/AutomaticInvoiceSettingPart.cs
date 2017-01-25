using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Entities
{
    public class AutomaticInvoiceSettingPart : InvoiceSettingPart
    {
        public bool IsEnabled { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("0356AB64-58D4-4645-AF56-CCB19585D7C3"); }
        }

    }
}

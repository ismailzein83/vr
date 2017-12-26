using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Arguments
{

    public class AutomaticInvoiceProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid InvoiceTypeId { get; set; }
        public int EndDateOffsetFromToday { get; set; }
        public int IssueDateOffsetFromToday { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public VRAccountStatus Status { get; set; }
        public PartnerGroup PartnerGroup { get; set; }
        public int AccountStatus { get; set; }
        public InvoiceGapAction InvoiceGapAction { get; set; }
        public override string GetTitle()
        {
            return "Automatic Invoice Process";
        }
    }
}

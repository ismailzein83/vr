using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class PartnerNameManagerContext : IPartnerNameManagerContext
    {
        public string PartnerId { get; set; }
    }
    public class PartnerDuePeriodContext : IPartnerDuePeriodContext
    {
        public string PartnerId { get; set; }
    }
    public class ActualPartnerContext : IActualPartnerContext
    {
        public string PartnerId { get; set; }
    }
    public class PartnerManagerInfoContext : IPartnerManagerInfoContext
    {
        public string PartnerId { get; set; }

        public string InfoType { get; set; }

        public InvoicePartnerManager InvoicePartnerManager { get; set; }
    }
}

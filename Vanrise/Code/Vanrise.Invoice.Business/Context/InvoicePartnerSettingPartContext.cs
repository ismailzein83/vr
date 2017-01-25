using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class InvoicePartnerSettingPartContext : IInvoicePartnerSettingPartContext
    {
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceSettingId { get; set; }
        public string PartnerId { get; set; }
      
    }
}

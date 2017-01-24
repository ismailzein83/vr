using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGeneratorAction
    {
        public string Title { get; set; }
        public VRButtonType ButtonType { get; set; }
        public Guid InvoiceGeneratorActionId { get; set; }
        public PartnerInvoiceFilterCondition FilterCondition { get; set; }

    }

}

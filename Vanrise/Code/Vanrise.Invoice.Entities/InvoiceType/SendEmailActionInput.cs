using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class SendEmailActionInput
    {
        public long InvoiceId { get; set; }
        public Guid InvoiceActionId { get; set; }    
        public VRMailEvaluatedTemplate EmailTemplate { get; set; }
    }
}

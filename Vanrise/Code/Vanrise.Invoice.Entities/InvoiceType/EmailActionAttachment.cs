using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class EmailActionAttachment
    {
        public Guid EmailActionAttachmentId { get; set; }
        public string Title { get; set; }
        public InvoiceFileConverter InvoiceFileConverter { get; set; }
    }
}

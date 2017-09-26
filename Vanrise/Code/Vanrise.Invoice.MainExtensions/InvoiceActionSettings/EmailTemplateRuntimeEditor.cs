using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class EmailTemplateRuntimeEditor
    {
        public VRMailEvaluatedTemplate VRMailEvaluatedTemplate { get; set; }
        public List<EmailAttachment> EmailAttachments { get; set; }
    }
    public class EmailAttachment
    {
        public string AttachmentName { get; set; }
        public Guid AttachmentId { get; set; }
    }
}

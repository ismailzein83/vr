using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Business.UCT
{
    public class UctMailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<UtcMailMessageAttachment> Attachments { get; set; }
    }

    public class UtcMailMessageAttachment
    {
        public string Name { get; set; }

        public byte[] Content { get; set; }
    }
}

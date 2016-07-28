using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailMessage
    {
        public List<VRMailMessageAddress> To { get; set; }

        public List<VRMailMessageAddress> CC { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<VRMailMessageAttachment> Attachments { get; set; }
    }

    public class VRMailMessageAddress
    {
        public string Email { get; set; }
    }

    public class VRMailMessageAttachment
    {

    }
}

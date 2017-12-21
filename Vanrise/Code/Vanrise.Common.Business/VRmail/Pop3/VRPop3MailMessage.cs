using OpenPop.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRPop3MailMessage : VRReceivedMailMessage
    {

    }

    public class VRReceivedMailMessage
    {
        public VRReceivedMailMessageHeader Header { get; set; }
        public List<VRFile> Attachments { get; set; }
    }

}

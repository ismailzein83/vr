using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Mime.Header;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRPop3MailMessageHeader : VRReceivedMailMessageHeader
    {
        public string Sender { get; set; }
        public int MessageIndex { get; set; }
    }
    public abstract class VRReceivedMailMessageHeader
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime MessageSendTime { get; set; }
        public string MessageId { get; set; }
    }
}

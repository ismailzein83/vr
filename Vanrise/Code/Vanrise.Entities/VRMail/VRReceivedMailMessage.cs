using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRReceivedMailMessage
    {
        public abstract VRReceivedMailMessageHeader Header { get; }
        public abstract List<VRFile> GetAttachments();
    }
    public abstract class VRReceivedMailMessageHeader
    {
        public abstract string From { get; }
        public abstract string Subject { get; }
        public abstract string MessageId { get; }
        public abstract DateTime MessageSendTime { get; }
    }
}

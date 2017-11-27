using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    /// <summary>
    /// This connection can be only called in a single thread. 
    /// </summary>
    public class VRPop3Connection : VRConnectionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public TimeSpan TimeToKeepReadMessageIdsInState { get; set; }

        public bool HasNewMessages(string senderIdentifier)
        {
            throw new NotImplementedException();
        }

        public void ReadNewMessages(string senderIdentifier, Func<VRPop3MailMessageHeader, bool> mailMessageFilter, Action<VRPop3MailMessage> onMessageRead)
        {
            throw new NotImplementedException();
        }

        public void SetMessagesRead(string senderIdentifier, List<VRPop3MailMessage> messages)
        {
            throw new NotImplementedException();
        }
    }
}

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
        public override Guid ConfigId { get { return new Guid("E3BF7C73-14BA-402B-9158-A67D03635447"); } }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }

        public TimeSpan TimeToKeepReadMessageIdsInState { get; set; }

        public bool HasNewMessages(string senderIdentifier)
        {
            throw new NotImplementedException();
        }

        public void ReadNewMessages(string senderIdentifier, Func<VRPop3MailMessageHeader, bool> mailMessageFilter, Action<VRPop3MailMessage> onMessageRead)
        {
            List<VRPop3MailMessageHeader> pop3List = new List<VRPop3MailMessageHeader>();
            IEnumerable<VRPop3MailMessageHeader> filtered = pop3List.Where(mailMessageFilter);

            
        }

        public void SetMessagesRead(string senderIdentifier, List<VRPop3MailMessage> messages)
        {
            throw new NotImplementedException();
        }
    }

    public class Pop3ConnectionFilter : IVRConnectionFilter
    {
        public Guid ConfigId { get { return new Guid("E3BF7C73-14BA-402B-9158-A67D03635447"); } }

        public bool IsMatched(VRConnection vrConnection)
        {
            if (vrConnection == null)
                throw new NullReferenceException("connection");

            if (vrConnection.Settings == null)
                throw new NullReferenceException("vrConnection.Settings");

            if (vrConnection.Settings.ConfigId != ConfigId)
                return false;

            return true;
        }
    }    
}

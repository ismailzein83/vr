using OpenPop.Mime;
using OpenPop.Mime.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
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

        public void ReadNewMessages(Guid connectionId, string senderIdentifier, Func<VRPop3MailMessageHeader, bool> mailMessageFilter, Action<VRReceivedMailMessage> onMessageRead)
        {
            OpenPop.Pop3.Pop3Client openPopClient = new OpenPop.Pop3.Pop3Client();
            openPopClient.Connect(Server, Port, SSL);
            openPopClient.Authenticate(UserName,Password);

            IVRPop3MailMessageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRPop3MailMessageDataManager>();
            DateTime lastMessageSendTime = dataManager.GetLastMessageSendTime(connectionId,senderIdentifier);
            lastMessageSendTime=lastMessageSendTime.AddHours(-1);
            List<string> messagesId = dataManager.GetPop3MailMessagesIdsFromDateTime(connectionId, senderIdentifier, lastMessageSendTime);
            
            
            List<VRPop3MailMessageHeader> pop3MailMessageHeaders = new List<VRPop3MailMessageHeader>();
            
            int messagesCount = openPopClient.GetMessageCount();
            int messageIndex = messagesCount;
            
            while (messageIndex >= 0)
            {
                MessageHeader messageHeader = openPopClient.GetMessageHeaders(messageIndex);
                if (messageHeader.DateSent < lastMessageSendTime) break;
                VRPop3MailMessageHeader pop3MailMessageHeader = new VRPop3MailMessageHeader 
                {
                    From=messageHeader.From.Address,
                    Subject=messageHeader.Subject,
                    MessageSendTime=messageHeader.DateSent,
                    MessageId=messageHeader.MessageId,
                    Sender=messageHeader.Sender.Address,
                    MessageIndex = messageIndex,
                };
                
                if (!messagesId.Contains(messageHeader.MessageId) && mailMessageFilter(pop3MailMessageHeader))
                {
                    pop3MailMessageHeaders.Add(pop3MailMessageHeader);
                }
                messageIndex--;
            }
            for (var i = pop3MailMessageHeaders.Count - 1; i >= 0; i--)
            {
                Message message = openPopClient.GetMessage(pop3MailMessageHeaders[i].MessageIndex);
                List<MessagePart> files = message.FindAllAttachments();
                List<VRFile> vrFiles = new List<VRFile>();
                foreach(var file in files)
                {

                }
                VRReceivedMailMessage pop3MailMessage = new VRPop3MailMessage
                {
                    Header = pop3MailMessageHeaders[i],
                    Attachments = vrFiles,
                };
                onMessageRead(pop3MailMessage);
            }
        }

        public void SetMessagesRead(string senderIdentifier, List<VRPop3MailMessage> messages)
        {
            DateTime lastSendTime = messages.Max(item => item.Header.MessageSendTime);
            //add new messages and delete all messages before 1 hour
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

    public abstract class VRPop3MessageFilter
    {
        public abstract Guid ConfigId { get; }
        public Func<VRReceivedMailMessageHeader, bool> IsApplicable;
        public abstract bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader); 
    }

    public class VRPop3MessageFilterConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_Pop3MessageFilter";
        public string Editor { get; set; }
    }
}

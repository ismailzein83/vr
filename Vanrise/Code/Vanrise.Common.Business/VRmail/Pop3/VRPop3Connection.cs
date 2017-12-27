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
            openPopClient.Authenticate(UserName, Password);

            IVRReceivedMailMessageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRReceivedMailMessageDataManager>();
            DateTime lastMessageSendTime = dataManager.GetLastMessageSendTime(connectionId, senderIdentifier);
            Vanrise.Common.Business.UtilityManager utilityManager = new Vanrise.Common.Business.UtilityManager();
            utilityManager.GetDateTimeRange();
            if (lastMessageSendTime > utilityManager.GetDateTimeRange().From.AddHours(1))
                lastMessageSendTime = lastMessageSendTime.AddHours(-1);
            else lastMessageSendTime = utilityManager.GetDateTimeRange().From;

            List<string> messagesIds = dataManager.GetReceivedMailMessagesIdsFromSpecificTime(connectionId, senderIdentifier, lastMessageSendTime);
            if (messagesIds == null)
                messagesIds = new List<string>();

            List<VRPop3MailMessageHeader> pop3MailMessageHeaders = new List<VRPop3MailMessageHeader>();

            int messageIndex = openPopClient.GetMessageCount();

            while (messageIndex > 0)
            {
                MessageHeader messageHeader = openPopClient.GetMessageHeaders(messageIndex);
                if (messageHeader.DateSent < lastMessageSendTime) break;
                VRPop3MailMessageHeader pop3MailMessageHeader = new VRPop3MailMessageHeader(messageHeader);
                pop3MailMessageHeader.MessageIndex = messageIndex;

                if (!messagesIds.Contains(messageHeader.MessageId) && mailMessageFilter(pop3MailMessageHeader))
                {
                    pop3MailMessageHeaders.Add(pop3MailMessageHeader);
                }
                messageIndex--;
            }
            for (var i = pop3MailMessageHeaders.Count - 1; i >= 0; i--)
            {
                Message message = openPopClient.GetMessage(pop3MailMessageHeaders[i].MessageIndex);
                List<MessagePart> attachments = message.FindAllAttachments();
                List<VRFile> vrFiles = new List<VRFile>();


                
                VRReceivedMailMessage pop3MailMessage = new VRPop3MailMessage(message);
                onMessageRead(pop3MailMessage);
            }
        }

        public void SetMessagesRead(Guid connectionId, string senderIdentifier, List<VRReceivedMailMessage> messages)
        {
            IVRReceivedMailMessageDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRReceivedMailMessageDataManager>();
            dataManager.Insert(connectionId, senderIdentifier, messages);
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
        public abstract bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader);
    }

    public class VRPop3MessageFilterConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VRCommon_Pop3MessageFilter";
        public string Editor { get; set; }
    }
}

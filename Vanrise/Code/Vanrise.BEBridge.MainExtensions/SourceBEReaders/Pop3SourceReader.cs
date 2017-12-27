using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class Pop3SourceReader : SourceBEReader
    {
        public Pop3SourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            VRPop3Connection pop3Connection = new VRConnectionManager().GetVRConnection(Setting.VRConnectionId).Settings as VRPop3Connection;
            string senderIdentifier = getSenderIdentifier(context.BEReceiveDefinitionId);
            List<VRReceivedMailMessage> messages = new List<VRReceivedMailMessage>();

            var OnMessageRead = new Action<VRReceivedMailMessage>(delegate(VRReceivedMailMessage receivedMailMessage)
            {
                messages.Add(receivedMailMessage);
                if (messages.Count == Setting.BatchSize)
                {
                    var receivedMailSourceBatch = new ReceivedMailSourceBatch
                    {
                        Messages = messages,
                    };
                    context.OnSourceBEBatchRetrieved(receivedMailSourceBatch, null);
                    messages = new List<VRReceivedMailMessage>();
                }
            });
            pop3Connection.ReadNewMessages(Setting.VRConnectionId, senderIdentifier, Setting.Pop3MessageFilter.IsApplicableFunction, OnMessageRead);

            if (messages.Count > 0)
            {
                var receivedMailSourceBatch = new ReceivedMailSourceBatch
                {
                    Messages = messages,
                };
                context.OnSourceBEBatchRetrieved(receivedMailSourceBatch, null);
            }
        }

        public override void SetBatchCompleted(ISourceBEReaderSetBatchImportedContext context)
        {
            VRPop3Connection pop3Connection = new VRConnectionManager().GetVRConnection(Setting.VRConnectionId).Settings as VRPop3Connection;
            string senderIdentifier = getSenderIdentifier(context.BEReceiveDefinitionId);
            List<VRReceivedMailMessage> messagesList = (context.Batch as ReceivedMailSourceBatch).Messages;
            pop3Connection.SetMessagesRead(Setting.VRConnectionId, senderIdentifier, messagesList);
            base.SetBatchCompleted(context);
        }

        private string getSenderIdentifier(Guid BEReceiveDefinitionId)
        {
            return "BEReceiveDefinition_" + BEReceiveDefinitionId;
        }
    }

    public class Pop3SourceReaderSetting
    {
        public Guid VRConnectionId { get; set; }
        public VRPop3MessageFilter Pop3MessageFilter { get; set; }
        public int BatchSize { get; set; }
    }
}

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
        private OpenPop.Mime.Header.MessageHeader _messageHeader;

        public VRPop3MailMessageHeader(OpenPop.Mime.Header.MessageHeader messageHeader)
        {
            this._messageHeader = messageHeader;
        }
        public override string From
        {
            get { return this._messageHeader.From.Address; }
        }
        public override string Subject
        {
            get { return this._messageHeader.Subject; }
        }
        public override string MessageId
        {
            get { return this._messageHeader.MessageId; }
        }
        public override DateTime MessageSendTime
        {
            get { return this._messageHeader.DateSent; }
        }
        public int MessageIndex { get; set; }
    }
}

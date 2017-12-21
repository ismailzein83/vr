using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Common.MainExtensions
{
    class Pop3MailMessageConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Received Mail Message Convertor";
            }
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            ReceivedMailSourceBatch receivedMailSourceBatch = context.SourceBEBatch as ReceivedMailSourceBatch;
            List<ITargetBE> targetBEs = new List<ITargetBE>();

            foreach (VRReceivedMailMessage receivedMailMessage in receivedMailSourceBatch.Messages)
            {
                try
                {
                    VRObjectsTargetBE targetBe = new VRObjectsTargetBE();
                    targetBe.TargetObjects.Add("Message", receivedMailMessage);
                    targetBEs.Add(targetBe);
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to send message '{0}' due to conversion error", receivedMailMessage.Header.MessageId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = targetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
        }
    }
}

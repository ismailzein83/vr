using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SMSSendHandler
    {
        public SMSSendHandlerSettings Settings { get; set; }
    }

    public abstract class SMSSendHandlerSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void SendSMS(ISMSHandlerSendSMSContext context);
    }

    public interface ISMSHandlerSendSMSContext
    {
        string MobileNumber { get; }

        string Message { get; }

        DateTime MessageTime { get; }
    }
}

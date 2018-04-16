using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class SMSManager
    {
        public void SendSMS(string mobileNumber, string message, DateTime messageTime, SMSSendHandler smsSendHandler)
        {
            smsSendHandler.ThrowIfNull("smsSendHandler");
            smsSendHandler.Settings.ThrowIfNull("smsSendHandler.Settings");

            SMSHandlerSendSMSContext context = new SMSHandlerSendSMSContext
            {
                Message = message,
                MessageTime = messageTime,
                MobileNumber = mobileNumber
            };
            smsSendHandler.Settings.SendSMS(context);
        }

        public void SendSMS(Guid smsTemplateId, Dictionary<string, dynamic> objects, SMSSendHandler smsSendHandler)
        {

        } 
    }

    public class SMSHandlerSendSMSContext : ISMSHandlerSendSMSContext
    {
        public string MobileNumber { get; set; }

        public string Message { get; set; }

        public DateTime MessageTime { get; set; }
    }
}

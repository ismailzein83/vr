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

        }

        public void SendSMS(Guid smsTemplateId, Dictionary<string, dynamic> objects, SMSSendHandler smsSendHandler)
        {

        } 
    }
}

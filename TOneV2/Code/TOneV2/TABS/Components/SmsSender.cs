using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace TABS.Components
{
    public class SmsSender
    {
        SpecialSystemParameters.SmsInfo smsInfo = new TABS.SpecialSystemParameters.SmsInfo();
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(SmsSender));

        public SmsSender()
        {
        }

        private void SendSMS(string smsText, string sendTo)
        {
            string userId = smsInfo.User;
            string password = smsInfo.Password;
            string host = smsInfo.Host;
            string senderID = smsInfo.SenderID;

            StringBuilder postData = new StringBuilder();
            string responseMessage = string.Empty;
            HttpWebRequest request = null;

            postData.Append("ServerRoot=http://www.moursel.com/mrWebAPISend.aspx");
            postData.Append("&SUsername=" + userId);
            postData.Append("&Spassword=" + password);
            postData.Append("&SenderID=" + senderID);
            postData.Append("&MessToSent=" + smsText);
            postData.Append("&mobnum=" + sendTo);
            postData.Append("&mt=1");

            byte[] data = new System.Text.ASCIIEncoding().GetBytes(postData.ToString());

            request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;


            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }


            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {

                using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                {
                    responseMessage = srResponse.ReadToEnd();
                }
            }
        }

        public static bool Send(string smsText, string sendTo, out Exception ex)
        {
            ex = null;

            try
            {
                SmsSender sender = new SmsSender();
                sender.SendSMS(smsText, sendTo);
                return true;
            }
            catch (Exception e)
            {
                ex = e;
                log.Error("Error Sending Mail: " + smsText, ex);
                return false;
            }
        }

        public static string SendViaSMSService(TABS.SMS message, out Exception ex)
        {
            return SendViaSMSService(System.Configuration.ConfigurationSettings.AppSettings["SMSServiceKey"], message, out ex);
        }

        public static string SendViaSMSService(string smsServiceKey, TABS.SMS message, out Exception ex)
        {
            ex = null;
            if (string.IsNullOrEmpty(smsServiceKey)) return "Empty SMS Service key.";
            if (string.IsNullOrEmpty(message.To)) return "Empty input numbers.";
            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            try
            {
                switch (smsServiceKey.ToLower())
                {
                    case "drksmsservice":
                        DrkSMSService.DRKSMSService smsService = new TABS.DrkSMSService.DRKSMSService();
                        string[] numbers = message.To.ToString().Trim().Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                        int result = 0;

                        int numbersCount = numbers.Count();
                        List<string> failedNumbers = new List<string>();

                        foreach (string number in numbers)
                        {
                            result = smsService.SendSmsMessageFromVoip(number, message.Body);
                            if (result != 1)
                                failedNumbers.Add(number);
                        }
                        if (numbersCount != 0)
                        {
                            if (failedNumbers.Count == numbersCount)
                                sb.Append("SMS sending failed.").AppendLine();
                            else
                                if (failedNumbers.Count > 0)
                                    sb.AppendFormat("SMS failed numbers : ", failedNumbers.Aggregate((a, b) => a + "," + b));
                        }
                        break;
                    default:
                        return "SMS Service key not defined.";
                }
            }
            catch (Exception exc)
            {
                ex = exc;
                log.Error("Error during send SMS.", exc);
                return null;
            }
            if (sb.Length != 0) return sb.ToString();
            return "1";
        }
    }
}


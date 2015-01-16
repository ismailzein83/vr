
namespace TABS.SpecialSystemParameters
{
   public class SmsInfo : SimpleParameterXml
    {
       
       /// <summary>
       /// The address of SMS Host
       /// </summary>
        public string Host { get { return base.Get("Host"); } set { base.Set("Host", value); } }
        public string User { get { return base.Get("Username"); } set { base.Set("Username", value); } }
        public string Password { get { return base.Get("Password"); } set { base.Set("Password", value); } }
        public string SenderID { get { return base.Get("SenderID"); } set { base.Set("SenderID", value); } }

     protected SmsInfo(string xml) : base(xml)
        {
            
        }

     public SmsInfo()
         : base(SystemParameter.SMS_Info.LongTextValue)
     {

     }
    }
}

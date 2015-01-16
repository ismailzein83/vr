
namespace TABS.SpecialSystemParameters
{
    public class SmtpInfo : SimpleParameterXml
    {
        public string Host { get { return base.Get("Host", "localhost"); } set { base.Set("Host", value); } }
        public string User { get { return base.Get("User"); } set { base.Set("User", value); } }
        public string Password { get { return base.Get("Password"); } set { base.Set("Password", value); } }
        public int Port { get { int value = 25; if (int.TryParse(base.Get("Port"), out value)) return value; else return 25; } set { base.Set("Port", value.ToString()); } }
        public bool EnableSsl { get { return "Y".Equals(base.Get("EnableSsl")); } set { base.Set("EnableSsl", value ? "Y" : "N"); } }

        public string Default_From { get { return base.Get("Default_From", "tabs@vanrise.com"); } set { base.Set("Default_From", value); } }

        protected SmtpInfo(string xml) : base(xml)
        {
            
        }

        public SmtpInfo() : base(SystemParameter.SMTP_Info.LongTextValue)
        {

        }
    }
}
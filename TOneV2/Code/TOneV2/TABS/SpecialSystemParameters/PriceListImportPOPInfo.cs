
namespace TABS.SpecialSystemParameters
{
    public class PriceListImportPOPInfo : SimpleParameterXml
    {
        public string Host { get { return base.Get("Host", "localhost"); } set { base.Set("Host", value); } }
        public string User { get { return base.Get("User"); } set { base.Set("User", value); } }
        public string Password { get { return base.Get("Password"); } set { base.Set("Password", value); } }
        public int Port { get { int value = 110; if (int.TryParse(base.Get("Port"), out value)) return value; else return 110; } set { base.Set("Port", value.ToString()); } }
        public bool EnableSsl { get { return "Y".Equals(base.Get("EnableSsl")); } set { base.Set("EnableSsl", value ? "Y" : "N"); } }        

        protected PriceListImportPOPInfo(string xml)
            : base(xml)
        {
            
        }
        public PriceListImportPOPInfo()
            : base(SystemParameter.PriceList_Import_POP_Info.LongTextValue)
        {

        }
    }
}

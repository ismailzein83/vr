using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS.SpecialSystemParameters
{
    public class CustomerInvoiceSerialGenerator : BaseXmlDetails
    {
        public CarrierAccount Customer { get { return TABS.CarrierAccount.All[Get("CustomerID")]; } set { Set("CustomerID", value.CarrierAccountID); } }
        public int InvoiceCounter { get { return int.Parse(Get("InvoiceCounter")); } set { Set("InvoiceCounter", value.ToString()); } }

        public static List<CustomerInvoiceSerialGenerator> Get(SystemParameter parameter) { return BaseXmlDetails.Get<CustomerInvoiceSerialGenerator>(parameter); }

        public static Exception Save(List<CustomerInvoiceSerialGenerator> details)
        {
            return BaseXmlDetails.Save(TABS.SystemParameter.sys_CustomerServialGeneratorInfo, details, SystemParameter.DefaultXml);
        }

        public static Dictionary<CarrierAccount, CustomerInvoiceSerialGenerator> All
        {
            get
            {
                Dictionary<CarrierAccount, CustomerInvoiceSerialGenerator> _All = new Dictionary<CarrierAccount, CustomerInvoiceSerialGenerator>();
                _All = Get(TABS.SystemParameter.sys_CustomerServialGeneratorInfo).ToDictionary(s => s.Customer);
                return _All;
            }
        }

        public string GenerateSerial()
        {
            string Serial = string.Empty;

            var DefaultSerialFormat = TABS.SystemParameter.InvoiceSerialNumberFormat.TextValue;

            var isProfileInvoice = this.Customer.CarrierProfile.IsProfileInvoice;



            return Serial;
        }


        public static string DefaultXml
        {
            get
            {
                return string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <SystemParameter>
                                          <sys_CustomerServialGeneratorInfo>
                                              <CustomerID>{0}</CustomerID> 
                                              <InvoiceCounter>{1}</InvoiceCounter> 
                                          </sys_CustomerServialGeneratorInfo>
                                        </SystemParameter>",
                                                           "SYS"
                                                           , 0
                                                           );
            }
        }


    }
}

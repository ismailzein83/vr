using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public  enum InvoiceType  { Customer = 0, Supplier = 1 }
    public class CarrierInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FAD2C45F-FB61-4D65-9896-4CCADC2A656F"); }
        }
        public InvoiceType InvoiceType { get; set; }
        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new CustomerInvoiceGenerator();
        }

        public override InvoicePartnerSettings GetPartnerSettings()
        {
            return new CarrierPartnerSettings { InvoiceType = this.InvoiceType };
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {

           
            switch(context.InfoType)
            {
                case "CustomerMailTemplate":
                     ConfigManager manager = new ConfigManager();
                    var invoiceTemplateId = manager.GetDefaultCustomerInvoiceTemplateMessageId();
                    VRMailManager vrMailManager = new VRMailManager();
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    var invoiceDetails = context.Invoice.Details as CustomerInvoiceDetails;
                    objects.Add("CustomerInvoice", invoiceDetails);
                    VRMailEvaluatedTemplate template = vrMailManager.EvaluateMailTemplate(invoiceTemplateId, objects);
                    return template;
            }
            return null;
        }
    }
}
